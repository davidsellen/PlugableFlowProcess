using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Workflow;

namespace PlugableFlowProcess.PlugableApprovalFlow
{
    public partial class AssociationForm : LayoutsPageBase
    {
        private const int CreateListTryCount = 100;
        private string historyListDescription = "Custom History List";
        private string taskListDescription = "Custom Task List";
        private string listCreationFailed = "Failed to create list {0} as a list with same name already exists";
        private string workflowAssociationFailed = "Error occured while associating Workflow template. {0}";

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeParams();
        }

        private void PopulateFormFields(SPWorkflowAssociation existingAssociation)
        {
            // Optionally, add code here to pre-populate your form fields.
        }

        // This method is called when the user clicks the button to associate the workflow.
        private string GetAssociationData()
        {
            var selected = "GeneralApprovalProcessor";

            switch (ddlWorkflowProcessor.SelectedIndex)
            {
                case 1: selected = "JobRequestApprovalProcessor"; break;
                case 2: selected = "CreateRoomApprovalProcessor"; break;
                case 3: selected = "LocationApprovalProcessor"; break;
                default:
                    break;
            }
            return selected;
        }

        protected void AssociateWorkflow_Click(object sender, EventArgs e)
        {
            // Optionally, add code here to perform additional steps before associating your workflow
            try
            {
                CreateTaskList();
                CreateHistoryList();
                HandleAssociateWorkflow();
                SPUtility.Redirect("WrkSetng.aspx", SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current, Page.ClientQueryString);
            }
            catch (Exception ex)
            {
                SPUtility.TransferToErrorPage(String.Format(CultureInfo.CurrentCulture, workflowAssociationFailed, ex.Message));
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect("WrkSetng.aspx", SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current, Page.ClientQueryString);
        }

        #region Workflow Association Code - Typically, the following code should not be changed

        private AssociationParams associationParams;

        [Serializable]
        private enum WorkflowAssociationType
        {
            ListAssociation,
            WebAssociation,
            ListContentTypeAssociation,
            SiteContentTypeAssociation
        }

        [Serializable]
        private struct AssociationParams
        {
            public string AssociationName;
            public string BaseTemplate;
            public bool AutoStartCreate;
            public bool AutoStartChange;
            public bool AllowManual;
            public bool RequireManagedListPermisions;
            public bool SetDefaultApprovalWorkflow;
            public bool LockItem;
            public Guid AssociationGuid;
            public WorkflowAssociationType AssociationType;
            public Guid TargetListGuid;
            public Guid TaskListGuid;
            public string TaskListName;
            public Guid HistoryListGuid;
            public string HistoryListName;
            public SPContentTypeId ContentTypeId;
            public bool ContentTypePushDown;
        }

        private void InitializeParams()
        {
            // Check if the page is loaded for first time
            if (ViewState["associationParams"] == null)
            {
                InitializeAssociationParams();
                ViewState["associationParams"] = this.associationParams;
                SPWorkflowAssociation existingAssociation = GetExistingAssociation();
                PopulateFormFields(existingAssociation);
            }
            else
            {
                this.associationParams = (AssociationParams)ViewState["associationParams"];
            }
        }

        private void InitializeAssociationParams()
        {
            this.associationParams = new AssociationParams();
            this.associationParams.AssociationName = Request.Params["WorkflowName"];
            this.associationParams.BaseTemplate = Request.Params["WorkflowDefinition"];
            this.associationParams.AutoStartCreate = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["AutoStartCreate"], "ON") == 0);
            this.associationParams.AutoStartChange = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["AutoStartChange"], "ON") == 0);
            this.associationParams.AllowManual = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["AllowManual"], "ON") == 0);
            this.associationParams.RequireManagedListPermisions = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["ManualPermManageListRequired"], "ON") == 0);
            this.associationParams.SetDefaultApprovalWorkflow = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["SetDefault"], "ON") == 0);
            this.associationParams.LockItem = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["AllowEdits"], "FALSE") == 0);
            this.associationParams.ContentTypePushDown = (StringComparer.OrdinalIgnoreCase.Compare(Request.Params["UpdateLists"], "TRUE") == 0);

            string associationGuid = Request.Params["GuidAssoc"];
            if (!String.IsNullOrEmpty(associationGuid))
            {
                this.associationParams.AssociationGuid = new Guid(associationGuid);
            }

            InitializeAssociationTypeParams();
            InitializeTaskListParams();
            InitializeHistoryListParams();
        }

        private void InitializeAssociationTypeParams()
        {
            string listGuid = Request.QueryString["List"];
            string contentTypeId = Request.QueryString["ctype"];

            if (!String.IsNullOrEmpty(contentTypeId))
            {
                if (!String.IsNullOrEmpty(listGuid))
                {
                    this.associationParams.AssociationType = WorkflowAssociationType.ListContentTypeAssociation;
                    this.associationParams.TargetListGuid = new Guid(listGuid);
                }
                else
                {
                    this.associationParams.AssociationType = WorkflowAssociationType.SiteContentTypeAssociation;
                }
                this.associationParams.ContentTypeId = new SPContentTypeId(contentTypeId);
            }
            else
            {
                if (!String.IsNullOrEmpty(listGuid))
                {
                    this.associationParams.AssociationType = WorkflowAssociationType.ListAssociation;
                    this.associationParams.TargetListGuid = new Guid(listGuid);
                }
                else
                {
                    this.associationParams.AssociationType = WorkflowAssociationType.WebAssociation;
                }
            }
        }

        private void InitializeTaskListParams()
        {
            string taskListParam = Request.Params["TaskList"];

            if (this.associationParams.AssociationType == WorkflowAssociationType.SiteContentTypeAssociation)
            {
                this.associationParams.TaskListName = taskListParam;
            }
            else
            {

                if (taskListParam.StartsWith("z"))
                {
                    // Create a new list if the value starts with 'z'
                    this.associationParams.TaskListName = taskListParam.Substring(1);
                }
                else
                {
                    // Use existing list
                    this.associationParams.TaskListGuid = new Guid(taskListParam);
                }
            }
        }

        private void InitializeHistoryListParams()
        {
            string historyListParam = Request.Params["HistoryList"];

            if (this.associationParams.AssociationType == WorkflowAssociationType.SiteContentTypeAssociation)
            {
                this.associationParams.HistoryListName = historyListParam;
            }
            else
            {
                if (historyListParam.StartsWith("z"))
                {
                    // Create a new list if the value starts with 'z'
                    this.associationParams.HistoryListName = historyListParam.Substring(1);
                }
                else
                {
                    // Use existing list
                    this.associationParams.HistoryListGuid = new Guid(historyListParam);
                }
            }
        }

        private SPWorkflowAssociation GetExistingAssociation()
        {
            if (this.associationParams.AssociationGuid != Guid.Empty)
            {
                SPWorkflowAssociationCollection workflowAssociationCollection;
                switch (this.associationParams.AssociationType)
                {
                    case WorkflowAssociationType.ListAssociation:
                        workflowAssociationCollection = Web.Lists[this.associationParams.TargetListGuid].WorkflowAssociations;
                        break;
                    case WorkflowAssociationType.ListContentTypeAssociation:
                        workflowAssociationCollection = Web.Lists[this.associationParams.TargetListGuid].ContentTypes[this.associationParams.ContentTypeId].WorkflowAssociations;
                        break;
                    case WorkflowAssociationType.SiteContentTypeAssociation:
                        workflowAssociationCollection = Web.ContentTypes[this.associationParams.ContentTypeId].WorkflowAssociations;
                        break;
                    default:
                        workflowAssociationCollection = Web.WorkflowAssociations;
                        break;
                }
                return workflowAssociationCollection[this.associationParams.AssociationGuid];
            }
            return null;
        }

        private void CreateTaskList()
        {
            if (this.associationParams.TaskListGuid == Guid.Empty && this.associationParams.AssociationType != WorkflowAssociationType.SiteContentTypeAssociation)
            {
                this.associationParams.TaskListGuid = CreateList(this.associationParams.TaskListName, taskListDescription, SPListTemplateType.Tasks);
            }
        }

        private void CreateHistoryList()
        {
            if (this.associationParams.HistoryListGuid == Guid.Empty && this.associationParams.AssociationType != WorkflowAssociationType.SiteContentTypeAssociation)
            {
                this.associationParams.HistoryListGuid = CreateList(this.associationParams.HistoryListName, historyListDescription, SPListTemplateType.WorkflowHistory);
            }
        }

        private Guid CreateList(string name, string description, SPListTemplateType type)
        {
            string listName = name;
            for (int i = 0; i <= CreateListTryCount; i++)
            {
                if (Web.Lists.TryGetList(listName) == null)
                {
                    return Web.Lists.Add(listName, description, type);
                }
                listName = String.Concat(name, i.ToString(CultureInfo.InvariantCulture));
            }
            throw new Exception(String.Format(CultureInfo.CurrentCulture, listCreationFailed, name));
        }

        private void HandleAssociateWorkflow()
        {
            switch (this.associationParams.AssociationType)
            {
                case WorkflowAssociationType.ListAssociation:
                    AssociateListWorkflow();
                    break;
                case WorkflowAssociationType.WebAssociation:
                    AssociateSiteWorkflow();
                    break;
                case WorkflowAssociationType.ListContentTypeAssociation:
                    AssociateListContentTypeWorkflow();
                    break;
                case WorkflowAssociationType.SiteContentTypeAssociation:
                    AssociateSiteContentTypeWorkflow();
                    break;
            }
        }

        private void AssociateSiteContentTypeWorkflow()
        {
            SPContentType contentType = Web.ContentTypes[this.associationParams.ContentTypeId];
            SPWorkflowAssociation association;
            if (this.associationParams.AssociationGuid == Guid.Empty)
            {
                association = SPWorkflowAssociation.CreateWebContentTypeAssociation(
                                                  Web.WorkflowTemplates[new Guid(this.associationParams.BaseTemplate)],
                                                  this.associationParams.AssociationName,
                                                  this.associationParams.TaskListName,
                                                  this.associationParams.HistoryListName);
                PopulateAssociationParams(association);
                contentType.WorkflowAssociations.Add(association);
            }
            else
            {
                association = contentType.WorkflowAssociations[this.associationParams.AssociationGuid];
                association.TaskListTitle = this.associationParams.TaskListName;
                association.HistoryListTitle = this.associationParams.HistoryListName;
                PopulateAssociationParams(association);
                contentType.WorkflowAssociations.Update(association);
            }

            if (this.associationParams.ContentTypePushDown)
            {
                contentType.UpdateWorkflowAssociationsOnChildren(false);
            }
        }

        private void AssociateListContentTypeWorkflow()
        {
            SPContentType contentType = Web.Lists[associationParams.TargetListGuid].ContentTypes[associationParams.ContentTypeId];
            SPWorkflowAssociation association;
            if (associationParams.AssociationGuid == Guid.Empty)
            {
                association = SPWorkflowAssociation.CreateListContentTypeAssociation(
                                                  Web.WorkflowTemplates[new Guid(this.associationParams.BaseTemplate)],
                                                  this.associationParams.AssociationName,
                                                  Web.Lists[this.associationParams.TaskListGuid],
                                                  Web.Lists[this.associationParams.HistoryListGuid]);
                PopulateAssociationParams(association);
                contentType.WorkflowAssociations.Add(association);
            }
            else
            {
                association = contentType.WorkflowAssociations[this.associationParams.AssociationGuid];
                association.SetTaskList(Web.Lists[this.associationParams.TaskListGuid]);
                association.SetHistoryList(Web.Lists[this.associationParams.HistoryListGuid]);
                PopulateAssociationParams(association);
                contentType.WorkflowAssociations.Update(association);
            }

            if (this.associationParams.ContentTypePushDown)
            {
                contentType.UpdateWorkflowAssociationsOnChildren(false);
            }
        }

        private void AssociateListWorkflow()
        {
            SPList targetList = Web.Lists[this.associationParams.TargetListGuid];
            SPWorkflowAssociation association;
            if (associationParams.AssociationGuid == Guid.Empty)
            {
                association = SPWorkflowAssociation.CreateListAssociation(
                                                  Web.WorkflowTemplates[new Guid(this.associationParams.BaseTemplate)],
                                                  this.associationParams.AssociationName,
                                                  Web.Lists[this.associationParams.TaskListGuid],
                                                  Web.Lists[this.associationParams.HistoryListGuid]);
                PopulateAssociationParams(association);
                targetList.WorkflowAssociations.Add(association);
            }
            else
            {
                association = targetList.WorkflowAssociations[this.associationParams.AssociationGuid];
                association.SetTaskList(Web.Lists[this.associationParams.TaskListGuid]);
                association.SetHistoryList(Web.Lists[this.associationParams.HistoryListGuid]);
                PopulateAssociationParams(association);
                targetList.WorkflowAssociations.Update(association);
            }

            SetDefaultContentApprovalWorkflow(targetList, association);
        }

        private void SetDefaultContentApprovalWorkflow(SPList targetList, SPWorkflowAssociation association)
        {
            if (targetList.EnableMinorVersions)
            {
                if (targetList.DefaultContentApprovalWorkflowId != association.Id && this.associationParams.SetDefaultApprovalWorkflow)
                {
                    if (!targetList.EnableModeration)
                    {
                        targetList.EnableModeration = true;
                        targetList.DraftVersionVisibility = DraftVisibilityType.Approver;
                    }
                    targetList.DefaultContentApprovalWorkflowId = association.Id;
                    targetList.Update();
                }
                else if (targetList.DefaultContentApprovalWorkflowId == association.Id && !this.associationParams.SetDefaultApprovalWorkflow)
                {
                    targetList.DefaultContentApprovalWorkflowId = Guid.Empty;
                    targetList.Update();
                }
            }
        }

        private void AssociateSiteWorkflow()
        {
            if (this.associationParams.AssociationGuid == Guid.Empty)
            {
                SPWorkflowAssociation association = SPWorkflowAssociation.CreateWebAssociation(
                                                  Web.WorkflowTemplates[new Guid(this.associationParams.BaseTemplate)],
                                                  this.associationParams.AssociationName,
                                                  Web.Lists[this.associationParams.TaskListGuid],
                                                  Web.Lists[this.associationParams.HistoryListGuid]);
                PopulateAssociationParams(association);
                Web.WorkflowAssociations.Add(association);
            }
            else
            {
                SPWorkflowAssociation association = Web.WorkflowAssociations[this.associationParams.AssociationGuid];
                association.SetTaskList(Web.Lists[this.associationParams.TaskListGuid]);
                association.SetHistoryList(Web.Lists[this.associationParams.HistoryListGuid]);
                PopulateAssociationParams(association);
                Web.WorkflowAssociations.Update(association);
            }
        }

        private void PopulateAssociationParams(SPWorkflowAssociation association)
        {
            association.Name = this.associationParams.AssociationName;
            association.AutoStartCreate = this.associationParams.AutoStartCreate;
            association.AutoStartChange = this.associationParams.AutoStartChange;
            association.AllowManual = this.associationParams.AllowManual;
            association.LockItem = this.associationParams.LockItem;
            association.ContentTypePushDown = this.associationParams.ContentTypePushDown;

            if (association.AllowManual)
            {
                association.PermissionsManual = SPBasePermissions.EmptyMask;
                if (this.associationParams.RequireManagedListPermisions)
                {
                    association.PermissionsManual |= (this.associationParams.TargetListGuid != Guid.Empty) ? SPBasePermissions.ManageLists : SPBasePermissions.ManageWeb;
                }
            }
            association.AssociationData = GetAssociationData();
        }
        #endregion
    }
}

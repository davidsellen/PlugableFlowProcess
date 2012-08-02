using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace PlugableFlowProcess.PlugableApprovalFlow
{
    public sealed partial class PlugableApprovalFlow
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.logToHistoryFirstMailSended = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.sendFirstEmail = new Microsoft.SharePoint.WorkflowActions.SendEmail();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // logToHistoryFirstMailSended
            // 
            this.logToHistoryFirstMailSended.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryFirstMailSended.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryFirstMailSended.HistoryDescription = "";
            this.logToHistoryFirstMailSended.HistoryOutcome = "";
            this.logToHistoryFirstMailSended.Name = "logToHistoryFirstMailSended";
            this.logToHistoryFirstMailSended.OtherData = "";
            this.logToHistoryFirstMailSended.UserId = -1;
            this.logToHistoryFirstMailSended.MethodInvoking += new System.EventHandler(this.logToHistoryFirstMailSended_MethodInvoking);
            // 
            // sendFirstEmail
            // 
            this.sendFirstEmail.BCC = null;
            this.sendFirstEmail.Body = null;
            this.sendFirstEmail.CC = null;
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "PlugableApprovalFlow";
            this.sendFirstEmail.CorrelationToken = correlationtoken1;
            this.sendFirstEmail.From = null;
            this.sendFirstEmail.Headers = null;
            this.sendFirstEmail.IncludeStatus = false;
            this.sendFirstEmail.Name = "sendFirstEmail";
            this.sendFirstEmail.Subject = null;
            this.sendFirstEmail.To = null;
            this.sendFirstEmail.MethodInvoking += new System.EventHandler(this.sendFirstEmail_MethodInvoking);
            activitybind2.Name = "PlugableApprovalFlow";
            activitybind2.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind1.Name = "PlugableApprovalFlow";
            activitybind1.Path = "workflowProperties";
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // PlugableApprovalFlow
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.sendFirstEmail);
            this.Activities.Add(this.logToHistoryFirstMailSended);
            this.Name = "PlugableApprovalFlow";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryFirstMailSended;

        private Microsoft.SharePoint.WorkflowActions.SendEmail sendFirstEmail;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;



    }
}

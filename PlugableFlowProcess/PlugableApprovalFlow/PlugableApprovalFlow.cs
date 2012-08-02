using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

namespace PlugableFlowProcess.PlugableApprovalFlow
{
    public sealed partial class PlugableApprovalFlow : SequentialWorkflowActivity
    {
        public PlugableApprovalFlow()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public IEnterpriseApprovalProcessor approvalProcessor;

        private void sendFirstEmail_MethodInvoking(object sender, EventArgs e)
        {
            approvalProcessor = EnterpriseApprovalProcessorFactory.Create(workflowProperties.Site, workflowProperties.AssociationData);
            var emailDefinition = approvalProcessor.GetFirstMailDefinition(onWorkflowActivated1.WorkflowProperties.Item);
            sendFirstEmail.To = emailDefinition.To;
            sendFirstEmail.Subject = emailDefinition.Subject;
            sendFirstEmail.Body = emailDefinition.Body;
        }

        private void logToHistoryFirstMailSended_MethodInvoking(object sender, EventArgs e)
        {
            approvalProcessor = EnterpriseApprovalProcessorFactory.Create(workflowProperties.Site, workflowProperties.AssociationData);            
            logToHistoryFirstMailSended.HistoryOutcome = "Workflow executed " + approvalProcessor.GetTitle();
        }
    }
}

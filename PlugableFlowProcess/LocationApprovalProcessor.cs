using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlugableFlowProcess
{
    public class LocationApprovalProcessor : IEnterpriseApprovalProcessor
    {
        #region IEnterpriseApprovalProcessor Members

        public EmailDefinition GetFirstMailDefinition(Microsoft.SharePoint.SPListItem sPListItem)
        {
            EmailDefinition emailDefinition = new EmailDefinition();
            emailDefinition.To = "hugo.silva@netpartners.com.br";
            emailDefinition.Body = "e aí?";
            emailDefinition.Subject = "assunto";

            return emailDefinition;
        }

        public string GetTitle()
        {
            return "LocationApprovalProcessor";
        }

        #endregion
    }
}

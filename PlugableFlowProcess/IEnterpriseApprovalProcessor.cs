using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlugableFlowProcess
{
    public interface IEnterpriseApprovalProcessor
    {
        EmailDefinition GetFirstMailDefinition(Microsoft.SharePoint.SPListItem sPListItem);

        string GetTitle();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;

namespace PlugableFlowProcess
{
    public class EnterpriseApprovalProcessorFactory
    {        
        public static IEnterpriseApprovalProcessor Create(SPSite site, string workflowType)
        {
            IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent(site);

            IEnterpriseApprovalProcessor processor;

            switch (workflowType)
            {
                case "JobRequestApprovalProcessor":
                    processor = serviceLocator.GetInstance<IEnterpriseApprovalProcessor>("JobRequestApprovalProcessor");
                    break;
                case  "CreateRoomApprovalProcessor":
                    processor = serviceLocator.GetInstance<IEnterpriseApprovalProcessor>("CreateRoomApprovalProcessor");
                    break;
                case "LocationApprovalProcessor":
                    processor = serviceLocator.GetInstance<IEnterpriseApprovalProcessor>("LocationApprovalProcessor");
                    break;
                default:
                    processor = serviceLocator.GetInstance<IEnterpriseApprovalProcessor>("GeneralApprovalProcessor");

                    break;
            }
            return processor;
        }
    }
}

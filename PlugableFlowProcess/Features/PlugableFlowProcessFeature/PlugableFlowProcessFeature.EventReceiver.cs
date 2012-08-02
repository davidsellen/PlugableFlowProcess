using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;

namespace PlugableFlowProcess.Features.PlugableFlowProcessFeature
{
  
    [Guid("48cb15f8-61c2-4f06-868e-36ed7f63e774")]
    public class PlugableFlowProcessFeatureEventReceiver : SPFeatureReceiver
    {
        
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
            IServiceLocatorConfig typeMappings = serviceLocator.GetInstance<IServiceLocatorConfig>();
            typeMappings.Site = properties.Feature.Parent as SPSite;
            typeMappings.RegisterTypeMapping<IEnterpriseApprovalProcessor, JobRequestApprovalProcessor>("JobRequestApprovalProcessor");
            typeMappings.RegisterTypeMapping<IEnterpriseApprovalProcessor, GeneralApprovalProcessor>("GeneralApprovalProcessor");
            typeMappings.RegisterTypeMapping<IEnterpriseApprovalProcessor, CreateRoomApprovalProcessor>("CreateRoomApprovalProcessor");
            typeMappings.RegisterTypeMapping<IEnterpriseApprovalProcessor, LocationApprovalProcessor>("LocationApprovalProcessor");
        }
         
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
            IServiceLocatorConfig typeMappings = serviceLocator.GetInstance<IServiceLocatorConfig>();
            typeMappings.Site = properties.Feature.Parent as SPSite;
            typeMappings.RemoveTypeMapping<IEnterpriseApprovalProcessor>(null);
        }        
    }
}

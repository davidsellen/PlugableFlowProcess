using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlugableFlowProcess
{
    public class CreateRoomApprovalProcessor : IEnterpriseApprovalProcessor
    {

        private string _title = "Room Approval Request";

        public string GetTitle()
        {
            return _title;
        }


        public EmailDefinition GetFirstMailDefinition(Microsoft.SharePoint.SPListItem sPListItem)
        {
            return new EmailDefinition
            {
                To = "davidsellenl@gmail.com",
                Subject = "Room Approval Request",
                Body = string.Format("Hi, there is a new Room request named {0}, please click <a href='{1}/{2}'>here</a> to approve/rejected that.",
                                       sPListItem.Title,
                                       sPListItem.Web.Site.Url,
                                       sPListItem.Url)
            };
        }
        
    }
}

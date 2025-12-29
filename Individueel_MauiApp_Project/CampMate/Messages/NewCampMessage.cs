using CampMate.Shared.DTOs; 
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CampMate.Messages
{
   
    public class NewCampMessage : ValueChangedMessage<CampSiteDto>
    {
        public NewCampMessage(CampSiteDto value) : base(value)
        {
        }
    }
}

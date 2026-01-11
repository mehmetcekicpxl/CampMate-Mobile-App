using CampMate.Shared.DTOs;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CampMate.Messages
{
   
    public class CampUpdatedMessage : ValueChangedMessage<CampSiteDto>
    {
        public CampUpdatedMessage(CampSiteDto camp) : base(camp)
        {
        }
    }
}
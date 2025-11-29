using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CampMate.Models;

namespace CampMate.Messages
{
    public class NewCampMessage: ValueChangedMessage<CampSite>
    {
        public NewCampMessage(CampSite value) : base(value)
        {
        }
    }
}

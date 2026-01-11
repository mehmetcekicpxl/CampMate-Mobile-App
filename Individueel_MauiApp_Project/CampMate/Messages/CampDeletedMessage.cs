using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Messages
{
   
    public class CampDeletedMessage : ValueChangedMessage<int>
    {
        public CampDeletedMessage(int campId) : base(campId)
        {
        }
    }
} 


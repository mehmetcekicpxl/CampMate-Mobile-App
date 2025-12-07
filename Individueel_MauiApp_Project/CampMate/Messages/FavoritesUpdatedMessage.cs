using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Messages
{
    class FavoritesUpdatedMessage : ValueChangedMessage<bool>
    {
        public FavoritesUpdatedMessage(bool updated) : base(updated)
        {
        }
    }
}

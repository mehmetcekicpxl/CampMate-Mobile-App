using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampMate.Models;

namespace CampMate.Messages
{
    public class FilterCriteriaMessage : ValueChangedMessage<FilterOptions>
    {
        public FilterCriteriaMessage(FilterOptions value) : base(value)
        {
        }
    }
}

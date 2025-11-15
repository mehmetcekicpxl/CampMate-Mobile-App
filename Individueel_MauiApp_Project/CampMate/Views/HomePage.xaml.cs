using System.Collections.Generic;
using Microsoft.Maui.Controls;
using CampMate.Models;

namespace CampMate.Views
{
    public partial class HomePage : ContentPage
    {
        public List<CampSite> CampList { get; set; }

        public HomePage()
        {
            InitializeComponent();

            // 
            CampList = new List<CampSite>
            {
                new CampSite { Name="Camp A", Location="Antalya", Stars=4, Facilities="Elektrisch, zwembad", Price=50, Image="kampa.jpg" },
                new CampSite { Name="Camp B", Location="Bodrum", Stars=3, Facilities="Elektrisch", Price=30, Image="kampb.jpg" },
                new CampSite { Name="Camp C", Location="Istanbul", Stars=4, Facilities="Elektrisch, cafe,fietsverhuur", Price=50, Image="bg.jpg" },
                new CampSite { Name="Camp D", Location="Hatay", Stars=3, Facilities="Elektrisch", Price=30, Image="dotnet_bot.png" }
            };

            CampListView.ItemsSource = CampList;
        }

        private void FilterButton_Clicked(object sender, System.EventArgs e)
        {
            
        }
    }

   
}

using CampMate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampMate.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CampMateContext context)
        {

            // context.Database.EnsureCreated();
            context.Database.Migrate();


            if (context.CampSites.Any())
            {
                return; 
            }


            // Voeg Users toe
            // We voegen de wachtwoorden voorlopig toe als platte tekst zonder ze te hashlen (Password="123").
            var admin = new User
            {
                 Name="Admin", Email="mehmet@campmate.com", Password="123", PhoneNumber="+32 666 666 666", Location="Hasselt" , IsAdmin=true
            };
            context.Users.Add(admin);
            context.SaveChanges();



            // Voeg CampSites toe
            var camps = new CampSite[]
            {
                new CampSite
                {
                    Name="City Camp Valkenburg",
                    Location="Valkenburg, Nederland",
                    Facilities="zwembad,fietsverhuur,Elektrisch",
                    Price=50,
                    Image="kampa.jpg",
                    UserId=admin.Id,
                    WebsiteUrl="https://www.camping-valkenburg.com/"

                },
                new CampSite 
                { 
                    Name="Camping Jungfrau", Location="Lauterbrunnen, Zwitserland", Facilities="Elektrisch", Price=30, Image="kampb.jpg" ,UserId=admin.Id,WebsiteUrl="https://www.campingjungfrau.swiss"
                },
                new CampSite { Name="Camping Village Roma Capitol", Location="Rome, Italië", Facilities="cafe,fietsverhuur", Price=50, Image="bg.jpg" ,UserId=admin.Id,WebsiteUrl="https://www.campingcapitol.com/en/"},
                new CampSite { Name="Camping De La Seine", Location="Jumièges, Frankrijk", Facilities="Elektrisch", Price=30, Image="kampa.jpg" ,UserId=admin.Id,WebsiteUrl="https://www.campingdelaseine.com/"}
            };
            context.CampSites.AddRange(camps);
            context.SaveChanges();

         

            // Voeg Comments toe
            var comments = new Comment[]
            {
                new Comment { UserName="Mehmet", Text="Geweldige camping!", CampSiteId=camps[0].Id ,UserId=admin.Id },
                new Comment { UserName="Mehmet", Text="Heel mooi terrein.", CampSiteId=camps[1].Id ,UserId=admin.Id }
            };
            context.Comments.AddRange(comments);
            context.SaveChanges();
        }
    }
}
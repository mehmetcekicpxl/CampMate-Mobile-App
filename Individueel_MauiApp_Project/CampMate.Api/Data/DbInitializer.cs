using CampMate.Api.Models;

namespace CampMate.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CampMateContext context)
        {
            
            context.Database.EnsureCreated();

           
            if (context.CampSites.Any())
            {
                return; 
            }

            

            // Voeg CampSites toe
            var camps = new CampSite[]
            {
                new CampSite { Name="City Camp Valkenburg", Location="Valkenburg, Nederland", Facilities="zwembad,fietsverhuur,Elektrisch", Price=50, Image="kampa.jpg" },
                new CampSite { Name="Camping Jungfrau", Location="Lauterbrunnen, Zwitserland", Facilities="Elektrisch", Price=30, Image="kampb.jpg" },
                new CampSite { Name="Camping Village Roma Capitol", Location="Rome, Italië", Facilities="cafe,fietsverhuur", Price=50, Image="bg.jpg" },
                new CampSite { Name="Camping De La Seine", Location="Jumièges, Frankrijk", Facilities="Elektrisch", Price=30, Image="kampa.jpg" }
            };
            context.CampSites.AddRange(camps);
            context.SaveChanges();

            // Voeg Users toe
            // We voegen de wachtwoorden voorlopig toe als platte tekst zonder ze te hashlen (Password="123").
            var users = new User[]
            {
                new User { Name="Mehmet", Email="mehmet@campmate.com", Password="123", PhoneNumber="+32 666 666 666", Location="Hasselt" }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // Voeg Comments toe
            var comments = new Comment[]
            {
                new Comment { UserName="Mehmet", Text="Geweldige camping!", CampSiteId=camps[0].Id  },
                new Comment { UserName="Ahmet", Text="Heel mooi terrein.", CampSiteId=camps[1].Id  }
            };
            context.Comments.AddRange(comments);
            context.SaveChanges();
        }
    }
}
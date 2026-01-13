using CampMate.Api.Data;
using CampMate.Shared.DTOs;
using CampMate.Api.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampMate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampsitesController : ControllerBase
    {
        private readonly CampMateContext _context;

        public CampsitesController(CampMateContext context)
        {
            _context = context;
        }

        // GET: api/campsites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampSiteDto>>> GetCampsites()
        {
            
            var camps = await _context.CampSites
                .Include(c => c.Comments)
                .Include(c => c.User)
                .ToListAsync();

            // Entiteit -> DTO omzetten
            var dtos = camps.Select(c => new CampSiteDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Name = c.Name,
                Location = c.Location,
                Facilities = c.Facilities,
                Price = c.Price,
                Image = c.Image,
                TotalStars = c.TotalStars,
                RatingCount = c.RatingCount,
                WebsiteUrl = c.WebsiteUrl,
                UserName = c.User != null ? c.User.Name : "Onbekend",


                Comments = c.Comments.Select(cm => new CommentDto
                {
                    Id = cm.Id,
                    UserName = cm.UserName,
                    Text = cm.Text,
                    CampSiteId = cm.CampSiteId,
                    UserId = cm.UserId
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }
        // GET: api/campsites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CampSiteDto>> GetCampSite(int id)
        {
            
            var camp = await _context.CampSites
                .Include(c => c.Comments)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (camp == null)
            {
                return NotFound();
            }

            // We zetten het databasemodel om naar DTO  (Frontend wacht daarop)
            var campDto = new CampSiteDto
            {
                Id = camp.Id,
                UserId = camp.UserId,
                Name = camp.Name,
                Location = camp.Location,
                Facilities = camp.Facilities,
                Price = camp.Price,
                Image = camp.Image,
                TotalStars = camp.TotalStars,
                RatingCount = camp.RatingCount,
                WebsiteUrl = camp.WebsiteUrl,
                UserName = camp.User != null ? camp.User.Name : "Onbekend",

                // We zetten de opmerkingen om in een lijst.
                Comments = camp.Comments.Select(cm => new CommentDto
                {
                    Id = cm.Id,
                    UserName = cm.UserName,
                    Text = cm.Text,
                    CampSiteId = cm.CampSiteId,
                    UserId = cm.UserId
                }).ToList()
            };

            return Ok(campDto);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto commentDto)
        {
            var camp = await _context.CampSites
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(c => c.Id == commentDto.CampSiteId);

            if (camp == null) return NotFound("Camping niet gevonden.");

           
            string fixedText = commentDto.Text;

            if (string.IsNullOrWhiteSpace(fixedText))
            {
                if (commentDto.Rating > 0)
                {
                    
                    fixedText = $"{commentDto.Rating} Ster gegeven ★";
                }
                else
                {
                    
                    return BadRequest("Schrijf een beoordeling of geef een cijfer.");
                }
            }

            var newComment = new Comment
            {
                UserName = commentDto.UserName,
                Text = fixedText, 
                UserId = commentDto.UserId,
                CampSiteId = commentDto.CampSiteId
            };

            camp.Comments.Add(newComment);

            
            if (commentDto.Rating > 0)
            {
                camp.TotalStars += commentDto.Rating;
                camp.RatingCount++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "De actie is geslaagd!" });
        }

       
        [HttpPost]
        public async Task<ActionResult<CampSiteDto>> CreateCampSite(CampSiteDto campDto)
        {
           
            if (string.IsNullOrEmpty(campDto.Image))
            {
                campDto.Image = "campmatelogo.png";
            }

            // 1. Van DTO naar databasemodel converteren
            var campSite = new CampSite
            {
                Name = campDto.Name,
                Location = campDto.Location,
                Facilities = campDto.Facilities,
                Price = (int)campDto.Price,

               
                Image = campDto.Image,

                UserId = campDto.UserId,
                WebsiteUrl = campDto.WebsiteUrl,



                TotalStars = 0,
                RatingCount = 0,
                IsFavorite = false
            };

            // 2. Toevoegen aan database
            _context.CampSites.Add(campSite);
            await _context.SaveChangesAsync(); 

            
            campDto.Id = campSite.Id;

            return CreatedAtAction("GetCampsites", new { id = campSite.Id }, campDto);
        }


        // 1. KAMP VERWIJDEREN ENDPOINT
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampSite(int id)
        {
            var camp = await _context.CampSites.FindAsync(id);
            if (camp == null) return NotFound("Kamp niet gevonden.");

            _context.CampSites.Remove(camp);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Het kamp is verwijderd." });
        }

        // 2. COMMENTAAR VERWIJDEREN ENDPOINT 
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
          

            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null) return NotFound("Er zijn geen reacties gevonden.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "De reactie is verwijderd." });
        }
    }
}
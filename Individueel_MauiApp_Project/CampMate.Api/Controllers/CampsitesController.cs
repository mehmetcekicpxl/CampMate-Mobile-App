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

        // GET: api/campsites?userId=5
        // Haal alle campings op en controleer of ze favoriet zijn voor de opgegeven gebruiker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampSiteDto>>> GetCampsites([FromQuery] int? userId)
        {
            // 1. Haal alle campings op inclusief relaties
            var camps = await _context.CampSites
                .Include(c => c.Comments)
                .Include(c => c.User)
                .ToListAsync();

            // 2. Haal de favorieten op als er een userId is meegegeven
            int currentUserId = userId ?? 0;
            var userFavoriteIds = new List<int>();

            if (currentUserId > 0)
            {
                userFavoriteIds = await _context.Favorites
                    .Where(f => f.UserId == currentUserId)
                    .Select(f => f.CampSiteId)
                    .ToListAsync();
            }

            // 3. Zet om naar DTO's
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

                // Controleer of dit kamp in de favorietenlijst staat
                IsFavorite = userFavoriteIds.Contains(c.Id),

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

        // GET: api/campsites/5?userId=1
        [HttpGet("{id}")]
        public async Task<ActionResult<CampSiteDto>> GetCampSite(int id, [FromQuery] int? userId)
        {
            var camp = await _context.CampSites
                .Include(c => c.Comments)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (camp == null) return NotFound();

            int currentUserId = userId ?? 0;
            bool isFavorite = false;

            // Controleer favoriet status
            if (currentUserId > 0)
            {
                isFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == currentUserId && f.CampSiteId == id);
            }

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
                IsFavorite = isFavorite,
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

        // POST: api/campsites/toggle-favorite/5?userId=1
        // Eenvoudige favoriet toggle zonder ingewikkelde tokens
        [HttpPost("toggle-favorite/{campId}")]
        public async Task<IActionResult> ToggleFavorite(int campId, [FromQuery] int userId)
        {
            if (userId <= 0) return BadRequest("Ongeldige gebruiker ID.");

            var existingFav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CampSiteId == campId);

            if (existingFav != null)
            {
                // Verwijderen uit favorieten
                _context.Favorites.Remove(existingFav);
                await _context.SaveChangesAsync();
                return Ok(new { IsFavorite = false });
            }
            else
            {
                // Toevoegen aan favorieten
                var newFav = new Favorite { UserId = userId, CampSiteId = campId };
                _context.Favorites.Add(newFav);
                await _context.SaveChangesAsync();
                return Ok(new { IsFavorite = true });
            }
        }

        

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto commentDto)
        {
            var camp = await _context.CampSites.Include(c => c.Comments).FirstOrDefaultAsync(c => c.Id == commentDto.CampSiteId);
            if (camp == null) return NotFound();

            string fixedText = string.IsNullOrWhiteSpace(commentDto.Text) ? $"{commentDto.Rating} Ster ★" : commentDto.Text;

            camp.Comments.Add(new Comment
            {
                UserName = commentDto.UserName,
                Text = fixedText,
                UserId = commentDto.UserId,
                CampSiteId = commentDto.CampSiteId
            });

            if (commentDto.Rating > 0) { camp.TotalStars += commentDto.Rating; camp.RatingCount++; }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Succes" });
        }

        [HttpPost]
        public async Task<ActionResult<CampSiteDto>> CreateCampSite(CampSiteDto campDto)
        {
            var campSite = new CampSite
            {
                Name = campDto.Name,
                Location = campDto.Location,
                Facilities = campDto.Facilities,
                Price = (int)campDto.Price,
                Image = string.IsNullOrEmpty(campDto.Image) ? "campmatelogo.png" : campDto.Image,
                UserId = campDto.UserId,
                WebsiteUrl = campDto.WebsiteUrl
            };
            _context.CampSites.Add(campSite);
            await _context.SaveChangesAsync();
            campDto.Id = campSite.Id;
            return CreatedAtAction("GetCampsites", new { id = campSite.Id }, campDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampSite(int id)
        {
            var camp = await _context.CampSites.FindAsync(id);
            if (camp == null) return NotFound();
            _context.CampSites.Remove(camp);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return NotFound();
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
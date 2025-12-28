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
                .ToListAsync();

            // Entiteit -> DTO omzetten
            var dtos = camps.Select(c => new CampSiteDto
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                Facilities = c.Facilities,
                Price = c.Price,
                Image = c.Image,
                TotalStars = c.TotalStars,
                RatingCount = c.RatingCount,
                AverageStars = c.RatingCount > 0 ? c.TotalStars / c.RatingCount : 0,
                Comments = c.Comments.Select(cm => new CommentDto
                {
                    Id = cm.Id,
                    UserName = cm.UserName,
                    Text = cm.Text
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }
    }
}
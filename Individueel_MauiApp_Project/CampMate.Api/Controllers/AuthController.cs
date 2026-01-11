using CampMate.Api.Data;
using CampMate.Api.Models;
using CampMate.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampMate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CampMateContext _context;

        public AuthController(CampMateContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto request)
        {
           
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Dit e-mailadres wordt al gebruikt.");
            }

          
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password, 
                PhoneNumber = request.PhoneNumber,
                Location = request.Location
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. UserDto retourneren
            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = "dummy-token",
                PhoneNumber = user.PhoneNumber,
                Location = user.Location
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto request)
        {
            // Zoek de gebruiker
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("E-mailadres of wachtwoord onjuist.");
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = "dummy-token",
                PhoneNumber = user.PhoneNumber,
                Location = user.Location,
                ProfileImage = user.ProfileImage,
                IsAdmin = user.IsAdmin
            });
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
        {
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden."); 
            }

            // Is het oude wachtwoord correct?
            if (user.Password != request.OldPassword)
            {
                return BadRequest("Het huidige wachtwoord is onjuist.");
            }

            // Wachtwoord bijwerken
            user.Password = request.NewPassword;
            await _context.SaveChangesAsync();

            return Ok("Wachtwoord succesvol gewijzigd.");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserUpdateDto updateDto)
        {
            // 1. Zoek de gebruiker in de database via ID
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden.");
            }

            // 2. Controleer of het nieuwe e-mailadres al in gebruik is door iemand anders
            
            if (user.Email != updateDto.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == updateDto.Email);
                if (emailExists)
                {
                    return BadRequest("Dit e-mailadres is al in gebruik.");
                }
            }

            // 3. Update de gegevens
            user.Name = updateDto.Name;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.Location = updateDto.Location;

            if (!string.IsNullOrEmpty(updateDto.ProfileImage))
            {
                user.ProfileImage = updateDto.ProfileImage;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 4. Wijzigingen opslaan in de database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 5. Stuur de bijgewerkte gegevens terug (Handig voor de app)
            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Location = user.Location,
                ProfileImage = user.ProfileImage,
                Token = "Token-blijft-hetzelfde"
            });
        }
    }
}
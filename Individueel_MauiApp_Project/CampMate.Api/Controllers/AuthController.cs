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
            // Controleer of het e-mailadres al bestaat
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Dit e-mailadres wordt al gebruikt.");
            }

            // Maak een nieuwe gebruiker aan
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

            // Retourneer UserDto
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
                Token = "dummy-token", // Hier komt later een echte JWT
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
            if (updateDto == null) return BadRequest("Er zijn geen gegevens ontvangen.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Gebruiker niet gevonden.");

            
            if (!string.IsNullOrEmpty(updateDto.Email) && user.Email != updateDto.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == updateDto.Email))
                    return BadRequest("Dit e-mailadres is al in gebruik.");

                user.Email = updateDto.Email;
            }

            
            if (!string.IsNullOrEmpty(updateDto.Name)) user.Name = updateDto.Name;

            
            user.PhoneNumber = updateDto.PhoneNumber ?? "";
            user.Location = updateDto.Location ?? "";

            
            if (!string.IsNullOrEmpty(updateDto.ProfileImage))
            {
                user.ProfileImage = updateDto.ProfileImage;
            }

           
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine("DB fout: " + errorMessage);
                return StatusCode(500, "server(DB) fout: " + errorMessage);
            }

            
            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Location = user.Location,
                ProfileImage = user.ProfileImage,
                Token = "Token-blijft-hetzelfde",
                IsAdmin = user.IsAdmin
            });
        }
    }
}
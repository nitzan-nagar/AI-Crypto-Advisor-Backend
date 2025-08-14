using AI.CryptoAdvisor.Api.Data;
using AI.CryptoAdvisor.Api.Models.Enums;
using AI.CryptoAdvisor.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AI.CryptoAdvisor.Api.Dtos;

namespace AI.CryptoAdvisor.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/me/preferences")]
    public class PreferencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PreferencesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetPreferences()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var prefs = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (prefs == null) return NotFound();
            return Ok(prefs);
        }

        [HttpPost]
        public async Task<IActionResult> SetPreferences([FromBody] OnboardingDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var prefs = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (prefs == null)
            {
                prefs = new UserPreferences
                {
                    UserId = userId,
                    User = await _context.Users.FindAsync(userId)
                };
                _context.UserPreferences.Add(prefs);
            }
            Enum.TryParse<InvestorType>(dto.InvestorType, true, out var investorType);
            prefs.PreferredAssets.AddRange(dto.PreferredAssets.Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrEmpty(a)));
            prefs.ContentTypes = dto.ContentTypes.Cast<string>().ToList();
            prefs.InvestorType = investorType;

            await _context.SaveChangesAsync();

            return Ok(prefs);
        }
    }
}

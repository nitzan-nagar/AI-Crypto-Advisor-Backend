using AI.CryptoAdvisor.Api.Data;
using AI.CryptoAdvisor.Api.Dtos;
using AI.CryptoAdvisor.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.CryptoAdvisor.Api.Controllers
{
    [ApiController]
    [Route("api/votes")]
    [Authorize]
    public class VoteController : Controller
    {
        private readonly AppDbContext _context;

        public VoteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVote([FromBody] VoteDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var vote = new UserVote
            {
                UserId = userId,
                User = await _context.Users.FindAsync(userId),
                CardType = request.CardType,
                Vote = request.Vote
            };

            _context.UserVotes.Add(vote);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vote saved successfully!" });
        }
    }
}


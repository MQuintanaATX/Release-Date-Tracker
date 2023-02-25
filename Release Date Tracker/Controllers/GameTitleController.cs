using Microsoft.AspNetCore.Mvc;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameTitleController : Controller
    {
        private readonly IGameTitleManager _gameTitleManager;

        public GameTitleController(IGameTitleManager gameTitleManager)
        {
            _gameTitleManager = gameTitleManager;
        }

        [HttpGet(("getAllTitles"))]
        public async Task<List<GameTitle>> GetAllTitlesAsync()
        {
            return await _gameTitleManager.GetAllGames();
        }
    }
}

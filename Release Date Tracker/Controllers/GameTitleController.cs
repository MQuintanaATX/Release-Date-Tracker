using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GameTitleController : Controller
{
    private readonly IIgdbManager _igdbManager;

    public GameTitleController(IIgdbManager igdbManager)
    {
        _igdbManager = igdbManager;
    }

    [HttpGet(("getAllTitles"))]
    public async Task<List<GameTitle>> GetAllTitlesAsync()
    {
        var gameTitlesDictionary = await _igdbManager.GetGameAllTitlesAsync();
        return gameTitlesDictionary.Titles.Values.ToList();
    }
}
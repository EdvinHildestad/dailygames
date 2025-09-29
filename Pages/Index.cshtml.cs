using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DailyGames.Models;
using DailyGames.Services;

namespace DailyGames.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IProfileService _profileService;
    private readonly IGameService _gameService;

    public IndexModel(ILogger<IndexModel> logger, IProfileService profileService, IGameService gameService)
    {
        _logger = logger;
        _profileService = profileService;
        _gameService = gameService;
    }

    public List<Profile> Profiles { get; set; } = new();
    public List<GameInfo> Games { get; set; } = new();
    public Profile? SelectedProfile { get; set; }
    public Dictionary<GameType, GameScore?> TodayScores { get; set; } = new();

    [BindProperty]
    public int? SelectedProfileId { get; set; }

    public async Task OnGetAsync(int? profileId = null)
    {
        Profiles = await _profileService.GetAllProfilesAsync();
        Games = _gameService.GetAllGames();
        
        if (profileId.HasValue)
        {
            SelectedProfile = await _profileService.GetProfileByIdAsync(profileId.Value);
            if (SelectedProfile != null)
            {
                SelectedProfileId = profileId.Value;
                await LoadTodayScores();
            }
        }
        else if (Profiles.Any())
        {
            SelectedProfile = Profiles.First();
            SelectedProfileId = SelectedProfile.Id;
            await LoadTodayScores();
        }
    }

    public IActionResult OnPostSelectProfile()
    {
        if (SelectedProfileId.HasValue)
        {
            return RedirectToPage("Index", new { profileId = SelectedProfileId.Value });
        }
        return RedirectToPage("Index");
    }

    private async Task LoadTodayScores()
    {
        if (SelectedProfile == null) return;

        foreach (var game in Games)
        {
            var todayScore = await _gameService.GetTodayScoreAsync(SelectedProfile.Id, game.GameType);
            TodayScores[game.GameType] = todayScore;
        }
    }
}

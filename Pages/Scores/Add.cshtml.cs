using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DailyGames.Models;
using DailyGames.Services;

namespace DailyGames.Pages.Scores;

public class AddModel : PageModel
{
    private readonly IProfileService _profileService;
    private readonly IGameService _gameService;

    public AddModel(IProfileService profileService, IGameService gameService)
    {
        _profileService = profileService;
        _gameService = gameService;
    }

    [BindProperty]
    public int ProfileId { get; set; }

    [BindProperty]
    public GameType GameType { get; set; }

    [BindProperty]
    public int? Tries { get; set; }

    [BindProperty]
    public bool IsCompleted { get; set; }

    [BindProperty]
    public DateTime GameDate { get; set; } = DateTime.UtcNow.Date;

    public Profile? Profile { get; set; }
    public GameInfo? Game { get; set; }
    public List<Profile> Profiles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? profileId = null, GameType? gameType = null)
    {
        Profiles = await _profileService.GetAllProfilesAsync();
        
        if (profileId.HasValue)
        {
            ProfileId = profileId.Value;
            Profile = await _profileService.GetProfileByIdAsync(profileId.Value);
        }

        if (gameType.HasValue)
        {
            GameType = gameType.Value;
            Game = _gameService.GetAllGames().FirstOrDefault(g => g.GameType == gameType.Value);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Profile = await _profileService.GetProfileByIdAsync(ProfileId);
        if (Profile == null)
        {
            ModelState.AddModelError("", "Invalid profile selected.");
            Profiles = await _profileService.GetAllProfilesAsync();
            return Page();
        }

        if (IsCompleted && (!Tries.HasValue || Tries.Value < 1 || Tries.Value > 20))
        {
            ModelState.AddModelError(nameof(Tries), "Number of tries must be between 1 and 20 when completed.");
            Game = _gameService.GetAllGames().FirstOrDefault(g => g.GameType == GameType);
            Profiles = await _profileService.GetAllProfilesAsync();
            return Page();
        }

        // Check if score already exists for this date
        var existingScore = await _gameService.GetTodayScoreAsync(ProfileId, GameType);
        if (existingScore != null && existingScore.GameDate.Date == GameDate.Date)
        {
            ModelState.AddModelError("", "A score for this game and date already exists.");
            Game = _gameService.GetAllGames().FirstOrDefault(g => g.GameType == GameType);
            Profiles = await _profileService.GetAllProfilesAsync();
            return Page();
        }

        await _gameService.AddScoreAsync(ProfileId, GameType, IsCompleted ? Tries : null, IsCompleted, GameDate);
        return RedirectToPage("/Index", new { profileId = ProfileId });
    }
}
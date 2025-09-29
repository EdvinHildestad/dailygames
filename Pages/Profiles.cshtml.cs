using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DailyGames.Models;
using DailyGames.Services;

namespace DailyGames.Pages;

public class ProfilesModel : PageModel
{
    private readonly IProfileService _profileService;

    public ProfilesModel(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public List<Profile> Profiles { get; set; } = new();

    [BindProperty]
    public string NewProfileName { get; set; } = string.Empty;

    [BindProperty]
    public string? NewProfileAvatar { get; set; }

    public async Task OnGetAsync()
    {
        Profiles = await _profileService.GetAllProfilesAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProfileName))
        {
            ModelState.AddModelError(nameof(NewProfileName), "Profile name is required.");
            Profiles = await _profileService.GetAllProfilesAsync();
            return Page();
        }

        await _profileService.CreateProfileAsync(NewProfileName.Trim(), NewProfileAvatar);
        return RedirectToPage("Profiles");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _profileService.DeleteProfileAsync(id);
        return RedirectToPage("Profiles");
    }
}
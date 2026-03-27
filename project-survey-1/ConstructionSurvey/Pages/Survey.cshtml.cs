using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConstructionSurvey.Models;
using ConstructionSurvey.Services;

namespace ConstructionSurvey.Pages;

public class SurveyModel : PageModel
{
    private readonly SurveyDataService _dataService;

    public List<SurveyQuestion> Questions { get; set; } = new();

    [BindProperty]
    public string WorkerName { get; set; } = string.Empty;

    [BindProperty]
    public string Company { get; set; } = string.Empty;

    [BindProperty]
    public string Trade { get; set; } = string.Empty;

    public string AccessTime { get; set; } = string.Empty;

    public SurveyModel(SurveyDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(WorkerName) || string.IsNullOrWhiteSpace(Company) || string.IsNullOrWhiteSpace(Trade))
        {
            return RedirectToPage("/Index");
        }

        AccessTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Questions = _dataService.GetQuestions();
        return Page();
    }
}

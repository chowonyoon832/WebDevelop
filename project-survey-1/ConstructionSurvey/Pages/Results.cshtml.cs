using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConstructionSurvey.Models;
using ConstructionSurvey.Services;

namespace ConstructionSurvey.Pages;

public class ResultsModel : PageModel
{
    private readonly JsonResultService _jsonService;

    public List<SurveySubmission> Submissions { get; set; } = new();
    public int TotalCount { get; set; }
    public int GreenCount { get; set; }
    public int YellowCount { get; set; }
    public int RedCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    public ResultsModel(JsonResultService jsonService)
    {
        _jsonService = jsonService;
    }

    public void OnGet()
    {
        var allSubmissions = _jsonService.GetAllSubmissions();

        // 통계는 전체 데이터 기준
        TotalCount = allSubmissions.Count;
        GreenCount = allSubmissions.Count(s => s.RiskLevel == "양호");
        YellowCount = allSubmissions.Count(s => s.RiskLevel == "주의");
        RedCount = allSubmissions.Count(s => s.RiskLevel == "위험");

        // 필터 적용
        if (!string.IsNullOrEmpty(Filter))
        {
            Submissions = allSubmissions.Where(s => s.RiskLevel == Filter).ToList();
        }
        else
        {
            Submissions = allSubmissions;
        }
    }
}

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

    /// <summary>해당 날짜에 제출된 데이터만 표시</summary>
    public List<string> AvailableDates { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Date { get; set; }

    public ResultsModel(JsonResultService jsonService)
    {
        _jsonService = jsonService;
    }

    public void OnGet()
    {
        var allSubmissions = _jsonService.GetAllSubmissions();

        // 날짜 목록 추출 (SubmitTime 형식: "2026-03-27 19:17:51")
        AvailableDates = allSubmissions
            .Select(s => s.SubmitTime?.Split(' ').FirstOrDefault() ?? "")
            .Where(d => !string.IsNullOrEmpty(d))
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        // 날짜 필터 적용
        var dateFiltered = allSubmissions;
        if (!string.IsNullOrEmpty(Date))
        {
            dateFiltered = allSubmissions
                .Where(s => s.SubmitTime != null && s.SubmitTime.StartsWith(Date))
                .ToList();
        }

        // 통계는 날짜 필터 기준
        TotalCount = dateFiltered.Count;
        GreenCount = dateFiltered.Count(s => s.RiskLevel == "양호");
        YellowCount = dateFiltered.Count(s => s.RiskLevel == "주의");
        RedCount = dateFiltered.Count(s => s.RiskLevel == "위험");

        // 위험등급 필터 적용
        if (!string.IsNullOrEmpty(Filter))
        {
            Submissions = dateFiltered.Where(s => s.RiskLevel == Filter).ToList();
        }
        else
        {
            Submissions = dateFiltered;
        }
    }
}

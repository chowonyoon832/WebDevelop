using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConstructionSurvey.Models;
using ConstructionSurvey.Services;

namespace ConstructionSurvey.Pages;

public class ResultModel : PageModel
{
    private readonly ScoringService _scoringService;
    private readonly SurveyDataService _dataService;
    private readonly JsonResultService _jsonService;

    public SurveyResult Result { get; set; } = new();
    public string WorkerName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Trade { get; set; } = string.Empty;
    public string AccessTime { get; set; } = string.Empty;
    public string SubmitTime { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;

    public ResultModel(ScoringService scoringService, SurveyDataService dataService, JsonResultService jsonService)
    {
        _scoringService = scoringService;
        _dataService = dataService;
        _jsonService = jsonService;
    }

    public IActionResult OnGet(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/Index");

        var submission = _jsonService.GetSubmission(id);
        if (submission == null)
            return RedirectToPage("/Index");

        WorkerName = submission.Name;
        Company = submission.Company;
        Trade = submission.Trade;
        AccessTime = submission.AccessTime;
        SubmitTime = submission.SubmitTime;
        Duration = submission.Duration;
        Result = _scoringService.Calculate(submission.Answers);

        return Page();
    }

    public IActionResult OnPost()
    {
        // Read personal info
        WorkerName = Request.Form["WorkerName"].ToString();
        Company = Request.Form["Company"].ToString();
        Trade = Request.Form["Trade"].ToString();
        AccessTime = Request.Form["AccessTime"].ToString();

        // Record submit time
        var submitDateTime = DateTime.Now;
        SubmitTime = submitDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Calculate duration
        if (DateTime.TryParse(AccessTime, out var accessDateTime))
        {
            var duration = submitDateTime - accessDateTime;
            var totalMinutes = (int)duration.TotalMinutes;
            var seconds = duration.Seconds;
            Duration = totalMinutes > 0 ? $"{totalMinutes}분 {seconds}초" : $"{seconds}초";
        }

        // Read answers
        var questions = _dataService.GetQuestions();
        var answers = new Dictionary<int, int>();

        foreach (var q in questions)
        {
            var key = $"answer_{q.Number}";
            if (Request.Form.TryGetValue(key, out var value) && int.TryParse(value, out int answer))
            {
                answers[q.Number] = answer;
            }
        }

        if (answers.Count != questions.Count)
        {
            return RedirectToPage("/Index");
        }

        Result = _scoringService.Calculate(answers);

        // Risk level text
        var riskLevelText = Result.RiskLevel switch
        {
            RiskLevel.Green => "양호",
            RiskLevel.Yellow => "주의",
            _ => "위험"
        };

        // Critical flags text
        var flagTexts = Result.CriticalFlags
            .Select(f => $"Q{f.QuestionNumber}: {f.Reason}")
            .ToList();
        var criticalFlagsText = flagTexts.Any() ? string.Join(" / ", flagTexts) : "없음";

        // Save to JSON
        var submission = new SurveySubmission
        {
            Name = WorkerName,
            Company = Company,
            Trade = Trade,
            AccessTime = AccessTime,
            SubmitTime = SubmitTime,
            Duration = Duration,
            Answers = answers,
            TotalScore = Result.TotalScore,
            RiskLevel = riskLevelText,
            CriticalFlags = criticalFlagsText
        };

        var savedId = _jsonService.SaveSubmission(submission);
        if (savedId == null)
            return RedirectToPage("/Index");

        // PRG 패턴: POST 후 GET으로 리다이렉트하여 새로고침 시 중복 저장 방지
        return RedirectToPage("/Result", new { id = savedId });
    }
}

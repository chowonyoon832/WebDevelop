namespace ConstructionSurvey.Models;

public class SurveySubmission
{
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Trade { get; set; } = string.Empty;
    public string AccessTime { get; set; } = string.Empty;
    public string SubmitTime { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public Dictionary<int, int> Answers { get; set; } = new();
    public int TotalScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string CriticalFlags { get; set; } = string.Empty;
}

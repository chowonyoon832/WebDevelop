namespace ConstructionSurvey.Models;

public enum RiskLevel
{
    Green,  // 양호 (14~24)
    Yellow, // 주의 (25~37)
    Red     // 위험 (38~56)
}

public class CriticalFlag
{
    public int QuestionNumber { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class SurveyResult
{
    public int TotalScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public List<CriticalFlag> CriticalFlags { get; set; } = new();
}

namespace ConstructionSurvey.Models;

public class SurveyQuestion
{
    public int Number { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsReverseScored { get; set; }
    public bool IsAlcoholQuestion { get; set; }
}

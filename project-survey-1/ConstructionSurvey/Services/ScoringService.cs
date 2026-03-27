using ConstructionSurvey.Models;

namespace ConstructionSurvey.Services;

public class ScoringService
{
    private readonly SurveyDataService _dataService;

    public ScoringService(SurveyDataService dataService)
    {
        _dataService = dataService;
    }

    public SurveyResult Calculate(Dictionary<int, int> answers)
    {
        var questions = _dataService.GetQuestions();
        var result = new SurveyResult();
        int totalScore = 0;

        foreach (var question in questions)
        {
            if (!answers.TryGetValue(question.Number, out int rawAnswer))
                continue;

            int score = question.IsReverseScored ? (5 - rawAnswer) : rawAnswer;
            totalScore += score;

            // 위험 플래그 검출
            bool isCritical = false;
            string reason = string.Empty;

            if (!question.IsReverseScored && rawAnswer == 4)
            {
                isCritical = true;
                reason = "매우 그렇다(4점) 응답";
            }
            else if (question.IsReverseScored && rawAnswer == 1)
            {
                isCritical = true;
                reason = "전혀 아니다(1점) 응답 (역채점 문항)";
            }

            if (question.IsAlcoholQuestion && rawAnswer >= 3)
            {
                isCritical = true;
                reason = "음주 관련 문항 3점 이상 응답";
            }

            if (isCritical)
            {
                result.CriticalFlags.Add(new CriticalFlag
                {
                    QuestionNumber = question.Number,
                    QuestionText = question.Text,
                    Reason = reason
                });
            }
        }

        result.TotalScore = totalScore;
        result.RiskLevel = totalScore switch
        {
            <= 24 => RiskLevel.Green,
            <= 37 => RiskLevel.Yellow,
            _ => RiskLevel.Red
        };

        return result;
    }
}

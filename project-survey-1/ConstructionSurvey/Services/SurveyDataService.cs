using ConstructionSurvey.Models;

namespace ConstructionSurvey.Services;

public class SurveyDataService
{
    private static readonly List<SurveyQuestion> _questions = new()
    {
        new() { Number = 1, Category = "신체 피로", Text = "요즘 몸이 무겁고 피곤해서 작업하기 힘든 느낌이 있다", IsReverseScored = false },
        new() { Number = 2, Category = "수면의 질", Text = "최근 잠을 충분히 잤고, 개운하게 일어난 편이다", IsReverseScored = true },
        new() { Number = 3, Category = "신체 컨디션/통증", Text = "지금 몸에 아프거나 불편한 곳이 있어서 작업에 영향을 줄 것 같다", IsReverseScored = false },
        new() { Number = 4, Category = "정신적 피로/소진", Text = "요즘 머릿속이 복잡하거나, 정신적으로 지쳐 있는 느낌이 있다", IsReverseScored = false },
        new() { Number = 5, Category = "집중력/주의력", Text = "요즘 작업에 집중이 잘 안 되거나, 딴생각이 자주 나는 편이다", IsReverseScored = false },
        new() { Number = 6, Category = "음주", Text = "최근 음주로 인해 작업 중 집중력이나 컨디션이 저하된 적이 있다", IsReverseScored = false, IsAlcoholQuestion = true },
        new() { Number = 7, Category = "기분/정서(우울감)", Text = "요즘 일할 때 의욕이 떨어지거나, 기분이 가라앉는 느낌이 있다", IsReverseScored = false },
        new() { Number = 8, Category = "조직 체계", Text = "현장에서 힘들 때 도움을 주거나 이해해 주는 동료가 있다", IsReverseScored = true },
        new() { Number = 9, Category = "불안/걱정", Text = "요즘 작업 중이나 생활에서 걱정이나 불안한 마음이 자주 든다", IsReverseScored = false },
        new() { Number = 10, Category = "기분/정서(우울감)", Text = "최근 내가 하는 일에 대해 자신감이 예전보다 줄어든 느낌이 있다", IsReverseScored = false },
        new() { Number = 11, Category = "스트레스/감정조절", Text = "요즘 사소한 일에도 짜증이 나거나, 쉽게 화가 나는 편이다", IsReverseScored = false },
        new() { Number = 12, Category = "불안/걱정", Text = "최근 작업 중에 이유 없이 몸이 긴장되거나 심장이 빨리 뛰는 느낌이 있다", IsReverseScored = false },
        new() { Number = 13, Category = "스트레스/감정조절", Text = "요즘 감정을 다스리기 어렵거나, 작은 일에도 과하게 반응하는 편이다", IsReverseScored = false },
        new() { Number = 14, Category = "안전 자신감/작업 준비도", Text = "오늘 안전하게 작업할 수 있다는 자신감이 있고, 컨디션도 괜찮다", IsReverseScored = true },
    };

    public List<SurveyQuestion> GetQuestions() => _questions;
}

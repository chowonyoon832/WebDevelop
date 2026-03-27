using ClosedXML.Excel;
using ConstructionSurvey.Models;

namespace ConstructionSurvey.Services;

public class ExcelResultService
{
    private readonly string _resultsPath;
    private readonly ILogger<ExcelResultService> _logger;
    private readonly object _lock = new();

    public ExcelResultService(IWebHostEnvironment env, ILogger<ExcelResultService> logger)
    {
        _resultsPath = Path.Combine(env.ContentRootPath, "Results");
        _logger = logger;
        Directory.CreateDirectory(_resultsPath);
    }

    public bool SaveSubmission(SurveySubmission submission)
    {
        lock (_lock)
        {
            var filePath = Path.Combine(_resultsPath, "설문결과.xlsx");

            // 메인 파일 저장 시도, 실패 시 백업 파일로 저장
            if (TryWrite(filePath, submission))
                return true;

            // 메인 파일이 열려있는 경우 → 백업 파일에 저장
            var backupName = $"설문결과_{DateTime.Now:yyyyMMdd_HHmmss}_{submission.Name}.xlsx";
            var backupPath = Path.Combine(_resultsPath, backupName);
            _logger.LogWarning("Main file locked, saving to backup: {Path}", backupName);

            return TryWrite(backupPath, submission);
        }
    }

    private bool TryWrite(string filePath, SurveySubmission submission)
    {
        try
        {
            using var wb = File.Exists(filePath)
                ? new XLWorkbook(filePath)
                : new XLWorkbook();

            var ws = wb.Worksheets.FirstOrDefault() ?? wb.AddWorksheet("설문결과");

            if (ws.LastRowUsed() == null)
                WriteHeader(ws);

            var row = ws.LastRowUsed()!.RowNumber() + 1;
            var rowNum = row - 1;

            ws.Cell(row, 1).Value = rowNum;
            ws.Cell(row, 2).Value = submission.Name;
            ws.Cell(row, 3).Value = submission.Company;
            ws.Cell(row, 4).Value = submission.Trade;
            ws.Cell(row, 5).Value = submission.AccessTime;
            ws.Cell(row, 6).Value = submission.SubmitTime;
            ws.Cell(row, 7).Value = submission.Duration;

            for (int i = 1; i <= 14; i++)
            {
                ws.Cell(row, 7 + i).Value = submission.Answers.GetValueOrDefault(i, 0);
            }

            ws.Cell(row, 22).Value = submission.TotalScore;
            ws.Cell(row, 23).Value = submission.RiskLevel;
            ws.Cell(row, 24).Value = submission.CriticalFlags;

            // 위험등급별 행 배경색
            var rowRange = ws.Range(row, 1, row, 24);
            var bgColor = submission.RiskLevel switch
            {
                "위험" => XLColor.FromHtml("#fef2f2"),
                "주의" => XLColor.FromHtml("#fefce8"),
                _ => XLColor.FromHtml("#f0fdf4")
            };
            rowRange.Style.Fill.BackgroundColor = bgColor;

            ws.Columns().AdjustToContents();
            wb.SaveAs(filePath);

            _logger.LogInformation("Survey result saved for {Name} at {Path}", submission.Name, filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save to {Path}", filePath);
            return false;
        }
    }

    private void WriteHeader(IXLWorksheet ws)
    {
        string[] headers = [
            "번호", "이름", "소속업체", "공종",
            "접속시간", "제출시간", "소요시간",
            "Q1", "Q2", "Q3", "Q4", "Q5", "Q6", "Q7",
            "Q8", "Q9", "Q10", "Q11", "Q12", "Q13", "Q14",
            "총점", "위험등급", "위험 플래그"
        ];

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }

        var headerRange = ws.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e40af");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.SheetView.FreezeRows(1);
    }
}

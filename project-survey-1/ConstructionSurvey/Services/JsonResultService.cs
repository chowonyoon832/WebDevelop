using System.Text.Encodings.Web;
using System.Text.Json;
using ConstructionSurvey.Models;

namespace ConstructionSurvey.Services;

public class JsonResultService
{
    private readonly string _resultsPath;
    private readonly ILogger<JsonResultService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonResultService(IWebHostEnvironment env, ILogger<JsonResultService> logger)
    {
        _resultsPath = Path.Combine(env.ContentRootPath, "Results");
        _logger = logger;
        Directory.CreateDirectory(_resultsPath);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public string? SaveSubmission(SurveySubmission submission)
    {
        try
        {
            var safeName = SanitizeFileName(submission.Name);
            var guid = Guid.NewGuid().ToString("N")[..4];
            var fileName = $"{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}_{safeName}_{guid}.json";
            var filePath = Path.Combine(_resultsPath, fileName);

            var json = JsonSerializer.Serialize(submission, _jsonOptions);
            File.WriteAllText(filePath, json);

            _logger.LogInformation("Survey result saved: {FileName}", fileName);
            return Path.GetFileNameWithoutExtension(fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save survey result for {Name}", submission.Name);
            return null;
        }
    }

    public SurveySubmission? GetSubmission(string id)
    {
        try
        {
            var filePath = Path.Combine(_resultsPath, id + ".json");
            if (!File.Exists(filePath))
                return null;

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SurveySubmission>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read survey file: {Id}", id);
            return null;
        }
    }

    public List<SurveySubmission> GetAllSubmissions()
    {
        var submissions = new List<SurveySubmission>();

        if (!Directory.Exists(_resultsPath))
            return submissions;

        var files = Directory.GetFiles(_resultsPath, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var submission = JsonSerializer.Deserialize<SurveySubmission>(json, _jsonOptions);
                if (submission != null)
                    submissions.Add(submission);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read survey file: {File}", Path.GetFileName(file));
            }
        }

        // 제출시간 기준 내림차순 정렬
        submissions.Sort((a, b) => string.Compare(b.SubmitTime, a.SubmitTime, StringComparison.Ordinal));

        return submissions;
    }

    private static string SanitizeFileName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = name;
        foreach (var c in invalidChars)
        {
            sanitized = sanitized.Replace(c, '_');
        }
        return sanitized;
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace ConstructionSurvey.Pages;

public class QrCodeModel : PageModel
{
    private readonly IConfiguration _config;

    public string QrImageBase64 { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;

    public QrCodeModel(IConfiguration config)
    {
        _config = config;
    }

    public void OnGet()
    {
        TargetUrl = _config["SurveyUrl"] ?? $"{Request.Scheme}://{Request.Host}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(TargetUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);
        QrImageBase64 = Convert.ToBase64String(qrCodeBytes);
    }
}

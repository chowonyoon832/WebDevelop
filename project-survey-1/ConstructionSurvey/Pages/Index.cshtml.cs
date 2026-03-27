using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConstructionSurvey.Pages;

public class IndexModel : PageModel
{
    public string AccessTime { get; set; } = string.Empty;

    public void OnGet()
    {
        AccessTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

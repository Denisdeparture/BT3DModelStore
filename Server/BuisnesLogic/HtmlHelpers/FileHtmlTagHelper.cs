using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using RazorLight;
namespace Application.HtmlHelpers
{
    public static class FileHtmlTagHelper 
    {

       public static HtmlString ShowMarkingFromThisPath(this IHtmlHelper page, string pathfromhtmlfile, IWebHostEnvironment host, ILogger logger)
       {
            if (string.IsNullOrWhiteSpace(pathfromhtmlfile)) return new HtmlString(string.Empty);
            string path = host.WebRootPath + pathfromhtmlfile;
            string marking = string.Empty;
            try
            {
                marking = File.ReadAllText(path);
                if (string.IsNullOrEmpty(marking)) throw new Exception("marking was null");
            }
            catch(FileNotFoundException ex)
            {
                logger.LogError($"File in {ex.FileName} not found");
            }
            return new HtmlString(marking);
       }
    }
}

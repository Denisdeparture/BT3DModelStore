using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationInfrastructure;
namespace Application.HtmlHelpers
{
    public static class FileHtmlTagHelper 
    {

       public static HtmlString ShowMarkingFromThisPath(this IHtmlHelper page, string pathfromhtmlfile, IWebHostEnvironment host, ILogger<Program> logger)
       {
            
            if (string.IsNullOrWhiteSpace(pathfromhtmlfile)) return new HtmlString("");
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

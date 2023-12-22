using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FriendFace.Services;

public class FileLoader
{
    public static string LoadFile(ControllerContext controllerContext, string viewName, object model)
    {
        using (var sw = new StringWriter())
        {
            var engine = controllerContext.HttpContext.RequestServices.GetRequiredService<IRazorViewEngine>();
            var viewResult = engine.FindView(controllerContext, viewName, false);
        
            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(controllerContext, viewResult.View,
                viewDictionary,
                new TempDataDictionary(controllerContext.HttpContext, controllerContext.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>()),
                sw,
                new HtmlHelperOptions());

            var t = viewResult.View.RenderAsync(viewContext);

            t.Wait();

            return sw.GetStringBuilder().ToString();
        }
    }
}
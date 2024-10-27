using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Librarian;

public class RazorRenderer(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider
) : IRazorRenderer
{
    public async Task<string> RenderViewToStringAsync(
        string viewName,
        IDictionary<string, object?> viewBagValues
    )
    {
        await using var sw = new StringWriter();

        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = serviceProvider },
            new RouteData(),
            new ActionDescriptor()
        );

        var viewResult = viewEngine.FindView(actionContext, viewName, isMainPage: false);
        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"Could not find view '{viewName}'");
        }

        var viewDictionary = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary()
        );

        // Set ViewBag values from the dictionary
        foreach (var key in viewBagValues.Keys)
        {
            viewDictionary[key] = viewBagValues[key];
        }

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}

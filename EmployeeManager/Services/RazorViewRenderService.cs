using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Services
{
    public class RazorViewRenderService: IRenderViewService
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;
        private readonly IServiceProvider serviceProvider;

        public RazorViewRenderService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            this.razorViewEngine = razorViewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
        }

        public async Task<string> RazorViewToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using(var writer = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
                if(viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} is not found");
                }
                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model
                };
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    writer,
                    new HtmlHelperOptions());
                    await viewResult.View.RenderAsync(viewContext);
                return writer.ToString();
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VeriDoc_HR.Controllers;

namespace VeriDoc_HR
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalFilters.Filters.Add(new ValidateInputAttribute(false));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Do whatever you want to do with the error

            //Show the custom error page...
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "PageNotFound";

            if ((Context.Server.GetLastError() is HttpException) && ((Context.Server.GetLastError() as HttpException).GetHttpCode() != 404))
            {
                routeData.Values["action"] = "Index";
            }
            else
            {
                // Handle 404 error and response code
                Response.StatusCode = 404;
                routeData.Values["action"] = "Index";
            }
            Response.TrySkipIisCustomErrors = true; // If you are using IIS7, have this line
            IController errorsController = new PageNotFoundController();
            HttpContextWrapper wrapper = new HttpContextWrapper(Context);
            var rc = new System.Web.Routing.RequestContext(wrapper, routeData);
            errorsController.Execute(rc);

            Response.End();
        }
    }
}

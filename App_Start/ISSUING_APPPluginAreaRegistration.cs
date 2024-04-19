using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISSUING_APP.App_Start
{
    public class ISSUING_APPPluginAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "IssuingCard"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
              "IssuingCard_default",
              "IssuingCard/{controller}/{action}/{id}",
              new { controller = "MainIssuingCardController", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "ISSUING_APP.Controllers" }

          );
        }
    }
}
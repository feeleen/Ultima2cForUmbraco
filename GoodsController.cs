using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco;
using Examine;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Umbraco.Controllers
{
	public class GoodsController : PluginController
	{
		public GoodsController()
			: this(UmbracoContext.Current)
		{
		}

		public GoodsController(UmbracoContext umbracoContext)
			: base(umbracoContext)
		{
		}

        public ActionResult GetGood()
		{
			var goodNodeID = 1084;
			var content = Umbraco.TypedContent(goodNodeID);
			if (content == null)
				throw new System.Exception("Couldn't find any node with id = " + goodNodeID + ". Change goodNodeID value to correct ID of Goods page (see GetGood() method of App_Code/GoodsController.cs file)");

			var renderModel = CreateRenderModel(content);
			return View("Good", renderModel);
		}

		private RenderModel CreateRenderModel(IPublishedContent content)
		{
			var model = new RenderModel(content, CultureInfo.CurrentUICulture);

			//add an umbraco data token so the umbraco view engine executes
			RouteData.DataTokens["umbraco"] = model;

			return model;
		}

	}
}
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
	public class GoodPhotosController : PluginController
	{
		public GoodPhotosController()
			: this(UmbracoContext.Current)
		{
		}

		public GoodPhotosController(UmbracoContext umbracoContext)
			: base(umbracoContext)
		{
		}

        public ActionResult GetGoodPhoto()
		{
			var goodPhotoNodeID = 1124;
			var content = Umbraco.TypedContent(goodPhotoNodeID);
			if (content == null)
				throw new System.Exception("Couldn't find any node with id = " + goodPhotoNodeID + ". Change goodPhotoNodeID value to correct ID of GoodPhoto page (see GetGoodPhoto() method of App_Code/GoodPhotosController.cs file)");

			var renderModel = CreateRenderModel(content);
			return View("File", renderModel);
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
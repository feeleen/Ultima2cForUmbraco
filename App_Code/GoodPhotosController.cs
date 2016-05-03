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
using System.Web.Configuration;

namespace Ultima.PluginControllers
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
			var goodPhotoNodeID = WebConfigurationManager.AppSettings[InstallHelpers.GoodPhotoNodeIDKey];

			var content = Umbraco.TypedContent(goodPhotoNodeID);
			if (content == null)
				throw new System.Exception("Couldn't find any node with id = " + goodPhotoNodeID + ". Change app setting's " + InstallHelpers.GoodPhotoNodeIDKey + " value to correct ID of a GoodPhoto content page)");

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
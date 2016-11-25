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
	public class RouteController : PluginController
	{
		public RouteController()
			: this(UmbracoContext.Current)
		{
		}

		public RouteController(UmbracoContext umbracoContext)
			: base(umbracoContext)
		{
		}

        public ActionResult GetGood(int id = -1)
		{
			var goodNodeID = WebConfigurationManager.AppSettings[InstallHelpers.GoodNodeIDKey];

			var content = Umbraco.TypedContent(goodNodeID);
			if (content == null)
				throw new System.Exception("Couldn't find any node with id = " + goodNodeID + ". Change app setting's " + InstallHelpers.GoodNodeIDKey + " value to correct ID of a Good content page)");

			var renderModel = CreateRenderModel(content);
			ViewResult v = View("Good", renderModel);
			v.ViewData["GoodID"] = id;
			if (id > -1)
			{
				try
				{
					v.ViewData["good"] = UltimaWebService.GetProductInfo(id);
				}
				catch { }
			}
			return v;
		}
		
		public ActionResult GetDocument(int id = -1)
		{
			var cabinetNodeID = WebConfigurationManager.AppSettings[InstallHelpers.CabinetNodeID];

			var content = Umbraco.TypedContent(cabinetNodeID);
			//if (content == null)
		//		throw new System.Exception("Couldn't find any node with id = " + cabinetNodeID + ". Change app setting's " + InstallHelpers.CabinetNodeID + " value to correct ID of a Good content page)");

			var renderModel = CreateRenderModel(content);
			ViewResult v = View("Document", renderModel);
			//v.ViewData["GoodID"] = id;
			if (id > -1)
			{
				try
				{
					v.ViewData["good"] = "ddddd";//UltimaWebService.GetProductInfo(id);
				}
				catch { }
			}
			return v;
		}


		public ActionResult GetGoodPhoto(int id = -1)
		{
			var goodPhotoNodeID = WebConfigurationManager.AppSettings[InstallHelpers.GoodPhotoNodeIDKey];

			var content = Umbraco.TypedContent(goodPhotoNodeID);
			if (content == null)
				throw new System.Exception("Couldn't find any node with id = " + goodPhotoNodeID + ". Change app setting's " + InstallHelpers.GoodPhotoNodeIDKey + " value to correct ID of a GoodPhoto content page)");

			var renderModel = CreateRenderModel(content);
			return View("File", renderModel);
		}

		public ActionResult GetCategory(int id = -1)
		{
			var categoryNodeID = WebConfigurationManager.AppSettings[InstallHelpers.CategoryNodeIDKey];

			var content = Umbraco.TypedContent(categoryNodeID);
			if (content == null)
				throw new System.Exception("Couldn't find any node with id = " + categoryNodeID + ". Change app setting's " + InstallHelpers.CategoryNodeIDKey + " value to correct ID of a Category content page)");

			RouteData.DataTokens["CategoryID"] = id;
			var renderModel = CreateRenderModel(content);
			ViewResult v = View("Goods", renderModel);
			v.ViewData["CategoryID"] = id;
			if (id > -1)
			{
				try
				{
					v.ViewData["category"] = UltimaWebService.GetCategoryInfo(id);
				}
				catch { }
			}
			return v;
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
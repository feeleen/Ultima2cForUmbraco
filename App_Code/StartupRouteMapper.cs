using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace Umbraco
{
	public class StartupRouteMapper : IApplicationEventHandler
	{
		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			//Create a custom routes
			RouteTable.Routes.MapRoute(
				"",
				"cabinet/document/{id}",
				new
				{
					controller = "Route",
					action = "GetDocument",
					id = UrlParameter.Optional
				});

			RouteTable.Routes.MapRoute(
				"",
				"good/{id}",
				new
				{
					controller = "Route",
					action = "GetGood",
					id = UrlParameter.Optional
				});

			RouteTable.Routes.MapRoute(
				"",
				"goods/{id}",
				new
				{
					controller = "Route",
					action = "GetCategory",
					id = UrlParameter.Optional
				});

			RouteTable.Routes.MapRoute(
				"",
				"goodphoto/{id}",
				new
				{
					controller = "Route",
					action = "GetGoodPhoto",
					id = UrlParameter.Optional
				});
		}

		public void OnApplicationInitialized(
			UmbracoApplicationBase umbracoApplication,
			ApplicationContext applicationContext)
		{
		}

		public void OnApplicationStarting(
			UmbracoApplicationBase umbracoApplication,
			ApplicationContext applicationContext)
		{
		}
	}
}
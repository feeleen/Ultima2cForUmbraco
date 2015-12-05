using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace Umbraco
{
	public class MyStartupHandler : IApplicationEventHandler
	{
		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			//Create a custom routes

			RouteTable.Routes.MapRoute(
				"",
				"good/{id}",
				new
				{
					controller = "Goods",
					action = "GetGood",
					id = UrlParameter.Optional
				});

			RouteTable.Routes.MapRoute(
				"",
				"goodphoto/{id}",
				new
				{
					controller = "GoodPhotos",
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
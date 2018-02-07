using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Ultima
{
    public class InstallHelpers
    {
		public const string AppSettingKey = "Ultima.StartupInstalled";
		public const string GoodNodeIDKey = "Ultima.GoodNodeID";
		public const string CabinetNodeID = "Ultima.CabinetNodeID";
		public const string GoodPhotoNodeIDKey = "Ultima.GoodPhotoNodeID";
		public const string CategoryNodeIDKey = "Ultima.CategoryNodeID";
		public const string UltimaWebServiceURL = "Ultima.WebServiceURL";
		public const string GoogleMapsKey = "Ultima.GoogleMapsKey";
		
		public static void PublishContentNodes()
		{
			IContentService cs = Umbraco.Core.ApplicationContext.Current.Services.ContentService;

			// Publish our nodes
			IContent root = cs.GetRootContent().FirstOrDefault();
			root.ParentId = Constants.System.Root;
			cs.Save(root);
			cs.Publish(root);

			var myCollection = cs.GetRootContent().FirstOrDefault().Descendants();
			foreach (IContent doc in myCollection)
			{
				cs.Publish(doc);
			}
			cs.RebuildXmlStructures();
		}


		public static void AddHttpModule()
		{
			var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
			var modules = webConfig.GetSection("system.web/httpModules") as HttpModulesSection;
			modules.Modules.Add(new HttpModuleAction("UltimaMembershipHttpModule", "UltimaMembershipHttpModule, App_Code"));
			webConfig.Save();
		}

		public static void RemoveHttpModule()
		{
			var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
			var modules = webConfig.GetSection("system.web/httpModules") as HttpModulesSection;
			modules.Modules.Remove("UltimaMembershipHttpModule");
			webConfig.Save();
		}


		public static void RemoveAppConfigSections()
		{
			var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
			//Remove AppSetting key when all done
			webConfig.AppSettings.Settings.Remove(InstallHelpers.AppSettingKey);
			webConfig.AppSettings.Settings.Remove(InstallHelpers.GoodNodeIDKey);
			webConfig.AppSettings.Settings.Remove(InstallHelpers.GoodPhotoNodeIDKey);
			webConfig.AppSettings.Settings.Remove(InstallHelpers.CategoryNodeIDKey);
			webConfig.AppSettings.Settings.Remove(InstallHelpers.UltimaWebServiceURL);
			webConfig.AppSettings.Settings.Remove(InstallHelpers.GoogleMapsKey);
			webConfig.AppSettings.Settings.Remove(InstallHelpers.CabinetNodeID);
			webConfig.AppSettings.Settings.Remove(SessionErrors.SettingsKey);
			webConfig.AppSettings.Settings.Remove(SessionTrace.SettingsKey);
			webConfig.Save();
		}

		public static void AddAppConfigSections()
		{
			IContentService cs = Umbraco.Core.ApplicationContext.Current.Services.ContentService;
			var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
			IContent node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "Good").FirstOrDefault();

			if (node != null && webConfig.AppSettings.Settings[InstallHelpers.GoodNodeIDKey] == null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.GoodNodeIDKey, node.Id.ToString());
			}

			node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "GoodPhoto").FirstOrDefault();

			if (node != null && webConfig.AppSettings.Settings[InstallHelpers.GoodPhotoNodeIDKey] == null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.GoodPhotoNodeIDKey, node.Id.ToString());
			}

			node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "Goods").FirstOrDefault();

			if (node != null && webConfig.AppSettings.Settings[InstallHelpers.CategoryNodeIDKey] == null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.CategoryNodeIDKey, node.Id.ToString());
			}

			node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "Cabinet").FirstOrDefault();

			if (node != null && webConfig.AppSettings.Settings[InstallHelpers.CabinetNodeID] == null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.CabinetNodeID, node.Id.ToString());
			}

			//All done installing our custom stuff
			//As we only want this to run once - not every startup of Umbraco

			if (webConfig.AppSettings.Settings[InstallHelpers.UltimaWebServiceURL] == null)
				webConfig.AppSettings.Settings.Add(InstallHelpers.UltimaWebServiceURL, "localhost:8080");
			if (webConfig.AppSettings.Settings[SessionErrors.SettingsKey] == null)
				webConfig.AppSettings.Settings.Add(SessionErrors.SettingsKey, false.ToString());
			if (webConfig.AppSettings.Settings[SessionTrace.SettingsKey] == null)
				webConfig.AppSettings.Settings.Add(SessionTrace.SettingsKey, false.ToString());
			if (webConfig.AppSettings.Settings[InstallHelpers.GoogleMapsKey] == null)
				webConfig.AppSettings.Settings.Add(InstallHelpers.GoogleMapsKey, "yOuRgOoGlEkEy");
			if (webConfig.AppSettings.Settings[InstallHelpers.AppSettingKey] == null)
				webConfig.AppSettings.Settings.Add(InstallHelpers.AppSettingKey, true.ToString());
			webConfig.Save();
		}

	}
}

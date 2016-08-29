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
		public const string GoodPhotoNodeIDKey = "Ultima.GoodPhotoNodeID";
		public const string CategoryNodeIDKey = "Ultima.CategoryNodeID";
		public const string UltimaWebServiceURL = "Ultima.WebServiceURL";
		
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

		/// <summary>
		/// Add & merge in tranlsations from our lang files into Umbraco lang files
		/// </summary>
		public static void AddTranslations()
        {
            TranslationHelper.AddTranslations();
        }

		/// <summary>
		/// Removes our custom translation keys from language files
		/// </summary>
		public static void RemoveTranslations()
		{
			TranslationHelper.RemoveTranslations();
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

		/// <summary>
		/// Adds the application/custom section to Umbraco
		/// </summary>
		/// <param name="applicationContext"></param>
		public static void AddSection(ApplicationContext applicationContext)
        {
            //Get SectionService
            var sectionService = applicationContext.Services.SectionService;

			//Try & find a section with the alias of "ultimaSection"
			var ultimaSection = sectionService.GetSections().SingleOrDefault(x => x.Alias.ToLower() == "ultima");

            //If we can't find the section - doesn't exist
            if (ultimaSection == null)
            {
                //So let's create it the section
                sectionService.MakeNew("Ultima", "ultima", "icon-equalizer");
            }
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
			webConfig.AppSettings.Settings.Remove(SessionErrors.SettingsKey);
			webConfig.AppSettings.Settings.Remove(SessionErrors.SettingsKey);
			webConfig.Save();
		}

		public static void AddAppConfigSections()
		{
			IContentService cs = Umbraco.Core.ApplicationContext.Current.Services.ContentService;
			var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
			IContent node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "Good").FirstOrDefault();

			if (node != null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.GoodNodeIDKey, node.Id.ToString());
			}

			node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "GoodPhoto").FirstOrDefault();

			if (node != null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.GoodPhotoNodeIDKey, node.Id.ToString());
			}

			node = cs.GetRootContent().FirstOrDefault().Descendants().Where(x => x.Name == "Goods").FirstOrDefault();

			if (node != null)
			{
				webConfig.AppSettings.Settings.Add(InstallHelpers.CategoryNodeIDKey, node.Id.ToString());
			}

			//All done installing our custom stuff
			//As we only want this to run once - not every startup of Umbraco

			webConfig.AppSettings.Settings.Add(InstallHelpers.AppSettingKey, true.ToString());
			webConfig.AppSettings.Settings.Add(InstallHelpers.UltimaWebServiceURL, "localhost:8080");
			webConfig.AppSettings.Settings.Add(SessionErrors.SettingsKey, false.ToString());
			webConfig.AppSettings.Settings.Add(SessionTrace.SettingsKey, false.ToString());
			webConfig.Save();
		}

		/// <summary>
		/// Removes the custom app/section from Umbraco
		/// </summary>
		public static void RemoveSection()
		{
			//Get the Umbraco Service's Api's
			var services = UmbracoContext.Current.Application.Services;

			//Check to see if the section is still here (should be)
			var ultimaSection = services.SectionService.GetByAlias("ultima");

			if (ultimaSection != null)
			{
				//Delete the section from the application
				services.SectionService.DeleteSection(ultimaSection);
			}
		}

		/// <summary>
		/// Adds the required XML to the dashboard.config file
		/// </summary>
		public static void AddSectionDashboard()
        {
            bool saveFile = false;

            //Open up language file
            //umbraco/config/lang/en.xml
            var dashboardPath = "~/config/dashboard.config";

            //Path to the file resolved
            var dashboardFilePath = HostingEnvironment.MapPath(dashboardPath);

            //Load settings.config XML file
            XmlDocument dashboardXml = new XmlDocument();
            dashboardXml.Load(dashboardFilePath);

            // Section Node
            XmlNode findSection = dashboardXml.SelectSingleNode("//section [@alias='UltimaDashboardSection']");

            //Couldn't find it
            if (findSection == null)
            {
                //Let's add the xml
                var xmlToAdd = "<section alias='UltimaDashboardSection'>" +
                                    "<areas>" +
										"<area>ultima</area>" +
                                    "</areas>" +
                                    "<tab caption='Ultima2c Control Panel'>" +
										"<control addPanel='true' panelCaption=''>/App_Plugins/Ultima/backOffice/UltimaTree/partials/dashboard.html</control>" +
                                    "</tab>" +
                               "</section>";

                //Get the main root <dashboard> node
                XmlNode dashboardNode = dashboardXml.SelectSingleNode("//dashBoard");

                if (dashboardNode != null)
                {
                    //Load in the XML string above
                    XmlDocument xmlNodeToAdd = new XmlDocument();
                    xmlNodeToAdd.LoadXml(xmlToAdd);

                    var toAdd = xmlNodeToAdd.SelectSingleNode("*");

                    //Append the xml above to the dashboard node
                    dashboardNode.AppendChild(dashboardNode.OwnerDocument.ImportNode(toAdd, true));



                    //Save the file flag to true
                    saveFile = true;
                }
            }

            //If saveFile flag is true then save the file
            if (saveFile)
            {
                //Save the XML file
                dashboardXml.Save(dashboardFilePath);
            }
        }

		/// <summary>
		/// Removes the XML for the Section Dashboard from the XML file
		/// </summary>
		public static void RemoveSectionDashboard()
		{
			bool saveFile = false;

			//Open up language file
			//umbraco/config/lang/en.xml
			var dashboardPath = "~/config/dashboard.config";

			//Path to the file resolved
			var dashboardFilePath = HostingEnvironment.MapPath(dashboardPath);

			//Load settings.config XML file
			XmlDocument dashboardXml = new XmlDocument();
			dashboardXml.Load(dashboardFilePath);

			// Dashboard Root Node
			// <dashboard>
			XmlNode dashboardNode = dashboardXml.SelectSingleNode("//dashboard");

			if (dashboardNode != null)
			{
				XmlNode findSectionKey = dashboardNode.SelectSingleNode("./section [@alias='UltimaDashboardSection']");

				if (findSectionKey != null)
				{
					//Let's remove the key from XML...
					dashboardNode.RemoveChild(findSectionKey);

					//Save the file flag to true
					saveFile = true;
				}
			}

			//If saveFile flag is true then save the file
			if (saveFile)
			{
				//Save the XML file
				dashboardXml.Save(dashboardFilePath);
			}
		}

	}
}

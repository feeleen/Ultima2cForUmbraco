using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Trees;
using umbraco;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Ultima
{
    public class UmbracoStartup : ApplicationEventHandler
    {
		
		/// <summary>
		/// Register Install & Uninstall Events
		/// </summary>
		/// <param name="umbracoApplication"></param>
		/// <param name="applicationContext"></param>
		protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Check to see if appSetting Ultima2c is true or even present
            var installAppSetting = WebConfigurationManager.AppSettings[InstallHelpers.AppSettingKey];

            if (string.IsNullOrEmpty(installAppSetting) || installAppSetting != true.ToString())
            {
               //Check to see if language keys for section needs to be added
				InstallHelpers.AddHttpModule();

				// we can't do this here (nodes not exists yet):
				//InstallHelpers.PublishContentNodes();
				//InstallHelpers.AddAppConfigSections();

				InstalledPackage.AfterSave += InstalledPackage_AfterSave;
			}

			//Add OLD Style Package Event
			InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
 
        }

		private void InstalledPackage_AfterSave(InstalledPackage sender, EventArgs e)
		{
			InstallHelpers.PublishContentNodes();
			InstallHelpers.AddAppConfigSections();
		}


		/// <summary>
		/// Uninstall Package - Before Delete (Old style events, no V6/V7 equivelant)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void InstalledPackage_BeforeDelete(InstalledPackage sender, System.EventArgs e)
        {
            //Check which package is being uninstalled
            if (sender.Data.Name.ToLower() == "ultima")
            {
               
				//Start Uninstall - clean up process...
				InstallHelpers.RemoveAppConfigSections();
				InstallHelpers.RemoveHttpModule();
			}
        }
    }
}

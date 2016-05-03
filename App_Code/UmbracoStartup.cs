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
            //Check to see if appSetting AnalyticsStartupInstalled is true or even present
            var installAppSetting = WebConfigurationManager.AppSettings[InstallHelpers.AppSettingKey];

            if (string.IsNullOrEmpty(installAppSetting) || installAppSetting != true.ToString())
            {
               //Check to see if language keys for section needs to be added
				InstallHelpers.AddTranslations();

				//Check to see if section needs to be added
				InstallHelpers.AddSection(applicationContext);

				//Add Section Dashboard XML
				InstallHelpers.AddSectionDashboard();


				// we can't do this here (nodes not exists yet):
				//InstallHelpers.PublishContentNodes();
				//InstallHelpers.AddAppConfigSections();
				
			}

			

			//Add OLD Style Package Event
			InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;

            //Add Tree Node Rendering Event - Used to check if user is admin to display settings node in tree
            TreeControllerBase.TreeNodesRendering += TreeControllerBase_TreeNodesRendering;
 
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TreeControllerBase_TreeNodesRendering(TreeControllerBase sender, TreeNodesRenderingEventArgs e)
        {
			return;

            //Get Current User
            var currentUser = User.GetCurrent();

			//This will only run on the ultimatree & if the user is NOT admin
			if (sender.TreeAlias.ToLower() == "ultimatree" && !currentUser.IsAdmin())
            {
                //setting node to remove
                var settingNode = e.Nodes.SingleOrDefault(x => x.Id.ToString() == "settings");

                //Ensure we found the node
                if (settingNode != null)
                {
                    //Remove the settings node from the collection
                    e.Nodes.Remove(settingNode);
                }
            }

			//This will only run on the ultimatree
			if (sender.TreeAlias.ToLower() == "ultimatree")
            {
				
            }
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
				InstallHelpers.RemoveSection();
				InstallHelpers.RemoveTranslations();
				InstallHelpers.RemoveSectionDashboard();

				InstallHelpers.RemoveAppConfigSections();
				
			}
        }
    }
}

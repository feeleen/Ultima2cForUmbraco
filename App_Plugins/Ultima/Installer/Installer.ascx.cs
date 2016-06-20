using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ultima;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;

public partial class App_Plugins_Ultima_Installer_Installer : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		InstallerLabel.Text += "One Step left to get it done, please press the BUTTON to complete the installation ;)";	
	}

	protected void CompleteInstallationButton_Click(object sender, EventArgs e)
	{
		StepPanel.Visible = false;
		InstallHelpers.PublishContentNodes();
		InstallerLabelResult.Text += " + Content Nodes has been published<br/>";
		InstallHelpers.AddAppConfigSections();
		InstallerLabelResult.Text += " + App.config has been modified<br/>";
		InstallerLabelResult.Text += "<br/><b>Congratulations!</b> <br/> Please check http://ultima2c.com/ for more information!";
	}

}
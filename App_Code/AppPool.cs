using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;
using Umbraco.Web.WebApi;

/// <summary>
/// Сводное описание для AppPool
/// </summary>
public class AppPoolController : UmbracoAuthorizedApiController
{
	const string appPoolRestartNodeName = "Ultima_LastAppPoolRestartTime";

	[HttpGet]
	public object Restart()
	{
		try
		{
			string time = DateTime.Now.ToString();

			var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
			if (webConfig.AppSettings.Settings[appPoolRestartNodeName] != null)
			{
				webConfig.AppSettings.Settings[appPoolRestartNodeName].Value = time;
			}
			else
			{
				webConfig.AppSettings.Settings.Add(appPoolRestartNodeName, time);
			}

			webConfig.Save();

			return true;
		}
		catch (Exception ex)
		{
		}

		return false;
	}
}
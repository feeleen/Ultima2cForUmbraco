using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

public class SessionClient
{
	private SessionClient()
	{
		
	}

	public static CClientInfo GetClientInfo()
	{
		CClientInfo clientInfo = (CClientInfo)HttpContext.Current.Session["ClientInfo"];
		if (clientInfo == null)
		{
			try
			{
				//clientInfo = UltimaWebService.GetClientInfo();
				//SetClientInfo(clientInfo);
				return new CClientInfo();
			}
			catch (Exception ex)
			{
				FormsAuthentication.SignOut();
				HttpContext.Current.Response.Redirect("/");
			}

			return new CClientInfo();
		}

		return clientInfo;
	}

	public static void SetClientInfo(CClientInfo clientInfo)
	{
		HttpContext.Current.Session["ClientInfo"] = clientInfo;
	}
}
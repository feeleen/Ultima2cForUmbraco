using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

public class UltimaMembershipHttpModule : IHttpModule
{
	private void Application_PostAuthenticateRequest(object sender, EventArgs e)
	{
		HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
		if (authCookie != null)
		{
			//Extract the forms authentication cookie
			FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

			// If caching roles in userData field then extract
			string clientId = authTicket.UserData;

			// Create the IIdentity instance
			IIdentity id = new FormsIdentity(authTicket);

			CClientInfo clientInfo = new CClientInfo();

			try
			{
				clientInfo = UltimaWebService.GetClientInfo();
			}
			catch
			{
				FormsAuthentication.SignOut();
				HttpContext.Current.Response.Redirect("/");
			}

			// Create the IPrinciple instance
			IPrincipal principal = new UltimaPrincipal(id, clientInfo);

			// Set the context user 
			HttpContext.Current.User = principal;
			System.Threading.Thread.CurrentPrincipal = principal;
		}
	}

	public void Dispose()
	{
	}

	public void Init(HttpApplication application)
	{
		application.PostAuthenticateRequest += new EventHandler(this.Application_PostAuthenticateRequest);
	}

	
}
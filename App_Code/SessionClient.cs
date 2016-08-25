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
		return GetClientInfo(false);
	}

	public static bool SignIn(string user, string password)
	{
		bool isSigned = UltimaWebService.SignInAgent(user, password);
		
		if (isSigned)
		{
			CClientInfo info = SessionClient.GetClientInfo(true);

			if (info != null)
			{
				bool isPersistent = false;
				string userData = info.Id.ToString();

				FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
					1,                                     // ticket version
					info.FirstName,                              // authenticated username
					DateTime.Now,                          // issueDate
					DateTime.Now.AddMinutes(30),           // expiryDate
					isPersistent,                          // true to persist across browser sessions
					userData,                              // can be used to store additional user data
					FormsAuthentication.FormsCookiePath);  // the path for the cookie

				string encryptedTicket = FormsAuthentication.Encrypt(ticket);

				HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
				cookie.HttpOnly = true;
				HttpContext.Current.Response.Cookies.Add(cookie);
				return true;
			}
		}
		else
		{
			SignOut();
		}

		return false;
	}

	public static CClientInfo GetClientInfo(bool askWebService)
	{
		CClientInfo clientInfo = (CClientInfo)HttpContext.Current.Session["ClientInfo"];
		if (clientInfo == null)
		{
			if (askWebService)
			{
				try
				{
					clientInfo = UltimaWebService.GetClientInfo();
					SetClientInfo(clientInfo);
					return clientInfo;
				}
				catch (Exception ex)
				{
					SessionErrors.Add(ex.Message);
					SignOut();
				}
			}
		}

		return clientInfo;
	}

	public static void SetClientInfo(CClientInfo clientInfo)
	{
		HttpContext.Current.Session["ClientInfo"] = clientInfo;
	}

	public static void Clear()
	{
		HttpContext.Current.Session["ClientInfo"] = null;
	}

	public static void SignOut()
	{
		FormsAuthentication.SignOut();
		Clear();
	}
}
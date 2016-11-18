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
		bool isSignedIn = UltimaWebService.SignInAgent(user, password);
		
		if (isSignedIn)
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
			else
			{
				SessionTrace.Add("ClientInfo is null");
			}
		}
		else
		{
			SessionTrace.Add("Not signed in - signing out..");
		}

		SignOut();
		
		return false;
	}

	public static CClientInfo GetClientInfo(bool forceRenew)
	{
		CClientInfo clientInfo = null;

		if (forceRenew)
		{
			try
			{
				clientInfo = UltimaWebService.GetClientInfo();
			}
			catch (Exception ex)
			{
				SessionErrors.Add(ex.Message);
				SignOut();
			}

			SetClientInfo(clientInfo);
		}
		else
		{
			clientInfo = (CClientInfo)HttpContext.Current.Session["ClientInfo"];
		}

		if (clientInfo == null && HttpContext.Current.User.Identity.IsAuthenticated)
		{
			SignOut();
		}

		return clientInfo;
	}
	
	public static List<CDeliveryAddress> GetDeliveryAddresses(bool forceRenew)
	{
		List<CDeliveryAddress> deliveryAddresses = null;

		if (forceRenew || deliveryAddresses == null)
		{
			try
			{
				deliveryAddresses = UltimaWebService.GetDeliveryAddresses();
			}
			catch (Exception ex)
			{
				SessionErrors.Add(ex.Message);
			}

			SetDeliveryAddresses(deliveryAddresses);
		}
		else
		{
			deliveryAddresses = (List<CDeliveryAddress>)HttpContext.Current.Session["DeliveryAddresses"];
		}

		return deliveryAddresses;
	}
	
	public static void SetDeliveryAddresses(List<CDeliveryAddress> deliveryAddresses )
	{
		HttpContext.Current.Session["DeliveryAddresses"] = deliveryAddresses;
	}

	public static void SetClientInfo(CClientInfo clientInfo)
	{
		HttpContext.Current.Session["ClientInfo"] = clientInfo;
	}

	public static void Clear()
	{
		SetClientInfo(null);
	}

	public static void SignOut()
	{
		FormsAuthentication.SignOut();
		Clear();
	}
}
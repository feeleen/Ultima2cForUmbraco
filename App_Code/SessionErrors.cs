using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для SessionErrors
/// </summary>
public class SessionErrors
{
	public const string Key = "CurrentErrors";

	private List<string> ErrorsList { set; get; }

	public static void Add(string error)
	{
		SessionErrors err = Current;
		err.ErrorsList.Add(error);
		err.Update();
	}

	private SessionErrors()
	{
		ErrorsList = new List<string>();
	}

	private void Update()
	{
		HttpContext.Current.Session[Key] = ErrorsList;
	}

	public static SessionErrors Current
	{
		get
		{
			SessionErrors session = (SessionErrors)HttpContext.Current.Session[Key];
			if (session == null)
			{
				session = new SessionErrors();
				HttpContext.Current.Session[Key] = session;
			}
			return session;
		}
	}
}
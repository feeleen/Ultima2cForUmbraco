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
	public const string SettingsKey = "Ultima.ShowErrors";
	private object lockObj = new object();
	private List<string> ErrorsList { set; get; }
	public IReadOnlyCollection<string> List
	{
		get {
			return SessionErrors.Current.ErrorsList.AsReadOnly();
		}
	}

	public static bool Allowed
	{
		get
		{
			return Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings[SessionErrors.SettingsKey]);
		}
	}

	public static void Add(Exception ex)
	{
		if (Allowed)
		{
			SessionErrors err = Current;
			lock (err.lockObj)
			{
				err.ErrorsList.Add(ex.Message + Environment.NewLine + ex.StackTrace);
				err.Update();
			}
		}
	}

	public static void Add(string error)
	{
		if (Allowed)
		{
			SessionErrors err = Current;
			lock (err.lockObj)
			{
				err.ErrorsList.Add(error);
				err.Update();
			}
		}
	}

	private SessionErrors()
	{
		ErrorsList = new List<string>();
	}

	public static void Reset()
	{
		SessionErrors err = Current;
		lock (err.lockObj)
		{
			err.ErrorsList = new List<string>();
			HttpContext.Current.Session.Remove(Key);
		}
	}

	private void Update()
	{
		HttpContext.Current.Session[Key] = this;
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
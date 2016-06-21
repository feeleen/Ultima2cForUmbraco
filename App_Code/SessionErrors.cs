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
	private object lockObj = new object();
	private List<string> ErrorsList { set; get; }
	public IReadOnlyCollection<string> List
	{
		get {
			return SessionErrors.Current.ErrorsList.AsReadOnly();
		}
	}

	public static void Add(Exception ex)
	{
		SessionErrors err = Current;
		lock (err.lockObj)
		{
			err.ErrorsList.Add(ex.Message + Environment.NewLine + ex.StackTrace);
			err.Update();
		}
	}

	public static void Add(string error)
	{
		SessionErrors err = Current;
		lock (err.lockObj)
		{
			err.ErrorsList.Add(error);
			err.Update();
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для SessionTrace
/// </summary>
public class SessionTrace
{
	public const string Key = "SessionTrace";
	private object lockObj = new object();
	private List<string> TraceList { set; get; }
	public IReadOnlyCollection<string> List
	{
		get {
			return SessionTrace.Current.TraceList.AsReadOnly();
		}
	}

	public static void Add(string error)
	{
		SessionTrace err = Current;
		lock (err.lockObj)
		{
			err.TraceList.Add(error);
			err.Update();
		}
	}

	private SessionTrace()
	{
		TraceList = new List<string>();
	}

	public static void Reset()
	{
		SessionTrace err = Current;
		lock (err.lockObj)
		{
			err.TraceList = new List<string>();
			HttpContext.Current.Session.Remove(Key);
		}
	}

	private void Update()
	{
		HttpContext.Current.Session[Key] = this;
	}

	public static SessionTrace Current
	{
		get
		{
			SessionTrace session = (SessionTrace)HttpContext.Current.Session[Key];
			if (session == null)
			{
				session = new SessionTrace();
				HttpContext.Current.Session[Key] = session;
			}
			return session;
		}
	}
}
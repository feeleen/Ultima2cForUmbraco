using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для Helper
/// </summary>
public class Helper
{
	private Helper()
	{	
	}


	public static string GetQueryString(string parameter, string value, bool includeUrl = false)
	{
		var nameValues = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());
		nameValues[parameter] = value;
		string url = HttpContext.Current.Request.Url.AbsolutePath;
		if (includeUrl)
			return url + "?" + nameValues.ToString();
		else
			return nameValues.ToString();
	}
}
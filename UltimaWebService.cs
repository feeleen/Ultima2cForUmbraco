using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for UltimaWebService
/// </summary>
public class UltimaWebService
{
	private UltimaWebService()
	{
		//
		// TODO: Add constructor logic here
		//
	}

	public static XmlDocument GetXmlResponse(string method, IDictionary par)
	{
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(GetWebResponse(method, par).GetResponseStream()))
		{
			doc.Load(reader);
		}
		return doc;
	}




	public static XmlDocument GetXmlResponse(string method)
	{
		return GetXmlResponse(method, null);
	}

	public static HttpWebResponse GetWebResponse(string method, IDictionary par )
	{
		string paramString = "";
		if (par != null)
		{
			foreach (var key in par.Keys)
			{
				if (par[key] is Array)
				{
					paramString += "&" + key + "=[" + string.Join(",", (long[])par[key]) + "]";
				}
				else	 
					paramString += "&" + key + "=" + par[key].ToString();
			}
		}
		
		var webRequest = (HttpWebRequest)WebRequest.Create(string.Format(@"http://localhost:8337/{0}?format=xml{1}", method, paramString));
		webRequest.Method = "GET";
		webRequest.ContentType = "application/json";
		webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:28.0) Gecko/20100101 Firefox/28.0";
		webRequest.ContentLength = 0;
		string autorization = "bitrix" + ":" + "bitrix";
		byte[] binaryAuthorization = System.Text.Encoding.UTF8.GetBytes(autorization);
		autorization = Convert.ToBase64String(binaryAuthorization);
		autorization = "Basic " + autorization;
		webRequest.Headers.Add("AUTHORIZATION", autorization);
		
		var webResponse = (HttpWebResponse)webRequest.GetResponse();
		if (webResponse.StatusCode != HttpStatusCode.OK)
		{
			throw new HttpException((int)webResponse.StatusCode, "Ultima Server returned error " + (int)webResponse.StatusCode);
		}

		return webResponse;
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;


public class UltimaWebService
{
	private static IDictionary<long, byte[]> prodPhotoCache = new Dictionary<long, byte[]>();
	private static IDictionary<long, XmlDocument> prodInfoCache = new Dictionary<long, XmlDocument>();

	private UltimaWebService()
	{
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


	public static string GetProductInfo(long goodID, string dataField)
	{
		return GetProductInfo(goodID, dataField, true);
    }

    public static string GetProductInfo(long goodID, string dataField, bool useCache)
	{
		XmlDocument doc;

		if (prodInfoCache.ContainsKey(goodID) && useCache)
		{
			doc = prodInfoCache[goodID];
		}
		else
		{
			Hashtable pars = new Hashtable();
			pars["ProductsIds"] = new long[] { goodID };
			doc = UltimaWebService.GetXmlResponse("GetProducts", pars);
			prodInfoCache[goodID] = doc;
        }
		XmlNamespaceManager nsmgr = doc.NsMan();
		
		return doc.DocumentElement.SelectSingleNode(String.Format("{0}:GetProductsResponse/{0}:{1}", doc.GetPrefix(), dataField), nsmgr).InnerText;
    }

	public static byte[] GetProductImage(long goodID)
	{
		return GetProductImage(goodID, true);
    }

    public static byte[] GetProductImage(long goodID, bool useCache)
	{
		if (prodPhotoCache.ContainsKey(goodID) && useCache)
			return prodPhotoCache[goodID];

		Hashtable pars = new Hashtable();
		pars["ProductIds"] = new long[] { goodID };
		XmlDocument doc = UltimaWebService.GetXmlResponse("GetProductPhotos", pars);
		XmlNamespaceManager nsmgr = doc.NsMan();

		string data = doc.DocumentElement.SelectSingleNode(String.Format("{0}:GetProductPhotosResponse/{0}:Content", doc.GetPrefix()), nsmgr).InnerText;
		byte[] bytes = Convert.FromBase64String(data);
		prodPhotoCache[goodID] = bytes;
        return bytes;
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
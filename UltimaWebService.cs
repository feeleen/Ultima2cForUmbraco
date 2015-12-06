using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;


public class UltimaWebService
{
	private static IDictionary<long, byte[]> prodPhotoCache = new Dictionary<long, byte[]>();
	private static IDictionary<long, decimal> prodPricesCache = new Dictionary<long, decimal>();
	private static IDictionary<long, XmlDocument> prodInfoCache = new Dictionary<long, XmlDocument>();
	private static byte[] noImageStub = null;
	private static CookieContainer cookieCont = new CookieContainer();

	private UltimaWebService()
	{
	}

	public static byte[] GetNoImageStub()
	{
		if (noImageStub != null)
			return noImageStub;

		Image img = DrawText("No image", SystemFonts.DefaultFont, Color.DarkGray, Color.LightGray);
		ImageConverter converter = new ImageConverter();
		noImageStub = (byte[])converter.ConvertTo(img, typeof(byte[]));

		return noImageStub;
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


	public static decimal GetProductPrice(long goodID)
	{
		if (prodPricesCache.Count == 0)
			ReloadProductsPrices();

		if (!prodPricesCache.ContainsKey(goodID))
			return 0;

		return prodPricesCache[goodID];
	}


    public static int ReloadProductsPrices()
	{
		XmlDocument doc;

		Hashtable pars = new Hashtable();
		pars["CategoryIds"] = new long[] { 1 };
		pars["ZoneIds"] = new long[] { 1 };
		doc = UltimaWebService.GetXmlResponse("GetProductPrices", pars);
		XmlNamespaceManager nsmgr = doc.NsMan();

		foreach (XmlNode node in doc.DocumentElement.SelectNodes(String.Format("{0}:GetProductPricesResponse", doc.GetPrefix()), nsmgr))
		{
			prodPricesCache[Convert.ToInt64(node.SelectSingleNode(doc.Prefixed("ProductId"), nsmgr).InnerText)] = Convert.ToDecimal(node.SelectSingleNode(doc.Prefixed("Value"), nsmgr).InnerText);
		}
		
		return prodPricesCache.Count;
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

		byte[] bytes = GetNoImageStub();
		try
		{
			string data = doc.DocumentElement.SelectSingleNode(String.Format("{0}:GetProductPhotosResponse/{0}:Content", doc.GetPrefix()), nsmgr).InnerText;
			bytes = Convert.FromBase64String(data);
		}
		catch { }

		prodPhotoCache[goodID] = bytes;
		return bytes;
	}

	private static Image DrawText(String text, Font font, Color textColor, Color backColor)
	{
		Image img = new Bitmap(1, 1);
		Graphics drawing = Graphics.FromImage(img);

		SizeF textSize = drawing.MeasureString(text, font);

		img.Dispose();
		drawing.Dispose();

		int width = (int)textSize.Width + 40;
		int height = width;

		img = new Bitmap(width, width);

		drawing = Graphics.FromImage(img);
		drawing.Clear(backColor);

		Brush textBrush = new SolidBrush(textColor);

		drawing.DrawString(text, font, textBrush, Convert.ToInt32((width - (int)textSize.Width) / 2), Convert.ToInt32((width - (int)textSize.Height) / 2));
		drawing.Save();

		textBrush.Dispose();
		drawing.Dispose();

		return img;
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
		webRequest.CookieContainer = cookieCont;
        
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
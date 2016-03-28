using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

		Image img = DrawText("No image", new Font(FontFamily.GenericSansSerif, 20), Color.DarkGray, Color.LightGray);
		ImageConverter converter = new ImageConverter();
		noImageStub = (byte[])converter.ConvertTo(img, typeof(byte[]));

		return noImageStub;
	}

	public static XmlDocument GetXmlResponse(string method, IDictionary par)
	{
		XmlDocument doc = new XmlDocument();
		using (XmlTextReader reader = new XmlTextReader(GetWebResponse(method, par).GetResponseStream()))
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


	public static bool SignInAgent(string email, string password)
	{
		var doc = GetXmlResponse("SignInClientWithEmail", new Hashtable { { "Email", email }, { "Password", password } });
		XmlNamespaceManager nsmgr = doc.NsMan();

		if (Convert.ToBoolean(doc.DocumentElement.SelectSingleNode(String.Format("{0}:Success", doc.GetPrefix()), nsmgr).InnerText))
        {
			return true;
		}
		return false;
	}

	public static bool IsClientExists(string email)
	{
		Hashtable pars = new Hashtable();
		pars["Email"] = email;
		
		XmlDocument doc = GetXmlResponse("IsClientExists", pars);
		XmlNamespaceManager nsmgr = doc.NsMan();
		return Convert.ToBoolean(doc.DocumentElement.SelectSingleNode(String.Format("{0}:Exists", doc.GetPrefix()), nsmgr).InnerText);
	}

	// CreateAgent("anonymous@ultima2c.com", "Ivan", "999999999", "test");
	public static long CreateAgent(string email, string name, string phone, string password, string address)
	{
		// first of all we create new default agent for orders (you should change the password!):
		// http://localhost:8337/CreateClient?format=xml&Email=anonymous@ultima2c.com&FirstName=Anonymous&LastName=None&MiddleName=None&ParentMlmClientId=9&Password=test&Phone=8999999999
		// then we should sign-in:
		// http://localhost:8337/SignInClientWithEmail?format=xml&Email=anonymous@ultima2c.com&Password=test

		Hashtable pars = new Hashtable();
		pars["Email"] = email;
		pars["FirstName"] = name;
		pars["LastName"] = address;
		pars["MiddleName"] = "";
		pars["ParentMlmClientId"] = 9;
		pars["Password"] = password;
		pars["Phone"] = phone;

		XmlDocument doc = GetXmlResponse("CreateClient", pars);
		XmlNamespaceManager nsmgr = doc.NsMan();

		return Convert.ToInt64(doc.DocumentElement.SelectSingleNode(String.Format("{0}:Id", doc.GetPrefix()), nsmgr).InnerText);
	}

	public static Dictionary<string, string> CreateReserve()
	{
		Dictionary<string, string> res = new Dictionary<string, string>();

		var basket = SessionBasket.GetBasket();

		ArticleInfo[] prodInfo = new ArticleInfo[basket.Where(x => x.Key > 0).Count()];

		int i = 0;
		foreach (var key in basket.Keys)
		{
			if (key > 0)
			{
				prodInfo[i] = new ArticleInfo(key, basket[key]);
				i++;
			}
        } 

		Hashtable pars = new Hashtable();
		pars["Articles"] = prodInfo;
		pars["ReserveOfficeId"] = 1;
		pars["ObtainMethod"] = "ownStorePickup"; // some magic hardcoded value in ultima2c..

		XmlDocument doc = GetXmlResponse("CreateReserve", pars);
		XmlNamespaceManager nsmgr = doc.NsMan();

		string reserveID = doc.DocumentElement.SelectSingleNode(String.Format("{0}:Id", doc.GetPrefix()), nsmgr).InnerText;
		string deadDate = doc.DocumentElement.SelectSingleNode(String.Format("{0}:DeadDate", doc.GetPrefix()), nsmgr).InnerText;

		return new Dictionary<string, string> { { "Id", reserveID }, { "DeadDate", deadDate } };
	}

 	public static XmlDocument GetXmlResponse(string method)
	{
		return GetXmlResponse(method, null);
	}

	public static HttpWebResponse GetWebResponse(string serviceName, IDictionary par )
	{
		string paramString = "{";
		if (par != null)
		{
			bool first = true;
			foreach (var key in par.Keys)
			{
				paramString += (first ? "" : ",") + "\"" + key + "\":" + JsonConvert.SerializeObject(par[key], Newtonsoft.Json.Formatting.None);
				first = false;
            }
		}
		paramString += "}";

		var webRequest = (HttpWebRequest)WebRequest.Create(string.Format(@"http://localhost:8337/{0}?format=xml", serviceName));
		webRequest.Method = "POST";
		webRequest.ContentType = "text/json";
		webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:28.0) Gecko/20100101 Firefox/28.0";
		webRequest.ContentLength = Encoding.UTF8.GetBytes(paramString).Length;
		webRequest.CookieContainer = cookieCont;
        
		string autorization = "bitrix" + ":" + "bitrix";
		byte[] binaryAuthorization = System.Text.Encoding.UTF8.GetBytes(autorization);
		autorization = Convert.ToBase64String(binaryAuthorization);
		autorization = "Basic " + autorization;
		webRequest.Headers.Add("AUTHORIZATION", autorization);

		using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
		{
			streamWriter.Write(paramString);
			streamWriter.Flush();
			streamWriter.Close();
		}

		try
		{
			var webResponse = (HttpWebResponse)webRequest.GetResponse();
			if (webResponse.StatusCode != HttpStatusCode.OK)
			{
				throw new HttpException((int)webResponse.StatusCode, "Ultima Server returned error: " + WebUtility.UrlDecode(webResponse.Headers["UltimaErrorText"]));
			}
			return webResponse;
		}
		catch (WebException ex)
		{
			HttpWebResponse res = (HttpWebResponse)ex.Response; 
			throw new HttpException((int)res.StatusCode, "Ultima Server error: " + WebUtility.UrlDecode(res.Headers["UltimaErrorText"]), ex);
		}


	}
}
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
using System.Web.Configuration;
using System.Xml;
using Ultima;

public class UltimaWebService
{
	private static IDictionary<long, byte[]> prodPhotoCache = new Dictionary<long, byte[]>();
	private static IDictionary<long, decimal> prodPricesCache = new Dictionary<long, decimal>();
	private static IDictionary<long, XmlDocument> prodInfoCacheXML = new Dictionary<long, XmlDocument>();
	private static IDictionary<int, CProductInfo> prodInfoCache = new Dictionary<int, CProductInfo>();
	private static IDictionary<int, CCategory> catInfoCache = new Dictionary<int, CCategory>();
	private static IDictionary<int?, List<CCategory>> breadCrumbsCache = new Dictionary<int?, List<CCategory>>();
	private static IDictionary<int?, List<CCategory>> breadCrumbsProductCache = new Dictionary<int?, List<CCategory>>();

	private static byte[] noImageStub = null;
	private static List<CCategory> rootCategoriesCache = null;
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

	public static string GetTextResponse(string method, IDictionary par)
	{
		using (var reader = new StreamReader(GetWebResponse(method, "json", par).GetResponseStream(), Encoding.UTF8))
		{
			return reader.ReadToEnd();
		}
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
			try
			{
				prodPricesCache[Convert.ToInt64(node.SelectSingleNode(doc.Prefixed("ProductId"), nsmgr).InnerText)] = Convert.ToDecimal(node.SelectSingleNode(doc.Prefixed("Value"), nsmgr).InnerText);
			}
			catch { }
		}
		
		return prodPricesCache.Count;
	}

	public static string GetProductInfo(long goodID, string dataField, bool useCache)
	{
		XmlDocument doc;

		if (useCache && prodInfoCacheXML.ContainsKey(goodID))
		{
			doc = prodInfoCacheXML[goodID];
		}
		else
		{
			Hashtable pars = new Hashtable();
			pars["ProductsIds"] = new long[] { goodID };
			doc = UltimaWebService.GetXmlResponse("GetProducts", pars);
			if (useCache)
			{
				prodInfoCacheXML[goodID] = doc;
			}
        }
		XmlNamespaceManager nsmgr = doc.NsMan();
		
		return doc.DocumentElement.SelectSingleNode(String.Format("{0}:GetProductsResponse/{0}:{1}", doc.GetPrefix(), dataField), nsmgr).InnerText;
    }

	public static CCatalog GetCatalog(int langid, int? CategoryId, string SortField, string SortOrder, int PageSize, int PageNo,
		string SearchQuery, int?[] BrandId, string[] BrandNames, decimal? PriceFrom, decimal? PriceTo, string Availablity, List<CRequestFilter> filter, DateTime? DateAdded = null)
	{
		try
		{
			Hashtable pars = new Hashtable();
			pars["langid"] = langid;
			pars["CategoryId"] = CategoryId;
			pars["SortField"] = SortField;
			pars["SortOrder"] = SortOrder;
			pars["PageSize"] = PageSize;
			pars["PageNo"] = PageNo;
			pars["SearchQuery"] = SearchQuery;
			pars["BrandId"] = BrandId;
			pars["PriceFrom"] = PriceFrom;
			pars["PriceTo"] = PriceTo;
			pars["Availability"] = Availablity;
			pars["Filter"] = filter;
			pars["BrandNames"] = BrandNames;
			pars["DateAdded"] = DateAdded;

			string resp = GetTextResponse("GetCatalog", pars);
			SessionTrace.Add(resp);

			CCatalog cat = JsonConvert.DeserializeObject<CCatalog>(resp);
			
			return cat;
		}
		catch (Exception ex)
		{
			SessionErrors.Add(ex.Message + Environment.NewLine + ex.StackTrace);
			throw;
		}
	}

	public static List<CCategory> GetRootCategories(int langid)
	{
		return GetRootCategories(langid, true);
	}

	public static List<CCategory> GetRootCategories(int langid, bool useCache)
	{
		if (useCache && rootCategoriesCache != null)
			return rootCategoriesCache;

		Hashtable pars = new Hashtable();
		pars["langid"] = langid;
		rootCategoriesCache = JsonConvert.DeserializeObject<List<CCategory>>(GetTextResponse("GetRootCategories", pars));
		return rootCategoriesCache;
	}


	public static List<CCategory> GetCategoryBreadCrumbs(int langid, int? prodId, int? catId, bool useCache = true)
	{
		if (useCache)
		{
			if (prodId != null && breadCrumbsProductCache != null && breadCrumbsProductCache.ContainsKey(prodId) && breadCrumbsProductCache[prodId] != null)
			{
				return breadCrumbsProductCache[prodId];
			}
			else if (catId != null && breadCrumbsCache != null && breadCrumbsCache.ContainsKey(catId) && breadCrumbsCache[catId] != null)
			{ 
				return breadCrumbsCache[catId];
			}
		}
		Hashtable pars = new Hashtable();
		pars["langid"] = langid;
		pars["catid"] = catId;
		pars["prodid"] = prodId;
		List<CCategory> crumbs = JsonConvert.DeserializeObject<List<CCategory>>(GetTextResponse("GetCategoryBreadCrumbs", pars));
		if (catId != null)
		{
			breadCrumbsCache[catId] = crumbs;
		}
		if (prodId != null)
		{
			breadCrumbsProductCache[prodId] = crumbs;
		}
		return crumbs;
	}

	public static CProductInfo GetProductInfo(int prodId)
	{
		return GetProductInfo(prodId, -1, true);
	}

	public static CProductInfo GetProductInfo(int prodId, int langid, bool useCache)
	{
		if (useCache && prodInfoCache != null && prodInfoCache.ContainsKey(prodId))
			return prodInfoCache[prodId];

		Hashtable pars = new Hashtable();
		pars["prodid"] = prodId;
		pars["langid"] = langid;
		CProductInfo pi = JsonConvert.DeserializeObject<CProductInfo>(GetTextResponse("GetProductInfo", pars));
		prodInfoCache[prodId] = pi;
		return pi;
	}

	public static CCategory GetCategoryInfo(int catId)
	{
		return GetCategoryInfo(catId, -1, true);
	}

	public static CCategory GetCategoryInfo(int catId, int langid, bool useCache)
	{
		if (useCache && catInfoCache != null && catInfoCache.ContainsKey(catId))
			return catInfoCache[catId];

		Hashtable pars = new Hashtable();
		pars["catid"] = catId;
		pars["langid"] = langid;
		CCategory pi = JsonConvert.DeserializeObject<CCategory>(GetTextResponse("GetCategoryInfo", pars));
		catInfoCache[catId] = pi;
		return pi;
	}

	public static byte[] GetProductImage(long goodID)
	{
		return GetProductImage(goodID, 0, true);
    }
	
	
	public static byte[] GetProductImage(string photoID)
	{
		return GetProductImage(photoID, true);
    }
	
	public static byte[] GetProductImage(string photoID, bool useCache)
	{
		string[] vals = photoID.Split('_');
		long goodID = Convert.ToInt64(vals[0]);
		int viewID = Convert.ToInt32(vals[1]);
		
		if (useCache && prodPhotoCache.ContainsKey(goodID))
			return prodPhotoCache[goodID];

		byte[] bytes = DiskCache.GetImage(goodID, 0, viewID);
		if (bytes != null)
			return bytes;
		else
			bytes = GetNoImageStub();
		
		Hashtable pars = new Hashtable();
		pars["Images"] = new String[] {photoID};
		CGoodImage[] images = JsonConvert.DeserializeObject<CGoodImage[]>(GetTextResponse("GetImages", pars));
		if (images == null)
		{
			bytes = GetNoImageStub();
		}	
		else
		{
			bytes = images[0].Image;
			if (bytes != null)
				DiskCache.WriteImage(bytes, goodID, 0, viewID);
		}
		
		if (useCache)
			prodPhotoCache[goodID] = bytes;
		
		return bytes;
    }

    
	
	public static byte[] GetProductImage(long goodID, int viewID, bool useCache)
	{
		if (useCache && prodPhotoCache.ContainsKey(goodID))
			return prodPhotoCache[goodID];

		byte[] bytes = DiskCache.GetImage(goodID, 0, viewID);
		if (bytes != null)
			return bytes;
		else
			bytes = GetNoImageStub();
		
		Hashtable pars = new Hashtable();
		pars["ProductIds"] = new long[] { goodID };
		XmlDocument doc = UltimaWebService.GetXmlResponse("GetProductPhotos", pars);
		XmlNamespaceManager nsmgr = doc.NsMan();

		try
		{
			if (doc.DocumentElement.ChildNodes.Count > 0)
			{
				string data = doc.DocumentElement.SelectSingleNode(String.Format("{0}:GetProductPhotosResponse/{0}:Content", doc.GetPrefix()), nsmgr).InnerText;
				viewID = Convert.ToInt32(doc.DocumentElement.SelectSingleNode(String.Format("{0}:GetProductPhotosResponse/{0}:ViewId", doc.GetPrefix()), nsmgr).InnerText);

				bytes = Convert.FromBase64String(data);
				if (bytes != null)
					DiskCache.WriteImage(bytes, goodID, 0, viewID);
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error Getting Image", ex);
		}

		if (useCache)
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

	public static HttpWebResponse GetWebResponse(string serviceName, IDictionary par)
	{
		return GetWebResponse(serviceName, "xml", par);
	}

	public static HttpWebResponse GetWebResponse(string serviceName, string format, IDictionary par )
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

		var webRequest = (HttpWebRequest)WebRequest.Create(string.Format(@"http://{0}/{1}?format={2}", WebConfigurationManager.AppSettings[InstallHelpers.UltimaWebServiceURL], serviceName, format));
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

		SessionTrace.Add(paramString);

		using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
		{
			streamWriter.Write(paramString);
			streamWriter.Flush();
			streamWriter.Close();
		}

		try
		{
			var webResponse = (HttpWebResponse)webRequest.GetResponse();
			return webResponse;
		}
		catch (WebException ex)
		{
			HttpWebResponse res = (HttpWebResponse)ex.Response;
			string msg = "Ultima Server error: " + WebUtility.UrlDecode(res.Headers["UltimaErrorText"] + " (" + ex.Message + ", " + webRequest.RequestUri + ", param: " + paramString + "), StatusCode: " + (int)res.StatusCode);
			SessionErrors.Add(msg);
			throw new Exception(msg, ex);
		}


	}
}
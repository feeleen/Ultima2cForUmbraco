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

	private static List<CCategory> rootCategoriesCache = null;
	private static CookieContainer cookieCont = new CookieContainer();


	private UltimaWebService()
	{
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

			CCatalog cat = GetObject<CCatalog>("GetCatalog", pars);

			return cat;
		}
		catch (Exception ex)
		{
			SessionErrors.Add(ex);
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
		rootCategoriesCache = GetObject<List<CCategory>>("GetRootCategories", pars);
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
		List<CCategory> crumbs = GetObject<List<CCategory>>("GetCategoryBreadCrumbs", pars);
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
		CProductInfo pi = GetObject<CProductInfo>("GetProductInfo", pars);
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
		CCategory pi = GetObject<CCategory>("GetCategoryInfo", pars);
		catInfoCache[catId] = pi;
		return pi;
	}


	#region Product Image Methods
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
			bytes = ImageUtility.GetNoImageStub();

		Hashtable pars = new Hashtable();
		pars["Images"] = new String[] { photoID };
		CGoodImage[] images = GetObject<CGoodImage[]>("GetImages", pars);
		if (images != null)
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
			bytes = ImageUtility.GetNoImageStub();

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
	#endregion

	public static CClientInfo GetClientInfo()
	{
		return GetObject<CClientInfo>("GetClientInfo", null);
	}

	public static bool ConfirmClientPasswordChangeRequest(string hash, string password)
	{
		Hashtable res = new Hashtable();
		res["Hash"] = hash;
		res["Password"] = password;
		return GetObject<BoolValue>("ConfirmClientPasswordChangeRequest", null).Value;
	}

	public static bool UpdateClientInfoFromRequestParameters()
	{
		List<string> names = new List<string> { "firstName", "lastName", "phone", "email"};
		Hashtable pars = GetParmetersFromRequest(names);
		if (GetObject<BoolValue>("UpdateClientInfo", pars).Value)
		{
			SessionClient.GetClientInfo(true);
			return true;
		}
		return false;
	}

	private static Hashtable GetParmetersFromRequest(List<string> names)
	{
		Hashtable res = new Hashtable();
		foreach (string name in names)
		{
			res[name] = GetParameterFromRequest(name);
		}
		return res;
	}

	private static string GetParameterFromRequest(string name)
	{
		return HttpContext.Current.Request.Form[name];
	}

	public static bool SignInAgent(string email, string password)
	{
		return GetObject<BoolValue>("SignInClientWithEmail", 
			new Hashtable { { "Email", email }, { "Password", password } })
			.Value;
	}

	public static bool IsClientExists(string email)
	{
		Hashtable pars = new Hashtable();
		pars["Email"] = email;
		
		return GetObject<BoolValue>("IsClientExists", pars).Value;
	}

	// CreateAgent("anonymous@ultima2c.com", "Ivan", "999999999", "test");
	public static long CreateAgent(string email, string name, string phone, string password, string address, string lastname = "-")
	{
		// first of all we create new default agent for orders (you should change the password!):
		// http://localhost:8337/CreateClient?format=xml&Email=anonymous@ultima2c.com&FirstName=Anonymous&LastName=None&MiddleName=None&ParentMlmClientId=9&Password=test&Phone=8999999999
		// then we should sign-in:
		// http://localhost:8337/SignInClientWithEmail?format=xml&Email=anonymous@ultima2c.com&Password=test

		Hashtable pars = new Hashtable();
		pars["Email"] = email;
		pars["FirstName"] = name;
		pars["LastName"] = lastname;
		pars["MiddleName"] = "";
		pars["ParentMlmClientId"] = 9;
		pars["Password"] = password;
		pars["Phone"] = phone;

		return GetObject<IdValue>("CreateClient", pars).Id;
	}

	public static COrder CreateReserve(long agentId, string address)
	{
		try
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
			//pars["AgentId"] = agentId; // agentid will be taken from session by the server! do not use this field!
			pars["ReserveOfficeId"] = 1;
			pars["ObtainMethod"] = "simplified_shipping"; // ownStorePickup, simplified_shipping - some magic hardcoded values in ultima2c..
			pars["ShippingAddress"] = address;

			COrder order = GetObject<COrder>("CreateReserve", pars);
				
			return order;
		}
		catch (Exception ex)
		{
			SessionErrors.Add(ex);
			throw;
		}
	}


	public static CDocuments GetDocuments(int pageNumber = 1, int recordsPerPage = 20)
	{
		Hashtable pars = new Hashtable();
		pars["PageNumber"] = pageNumber;
		pars["RecordsPerPage"] = recordsPerPage;
		return GetObject<CDocuments>("GetReserves", pars);
	}

	#region Web Request stuff

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
			string res = reader.ReadToEnd();
			SessionTrace.Add(res);
			return res;
		}
	}

	public static T GetObject<T>(string method)
	{
		return GetObject<T>(method, null);
	}

	public static T GetObject<T>(string method, IDictionary pars)
	{
		T obj = JsonConvert.DeserializeObject<T>(GetTextResponse(method, pars));
		return obj;
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

		try
		{
			using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
			{
				streamWriter.Write(paramString);
				streamWriter.Flush();
				streamWriter.Close();
			}

			var webResponse = (HttpWebResponse)webRequest.GetResponse();
			return webResponse;
		}
		catch (Exception ex)
		{
			string msg = Log(ex, webRequest.RequestUri.ToString(), paramString);
			throw new Exception(msg, ex);
		}
	}

	#endregion

	static string Log(Exception ex, string requestUri, string paramString)
	{
		string msg = "Ultima Server error: " + ex.Message + ", " + requestUri + ", param: " + paramString;
		if (ex is WebException)
		{
			HttpWebResponse res = (HttpWebResponse)((WebException)ex).Response;
			if (res != null)
			{
				msg = "Ultima Server error: " + WebUtility.UrlDecode((res == null? " - " : res.Headers["UltimaErrorText"]) + " (" + ex.Message + ", " + requestUri + ", param: " + paramString + "), StatusCode: " + (int)res.StatusCode);
			}
		}
		SessionErrors.Add(msg);
		Umbraco.Core.Logging.LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, msg.Replace("{","{{").Replace("}","}}"));
		return msg;
	}
}
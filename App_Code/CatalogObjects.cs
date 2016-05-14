using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CCategory
{
	public int CategoryId { get; set; }
	public string Category { get; set; }
	public string CategoryAlias { get; set; }
}

public class CBrand
{
	public string Brand { get; set; }
	public int BrandId { get; set; }
	public int RecordsCnt { get; set; }
}

public class CValue
{
	public int RecordsCnt { get; set; }
	public string Value { get; set; }
	public int ValueId { get; set; }
}

public class CFilter
{
	public string Alias { get; set; }
	public int MaxValue { get; set; }
	public int MinValue { get; set; }
	public string Name { get; set; }
	public int PropertyId { get; set; }
	public int PropertyType { get; set; }
	public List<CValue> Values { get; set; }
}

public class CProduct
{
	public string Avail { get; set; }
	public string BuyText { get; set; }
	public string IconId { get; set; }
	public DateTime IconMark { get; set; }
	public string Name { get; set; }
	public int Price { get; set; }
	public int ProdId { get; set; }
}

public class CCatalog
{
	public List<CBrand> Brands { get; set; }
	public List<CCategory> Categories { get; set; }
	public List<CFilter> Filters { get; set; }
	public int PagesOverall { get; set; }
	public int PriceMax { get; set; }
	public int PriceMin { get; set; }
	public List<CProduct> Products { get; set; }
}
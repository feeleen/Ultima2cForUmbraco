using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public struct Availability
{
	public const string Onstock = "onstock";
	public const string Outofstock = "outofstock";
}

public struct SortOrder
{
	public const string Asc = "asc";
	public const string Desc = "desc";
}

public struct SortField
{
	public const string Name = "name";
	public const string Price = "price";
}




public class CCategory
{
	public int CategoryId { get; set; }
	public string Category { get; set; }
	public string CategoryAlias { get; set; }
	public string BuyName { get; set; }
	public int RecordsCnt { get; set; }
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
	public decimal MaxValue { get; set; }
	public decimal MinValue { get; set; }
	public string Name { get; set; }
	public int PropertyId { get; set; }
	public int PropertyType { get; set; }
	public List<CValue> Values { get; set; }
}

public class CRequestFilter
{
	public int KeyId { get; set; }
	public decimal? ValueFrom { get; set; }
	public decimal? ValueTo { get; set; }
	public List<int> Values { get; set; }
}

public class CProduct
{
	public string Availability { get; set; }
	public string BuyText { get; set; }
	public string IconId { get; set; }
	public DateTime IconMark { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int ProdId { get; set; }
}

public class CProductExt
{
	public int BrandId { get; set; }
	public int CategoryId { get; set; }
	public DateTime CreationDate { get; set; }
	public int Id { get; set; }
	public string Name { get; set; }
	public int OriginalProductId { get; set; }
	public int Volume { get; set; }
	public int WarrantyPeriod { get; set; }
	public int WarrantyPeriodUnitId { get; set; }
	public int Weight { get; set; }
}

public class CCatalog
{
	public List<CBrand> Brands { get; set; }
	public List<CCategory> Categories { get; set; }
	public List<CFilter> Filters { get; set; }
	public int PagesOverall { get; set; }
	public decimal PriceMax { get; set; }
	public decimal PriceMin { get; set; }
	public List<CProduct> Products { get; set; }
}

public class CPhoto
{
	public int Order { get; set; }
	public string PhotoId { get; set; }
	public DateTime PhotoMark { get; set; }
}

public class CGoodImage
{
	public int ImageId { get; set; }
	public byte[] Image { get; set; }
}


public class CProperty
{
	public int Kind { get; set; }
	public string Name { get; set; }
	public int PropId { get; set; }
	public int Type { get; set; }
	public string Value { get; set; }
}

public class CProductInfo
{
	public string Name { get; set; }
	public List<CPhoto> Photos { get; set; }
	public int ProdId { get; set; }
	public List<CProperty> Properties { get; set; }
}
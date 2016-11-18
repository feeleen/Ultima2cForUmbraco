using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для ArticleInfo
/// </summary>
public class DeliveryInfo
{
	public long AddressId = 0;
	public long TimeId = 7;
	public string Option = "own";
	public DateTime Date = DateTime.Now;
	public string Comments = "";
	public decimal? Price = null;

	public DeliveryInfo(long addressId, string comments)
	{
		this.AddressId = addressId;
		this.Comments = comments;
	}
}
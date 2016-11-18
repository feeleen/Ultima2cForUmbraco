using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Ultima;


public class SessionBasket
{
	public SessionBasket()
	{
	}

	public static Dictionary<int, decimal> GetBasket()
	{
		Dictionary<int, decimal> basket = (Dictionary<int, decimal>)HttpContext.Current.Session["Basket"];
		if (basket == null)
		{
			basket = new Dictionary<int, decimal>();
			SetBasket(basket);
		}
		
		return basket;
	}
	
	public static CBasketTotal GetBasketTotal()
	{
		CBasketTotal basketTotal = (CBasketTotal)HttpContext.Current.Session["BasketTotal"];
		if (basketTotal == null)
		{
			basketTotal = new CBasketTotal();
			basketTotal.Total = 0M;
			basketTotal.GrandTotal = 0M;
			basketTotal.ItemCount = 0M;
			basketTotal.DeliveryAddressId = -1;
			basketTotal.DeliveryCost = 0M;
	
			SetBasketTotal(basketTotal);
		}
		
		return basketTotal;
	}

	public static decimal AddToBasket(int goodID, decimal quantity, decimal price)
	{
		Dictionary<int, decimal> basket = GetBasket();
		decimal newQuantity = GetBasketQuantity(goodID) + quantity;
		basket[goodID] = newQuantity;
		basket[-goodID] = price;
		SetBasket(basket);
		RecalcBasket();
		return newQuantity;
	}

	public static decimal UpdateBasketQuantity(int goodID, decimal newQuantity, decimal price)
	{
		Dictionary<int, decimal> basket = GetBasket();

		if (basket.ContainsKey(goodID) && newQuantity == 0)
		{
			DeleteFromBasket(goodID);
			return 0;
        }

		basket[goodID] = newQuantity;
		basket[-goodID] = price;
		SetBasket(basket);
		RecalcBasket();
		return newQuantity;
	}

	public static void DeleteFromBasket(int goodID)
	{
		Dictionary<int, decimal> basket = GetBasket();
		if (basket.ContainsKey(goodID))
		{
			basket.Remove(goodID);
			basket.Remove(-goodID);
			SetBasket(basket);
			RecalcBasket();
		}
	}

	public static void RecalcBasket()
	{
		Dictionary<int, decimal> basket = GetBasket();
		CBasketTotal basketTotal = GetBasketTotal();
		basketTotal.Total = 0M;
		basketTotal.GrandTotal = 0M;
		basketTotal.ItemCount = 0M;
		
		foreach(int key in basket.Keys)
		{
			if (key > 0)
			{
				decimal amount = basket[-key]*basket[key];
				basketTotal.Total += amount;
				basketTotal.ItemCount += basket[key];
			}
		}
		
		basketTotal.GrandTotal = basketTotal.Total + basketTotal.DeliveryCost;
		
		SetBasketTotal(basketTotal);
	}

	public static decimal GetBasketQuantity(int goodID)
	{
		Dictionary<int, decimal> basket = GetBasket();
		decimal quantity = 0;
		if (basket.ContainsKey(goodID))
			quantity = basket[goodID];

		return quantity;
	}

	public static void SetBasket(Dictionary<int, decimal> basket)
	{
		HttpContext.Current.Session["Basket"] = basket;
	}
	
	public static void SetBasketTotal(CBasketTotal basketTotal)
	{
		HttpContext.Current.Session["BasketTotal"] = basketTotal;
	}
	
	public static void SetDeliveryAddressId(long deliveryAddressId)
	{
		CBasketTotal basketTotal = GetBasketTotal();
		basketTotal.DeliveryAddressId = deliveryAddressId;
		if (deliveryAddressId > 0)
		{
			basketTotal.DeliveryCost = UltimaWebService.GetDeliveryCost(deliveryAddressId);
		}
		else
		{
			basketTotal.DeliveryCost = 0M;
		}
		
		basketTotal.GrandTotal = basketTotal.DeliveryCost + basketTotal.Total;
		SetBasketTotal(basketTotal);
	}

	public static void ClearBasket()
	{
		HttpContext.Current.Session["Basket"] = new Dictionary<int, decimal>();
		HttpContext.Current.Session["BasketTotal"] = null;
	}
	
	public static string GetGooglekey()
	{
		return (string)WebConfigurationManager.AppSettings[InstallHelpers.GoogleMapsKey];
	}
}
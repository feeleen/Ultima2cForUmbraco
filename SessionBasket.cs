using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


public class SessionBasket
{
	public SessionBasket()
	{
	}

	public static Dictionary<int, decimal> GetBasket()
	{
		Dictionary<int, decimal> basket = (Dictionary<int, decimal>)HttpContext.Current.Session["Basket"];
		if (basket == null)
			basket = new Dictionary<int, decimal>();
		
		return basket;
	}

	public static decimal AddToBasket(int goodID, decimal quantity, decimal price)
	{
		Dictionary<int, decimal> basket = GetBasket();
		decimal newQuantity = GetBasketQuantity(goodID) + quantity;
		basket[goodID] = newQuantity;
		basket[-goodID] = price * newQuantity;
		SetBasket(basket);
		return newQuantity;
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
}
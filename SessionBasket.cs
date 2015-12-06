using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SessionBasket
{
	public SessionBasket()
	{
	}

	public static Dictionary<int, int> GetBasket()
	{
		Dictionary<int, int> basket = (Dictionary<int, int>)HttpContext.Current.Session["Basket"];
		if (basket == null)
			basket = new Dictionary<int, int>();

		return basket;
	}

	public static int AddToBasket(int goodID, int quantity)
	{
		Dictionary<int, int> basket = GetBasket();
		int newQuantity = GetBasketQuantity(goodID) + quantity;
		basket[goodID] = newQuantity;
		SetBasket(basket);
		return newQuantity;
	}

	public static int GetBasketQuantity(int goodID)
	{
		Dictionary<int, int> basket = GetBasket();
		int quantity = 0;
		if (basket.ContainsKey(goodID))
			quantity = basket[goodID];

		return quantity;
	}

	public static void SetBasket(Dictionary<int, int> basket)
	{
		HttpContext.Current.Session["Basket"] = basket;
	}
}
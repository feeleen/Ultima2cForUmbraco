using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для ArticleInfo
/// </summary>
public class ArticleInfo
{
	public long Id = 0;
	public long Quantity = 0;

	public ArticleInfo(long goodID, decimal quantity)
	{
		this.Id = goodID;
		this.Quantity = Convert.ToInt64(quantity);
	}
}
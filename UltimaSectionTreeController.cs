using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;


namespace Ultima
{
	/// <summary>
	/// People tree controller class
	/// </summary>
	[Tree("Ultima", "ultimaTree", "Content for Ultima2c")]
	[PluginController("Ultima")]
	public class UltimaTreeController : TreeController
	{
		/// <summary>
		/// Gets the tree nodes.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="queryStrings">The query strings.</param>
		/// <returns>The content for approval tree collection.</returns>
		protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
		{
			var nodes = new TreeNodeCollection();

			IUser user = UmbracoContext.Security.CurrentUser;

			//check if we're rendering the root node's children
			if (id == Constants.System.Root.ToInvariantString())
			{
				//foreach (Ultima2cContent content in ctrl.GetAll(user))
				//{
					// Add the content nodes to the root of the tree
					TreeNode node = CreateTreeNode(
						"dashboards",
						"-1",
						queryStrings,
						"MyTestUltimaItem",
						"icon-umb-content",
						true);

					nodes.Add(node);
				//}
			}
			else
			{
				// We're rendering the node properties
				int level = id.Count(c => c == '-');

				if (level == 0)
				{
					
						TreeNode propNode = CreateTreeNode(
							"dashboardEdit",
							id,
							queryStrings,
							"MyTestSubItem",
							"icon-target",
							false,
							string.Format("Ultima/UltimaTree/Edit/{0}", "myRoutepath"));

						nodes.Add(propNode);
					
				}
			}

			return nodes;
		}

		/// <summary>
		/// Gets the menu for node.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="queryStrings">The query strings.</param>
		/// <returns>The menu items.</returns>
		protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
		{
			var menu = new MenuItemCollection();
			
			if (id == Constants.System.Root.ToInvariantString())
			{
				// root actions              
				menu.Items.Add<RefreshNode, ActionRefresh>(ApplicationContext.Services.TextService.Localize(ActionRefresh.Instance.Alias));

			}

			return menu;
		}
	}
}
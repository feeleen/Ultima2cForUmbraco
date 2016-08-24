using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;


/// <summary>
/// Unfortunately this custom principal is always rewritten by System.Web.Security.RolePrincipal somehow, 
/// maybe because of custom umbraco role manager or membership provider. Someone should dig into that to fix :(
/// </summary>
public class UltimaPrincipal : IPrincipal
{
	public IIdentity Identity { get; private set; }
	public bool IsInRole(string role) { return false; }

	public UltimaPrincipal(IIdentity id, CClientInfo clientInfo)
	{
		this.ClientInfo = clientInfo;
		this.Identity = id;
	}

	public CClientInfo ClientInfo { get; set; }
}

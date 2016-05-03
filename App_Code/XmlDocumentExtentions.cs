using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

/// <summary>
/// Сводное описание для XmlDocumentExtentions
/// </summary>
public static class XmlDocumentExtentions
{
	public static string uPrefix = "u";
	
	public static string GetPrefix(this XmlDocument doc)
	{
		return uPrefix;
	}

	public static string Prefixed(this XmlDocument doc, string name)
	{
		return String.Format("{0}:{1}", uPrefix, name);
    }

	public static XmlNamespaceManager NsMan(this XmlDocument doc)
	{
		XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
		nsmgr.AddNamespace(uPrefix, "http://ultimabusinessware.com/types");
		
		return nsmgr;
    }
}
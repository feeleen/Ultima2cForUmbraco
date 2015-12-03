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
	public static XmlNamespaceManager NsMan(this XmlDocument doc)
	{
		XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
		nsmgr.AddNamespace("u", "http://ultimabusinessware.com/types");
		
		return nsmgr;
    }
}
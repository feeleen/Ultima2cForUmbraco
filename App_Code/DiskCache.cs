using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Ultima
{
	public class DiskCache
	{
		private static object locker = new Object();
		static string uploadDir = "~/Img";

		public static void WriteImage(byte[] bytes, long productID, long photoID, int viewID = 0)
		{
			
			if (!string.IsNullOrEmpty(uploadDir))
			{

				if (uploadDir.IndexOf("~/") == 0)
					uploadDir = HttpContext.Current.Server.MapPath(uploadDir);

				if (uploadDir.LastIndexOf("/") == uploadDir.Length - 1)
					uploadDir = uploadDir.Substring(0, uploadDir.Length - 1);

				string fullFileName = GetFullImagePath(productID, photoID, viewID);

				lock (locker)
				{
					if (File.Exists(fullFileName))
					{
						File.Delete(fullFileName);
					}
					else
					{
						FileInfo fileInf = new FileInfo(fullFileName);
						fileInf.Directory.Create(); // If the directory already exists, this method does nothing.
					}

					File.WriteAllBytes(fullFileName, bytes);
				}
			}
		}

		public static string GetFullImagePath(long productID, long photoID, int viewID = 0)
		{
			return string.Format("{0}/{1}/{2}", uploadDir, productID, GetPhotoName(productID, photoID, viewID));
		}

		public static string GetPhotoName(long productID, long photoID, int viewID = 0)
		{
			return string.Format("{0}_{1}_{2}.png", productID, photoID, viewID);
		}

		public static byte[] GetImage(long productID, long photoID, int viewID = 0)
		{
			string fullFileName = GetFullImagePath(productID, photoID, viewID);

			if (File.Exists(fullFileName))
			{
				return File.ReadAllBytes(fullFileName);
			}

			return null;
		}
	}
}
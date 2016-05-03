using System.IO;
using System.Web.Hosting;
using System.Xml.Linq;

namespace Ultima
{

    /// <summary>
    /// This code is not pretty and not intented to be. Will be rewritten later, but for now it
    /// serves as a global way to read from and write to the settings file.
    /// </summary>
    public class Config
	{
		public const string ConfigPath = "~/App_Plugins/Ultima/settings.config";
		public static string UmbracoVersion
		{
			get { return Umbraco.Core.Configuration.UmbracoVersion.Current.ToString(); }
		}

		/// <summary>
		/// Reads the contents of the file at the specified <code>path</code>. The path may be either virtual, relative or absolute.
		/// </summary>
		/// <param name="path">The path to the file.</param>
		public static string ReadAllText(string path)
		{
			if (path.StartsWith("~/")) path = HostingEnvironment.MapPath(path);
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader textReader = new StreamReader(fileStream))
				{
					return textReader.ReadToEnd();
				}
			}
		}
		/// <summary>
		/// Gets the client ID from the config file.
		/// </summary>
		public static string ConfigFilePath {
            get { return HostingEnvironment.MapPath(ConfigPath); }
        }

        public static string GetSetting(string key) {
            XElement xUltima = XElement.Load(ConfigFilePath);
            XElement xSetting = xUltima.Element(key);
            return xSetting == null ? null : xSetting.Value;
        }

        public static void SetSetting(string key, string value) {
            XElement xUltima = XElement.Load(ConfigFilePath);
            XElement xSetting = xUltima.Element(key);
            if (xSetting == null) {
				xUltima.Add(new XElement(
                    key,
                    new XAttribute("label", key),
                    new XAttribute("description", ""),
                    value ?? ""
                ));
            } else {
                xSetting.Value = value;
            }
			xUltima.Save(ConfigFilePath);
        }
        
        /// <summary>
        /// Gets the client ID from the config file.
        /// </summary>
        public static string ClientId {
            get { return GetSetting("ClientId"); }
        }

        /// <summary>
        /// Gets the client secret from the config file.
        /// </summary>
        public static string ClientSecret {
            get { return GetSetting("ClientSecret"); }
        }

        /// <summary>
        /// Get the redirect URI from the config file.
        /// </summary>
        public static string RedirectUri {
            get { return GetSetting("RedirectUri"); }
        }

        /// <summary>
        /// The refresh token used to acquire new access tokens. This is sensitive information.
        /// </summary>
        public static string RefreshToken {
            get { return GetSetting("RefreshToken"); }
            set { SetSetting("RefreshToken", value); }
        }

    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Properties;

namespace Terminals.Connection.Manager
{
    // .NET namespaces

    // Terminals and framework namespaces

    /// <summary>
    ///     Loading of icons from files
    /// </summary>
    public static class ConnectionImageHandler
    {
        #region Private Fields (2)
        private static ImageList imageList;
        private static readonly Dictionary<string, Connection> Dictionary = new Dictionary<string, Connection>();
        #endregion

        #region Private Properties (1)
        /// <summary>
        ///     This is the error icon for connections that don't specify icons correctly.
        /// </summary>
        private static ConnectionImage UnknownConnectionIcon
        {
            get { return new ConnectionImage(Resources.Question) { Name = "2" }; }
        }
        #endregion

        #region Public Methods (4)
        /// <summary>
        ///     Gets the icon file name by icons defined in FavoritesTreeView imageListIcons
        /// </summary>
        public static string GetTreeviewImageListKey(FavoriteConfigurationElement favorite)
        {
            ConnectionImage image = GetProtocolImage(favorite);
            return image.Name;
        }

        /// <summary>
        ///     Gets every protocol's icon and puts them into an ImageList loaded by the FavoritesTreeView.
        /// </summary>
        /// <returns> </returns>
        public static ImageList GetProtocolImageList()
        {
            if (imageList == null || imageList.Images.Count == 0)
            {
                imageList = new ImageList();

                imageList.Images.Add("0", Resources.Places_folder_documents_icon);
                imageList.Images.Add("1", Resources.Places_folder_blue_icon);
                imageList.Images.Add(UnknownConnectionIcon.Name, UnknownConnectionIcon.Image);

                string[] protocols = ConnectionManager.GetProtocols();
                
                foreach (string protocol in protocols)
                {
					Connection connection = GetCachedConnection(protocol);
					
					if (connection == null)
						continue;
					
                    ConnectionImage[] images = connection.Images;

                    foreach (ConnectionImage image in images)
                    {
                        imageList.Images.Add(image.Name, image.Image);
                    }
                }

                return imageList;
            }

            return imageList;
        }

        /// <summary>
        ///     Returns the favorite's tool bar icon.
        /// </summary>
        /// <param name="favorite"> </param>
        /// <returns> </returns>
        public static Image GetFavoriteIcon(FavoriteConfigurationElement favorite)
        {
            return String.IsNullOrEmpty(favorite.ToolBarIcon) ? GetProtocolImage(favorite).Image : LoadImage(favorite.ToolBarIcon, Resources.smallterm);
        }

        /// <summary>
        ///     Loads an image from the specified url.
        /// </summary>
        /// <param name="imageFilePath"> The url for the icon to be loaded. </param>
        /// <param name="defaultIcon"> A default icon if the image hasn't been found. </param>
        /// <returns> The resulting image. </returns>
        public static Image LoadImage(String imageFilePath, Image defaultIcon)
        {
            try
            {
                if (!String.IsNullOrEmpty(imageFilePath) && File.Exists(imageFilePath))
                {
                    return Image.FromFile(imageFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error loading TreeView item image.", ex);
            }

            return defaultIcon;
        }

        /// <summary>
        ///     Gets the connection by name and stores each different connection type in a dictionary
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public static Connection GetCachedConnection(string name)
        {
            if (Dictionary.Count < 1)
                foreach (Type type in ConnectionManager.GetConnectionTypes())
                {
                    if (type.IsAbstract)
                        continue;

					try
					{
						Connection connection = (Connection)Activator.CreateInstance(type);
						
	                    string protocolName = type.GetProtocolName();
	
	                    if (!Dictionary.ContainsKey(protocolName))
	                        Dictionary.Add(protocolName, connection);
					}
					catch
					{
						// unable to create an instance from the specified type.
						Log.Debug("Error creating instance from cached connection '" + name + "'.");
					}
                }

            if (Dictionary.ContainsKey(name.ToUpper()))
                return Dictionary[name.ToUpper()];

            return null;
        }
        #endregion

        #region Private Methods (1)
        /// <summary>
        ///     Gets the active connection's icon. A connection might have more than one Icon defined.
        /// </summary>
        private static ConnectionImage GetProtocolImage(FavoriteConfigurationElement favorite)
        {
            Connection connection = GetCachedConnection(favorite.Protocol);

            // This might happen if the user is not allowed to
            // use all available connections e.g. 
            // if the user has a freeware version.
            if (connection == null)
                return UnknownConnectionIcon;

            ConnectionImage image = favorite.CustomImage;

            // If a custom icon has been set use it, otherwise ...
            if (image != null)
            {
                // Add the image to the image list if it hasn't been loaded.
                if (!imageList.Images.ContainsKey(image.Name))
                    imageList.Images.Add(image.Name, image.Image);

                return image;
            }

            connection.Favorite = favorite;

            // ... return either the connection specific image or the image for unknown types.
            return connection.Image ?? UnknownConnectionIcon;
        }
        #endregion
    }
}
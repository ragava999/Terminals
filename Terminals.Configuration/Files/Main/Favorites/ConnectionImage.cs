namespace Terminals.Configuration.Files.Main.Favorites
{
    // .NET namespaces
    using System.Drawing;
    
    /// <summary>
    ///     This class provides a way to store an image name. This is useful for ImageLists, which need a key to identify the images.
    /// </summary>
    public class ConnectionImage
    {
        #region Constructor (1)
        public ConnectionImage(Image image)
        {
            this.Image = image;
        }
        #endregion

        #region Public Properties (2)
        public Image Image { get; private set; }
        public string Name { get; set; }
        #endregion
    }
}
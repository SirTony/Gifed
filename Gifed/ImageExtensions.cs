using System.Drawing.Imaging;

namespace System.Drawing
{
    /// <summary>
    /// Provides a set of extensions for the <see cref="Image" /> class.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Gets the <see cref="PropertyItem" /> specified by the <see cref="PropertyTag" />.
        /// </summary>
        /// <param name="image">The image to get the <see cref="PropertyItem"/> from.</param>
        /// <param name="property">The requested property.</param>
        /// <returns>The requested <see cref="PropertyItem"/> on success</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Thrown when the requested property does not exist.</exception>
        public static PropertyItem GetPropertyItem( this Image image, PropertyTag property )
        {
            if( image == null )
                throw new ArgumentNullException( nameof( image ) );

            return image.GetPropertyItem( (int)property );
        }

        /// <summary>
        /// Attempts to get the <see cref="PropertyItem" /> specified by the <see cref="PropertyTag" />.
        /// </summary>
        /// <param name="image">The image to get the <see cref="PropertyItem"/> from.</param>
        /// <param name="propertyTag">The requested property.</param>
        /// <param name="propertyItem">The requested <see cref="PropertyItem"/>.</param>
        /// <returns>True on success, false if the property does not exist.</returns>
        public static bool TryGetPropertyItem( this Image image, PropertyTag propertyTag, out PropertyItem propertyItem )
        {
            try
            {
                propertyItem = image.GetPropertyItem( propertyTag );
                return true;
            }
            catch
            {
                propertyItem = null;
                return false;
            }
        }
    }
}
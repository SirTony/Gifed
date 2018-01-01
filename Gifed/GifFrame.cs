using System;
using System.Drawing;

namespace Gifed
{
    /// <summary>
    ///     Represents one individual frame of an animated GIF.
    /// </summary>
    public sealed class GifFrame : IDisposable
    {
        private Image _image;

        /// <summary>
        ///     The image content of the frame.
        /// </summary>
        public Image Image
        {
            get { return this._image; }
            set
            {
                if( value == null )
                    throw new ArgumentNullException( nameof( value ) );

                if( value.Width != this._image.Width || value.Height != this._image.Height )
                {
                    throw new ArgumentException( "New image dimensions do not match old image dimensions",
                        nameof( value ) );
                }

                this._image = value;
            }
        }

        /// <summary>
        ///     The delay of each frame.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        ///     Creates a new GIF frame from the specified image with the specified delay.
        /// </summary>
        /// <param name="image">The image content of the frame.</param>
        /// <param name="delay">The delay of the frame.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image" /> is <see langword="null" />.</exception>
        public GifFrame( Image image, TimeSpan delay )
        {
            if( image == null )
                throw new ArgumentNullException( nameof( image ) );

            this._image = image;
            this.Delay = delay;
        }

        /// <summary>
        ///     Releases all resources used by this object.
        /// </summary>
        public void Dispose()
            => this._image.Dispose();
    }
}
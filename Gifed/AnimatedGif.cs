using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Gifed
{
    /// <summary>
    /// Provides an interface of loading, manipulating, and created animated GIF images.
    /// </summary>
    public sealed class AnimatedGif : IEnumerable<GifFrame>
    {
        private readonly List<GifFrame> _frames;

        /// <summary>
        /// Get a single frame at the specified index.
        /// </summary>
        /// <param name="index">The index of the desired frame.</param>
        /// <returns>The requested <see cref="GifFrame" /> on success.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index" /> is less than 0, or <paramref name="index" /> is equal to or greater than <see cref="AnimatedGif.FrameCount" />.</exception>
        public GifFrame this[ int index ]
        {
            get { return this._frames[index]; }
            set
            {
                if( value == null )
                    throw new ArgumentNullException( nameof( value ) );

                this._frames[index] = value;
            }
        }

        /// <summary>
        /// How many times the animation should loop before it stops (0 for indefinite).
        /// </summary>
        public ushort LoopCount { get; set; }

        /// <summary>
        /// How many frames the animation contains.
        /// </summary>
        public int FrameCount => this._frames.Count;

        /// <summary>
        /// The total duration of the animation.
        /// </summary>
        public TimeSpan Duration => TimeSpan.FromSeconds( this._frames.Sum( f => f.Delay ) * 100 );

        /// <summary>
        /// Creates a new, unpopulated animation with the specified loop count.
        /// </summary>
        /// <param name="loopCount">How many times the animation should loop before it stops (0 for indefinite).</param>
        public AnimatedGif( ushort loopCount = 0 )
        {
            this._frames = new List<GifFrame>();
            this.LoopCount = loopCount;
        }
        
        private AnimatedGif( List<GifFrame> frames, ushort loopCount )
        {
            this._frames = frames;
            this.LoopCount = loopCount;
        }
        
        public IEnumerator<GifFrame> GetEnumerator()
            => this._frames.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ( (IEnumerable)this._frames ).GetEnumerator();

        /// <summary>
        /// Loads an existing GIF image from the specified file.
        /// </summary>
        /// <param name="path">A full or relative path an image on disk.</param>
        /// <returns>A new instance of <see cref="AnimatedGif" /> representing the file.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the file does not contain data for an animated GIF image.</exception>
        public static AnimatedGif LoadFrom( string path )
        {
            var image = Image.FromFile( path );
            return AnimatedGif.LoadFrames( image );
        }

        /// <summary>
        /// Loads an existing GIF image from the specified stream.
        /// </summary>
        /// <param name="stream">A readable stream containing image data.</param>
        /// <returns>A new instance of <see cref="AnimatedGif" /> representing the stream.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the stream does not contain data for an animated GIF image.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        public static AnimatedGif LoadFrom( Stream stream )
        {
            var image = Image.FromStream( stream );
            return AnimatedGif.LoadFrames( image );
        }

        private static AnimatedGif LoadFrames( Image image )
        {
            var frameCount = image.GetFrameCount( FrameDimension.Time );

            PropertyItem delayProperty, loopProperty;
            if( !image.TryGetPropertyItem( PropertyTag.FrameDelay, out delayProperty ) ||
                !image.TryGetPropertyItem( PropertyTag.LoopCount, out loopProperty ) )
                throw new InvalidOperationException( "Image is not an animated gif" );

            var loopCount = BitConverter.ToUInt16( loopProperty.Value, 0 );
            var delayValues =
                delayProperty.Value.InChunksOf( sizeof( uint ) )
                             .Select( b => b.ToArray() )
                             .Select( b => BitConverter.ToUInt32( b, 0 ) )
                             .ToArray();

            var clone = (Image)image.Clone();
            Func<int, Bitmap> cloneImage = i =>
            {
                if( i > 0 )
                    clone.SelectActiveFrame( FrameDimension.Time, i );

                var frameImage = new Bitmap( image.Width, image.Height, image.PixelFormat );
                using( var graphics = Graphics.FromImage( frameImage ) )
                {
                    graphics.Clear( Color.Black );
                    graphics.DrawImageUnscaled( clone, Point.Empty );
                    graphics.Flush();
                }

                return frameImage;
            };

            var frames = Enumerable.Range( 0, frameCount )
                                   .Select( cloneImage )
                                   .Zip( delayValues, ( img, delay ) => new GifFrame( img, delay ) )
                                   .ToList();

            return new AnimatedGif( frames, loopCount );
        }

        /// <summary>
        /// Adds a single frame to the animation with the specified delay.
        /// </summary>
        /// <param name="image">The image data of the frame.</param>
        /// <param name="delay">The delay (in hundredths of a second) of the frame.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image" /> is <see langword="null"/>.</exception>
        public void AddFrame( Image image, uint delay )
        {
            var frame = new GifFrame( image, delay );
            this.AddFrame( frame );
        }

        /// <summary>
        /// Adds a single frame to the animation.
        /// </summary>
        /// <param name="frame">The <see cref="GifFrame" /> to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="frame" /> is <see langword="null"/>.</exception>
        public void AddFrame( GifFrame frame )
        {
            if( frame == null )
                throw new ArgumentNullException( nameof( frame ) );

            this._frames.Add( frame );
        }

        /// <summary>
        /// Adds multiple frames to the animation.
        /// </summary>
        /// <param name="frames">An array of <see cref="GifFrame" /> to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="frames" /> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any individual <see cref="GifFrame"/> in <paramref name="frames" /> is <see langword="null"/>.</exception>
        public void AddFrames( params GifFrame[] frames )
            => this.AddFrames( (IEnumerable<GifFrame>)frames );

        /// <summary>
        /// Adds multiple frames to the animation.
        /// </summary>
        /// <param name="frames"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="frames" /> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any individual <see cref="GifFrame"/> in <paramref name="frames" /> is <see langword="null"/>.</exception>
        public void AddFrames( IEnumerable<GifFrame> frames )
        {
            if( frames == null )
                throw new ArgumentNullException( nameof( frames ) );

            foreach( var frame in frames )
            {
                if( frame == null )
                    throw new ArgumentNullException( nameof( frames ), "Collection cannot contain null frames." );

                this._frames.Add( frame );
            }
        }

        /// <summary>
        /// Removes a single frame from the animation.
        /// </summary>
        /// <param name="index">The zero-based index of the frame to remove.</param>
        public void RemoveFrame( int index )
            => this._frames.RemoveAt( index );

        /// <summary>
        /// Removes a single frame from the animation.
        /// </summary>
        /// <param name="frame">The desired <see cref="GifFrame" /> to remove.</param>
        /// <returns><see langword="true" /> if the frame was removed, <see langword="false" /> otherwise.</returns>
        public bool RemoveFrame( GifFrame frame )
            => this._frames.Remove( frame );

        /// <summary>
        /// Removes a range of frames from the animation.
        /// </summary>
        /// <param name="index">The zero-based index of the position to start removing at.</param>
        /// <param name="count">The number of frames to remove after <paramref name="index" />.</param>
        public void RemoveFrames( int index, int count )
            => this._frames.RemoveRange( index, count );

        /// <summary>
        /// Removes all frames from the animation that match a predicate.
        /// </summary>
        /// <param name="pred">The predicate used to test frames for removal.</param>
        /// <returns>The number of frames that were removed from the animation.</returns>
        public int RemoveAllFrames( Predicate<GifFrame> pred )
            => this._frames.RemoveAll( pred );

        /// <summary>
        /// Removes all frames from the animation.
        /// </summary>
        public void RemoveAllFrames()
            => this._frames.Clear();

        /// <summary>
        /// Saves the animation to the specified file on disk.
        /// </summary>
        /// <param name="path">An absolute or relative path to save the animation to.</param>
        public void Save( string path )
        {
            using( var stream = File.Open( path, FileMode.Create, FileAccess.Write, FileShare.None ) )
                this.Save( stream );
        }

        /// <summary>
        /// Saves the image data for the animation to the specified stream.
        /// </summary>
        /// <param name="stream">A writeable <see cref="Stream" /> to save the image data to.</param>
        /// <exception cref="InvalidOperationException">Thrown when the animation does not contain any frames.</exception>
        public void Save( Stream stream )
        {
            if( this.FrameCount < 1 )
                throw new InvalidOperationException( "Image has no frames" );

            var delayProperty = (PropertyItem)Activator.CreateInstance( typeof( PropertyItem ), true );
            var loopProperty = (PropertyItem)Activator.CreateInstance( typeof( PropertyItem ), true );

            delayProperty.Id = (int)PropertyTag.FrameDelay;
            delayProperty.Type = (int)PropertyItemType.UInt32Array;
            delayProperty.Value = this._frames.Select( f => f.Delay ).SelectMany( BitConverter.GetBytes ).ToArray();
            delayProperty.Len = sizeof( uint ) * this.FrameCount;

            loopProperty.Id = (int)PropertyTag.LoopCount;
            loopProperty.Type = (int)PropertyItemType.UInt16Array;
            loopProperty.Value = BitConverter.GetBytes( this.LoopCount );
            loopProperty.Len = sizeof( uint );

            var encoder = ImageCodecInfo.GetImageEncoders().First( e => e.FormatID == ImageFormat.Gif.Guid );
            var encoderParameters = new EncoderParameters();
            encoderParameters.Param[0] = new EncoderParameter( Encoder.SaveFlag, (long)EncoderValue.MultiFrame );

            var gif = (Image)this._frames[0].Image.Clone();
            gif.SetPropertyItem( delayProperty );
            gif.SetPropertyItem( loopProperty );
            gif.Save( stream, encoder, encoderParameters );

            encoderParameters.Param[0] = new EncoderParameter( Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime );
            foreach( var frame in this._frames.Skip( 1 ) )
                gif.SaveAdd( frame.Image, encoderParameters );

            encoderParameters.Param[0] = new EncoderParameter( Encoder.SaveFlag, (long)EncoderValue.Flush );
            gif.SaveAdd( encoderParameters );
            stream.Flush();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gifed;

namespace Watermark
{
    internal static class Program
    {
        private static void Main( string[] args )
        {
            // Declare the text that will be put onto the image
            const string WatermarkText = "Watermark Test";

            // Create the colour for our watermark, 75% opacity white
            var colour = Color.FromArgb( (int)( byte.MaxValue * 0.75 ), Color.White );
            
            // The brush the text will be drawn with
            var brush = new SolidBrush( colour );

            // Create a font for our watermark.
            using( var font = new Font( FontFamily.GenericSerif, 14, FontStyle.Regular, GraphicsUnit.Point ) )
            using( var gif = AnimatedGif.LoadFrom( "..\\..\\Assets\\Pumpjack.gif" ) )
            {
                // Iterate over each frame
                foreach( var frame in gif )
                {
                    // Construct a Graphics object for the current frame image
                    using( var graphics = Graphics.FromImage( frame.Image ) )
                    {
                        // Measure the string for positioning
                        var textSize = graphics.MeasureString( WatermarkText, font );

                        // Position the text in the lower-right corner, with a 10 pixel margin on the horizonatl axis
                        var textLocation = new PointF
                        {
                            X = frame.Image.Width - textSize.Width - 10,
                            Y = frame.Image.Height - textSize.Height,
                        };

                        // Draw the string
                        graphics.DrawString( WatermarkText, font, brush, textLocation );

                        // Flush any pending operations
                        graphics.Flush();
                    }
                }

                // Write the newly watermarked image back to disk
                gif.Save( "Watermarked.gif" );
            }
        }
    }
}

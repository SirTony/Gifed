using System.Drawing.Imaging;

namespace System.Drawing
{
    /// <summary>
    /// The types used for the <see cref="PropertyItem.Type" /> field.
    /// </summary>
    public enum PropertyItemType
    {
        /// <summary>
        /// Array of unsigned 8-bit integers.
        /// </summary>
        ByteArray = 1,

        /// <summary>
        /// ASCII encoded, null-terminated string.
        /// </summary>
        ASCII = 2,
        
        /// <summary>
        /// Array of unsigned 16-bit integers.
        /// </summary>
        UInt16Array = 3,

        /// <summary>
        /// Array of unsigned 32-bit integers.
        /// </summary>
        UInt32Array = 4,

        /// <summary>
        /// Array of unsigned 32-bit integer pairs.
        /// Each pair represents a fraction.
        /// The first number is the numerator, the second number is the denominator.
        /// </summary>
        UInt32PairArray = 5,

        /// <summary>
        /// An array of bytes that may be interpretted as any type.
        /// </summary>
        UntypedArray = 6,

        /// <summary>
        /// Array of signed 32-bit integers.
        /// </summary>
        Int32Array = 7,

        /// <summary>
        /// Array of signed 32-bit integer pairs.
        /// Each pair represents a fraction.
        /// The first number is the numerator, the second number is the denominator.
        /// </summary>
        Int32PairArray = 10
    }
}
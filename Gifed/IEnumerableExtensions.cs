using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    ///     Provies a set of extensions for <see cref="IEnumerable{T}" />.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        ///     Splits an <see cref="IEnumerable{T}" /> into evenly sized chunks.
        /// </summary>
        /// <param name="enumerable">The source enumerable.</param>
        /// <param name="chunkSize">The size of the chunks to return.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> containing other enumerables representing the chunks.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="chunkSize" /> is less than 1.</exception>
        public static IEnumerable<IEnumerable<T>> InChunksOf<T>( this IEnumerable<T> enumerable, int chunkSize )
        {
            if( enumerable == null )
                throw new ArgumentNullException( nameof( enumerable ) );

            if( chunkSize < 1 )
            {
                throw new ArgumentException( "Chunk size must be a positive integer greater than zero",
                    nameof( chunkSize ) );
            }

            return chunkSize == 1
                ? enumerable.Select( item => new[] { item } )
                : IEnumerableExtensions.ChunksImpl( enumerable, chunkSize );
        }

        private static IEnumerable<IEnumerable<T>> ChunksImpl<T>( IEnumerable<T> enumerable, int chunkSize )
        {
            using( var enumerator = enumerable.GetEnumerator() )
            {
                while( enumerator.MoveNext() )
                {
                    int i;
                    var buffer = new T[chunkSize];
                    for( i = 0; i < chunkSize; ++i )
                    {
                        buffer[i] = enumerator.Current;

                        if( i < chunkSize - 1 && !enumerator.MoveNext() )
                            break;
                    }

                    if( i < chunkSize - 1 )
                        Array.Resize( ref buffer, i + 1 );

                    yield return buffer;
                }
            }
        }
    }
}
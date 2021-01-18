using System;
using System.Collections.Generic;

namespace HomographySharp
{
    public abstract class HomographyMatrix<T> where T : struct, IEquatable<T>
    {
        public abstract Point2<T> Translate(T srcX, T srcY);

        public abstract IReadOnlyList<T> Elements { get; }

        internal HomographyMatrix()
        {
        }
    }
}

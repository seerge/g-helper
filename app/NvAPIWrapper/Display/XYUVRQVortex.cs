using System;
using System.Collections.Generic;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents a XYUVRQ scan-out warping vortex
    /// </summary>
    public class XYUVRQVortex : IEquatable<XYUVRQVortex>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="XYUVRQVortex" />.
        /// </summary>
        /// <param name="x">The target view port mesh horizontal coordinate</param>
        /// <param name="y">The target view port mesh vertical coordinate</param>
        /// <param name="u">The desktop view port texture horizontal coordinate</param>
        /// <param name="v">The desktop view port texture vertical coordinate</param>
        /// <param name="r">The 3D warp perspective R factor</param>
        /// <param name="q">The 3D warp perspective Q factor</param>
        // ReSharper disable once TooManyDependencies
        public XYUVRQVortex(int x, int y, int u, int v, float r, float q)
        {
            X = x;
            Y = y;
            U = u;
            V = v;
            R = r;
            Q = q;
        }

        /// <summary>
        ///     3D warp perspective Q factor
        /// </summary>
        public float Q { get; }

        /// <summary>
        ///     3D warp perspective R factor
        /// </summary>
        public float R { get; }

        /// <summary>
        ///     Desktop view port texture horizontal coordinate
        /// </summary>
        public int U { get; }

        /// <summary>
        ///     Desktop view port texture vertical coordinate
        /// </summary>
        public int V { get; }

        /// <summary>
        ///     Target view port mesh horizontal coordinate
        /// </summary>
        public int X { get; }

        /// <summary>
        ///     Target view port mesh vertical coordinate
        /// </summary>
        public int Y { get; }

        /// <inheritdoc />
        public bool Equals(XYUVRQVortex other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Math.Abs(Q - other.Q) < 0.0001 &&
                   Math.Abs(R - other.R) < 0.0001 &&
                   U == other.U &&
                   V == other.V &&
                   X == other.X &&
                   Y == other.Y;
        }

        /// <summary>
        ///     Parses an array of floats and returns the corresponding <see cref="XYUVRQVortex" />s.
        /// </summary>
        /// <param name="floats">The array of float representing one or more <see cref="XYUVRQVortex" />s.</param>
        /// <returns>Instances of <see cref="XYUVRQVortex" />.</returns>
        public static IEnumerable<XYUVRQVortex> FromFloatArray(float[] floats)
        {
            for (var i = 0; i + 6 <= floats.Length; i += 6)
            {
                yield return new XYUVRQVortex(
                    (int) floats[i],
                    (int) floats[i + 1],
                    (int) floats[i + 2],
                    (int) floats[i + 3],
                    floats[i + 4],
                    floats[i + 5]
                );
            }
        }

        /// <summary>
        ///     Compares two instance of <see cref="XYUVRQVortex" /> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><see langword="true" /> if both instances are equal, otherwise <see langword="false" /></returns>
        public static bool operator ==(XYUVRQVortex left, XYUVRQVortex right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instance of <see cref="XYUVRQVortex" /> for in-equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><see langword="true" /> if both instances are not equal, otherwise <see langword="false" /></returns>
        public static bool operator !=(XYUVRQVortex left, XYUVRQVortex right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as XYUVRQVortex);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Q.GetHashCode();
                hashCode = (hashCode * 397) ^ R.GetHashCode();
                hashCode = (hashCode * 397) ^ U;
                hashCode = (hashCode * 397) ^ V;
                hashCode = (hashCode * 397) ^ X;
                hashCode = (hashCode * 397) ^ Y;

                return hashCode;
            }
        }

        /// <summary>
        /// Returns this instance of <see cref="XYUVRQVortex"/> as a float array.
        /// </summary>
        /// <returns>An array of float values representing this instance of <see cref="XYUVRQVortex"/>.</returns>
        public float[] AsFloatArray()
        {
            return new[] {X, Y, U, V, R, Q};
        }
    }
}
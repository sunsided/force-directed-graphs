using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace widemeadows.Graphs.Model
{
    /// <summary>
    /// Struct Location
    /// </summary>
    [DebuggerDisplay("{X,nq}, {Y,nq}")]
    public struct Vector
    {
        /// <summary>
        /// Location on the X axis
        /// </summary>
        public readonly double X;

        /// <summary>
        /// Location on the Y axis
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// The zero vector
        /// </summary>
        [NotNull]
        public static readonly Vector Zero = new Vector(0D, 0D);

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Calculates the squared 2-norm of this instance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double SquaredNorm()
        {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Calculates the 2-norm of this instance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double Norm()
        {
            return Math.Sqrt(SquaredNorm());
        }

        /// <summary>
        /// Returns the normalized vector.
        /// </summary>
        /// <param name="norm">The norm.</param>
        /// <returns>Vector.</returns>
        public Vector Normalized(out double norm)
        {
            norm = Norm();
            var inverseNorm = 1.0D / norm;
            return new Vector(X*inverseNorm, Y*inverseNorm);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Vector" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(Vector other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector && Equals((Vector)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="s">The s.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector operator *(Vector v, double s)
        {
            return new Vector(v.X*s, v.Y*s);
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }
    }
}

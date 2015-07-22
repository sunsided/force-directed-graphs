using System.Diagnostics;

namespace widemeadows.Graphs.Model
{
    /// <summary>
    /// Struct Location
    /// </summary>
    [DebuggerDisplay("{X,nq}, {Y,nq}")]
    public struct Location
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
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Location(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Location" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Location" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(Location other)
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
            return obj is Location && Equals((Location) obj);
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
        /// Implements the -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector operator -(Location a, Location b)
        {
            var dX = a.X - b.X;
            var dY = a.Y - b.Y;
            return new Vector(dX, dY);
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Location operator +(Location a, Vector v)
        {
            var x = a.X + v.X;
            var y = a.Y + v.Y;
            return new Location(x, y);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static Location operator -(Location a, Vector v)
        {
            var x = a.X - v.X;
            var y = a.Y - v.Y;
            return new Location(x, y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Location"/> to <see cref="Vector"/>.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector(Location l)
        {
            return new Vector(l.X, l.Y);
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

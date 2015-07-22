using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace widemeadows.Graphs.Model
{
    /// <summary>
    /// Class Edge. This class cannot be inherited.
    /// </summary>
    public sealed class Edge : IEquatable<Edge>, IEquatable<Vertex>
    {
        /// <summary>
        /// Gets the left vertex.
        /// </summary>
        /// <value>The left.</value>
        [NotNull]
        public Vertex Left { get; private set; }

        /// <summary>
        /// Gets the right vertex.
        /// </summary>
        /// <value>The left.</value>
        [NotNull]
        public Vertex Right { get; private set; }

        /// <summary>
        /// Gets the connection weight.
        /// </summary>
        /// <value>The weight.</value>
        public double Weight { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="weight">The weight.</param>
        public Edge([NotNull] Vertex left, [NotNull] Vertex right, double weight)
        {
            Left = left;
            Right = right;
            Weight = weight;
        }

        /// <summary>
        /// Determines whether this edge contains the specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns><c>true</c> if this edge contains the specified vertex; otherwise, <c>false</c>.</returns>
        public bool Contains([CanBeNull] Vertex vertex)
        {
            return Left.Equals(vertex) || Right.Equals(vertex);
        }

        /// <summary>
        /// Obtains the other end of an edge that contains the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>The other end if this edge contains the specified <paramref name="vertex"/>.</returns>
        public Vertex Other([CanBeNull] Vertex vertex)
        {
            Debug.Assert(Contains(vertex), "Contains(vertex)");
            return Left.Equals(vertex) ? Right : Left;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Edge" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="Edge" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals([CanBeNull] Edge other)
        {
            return other != null && EqualsInternal(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Edge" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="Edge" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        private bool EqualsInternal([NotNull] Edge other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right) ||
                   Left.Equals(other.Right) && Right.Equals(other.Left);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] Vertex other)
        {
            return Contains(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Edge && EqualsInternal((Edge)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Left.GetHashCode()*397) ^ Right.GetHashCode();
            }
        }
    }
}

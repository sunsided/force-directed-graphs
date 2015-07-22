using System;
using System.Collections.Generic;

namespace widemeadows.Graphs.Model
{
    /// <summary>
    /// Class DataVertex.
    /// </summary>
    public sealed class Vertex<TData> : Vertex, IEquatable<Vertex<TData>>
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public TData Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataVertex{TData}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Vertex(TData data)
        {
            Data = data;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(Vertex<TData> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && EqualityComparer<TData>.Default.Equals(Data, other.Data);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Vertex<TData> && Equals((Vertex<TData>) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<TData>.Default.GetHashCode(Data);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Data: {0}", Data);
        }
    }
}

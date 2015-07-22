using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace widemeadows.Graphs.Model
{
    /// <summary>
    /// Class Graph. This class cannot be inherited.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// The edges
        /// </summary>
        [NotNull]
        private readonly HashSet<Edge> _edges;

        /// <summary>
        /// The vertices
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Vertex, HashSet<Edge>> _vertices;

        /// <summary>
        /// Gets all edges.
        /// </summary>
        /// <value>The edges.</value>
        [NotNull]
        public IEnumerable<Edge> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// Gets all vertices.
        /// </summary>
        /// <value>The vertices.</value>
        [NotNull]
        public IEnumerable<Vertex> Vertices
        {
            get { return _vertices.Keys; }
        }

        /// <summary>
        /// Gets all edges of a given <paramref name="vertex"/>.
        /// </summary>
        /// <value>The edges.</value>
        [NotNull]
        public IEnumerable<Edge> this[[NotNull] Vertex vertex]
        {
            get { return _vertices[vertex]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph"/> class.
        /// </summary>
        /// <param name="edges">The edges.</param>
        public Graph([NotNull] params Edge[] edges)
            : this((IReadOnlyCollection<Edge>)edges)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph" /> class.
        /// </summary>
        /// <param name="edges">The edges.</param>
        public Graph([NotNull] IEnumerable<Edge> edges)
        {
            var edgeCollection = new HashSet<Edge>();
            var vertices = new ConcurrentDictionary<Vertex, HashSet<Edge>>();

            foreach (var edge in edges)
            {
                // skip duplicate edges
                if (edgeCollection.Contains(edge)) continue;

                edgeCollection.Add(edge);
                AddToEdgeLookup(vertices, edge);
            }

            _vertices = vertices;
            _edges = edgeCollection;
        }

        /// <summary>
        /// Adds the <paramref name="edge" /> to edge <paramref name="lookup"/>.
        /// </summary>
        /// <param name="lookup">The vertices.</param>
        /// <param name="edge">The edge.</param>
        private static void AddToEdgeLookup([NotNull] ConcurrentDictionary<Vertex, HashSet<Edge>> lookup, [NotNull] Edge edge)
        {
            AddToEdgeLookup(lookup, edge.Left, edge);
            AddToEdgeLookup(lookup, edge.Right, edge);
        }

        /// <summary>
        /// Adds the <paramref name="vertex"/> to edge <paramref name="lookup"/>.
        /// </summary>
        /// <param name="lookup">The vertices.</param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="edge">The edge.</param>
        private static void AddToEdgeLookup([NotNull] ConcurrentDictionary<Vertex, HashSet<Edge>> lookup, [NotNull] Vertex vertex, [NotNull] Edge edge)
        {
            lookup.AddOrUpdate(vertex,
                v => new HashSet<Edge> {edge},
                (v, list) =>
                {
                    list.Add(edge);
                    return list;
                });
        }

        /// <summary>
        /// Tries to obtain the edge between two vertices.
        /// </summary>
        /// <param name="firstVertex">The first vertex.</param>
        /// <param name="secondVertex">The second vertex.</param>
        /// <param name="edge">The edge.</param>
        /// <returns><c>true</c> if such an edge exists, <c>false</c> otherwise.</returns>
        [ContractAnnotation("=>true,edge:notnull;=>false,edge:null")]
        public bool TryGetEdge([NotNull] Vertex firstVertex, [NotNull] Vertex secondVertex, [CanBeNull] out Edge edge)
        {
            edge = null;

            // attempt to obtain the edge; should always succeed for existing nodes
            HashSet<Edge> edges;
            if (!_vertices.TryGetValue(firstVertex, out edges)) return false;

            // find the matching counterpart
            edge = edges.FirstOrDefault(e => e.Equals(secondVertex)); // TODO: optimize that
            return edge != null;
        }
    }
}

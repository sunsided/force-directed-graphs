using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace widemeadows.Graphs.Model
{
    /// <summary>
    /// Class Planner.
    /// </summary>
    public class Planner
    {
        /// <summary>
        /// The repulsion strength between two unconnected vertices
        /// </summary>
        private const double VertexRepulsionForceStrength = 1D;

        /// <summary>
        /// The attraction strength between two connected vertices
        /// </summary>
        private const double VertexAttractionForceStrength = 0.1D;

        /// <summary>
        /// The maximum number of iterations
        /// </summary>
        private const int MaximumIterations = 1000;

        /// <summary>
        /// Plans the specified graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        [NotNull]
        public IReadOnlyDictionary<Vertex, Location> Plan([NotNull] Graph graph)
        {
            // create initial random locations for each vertex
            var currentLocations = CreateRandomLocations(graph);

            // loop until the number of iterations exceeds the hard limit
            for (var i = 0; i < MaximumIterations; ++i)
            {
                // the total displacement, used as a stop condition
                var totalDisplacement = 0.0D;

                // prepare the new positions
                var newLocations = new Dictionary<Vertex, Location>(currentLocations);

                // iterate over all vertices ...
                foreach (var vertex in graph.Vertices) // TODO: if we directly loop over edges, can we cut loops by 1/2?
                {
                    // obtain the vertex location
                    var locationOf = currentLocations[vertex];

                    // and process against all other vertices
                    var netForce = Vector.Zero;
                    netForce += CalculateTotalRepulsion(graph, vertex, locationOf, currentLocations);
                    netForce += CalculateTotalAttraction(graph, vertex, locationOf, currentLocations);

                    // finally, update the vertex' position
                    locationOf += netForce;
                    newLocations[vertex] = locationOf;

                    // update the total displacement
                    totalDisplacement += netForce.SquaredNorm();
                }

                // calculate the center
                var center = newLocations.Aggregate(Vector.Zero, (vector, location) => vector + (Vector)location.Value) * (1D / currentLocations.Count);

                // adjust each node to prevent creep
                foreach (var location in newLocations)
                {
                    currentLocations[location.Key] = location.Value - center;
                }
            }

            return currentLocations;
        }

        /// <summary>
        /// Calculates the total repulsion force.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="vertexLocation"></param>
        /// <param name="currentLocations">The current locations.</param>
        /// <returns>Vector.</returns>
        private static Vector CalculateTotalRepulsion([NotNull] Graph graph, Vertex vertex, Location vertexLocation, [NotNull] IReadOnlyDictionary<Vertex, Location> currentLocations)
        {
            var forces =
                from other in graph.Vertices
                where !other.Equals(vertex)
                let otherLocation = currentLocations[other]
                select GetRepulsionForce(vertexLocation, otherLocation);
            var force = forces.Aggregate(Vector.Zero, (current, sumOfForces) => sumOfForces + current);
            return force;
        }

        /// <summary>
        /// Calculates the total attraction force.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="vertexLocation"></param>
        /// <param name="currentLocations">The current locations.</param>
        /// <returns>Vector.</returns>
        private static Vector CalculateTotalAttraction([NotNull] Graph graph, Vertex vertex, Location vertexLocation, [NotNull] IReadOnlyDictionary<Vertex, Location> currentLocations)
        {
            var forces =
                from edges in graph[vertex]
                let other = edges.Other(vertex)
                let otherLocation = currentLocations[other]
                select GetAttractionForce(graph, vertex, vertexLocation, other, otherLocation);
            var force = forces.Aggregate(Vector.Zero, (current, sumOfForces) => sumOfForces + current);
            return force;
        }

        /// <summary>
        /// Creates the initial random locations.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns>ILookup&lt;Vertex, Location&gt;.</returns>
        [NotNull]
        private static Dictionary<Vertex, Location> CreateRandomLocations([NotNull] Graph graph)
        {
            var random = new Random();
            var initialLocations = graph.Vertices.ToDictionary(v => v, v => new Location(random.NextDouble(), random.NextDouble()));
            return initialLocations;
        }

        /// <summary>
        /// Gets the repulsion force.
        /// </summary>
        /// <param name="of">The node under observation.</param>
        /// <param name="from">The other node.</param>
        /// <param name="repulsionForce">The repulsion force.</param>
        /// <returns>The force vector.</returns>
        private static Vector GetRepulsionForce(Location of, Location @from, double repulsionForce = VertexRepulsionForceStrength)
        {
            // get the proximity and the direction
            double currentDistance;
            var direction = (of - @from).Normalized(out currentDistance);

            // strength decrease is proportional to the squared distance;
            // this imitates Coulomb's Law of the repulsion of charged particles (F = k*Q1*Q2/r^2)
            // where we assume the charge to be equal for all particles.
            var strength = repulsionForce / (currentDistance * currentDistance);

            // return the force vector
            return direction*strength;
        }

        /// <summary>
        /// Gets the attraction force.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="of">The node under observation.</param>
        /// <param name="locationOf">The location of.</param>
        /// <param name="from">The other node.</param>
        /// <param name="locationFrom">The location from.</param>
        /// <param name="attractionStrength">The attraction strength.</param>
        /// <returns>The force vector.</returns>
        private static Vector GetAttractionForce([NotNull] Graph graph, [NotNull] Vertex of, Location locationOf, [NotNull] Vertex from, Location locationFrom, double attractionStrength = VertexAttractionForceStrength)
        {
            // if vertices are unconnected, there is no force that
            // pulls them together.
            Edge edge;
            if (!graph.TryGetEdge(of, from, out edge)) return Vector.Zero;

            // the actual distance between the two vertices is given by the edge weight
            var expectedDistance = edge.Weight;

            // fetch the force direction and the actual distance
            double currentDistance;
            var direction = (locationOf - locationFrom).Normalized(out currentDistance);

            // determine the spring strength by Hooke's law:
            // If the expected distance is larger than the current distance, the spring is
            // too short and should thus expand; hence the attraction force is zero.
            // If the expected distance is smaller than the current distance, the spring needs
            // to contract, hence the strength is positive.
            var strength = attractionStrength * Math.Max(currentDistance - expectedDistance, 0);

            // In order to contract, we reverse the force direction
            return direction*(-strength);
        }
    }
}

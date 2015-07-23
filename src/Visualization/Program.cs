using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using widemeadows.Graphs.Model;
using widemeadows.Graphs.Visualization;

namespace widemeadows.Graphs
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var network = CreateGraph();
            var planner = new Planner();
            var locations = planner.Plan(network);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new MainForm(network, locations);
            form.NewSeed += (s, a) =>
                            {
                                var newNetwork = CreateGraph();
                                var newLocations = planner.Plan(newNetwork);
                                form.SetNetwork(newNetwork, newLocations);
                            };

            Application.Run(form);
        }

        /// <summary>
        /// Creates the graph.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;Vertex&gt;.</returns>
        private static Graph CreateGraph()
        {
            // return CreatePentagraph();
            return CreateGrid();
        }

        /// <summary>
        /// Creates the pentagram-shaped graph.
        /// </summary>
        /// <returns>Graph.</returns>
        private static Graph CreatePentagraph()
        {
            /*
                A
              B   C
               D E
             */

            var a = new Vertex<string>("a");
            var b = new Vertex<string>("b");
            var c = new Vertex<string>("c");
            var d = new Vertex<string>("d");
            var e = new Vertex<string>("e");
            return new Graph(
                // inner connections
                new Edge(b, c, 2.0D),
                new Edge(c, d, 2.0D),
                new Edge(d, a, 2.0D),
                new Edge(a, e, 2.0D),
                new Edge(e, b, 2.0D),
                // outer connections
                new Edge(a, c, 1.0D),
                new Edge(c, e, 1.0D),
                new Edge(e, d, 1.0D),
                new Edge(d, b, 1.0D),
                new Edge(b, a, 1.0D)
                );
        }

        /// <summary>
        /// Creates the grid.
        /// </summary>
        /// <returns>Graph.</returns>
        private static Graph CreateGrid()
        {
            const int rows = 5, columns = 5;

            // create the vertices
            var grid = new Vertex[rows, columns];
            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    grid[row, column] = new Vertex<string>(String.Format("{0},{1}", row, column));
                }
            }

            // prepare the edge storage
            var edges = new Collection<Edge>();

            // link the vertices horizontally
            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns - 1; ++column)
                {
                    var left = grid[row, column];
                    var right = grid[row, column + 1];

                    var weight = 1.5D * (row + column);

                    var edge = new Edge(left, right, weight);
                    edges.Add(edge);
                }
            }

            // link the vertices vertically
            for (int row = 0; row < rows - 1; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    var top = grid[row, column];
                    var bottom = grid[row+1, column];

                    var weight = 1.5D * (row + column);

                    var edge = new Edge(top, bottom, weight);
                    edges.Add(edge);
                }
            }

            return new Graph(edges);
        }
    }
}

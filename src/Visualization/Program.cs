using System;
using System.Linq;
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

            var a = locations.Single(pair => ((Vertex<string>) pair.Key).Data == "a").Value;
            var b = locations.Single(pair => ((Vertex<string>) pair.Key).Data == "b").Value;
            var c = locations.Single(pair => ((Vertex<string>) pair.Key).Data == "c").Value;
            var d = locations.Single(pair => ((Vertex<string>) pair.Key).Data == "d").Value;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(network, locations));
        }

        /// <summary>
        /// Creates the graph.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;Vertex&gt;.</returns>
        private static Graph CreateGraph()
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
    }
}

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
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Creates the graph.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;Vertex&gt;.</returns>
        private static Graph CreateGraph()
        {
            var a = new Vertex<string>("a");
            var b = new Vertex<string>("b");
            var c = new Vertex<string>("c");
            var d = new Vertex<string>("d");

            var ab = new Edge(a, b, 1.0D);
            var ac = new Edge(a, c, 1.0D);
            var cd = new Edge(c, d, 1.0D);
            var bd = new Edge(b, d, 1.0D);

            return new Graph(ab, ac, cd, bd);
        }
    }
}

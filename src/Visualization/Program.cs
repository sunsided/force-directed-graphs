using System;
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
            planner.Plan(network);

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
            var a = new Vertex();
            var b = new Vertex();
            var c = new Vertex();
            var d = new Vertex();

            var ab = new Edge(a, b, 1.0D);
            var ac = new Edge(a, c, 1.0D);
            var cd = new Edge(c, d, 1.0D);
            var bd = new Edge(b, d, 1.0D);

            return new Graph(ab, ac, cd, bd);
        }
    }
}

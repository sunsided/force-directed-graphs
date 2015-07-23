using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;
using widemeadows.Graphs.Model;

namespace widemeadows.Graphs.Visualization
{
    /// <summary>
    /// Class MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The network replacement lock object
        /// </summary>
        [NotNull]
        private readonly object _networkReplaceLock = new object();

        /// <summary>
        /// The network
        /// </summary>
        [NotNull]
        private Graph _network;

        /// <summary>
        /// The locations
        /// </summary>
        [NotNull]
        private IReadOnlyDictionary<Vertex, Location> _locations;

        /// <summary>
        /// The basic scale
        /// </summary>
        private const float BaseScale = 10.0F;

        /// <summary>
        /// The scale
        /// </summary>
        private float _scale = BaseScale;

        /// <summary>
        /// Determines if the left mouse button is down
        /// </summary>
        private bool _mouseLeftDown;

        /// <summary>
        /// The mouse location at left button down
        /// </summary>
        private Point _locationAtMouseLeftDown;

        /// <summary>
        /// The current mouse location during mouse down
        /// </summary>
        private Point _translateInPixels;

        /// <summary>
        /// The value range of the current network's edges
        /// </summary>
        private Range _edgeRange;

        /// <summary>
        /// Occurs when a new seed is requested.
        /// </summary>
        public event EventHandler NewSeed;

        /// <summary>
        /// Struct Range
        /// </summary>
        private struct Range
        {
            /// <summary>
            /// The minimum
            /// </summary>
            public readonly double Min;

            /// <summary>
            /// The maximum
            /// </summary>
            public readonly double Max;

            /// <summary>
            /// Initializes a new instance of the <see cref="Range"/> struct.
            /// </summary>
            /// <param name="min">The minimum.</param>
            /// <param name="max">The maximum.</param>
            public Range(double min, double max)
            {
                Min = min;
                Max = max;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm" /> class.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="locations">The locations.</param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public MainForm([NotNull] Graph network, [NotNull] IReadOnlyDictionary<Vertex, Location> locations)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.Opaque |
                ControlStyles.UserPaint,
                true);
            InitializeComponent();

            // mouse wheel passthrough for zooming while hovering over the button
            buttonRestart.MouseWheel += (sender, args) => OnMouseWheel(args);

            SetNetwork(network, locations);
        }

        /// <summary>
        /// Sets the specified network.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="locations">The locations.</param>
        public void SetNetwork([NotNull] Graph network, [NotNull] IReadOnlyDictionary<Vertex, Location> locations)
        {
            lock (_networkReplaceLock)
            {
                _network = network;
                _locations = locations;

                // determine the edge weight range
                _edgeRange = _network.Edges.Aggregate(
                    new Range(
                        min: Double.PositiveInfinity,
                        max: Double.NegativeInfinity),
                    (minmax, current) =>
                        new Range(
                            min: Math.Min(minmax.Min, current.Weight),
                            max: Math.Max(minmax.Max, current.Weight)
                            ));

                // re-enable the button
                buttonRestart.Enabled = true;
            }

            Invalidate();
        }

        /// <summary>
        /// Handles the <see cref="E:Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // obtain the graph and the locations
            Graph network;
            IReadOnlyDictionary<Vertex, Location> locations;
            lock (_networkReplaceLock)
            {
                network = _network;
                locations = _locations;
            }

            var gr = e.Graphics;

            var scale = _scale;
            var centerX = (ClientRectangle.Width/2 + ClientRectangle.Left/2) / scale;
            var centerY = (ClientRectangle.Height/2 + ClientRectangle.Top/2) / scale;
            var offsetX = _translateInPixels.X;
            var offsetY = _translateInPixels.Y;

            // edge value range
            var range = _edgeRange;

            // colorful things
            var vertexBrush = new SolidBrush(Color.SteelBlue);

            // push the graphics state
            var state = gr.Save();
            try
            {
                gr.CompositingQuality = CompositingQuality.HighQuality;
                gr.InterpolationMode = InterpolationMode.High;
                gr.SmoothingMode = SmoothingMode.HighQuality;

                gr.TranslateTransform(offsetX, offsetY);
                gr.ScaleTransform(scale, scale);
                gr.Clear(Color.WhiteSmoke);

                RenderEdges(locations, network, centerX, centerY, gr, range);
                RenderVertices(locations, centerX, centerY, gr, vertexBrush);
            }
            finally
            {
                // pop the graphics state
                gr.Restore(state);
            }
        }

        /// <summary>
        /// Renders the edges.
        /// </summary>
        /// <param name="locations">The locations.</param>
        /// <param name="network">The network.</param>
        /// <param name="centerX">The center x.</param>
        /// <param name="centerY">The center y.</param>
        /// <param name="gr">The gr.</param>
        /// <param name="edgePen">The edge pen.</param>
        private static void RenderEdges(IReadOnlyDictionary<Vertex, Location> locations, Graph network, float centerX, float centerY, Graphics gr, Range range)
        {
            foreach (var edge in network.Edges)
            {
                // determine the line start and end coordinates
                var startLocation = locations[edge.Left];
                var endLocation = locations[edge.Right];

                var start = new PointF(
                    (float) startLocation.X + centerX,
                    (float) startLocation.Y + centerY);
                var end = new PointF(
                    (float) endLocation.X + centerX,
                    (float) endLocation.Y + centerY);

                // colorful things
                var edgeColor = LerpColor(Color.DarkSlateGray, Color.OrangeRed, edge.Weight, range);
                var edgePen = new Pen(edgeColor, 0.1F);

                // actually do something
                gr.DrawLine(edgePen, start, end);
            }
        }

        /// <summary>
        /// Linearly interpolates the color.
        /// </summary>
        /// <param name="lowColor">Color of the low.</param>
        /// <param name="highColor">Color of the high.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="range">The range.</param>
        /// <returns>Color.</returns>
        private static Color LerpColor(Color lowColor, Color highColor, double weight, Range range)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (range.Max == range.Min) return lowColor;
            double normalizedValue = (weight - range.Min)/(range.Max - range.Min);

            var a = (normalizedValue)*highColor.A + (1.0D - normalizedValue)*lowColor.A;
            var r = (normalizedValue)*highColor.R + (1.0D - normalizedValue)*lowColor.R;
            var g = (normalizedValue)*highColor.G + (1.0D - normalizedValue)*lowColor.G;
            var b = (normalizedValue)*highColor.B + (1.0D - normalizedValue)*lowColor.B;

            var color = Color.FromArgb((int)a, (int)r, (int)g, (int)b);
            return color;
        }

        /// <summary>
        /// Renders the vertices.
        /// </summary>
        /// <param name="locations">The locations.</param>
        /// <param name="centerX">The center x.</param>
        /// <param name="centerY">The center y.</param>
        /// <param name="gr">The gr.</param>
        /// <param name="vertexBrush">The vertex brush.</param>
        private static void RenderVertices([NotNull] IReadOnlyDictionary<Vertex, Location> locations, float centerX, float centerY, Graphics gr, SolidBrush vertexBrush)
        {
            const float vertexWidth = 1F;

            foreach (var pair in locations)
            {
                var location = pair.Value;

                var rect = new RectangleF(
                    (float) location.X - vertexWidth/2F + centerX,
                    (float) location.Y - vertexWidth/2F + centerY,
                    vertexWidth,
                    vertexWidth);

                gr.FillEllipse(vertexBrush, rect);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:Resize" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
            {
                _scale *= 1.1F;
            }
            else if (e.Delta > 0)
            {
                _scale *= 0.9F;
            }

            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;

            _mouseLeftDown = true;
            _locationAtMouseLeftDown = new Point(
                e.Location.X - _translateInPixels.X,
                e.Location.Y - _translateInPixels.Y);
            _translateInPixels = new Point();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                _mouseLeftDown = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_mouseLeftDown) return;

            var location = e.Location;
            _translateInPixels = new Point(
                location.X - _locationAtMouseLeftDown.X,
                location.Y - _locationAtMouseLeftDown.Y);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseClick" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if ((e.Button & MouseButtons.Middle) != MouseButtons.Middle) return;

            _translateInPixels = new Point();
            _scale = BaseScale;
            Invalidate();
        }

        /// <summary>
        /// Handles the Click event of the buttonRestart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            // disable the button
            buttonRestart.Enabled = false;

            // remove the focus so scrolling works as expected
            Focus();

            OnNewSeed();
        }

        /// <summary>
        /// Called when a new seed is requested.
        /// </summary>
        protected virtual void OnNewSeed()
        {
            var handler = NewSeed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// The locations
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<Vertex, Location> _locations;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="locations">The locations.</param>
        public MainForm([NotNull] IReadOnlyDictionary<Vertex, Location> locations)
        {
            _locations = locations;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the <see cref="E:Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var gr = e.Graphics;
            var centerX = ClientRectangle.Width/2 + ClientRectangle.Left/2;
            var centerY = ClientRectangle.Height/2 + ClientRectangle.Top/2;
            var scale = 10F;
            var width = 2F;

            var locations = _locations;
            foreach (var pair in locations)
            {
                var location = pair.Value;

                var rect = new RectangleF(
                    (float) location.X*scale - scale*width/2F + centerX,
                    (float) location.Y*scale - scale*width/2F + centerY,
                    width*scale,
                    width*scale);
                gr.DrawEllipse(new Pen(Color.DarkBlue), rect);
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
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Terminals.Forms.Render
{
    /// <summary>
    ///     Set the clipping region until instance disposed.
    /// </summary>
    public class UseClipping : IDisposable
    {
        #region Instance Fields

        private readonly Graphics _g;
        private readonly Region _old;

        #endregion

        #region Identity

        /// <summary>
        ///     Initialize a new instance of the UseClipping class.
        /// </summary>
        /// <param name="g"> Graphics instance. </param>
        /// <param name="path"> Clipping path. </param>
        public UseClipping(Graphics g, GraphicsPath path)
        {
            this._g = g;
            this._old = g.Clip;
            Region clip = this._old.Clone();
            clip.Intersect(path);
            this._g.Clip = clip;
        }

        /// <summary>
        ///     Initialize a new instance of the UseClipping class.
        /// </summary>
        /// <param name="g"> Graphics instance. </param>
        /// <param name="region"> Clipping region. </param>
        public UseClipping(Graphics g, Region region)
        {
            this._g = g;
            this._old = g.Clip;
            Region clip = this._old.Clone();
            clip.Intersect(region);
            this._g.Clip = clip;
        }

        /// <summary>
        ///     Revert clipping back to origina setting.
        /// </summary>
        public void Dispose()
        {
            this._g.Clip = this._old;
        }

        #endregion
    }
}
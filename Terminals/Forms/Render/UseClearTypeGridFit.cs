using System;
using System.Drawing;
using System.Drawing.Text;

namespace Terminals.Forms.Render
{
    /// <summary>
    ///     Set the TextRenderingHint.ClearTypeGridFit until instance disposed.
    /// </summary>
    public class UseClearTypeGridFit : IDisposable
    {
        #region Instance Fields

        private readonly Graphics _g;
        private readonly TextRenderingHint _old;

        #endregion

        #region Identity

        /// <summary>
        ///     Initialize a new instance of the UseClearTypeGridFit class.
        /// </summary>
        /// <param name="g"> Graphics instance. </param>
        public UseClearTypeGridFit(Graphics g)
        {
            this._g = g;
            this._old = this._g.TextRenderingHint;
            this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        /// <summary>
        ///     Revert the TextRenderingHint back to original setting.
        /// </summary>
        public void Dispose()
        {
            this._g.TextRenderingHint = this._old;
        }

        #endregion
    }
}
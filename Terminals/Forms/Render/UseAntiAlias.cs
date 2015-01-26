/********************************************************************/
/*  Office 2007 Renderer Project                                    */
/*                                                                  */
/*  Use the Office2007Renderer class as a custom renderer by        */
/*  providing it to the ToolStripManager.Renderer property. Then    */
/*  all tool strips, menu strips, status strips etc will be drawn   */
/*  using the Office 2007 style renderer in your application.       */
/*                                                                  */
/*   Author: Phil Wright                                            */
/*  Website: www.componentfactory.com                               */
/*  Contact: phil.wright@componentfactory.com                       */
/********************************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Terminals.Forms.Render
{
    /// <summary>
    ///     Set the SmoothingMode=AntiAlias until instance disposed.
    /// </summary>
    public class UseAntiAlias : IDisposable
    {
        #region Instance Fields

        private readonly Graphics _g;
        private readonly SmoothingMode _old;

        #endregion

        #region Identity

        /// <summary>
        ///     Initialize a new instance of the UseAntiAlias class.
        /// </summary>
        /// <param name="g"> Graphics instance. </param>
        public UseAntiAlias(Graphics g)
        {
            this._g = g;
            this._old = this._g.SmoothingMode;
            this._g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        /// <summary>
        ///     Revert the SmoothingMode back to original setting.
        /// </summary>
        public void Dispose()
        {
            this._g.SmoothingMode = this._old;
        }

        #endregion
    }
}
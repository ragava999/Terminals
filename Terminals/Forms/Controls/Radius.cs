namespace Terminals.Forms.Controls
{
    public class Radius
    {
        #region Properties (4)
        public int BottomRight { get; set; }

        public int TopLeft { get; set; }

        public int TopRight { get; set; }

        public int BottomLeft { get; set; }
        #endregion

        #region Constructor (1)
        public Radius()
        {
            BottomRight = 0;
            TopLeft = 0;
            TopRight = 0;
            BottomLeft = 0;
        }
        #endregion
    }
}

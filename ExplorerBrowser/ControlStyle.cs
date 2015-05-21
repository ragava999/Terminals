/*
 * Created by Oliver Kohl D.Sc.
 * E-Mail: oliver@kohl.bz
 * Date: 12.09.2012
 * Time: 19:54
 * 
 */
namespace ExplorerBrowser
{
	/// <summary>
	/// Sets the explorer style for the explorer window alignment.
	/// </summary>
	public enum ControlStyle
    {
    	//  +---+---+
    	//  |       |
    	//  +       +
    	//  |       |
    	//  +---+---+
    	/// <summary>
    	/// One big panel, like the Microsoft Windows explorer itself.
    	/// </summary>
    	Single = 0,
		//  +---+---+
    	//  |   |   |
    	//  +   +   +
    	//  |   |   |
    	//  +---+---+
    	/// <summary>
    	/// Two explorer windows, whereas the tile is vertical.
    	/// </summary>
    	DualVertical,
    	//  +---+---+
    	//  |       |
    	//  +---+---+
    	//  |       |
    	//  +---+---+
    	/// <summary>
    	/// Two explorer windows, whereas the tile is horizontal.
    	/// </summary>
    	DualHorizontal,
    	//  +---+---+
    	//  |       |
    	//  +---+---+
    	//  |   |   |
    	//  +---+---+
    	/// <summary>
    	/// Three explorer windows, only one at the top.
    	/// </summary>
    	TrippleTop,
    	//  +---+---+
    	//  |   |   |
    	//  +---+---+
    	//  |       |
    	//  +---+---+
    	/// <summary>
    	/// Two explorer windows, only one at the bottom.
    	/// </summary>
    	TrippleBottom,
    	//  +--+-+--+
    	//  |  | |  |
    	//  +---+---+
    	/// <summary>
    	/// Three explorer windows, whereas the tile is vertical.
    	/// </summary>
    	TrippleVertical,
    	//  +---+---+
    	//  +---+---+
        //  +---+---+
    	//  +---+---+
    	/// <summary>
    	/// Three explorer windows, whereas the tile is horizontal.
    	/// </summary>
    	TrippleHorizontal,
    	//  +---+---+
    	//  |   |   |
    	//  +   +---+
    	//  |   |   |
    	//  +---+---+
    	/// <summary>
    	/// Three explorer windows, one in the left pane.
    	/// </summary>
    	TrippleLeft,
    	//  +---+---+
    	//  |   |   |
    	//  +---+   +
    	//  |   |   |
    	//  +---+---+
    	/// <summary>
    	/// Three explorer windows, one in the right pane.
    	/// </summary>
    	TrippleRight,
    	//  +---+---+
    	//  |   |   |
    	//  +---+---+
    	//  |   |   |
    	//  +---+---+
			/// <summary>
    	/// Four equal sized explorer windows arranged.
    	/// </summary>
   		Quad,
    	//  +-+-+-+-+
    	//  | | | | |
    	//  +---+---+
    	/// <summary>
    	/// Four explorer windows, whereas the tile is vertical.
    	/// </summary>
    	QuadVertical,
    	//  +---+---+
    	//  +---+---+
    	//  +---+---+
    	//  +---+---+
        //  +---+---+
    	/// <summary>
    	/// Four explorer windows, whereas the tile is horizontal.
    	/// </summary>
    	QuadHorizontal
    }
}

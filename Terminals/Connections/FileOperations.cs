namespace Terminals.Connections
{
    public enum FileOperations
    {
        /// <summary>
        /// Move the files specified in pFrom to the location specified in pTo. 
        /// </summary>
        Move = 0x0001,
        /// <summary>
        /// Copy the files specified in the pFrom member to the location specified in the pTo member. 
        /// </summary>
        Copy = 0x0002,
        /// <summary>
        /// Delete the files specified in pFrom. 
        /// </summary>
        Delete = 0x0003,
        /// <summary>
        /// Rename the file specified in pFrom. You cannot use this flag to rename
        /// multiple files with a single function call. Use FO_MOVE instead. 
        /// </summary>
        Rename = 0x0004
    }
}
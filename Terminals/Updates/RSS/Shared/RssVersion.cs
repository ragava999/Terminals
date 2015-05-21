namespace Terminals.Updates.RSS.Shared
{
    /// <summary>
    ///     All RSS versions
    /// </summary>
    [System.Serializable]
    public enum RssVersion
    {
        /// <summary>
        ///     Not defined
        /// </summary>
        Empty,

        /// <summary>
        ///     Version is not directly supported
        /// </summary>
        NotSupported,

        /// <summary>
        ///     RDF Site Summary (RSS) 0.9
        /// </summary>
        RSS090,

        /// <summary>
        ///     Rich Site Summary (RSS) 0.91
        /// </summary>
        RSS091,

        /// <summary>
        ///     Rich Site Summary (RSS) 0.92
        /// </summary>
        RSS092,

        /// <summary>
        ///     RDF Site Summary (RSS) 1.0
        /// </summary>
        RSS10,

        /// <summary>
        ///     Really Simple Syndication (RSS) 2.0
        /// </summary>
        RSS20
    }
}
using System;

namespace Terminals.Network.TraceRoute
{
    /// <summary>
    ///     Contains data for the Completed event of TraceRoute.
    /// </summary>
    public class RouteHopFoundEventArgs : EventArgs
    {
        public RouteHopFoundEventArgs(TraceRouteHopData hop, Boolean isLast)
        {
            this.Hop = hop;
            this.IsLastNode = isLast;
        }

        /// <summary>
        ///     Gets or sets whether the value of the hop property is the last hop in the trace.
        /// </summary>
        private bool IsLastNode { get; set; }

        /// <summary>
        ///     The hop encountered during the route tracing.
        /// </summary>
        private TraceRouteHopData Hop { get; set; }
    }
}
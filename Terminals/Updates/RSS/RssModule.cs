/* RssModule.cs
 * ============
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright © 2002, 2003 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/

using System;
using System.Collections;
using Terminals.Updates.RSS.Collections;
using Terminals.Updates.RSS.Shared;

namespace Terminals.Updates.RSS
{
    /// <summary>
    ///     Base class for all RSS modules
    /// </summary>
    [System.Serializable]
    public abstract class RssModule
    {
        private readonly ArrayList alBindTo = new ArrayList();
        private readonly RssModuleItemCollection rssChannelExtensions = new RssModuleItemCollection();
        private readonly RssModuleItemCollectionCollection rssItemExtensions = new RssModuleItemCollectionCollection();
        private const string SNamespacePrefix = RssDefault.String;
        private readonly Uri uriNamespaceUrl = RssDefault.Uri;

        /// <summary>
        ///     Collection of RSSModuleItem that are to be placed in the channel
        /// </summary>
        public RssModuleItemCollection ChannelExtensions
        {
            get { return this.rssChannelExtensions; }
        }

        /// <summary>
        ///     Collection of RSSModuleItemCollection that are to be placed in the channel item
        /// </summary>
        public RssModuleItemCollectionCollection ItemExtensions
        {
            get { return this.rssItemExtensions; }
        }

        /// <summary>
        ///     Prefix for the given module namespace
        /// </summary>
        public string NamespacePrefix
        {
            get { return SNamespacePrefix; }
        }

        /// <summary>
        ///     URL for the given module namespace
        /// </summary>
        public Uri NamespaceUrl
        {
            get { return this.uriNamespaceUrl; }
        }

        /// <summary>
        ///     Check if a particular channel is bound to this module
        /// </summary>
        /// <param name="channelHashCode"> Hash code of the channel </param>
        /// <returns> true if this channel is bound to this module, otherwise false </returns>
        public bool IsBoundTo(int channelHashCode)
        {
            return (this.alBindTo.BinarySearch(0, this.alBindTo.Count, channelHashCode, null) >= 0);
        }
    }
}
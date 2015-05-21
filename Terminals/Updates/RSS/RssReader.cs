/* RssReader.cs
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
using System.IO;
using System.Text;
using System.Xml;
using Terminals.Updates.RSS.Collections;
using Terminals.Updates.RSS.RssChannel;
using Terminals.Updates.RSS.RssItem;
using Terminals.Updates.RSS.Shared;

namespace Terminals.Updates.RSS
{
    /// <summary>
    ///     Reads an RSS file.
    /// </summary>
    /// <remarks>
    ///     Provides fast, non-cached, forward-only access to RSS data.
    /// </remarks>
    public class RssReader
    {
        

        private readonly ExceptionCollection exceptions = new ExceptionCollection();
        private RssCategory category;

        private RssChannel.RssChannel channel;
        private RssCloud cloud;
        private StringBuilder elementText = new StringBuilder();
        private RssEnclosure enclosure;
        private RssGuid guid;
        private RssImage image;
        private RssItem.RssItem item;
        private XmlTextReader reader;
        private RssVersion rssVersion = RssVersion.Empty;
        private RssSource source;
        private RssTextInput textInput;
        private bool wroteChannel;
        private Stack xmlNodeStack = new Stack();

        /// <summary>
        ///     A collection of all exceptions the RssReader class has encountered.
        /// </summary>
        public ExceptionCollection Exceptions
        {
            get { return this.exceptions; }
        }

        /// <summary>
        ///     Gets the RSS version of the stream.
        /// </summary>
        /// <value> One of the <see cref="RssVersion" /> values. </value>
        public RssVersion Version
        {
            get { return this.rssVersion; }
        }

        private void InitReader()
        {
            this.reader.WhitespaceHandling = WhitespaceHandling.None;
            this.reader.XmlResolver = null;
        }

        /// <summary>
        ///     Reads the next RssElement from the stream.
        /// </summary>
        /// <returns> An RSS Element </returns>
        /// <exception cref="InvalidOperationException">RssReader has been closed, and can not be read.</exception>
        /// <exception cref="System.IO.FileNotFoundException">RSS file not found.</exception>
        /// <exception cref="System.Xml.XmlException">Invalid XML syntax in RSS file.</exception>
        /// <exception cref="System.IO.EndOfStreamException">Unable to read an RssElement. Reached the end of the stream.</exception>
        public RssElement Read()
        {
            bool readData = false;
            RssElement rssElement = null;
            int lineNumber = -1;
            int linePosition = -1;

            if (this.reader == null)
                throw new InvalidOperationException("RssReader has been closed, and can not be read.");

            do
            {
                bool pushElement = true;
                try
                {
                    readData = this.reader.Read();
                }
                catch (EndOfStreamException e)
                {
                    throw new EndOfStreamException("Unable to read an RssElement. Reached the end of the stream.", e);
                }
                catch (XmlException e)
                {
                    if (lineNumber != -1 || linePosition != -1)
                        if (this.reader.LineNumber == lineNumber && this.reader.LinePosition == linePosition)
                            throw this.exceptions.LastException;

                    lineNumber = this.reader.LineNumber;
                    linePosition = this.reader.LinePosition;

                    this.exceptions.Add(e); // just add to list of exceptions and continue :)
                }
                if (readData)
                {
                    string readerName = this.reader.Name.ToLower();
                    switch (this.reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                //if (reader.IsEmptyElement)
                                //	break;
                                // doesnt take empty elements into account :/
                                this.elementText = new StringBuilder();
                                switch (readerName)
                                {
                                    case "item":
                                        // is this the end of the channel element? (absence of </channel> before <item>)
                                        if (!this.wroteChannel)
                                        {
                                            this.wroteChannel = true;
                                            rssElement = this.channel; // return RssChannel
                                            readData = false;
                                        }
                                        this.item = new RssItem.RssItem(); // create new RssItem
                                        this.channel.Items.Add(this.item);
                                        break;
                                    case "source":
                                        this.source = new RssSource();
                                        this.item.Source = this.source;
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            switch (this.reader.Name.ToLower())
                                            {
                                                case "url":
                                                    try
                                                    {
                                                        this.source.Url = new Uri(this.reader.Value);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        this.exceptions.Add(e);
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "enclosure":
                                        this.enclosure = new RssEnclosure();
                                        this.item.Enclosure = this.enclosure;
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            switch (this.reader.Name.ToLower())
                                            {
                                                case "url":
                                                    try
                                                    {
                                                        this.enclosure.Url = new Uri(this.reader.Value);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        this.exceptions.Add(e);
                                                    }
                                                    break;
                                                case "length":
                                                    try
                                                    {
                                                        this.enclosure.Length = int.Parse(this.reader.Value);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        this.exceptions.Add(e);
                                                    }
                                                    break;
                                                case "type":
                                                    this.enclosure.Type = this.reader.Value;
                                                    break;
                                            }
                                        }
                                        break;
                                    case "guid":
                                        this.guid = new RssGuid();
                                        this.item.Guid = this.guid;
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            switch (this.reader.Name.ToLower())
                                            {
                                                case "ispermalink":
                                                    try
                                                    {
                                                        this.guid.PermaLink = bool.Parse(this.reader.Value);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        this.exceptions.Add(e);
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "category":
                                        this.category = new RssCategory();
                                        if ((string) this.xmlNodeStack.Peek() == "channel")
                                            this.channel.Categories.Add(this.category);
                                        else
                                            this.item.Categories.Add(this.category);
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            switch (this.reader.Name.ToLower())
                                            {
                                                case "url":
                                                    goto case "domain";
                                                case "domain":
                                                    this.category.Domain = this.reader.Value;
                                                    break;
                                            }
                                        }
                                        break;
                                    case "channel":
                                        this.channel = new RssChannel.RssChannel();
                                        this.textInput = null;
                                        this.image = null;
                                        this.cloud = null;
                                        this.source = null;
                                        this.enclosure = null;
                                        this.category = null;
                                        this.item = null;
                                        break;
                                    case "image":
                                        this.image = new RssImage();
                                        this.channel.Image = this.image;
                                        break;
                                    case "textinput":
                                        this.textInput = new RssTextInput();
                                        this.channel.TextInput = this.textInput;
                                        break;
                                    case "cloud":
                                        pushElement = false;
                                        this.cloud = new RssCloud();
                                        this.channel.Cloud = this.cloud;
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            switch (this.reader.Name.ToLower())
                                            {
                                                case "domain":
                                                    this.cloud.Domain = this.reader.Value;
                                                    break;
                                                case "port":
                                                    try
                                                    {
                                                        this.cloud.Port = ushort.Parse(this.reader.Value);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        this.exceptions.Add(e);
                                                    }
                                                    break;
                                                case "path":
                                                    this.cloud.Path = this.reader.Value;
                                                    break;
                                                case "registerprocedure":
                                                    this.cloud.RegisterProcedure = this.reader.Value;
                                                    break;
                                                case "protocol":
                                                    switch (this.reader.Value.ToLower())
                                                    {
                                                        case "xml-rpc":
                                                            this.cloud.Protocol = RssCloudProtocol.XmlRpc;
                                                            break;
                                                        case "soap":
                                                            this.cloud.Protocol = RssCloudProtocol.Soap;
                                                            break;
                                                        case "http-post":
                                                            this.cloud.Protocol = RssCloudProtocol.HttpPost;
                                                            break;
                                                        default:
                                                            this.cloud.Protocol = RssCloudProtocol.Empty;
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "rss":
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            if (this.reader.Name.ToLower() == "version")
                                                switch (this.reader.Value)
                                                {
                                                    case "0.91":
                                                        this.rssVersion = RssVersion.RSS091;
                                                        break;
                                                    case "0.92":
                                                        this.rssVersion = RssVersion.RSS092;
                                                        break;
                                                    case "2.0":
                                                        this.rssVersion = RssVersion.RSS20;
                                                        break;
                                                    default:
                                                        this.rssVersion = RssVersion.NotSupported;
                                                        break;
                                                }
                                        }
                                        break;
                                    case "rdf":
                                        for (int i = 0; i < this.reader.AttributeCount; i++)
                                        {
                                            this.reader.MoveToAttribute(i);
                                            if (this.reader.Name.ToLower() == "version")
                                                switch (this.reader.Value)
                                                {
                                                    case "0.90":
                                                        this.rssVersion = RssVersion.RSS090;
                                                        break;
                                                    case "1.0":
                                                        this.rssVersion = RssVersion.RSS10;
                                                        break;
                                                    default:
                                                        this.rssVersion = RssVersion.NotSupported;
                                                        break;
                                                }
                                        }
                                        break;
                                }
                                if (pushElement)
                                    this.xmlNodeStack.Push(readerName);
                                break;
                            }
                        case XmlNodeType.EndElement:
                            {
                                if (this.xmlNodeStack.Count == 1)
                                    break;
                                string childElementName = (string) this.xmlNodeStack.Pop();
                                string parentElementName = (string) this.xmlNodeStack.Peek();
                                switch (childElementName) // current element
                                {
                                        // item classes
                                    case "item":
                                        rssElement = this.item;
                                        readData = false;
                                        break;
                                    case "source":
                                        this.source.Name = this.elementText.ToString();
                                        rssElement = this.source;
                                        readData = false;
                                        break;
                                    case "enclosure":
                                        rssElement = this.enclosure;
                                        readData = false;
                                        break;
                                    case "guid":
                                        this.guid.Name = this.elementText.ToString();
                                        rssElement = this.guid;
                                        readData = false;
                                        break;
                                    case "category": // parent is either item or channel
                                        this.category.Name = this.elementText.ToString();
                                        rssElement = this.category;
                                        readData = false;
                                        break;
                                        // channel classes
                                    case "channel":
                                        if (this.wroteChannel)
                                            this.wroteChannel = false;
                                        else
                                        {
                                            this.wroteChannel = true;
                                            rssElement = this.channel;
                                            readData = false;
                                        }
                                        break;
                                    case "textinput":
                                        rssElement = this.textInput;
                                        readData = false;
                                        break;
                                    case "image":
                                        rssElement = this.image;
                                        readData = false;
                                        break;
                                    case "cloud":
                                        rssElement = this.cloud;
                                        readData = false;
                                        break;
                                }
                                switch (parentElementName) // parent element
                                {
                                    case "item":
                                        switch (childElementName)
                                        {
                                            case "title":
                                                this.item.Title = this.elementText.ToString();
                                                break;
                                            case "link":
                                                this.item.Link = new Uri(this.elementText.ToString());
                                                break;
                                            case "description":
                                                this.item.Description = this.elementText.ToString();
                                                break;
                                            case "author":
                                                this.item.Author = this.elementText.ToString();
                                                break;
                                            case "comments":
                                                this.item.Comments = this.elementText.ToString();
                                                break;
                                            case "pubdate":
                                                try
                                                {
                                                    this.item.PubDate = DateTime.Parse(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    try
                                                    {
                                                        string tmp = this.elementText.ToString();
                                                        tmp = tmp.Substring(0, tmp.Length - 5);
                                                        tmp += "GMT";
                                                        this.item.PubDate = DateTime.Parse(tmp);
                                                    }
                                                    catch
                                                    {
                                                        this.exceptions.Add(e);
                                                    }
                                                }
                                                break;
                                        }
                                        break;
                                    case "channel":
                                        switch (childElementName)
                                        {
                                            case "title":
                                                this.channel.Title = this.elementText.ToString();
                                                break;
                                            case "link":
                                                try
                                                {
                                                    this.channel.Link = new Uri(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                            case "description":
                                                this.channel.Description = this.elementText.ToString();
                                                break;
                                            case "language":
                                                this.channel.Language = this.elementText.ToString();
                                                break;
                                            case "copyright":
                                                this.channel.Copyright = this.elementText.ToString();
                                                break;
                                            case "managingeditor":
                                                this.channel.ManagingEditor = this.elementText.ToString();
                                                break;
                                            case "webmaster":
                                                this.channel.WebMaster = this.elementText.ToString();
                                                break;
                                            case "rating":
                                                this.channel.Rating = this.elementText.ToString();
                                                break;
                                            case "pubdate":
                                                try
                                                {
                                                    this.channel.PubDate = DateTime.Parse(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                            case "lastbuilddate":
                                                try
                                                {
                                                    this.channel.LastBuildDate =
                                                        DateTime.Parse(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                            case "generator":
                                                this.channel.Generator = this.elementText.ToString();
                                                break;
                                            case "docs":
                                                this.channel.Docs = this.elementText.ToString();
                                                break;
                                            case "ttl":
                                                try
                                                {
                                                    this.channel.TimeToLive = int.Parse(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                        }
                                        break;
                                    case "image":
                                        switch (childElementName)
                                        {
                                            case "url":
                                                try
                                                {
                                                    this.image.Url = new Uri(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                            case "title":
                                                this.image.Title = this.elementText.ToString();
                                                break;
                                            case "link":
                                                try
                                                {
                                                    this.image.Link = new Uri(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                            case "description":
                                                this.image.Description = this.elementText.ToString();
                                                break;
                                            case "width":
                                                try
                                                {
                                                    this.image.Width = Byte.Parse(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                            case "height":
                                                try
                                                {
                                                    this.image.Height = Byte.Parse(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                        }
                                        break;
                                    case "textinput":
                                        switch (childElementName)
                                        {
                                            case "title":
                                                this.textInput.Title = this.elementText.ToString();
                                                break;
                                            case "description":
                                                this.textInput.Description = this.elementText.ToString();
                                                break;
                                            case "name":
                                                this.textInput.Name = this.elementText.ToString();
                                                break;
                                            case "link":
                                                try
                                                {
                                                    this.textInput.Link = new Uri(this.elementText.ToString());
                                                }
                                                catch (Exception e)
                                                {
                                                    this.exceptions.Add(e);
                                                }
                                                break;
                                        }
                                        break;
                                    case "skipdays":
                                        if (childElementName == "day")
                                            switch (this.elementText.ToString().ToLower())
                                            {
                                                case "monday":
                                                    this.channel.SkipDays[0] = true;
                                                    break;
                                                case "tuesday":
                                                    this.channel.SkipDays[1] = true;
                                                    break;
                                                case "wednesday":
                                                    this.channel.SkipDays[2] = true;
                                                    break;
                                                case "thursday":
                                                    this.channel.SkipDays[3] = true;
                                                    break;
                                                case "friday":
                                                    this.channel.SkipDays[4] = true;
                                                    break;
                                                case "saturday":
                                                    this.channel.SkipDays[5] = true;
                                                    break;
                                                case "sunday":
                                                    this.channel.SkipDays[6] = true;
                                                    break;
                                            }
                                        break;
                                    case "skiphours":
                                        if (childElementName == "hour")
                                            this.channel.SkipHours[Byte.Parse(this.elementText.ToString().ToLower())] =
                                                true;
                                        break;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            this.elementText.Append(this.reader.Value);
                            break;
                        case XmlNodeType.CDATA:
                            this.elementText.Append(this.reader.Value);
                            break;
                    }
                }
            } while (readData);
            return rssElement;
        }

        /// <summary>
        ///     Closes connection to file.
        /// </summary>
        /// <remarks>
        ///     This method also releases any resources held while reading.
        /// </remarks>
        public void Close()
        {
            this.textInput = null;
            this.image = null;
            this.cloud = null;
            this.channel = null;
            this.source = null;
            this.enclosure = null;
            this.category = null;
            this.item = null;
            if (this.reader != null)
            {
                this.reader.Close();
                this.reader = null;
            }
            this.elementText = null;
            this.xmlNodeStack = null;
        }

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the RssReader class with the specified URL or filename.
        /// </summary>
        /// <param name="url"> The URL or filename for the file containing the RSS data. </param>
        /// <exception cref="ArgumentException">Occures when unable to retrieve file containing the RSS data.</exception>
        public RssReader(string url)
        {
            try
            {
                this.reader = new XmlTextReader(url);
                this.InitReader();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to retrieve file containing the RSS data.", e);
            }
        }

        /// <summary>
        ///     Creates an instance of the RssReader class using the specified TextReader.
        /// </summary>
        /// <param name="textReader"> specified TextReader </param>
        /// <exception cref="ArgumentException">Occures when unable to retrieve file containing the RSS data.</exception>
        public RssReader(TextReader textReader)
        {
            try
            {
                this.reader = new XmlTextReader(textReader);
                this.InitReader();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to retrieve file containing the RSS data.", e);
            }
        }

        /// <summary>
        ///     Creates an instance of the RssReader class using the specified Stream.
        /// </summary>
        /// <exception cref="ArgumentException">Occures when unable to retrieve file containing the RSS data.</exception>
        /// <param name="stream"> Stream to read from </param>
        public RssReader(Stream stream)
        {
            try
            {
                this.reader = new XmlTextReader(stream);
                this.InitReader();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to retrieve file containing the RSS data.", e);
            }
        }

        #endregion
    }
}
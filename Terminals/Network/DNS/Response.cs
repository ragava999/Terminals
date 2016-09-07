namespace Terminals.Network.DNS
{
    using System;

    using Kohl.Framework.Logging;

    /// <summary>
    ///     A Response is a logical representation of the byte data returned from a DNS query
    /// </summary>
    public class Response
    {
        // these are fields we're interested in from the message
        private readonly AdditionalRecord[] additionalRecords;
        private readonly Answer[] answers;
        private readonly NameServer[] nameServers;
        private readonly Question[] questions;

        /// <summary>
        ///     Construct a Response object from the supplied byte array
        /// </summary>
        /// <param name="message"> a byte array returned from a DNS server query </param>
        public Response(byte[] message)
        {
            // create the arrays of response objects
            this.questions = new Question[GetShort(message, 4)];
            this.answers = new Answer[GetShort(message, 6)];
            this.nameServers = new NameServer[GetShort(message, 8)];
            this.additionalRecords = new AdditionalRecord[GetShort(message, 10)];

            // need a pointer to do this, position just after the header
            Pointer pointer = new Pointer(message, 12);

            // and now populate them, they always follow this order
            for (int index = 0; index < this.questions.Length; index++)
            {
                try
                {
                    // try to build a quesion from the response
                    this.questions[index] = new Question(pointer);
                }
                catch (Exception ex)
                {
					Log.Error("A DNS response question failure occured.", ex);
                    // something grim has happened, we can't continue
                    throw new InvalidResponseException(ex);
                }
            }
            for (int index = 0; index < this.answers.Length; index++)
            {
                this.answers[index] = new Answer(pointer);
            }
            for (int index = 0; index < this.nameServers.Length; index++)
            {
                this.nameServers[index] = new NameServer(pointer);
            }
            for (int index = 0; index < this.additionalRecords.Length; index++)
            {
                this.additionalRecords[index] = new AdditionalRecord(pointer);
            }
        }

        public Answer[] Answers
        {
            get { return this.answers; }
        }

        /// <summary>
        ///     Convert 2 bytes to a short. It would have been nice to use BitConverter for this,
        ///     it however reads the bytes in the wrong order (at least on Windows)
        /// </summary>
        /// <param name="message"> byte array to look in </param>
        /// <param name="position"> position to look at </param>
        /// <returns> short representation of the two bytes </returns>
        private static short GetShort(byte[] message, int position)
        {
            return (short) (message[position] << 8 | message[position + 1]);
        }
    }
}
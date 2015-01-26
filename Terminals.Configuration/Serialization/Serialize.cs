using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Terminals.Configuration.Serialization
{
    public static class Serialize
    {
        public static MemoryStream SerializeBinary(object request)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream1 = new MemoryStream();
            binaryFormatter.Serialize(memoryStream1, request);
            return memoryStream1;
        }

        public static object DeSerializeBinary(MemoryStream memStream)
        {
            memStream.Position = 0;
            object local1 = new BinaryFormatter().Deserialize(memStream);
            memStream.Close();
            return local1;
        }

        public static void SerializeXmlToDisk(object request, string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            File.WriteAllText(filename, SerializeXmlAsString(request), Encoding.UTF8);
        }

        private static string SerializeXmlAsString(object request)
        {
            using (MemoryStream stream = SerializeXml(request))
            {
                if (stream == null)
                    return null;

                Encoding eng = Encoding.UTF8;
                return eng.GetString(StreamToBytes(stream));
            }
        }

        private static byte[] StreamToBytes(Stream stream)
        {
            byte[] buffer = null;
            if (stream.Position > 0 && stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
            buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        private static MemoryStream SerializeXml(object request)
        {
            return SerializeXml(request, request.GetType());
        }

        public static MemoryStream SerializeXml(object request, Type type, bool throwException)
        {
            MemoryStream memoryStream2;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                MemoryStream memoryStream1 = new MemoryStream();
                xmlSerializer.Serialize(memoryStream1, request);
                memoryStream2 = memoryStream1;
            }
            catch (Exception exc)
            {
                memoryStream2 = null;
                if (throwException)
                    throw exc;
            }

            return memoryStream2;
        }

        private static MemoryStream SerializeXml(object request, Type type)
        {
            MemoryStream memoryStream2;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                MemoryStream memoryStream1 = new MemoryStream();
                xmlSerializer.Serialize(memoryStream1, request);
                memoryStream2 = memoryStream1;
            }
            catch (Exception)
            {
                memoryStream2 = null;
            }

            return memoryStream2;
        }

        public static object DeserializeXmlFromDisk(string filename, Type type)
        {
            if (File.Exists(filename))
            {
                string contents = null;
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
                {
                    using (StreamReader stream = new StreamReader(fs))
                    {
                        contents = stream.ReadToEnd();
                    }
                }

                return DeSerializeXml(contents, type);
            }

            return Activator.CreateInstance(type);
        }

        public static object DeSerializeXml(string envelope, Type type, bool throwException)
        {
            object local2;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(envelope));
                object local1 = xmlSerializer.Deserialize(memoryStream);
                memoryStream.Close();
                local2 = local1;
            }
            catch (Exception exc)
            {
                local2 = null;
                if (throwException)
                    throw exc;
            }

            return local2;
        }

        public static object DeSerializeXml(string envelope, Type type)
        {
            object local2;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(envelope));
                object local1 = xmlSerializer.Deserialize(memoryStream);
                memoryStream.Close();
                local2 = local1;
            }
            catch (Exception)
            {
                local2 = null;
            }

            return local2;
        }
    }
}
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.IO;
using System.Xml;

namespace BizTalkComponents.OrchestrationUtils
{
    public class XLANGMessageHandler : IDisposable
    {
        private XLANGMessage _message;
        private bool _disposed;

        public XLANGMessageHandler(XLANGMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Count == 0)
                throw new ArgumentException("The XLANGMessage does not contain any parts", "message");

            _message = message;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_message != null) _message.Dispose();
                _message = null;
            }
            _disposed = true;
        }

        public void LoadFromBase64(string base64Content)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            if (base64Content == null) throw new ArgumentNullException("base64Content");

            var writer = new BinaryWriter(new VirtualStream());
            writer.Write(Convert.FromBase64String(base64Content));
            LoadFromStream(writer.BaseStream);
        }

        public string RetrieveAsBase64()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            using (var stream = RetrieveAs<Stream>())
            {
                using (var reader = new BinaryReader(stream))
                {
                    return Convert.ToBase64String(reader.ReadBytes((int)stream.Length));
                }
            }
        }

        public void LoadFromString(string content)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            if (content == null) throw new ArgumentNullException("content");

            var writer = new StreamWriter(new VirtualStream());
            writer.Write(content);
            writer.Flush();
            LoadFromStream(writer.BaseStream);
        }

        public string RetrieveAsString()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            Stream stream = null;
            try
            {
                stream = RetrieveAs<Stream>();
                using (var reader = new StreamReader(stream))
                {
                    stream = null;
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }
        }

        public void LoadFromStream(Stream stream)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            if (stream == null) throw new ArgumentNullException("stream");

            Stream seekableStream = stream.CanSeek ? stream : new ReadOnlySeekableStream(stream);
            seekableStream.Seek(0, SeekOrigin.Begin);
            LoadFrom(seekableStream);
        }

        public Stream RetrieveAsStream()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            return RetrieveAs<Stream>();
        }

        public void LoadFrom(object source)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            if (source == null) throw new ArgumentNullException("source");

            _message[0].LoadFrom(source);
        }

        public T RetrieveAs<T>()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            return (T)_message[0].RetrieveAs(typeof(T));
        }

        public string GetRootNodeName()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            using (var xmlReader = RetrieveAs<XmlReader>())
            {
                while (!xmlReader.IsStartElement()) xmlReader.Read();
                return xmlReader.LocalName;
            }
        }

        /// <summary>
        /// Loads a Base64 encoded string into an empty message
        /// Usage in the Message Assignment Shape:
        /// <example>
        /// <code>
        /// // Initialize the message
        /// someMessage = null;
        /// // Load the content
        /// Microsoft.XLANGs.StreamingSDK.LoadContentFromString(someMessage, someBase64String);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">An empty message</param>
        /// <param name="base64Content">The Base64 encoded string to load</param>
        /// <exception cref="System.ArgumentNullException">Thrown when message or base64Content is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static void LoadContentFromBase64(XLANGMessage message, string base64Content)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                messageHandler.LoadFromBase64(base64Content);
            }
        }

        /// <summary>
        /// Returns the content of a message as a Base64 encoded string
        /// The message is not read so can handle binary content
        /// <example>
        /// <code>
        /// string base64String = Microsoft.XLANGs.StreamingSDK.XLANGMessageHandler.RetrieveContentAsBase64(someMessage);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">An XLANG message</param>
        /// <returns>A Base64 encoded string</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static string RetrieveContentAsBase64(XLANGMessage message)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                return messageHandler.RetrieveAsBase64();
            }
        }

        /// <summary>
        /// Loads a string into an empty message
        /// Usage in the Message Assignment Shape:
        /// <example>
        /// <code>
        /// // Initialize the message
        /// someMessage = null;
        /// // Load the content
        /// Microsoft.XLANGs.StreamingSDK.XLANGMessageHandler.LoadContentFromString(someMessage, someString);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">An empty message</param>
        /// <param name="content">The string to load</param>
        /// <exception cref="System.ArgumentNullException">Thrown when message or content is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static void LoadContentFromString(XLANGMessage message, string content)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                messageHandler.LoadFromString(content);
            }
        }

        /// <summary>
        /// Returns the content of a message as a string
        /// <example>
        /// <code>
        /// string contentString = Microsoft.XLANGs.StreamingSDK.XLANGMessageHandler.RetrieveContentAsString(someMessage);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">An XLANG message</param>
        /// <returns>A string</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static string RetrieveContentAsString(XLANGMessage message)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                return messageHandler.RetrieveAsString();
            }
        }

        /// <summary>
        /// Loads a stream into an empty message
        /// Usage in the Message Assignment Shape:
        /// <example>
        /// <code>
        /// // Initialize the message
        /// someMessage = null;
        /// // Load the content
        /// Microsoft.XLANGs.StreamingSDK.XLANGMessageHandler.LoadContentFromStream(someMessage, someStream);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">An empty message</param>
        /// <param name="stream">The stream to load</param>
        /// <exception cref="System.ArgumentNullException">Thrown when message or stream is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static void LoadContentFromStream(XLANGMessage message, Stream stream)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                messageHandler.LoadFromStream(stream);
            }
        }

        /// <summary>
        /// Returns the content of a message as a stream
        /// <example>
        /// <code>
        /// Stream contentStream = Microsoft.XLANGs.StreamingSDK.XLANGMessageHandler.RetrieveContentAsStream(someMessage);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">An XLANG message</param>
        /// <returns>A stream containing the content of the message</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static Stream RetrieveContentAsStream(XLANGMessage message)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                return messageHandler.RetrieveAsStream();
            }
        }

        /// <summary>
        /// Retrieves the name of the Root Node of an XLANGMessage
        /// <example>
        /// <code>
        /// String nodename = Microsoft.XLANGs.StreamingSDK.XLANGMessageHandler.GetRootNodeName(someMessage);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="message">A message</param>
        /// <returns>The name of the Root Node</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when message is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when message does not contain any parts</exception>
        public static string GetRootNodeName(XLANGMessage message)
        {
            using (var messageHandler = new XLANGMessageHandler(message))
            {
                return messageHandler.GetRootNodeName();
            }
        }
    }
}
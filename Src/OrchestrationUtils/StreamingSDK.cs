using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.XLANGs.BaseTypes;

namespace BizTalkComponents.OrchestrationUtils
{
    /// <summary>
    /// A stream factory with a file as its source of data
    /// </summary>
    public class FileStreamFactory : IStreamFactory
    {
        private readonly string _path;

        public FileStreamFactory(string path)
        {
            _path = path;
        }

        Stream IStreamFactory.CreateStream()
        {
            return new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }

    /// <summary>
    /// A stream factory with an XLANGPart as its source of
    /// data.
    /// </summary>
    public class XLANGPartStreamFactory : IStreamFactory
    {
        private readonly XLANGPart _part;

        public XLANGPartStreamFactory(XLANGPart part)
        {
            if (null == part)
                throw new ArgumentNullException("part");

            _part = part;
        }

        Stream IStreamFactory.CreateStream()
        {
            return (Stream) _part.RetrieveAs(typeof (Stream));
        }
    }

    /// <summary>
    /// A stream factory with a byte array region as its source
    /// of data.
    /// </summary>
    public class ByteArrayStreamFactory : IStreamFactory
    {
        private byte[] _byteArray;
        private int _index;
        private int _count;

        private void _initialize(byte[] byteArray, int index, int count)
        {
            _byteArray = byteArray;
            _index = index;
            _count = count;
        }

        public ByteArrayStreamFactory(byte[] byteArray)
        {
            if (null == byteArray)
                throw new ArgumentNullException("byteArray");
            _initialize(byteArray, 0, byteArray.Length);
        }

        public ByteArrayStreamFactory(byte[] byteArray, int index, int count)
        {
            // Test arguments---will throw if bad
            if (null == byteArray) throw new ArgumentNullException("byteArray");
            if (index < 0)throw new ArgumentException("index is less than zero", "index");
            if (count < 0)throw new ArgumentException("count is less than zero", "count");
            if(index + count > byteArray.Length)
                throw new ArgumentException("index + count is larger than the size of the bytearray");
            _initialize(byteArray, index, count);
        }

        Stream IStreamFactory.CreateStream()
        {
            return new MemoryStream(_byteArray, _index, _count, false, false);
        }
    }

    /// <summary>
    /// Adds the ability to query the current position to a crypto
    /// read stream.
    /// </summary>
    public class CryptoStreamWithPosition : CryptoStream
    {
        private long _pos;

        public CryptoStreamWithPosition(Stream stream, ICryptoTransform transform)
            : base(stream, transform, CryptoStreamMode.Read)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = base.Read(buffer, offset, count);
            _pos += read;
            return read;
        }

        public override int ReadByte()
        {
            int read = base.ReadByte();
            int readBytes = read == -1 ? 0 : 1;
            _pos += readBytes;
            return read;
        }

        public override long Position
        {
            get { return _pos; }
        }

        // Bug Bug: crypto stream tries to *write* into the
        // source stream if not consumed fully. Have to 
        // supress close then.
        public override void Close()
        {
        }
    }

    /// <summary>
    /// This is a stream factory that applies a crypto transform to
    /// streams obtains from a source stream factory. The transform
    /// to apply is determined by derived classes by overrding the
    /// CreateTransform method.
    /// </summary>
    public abstract class CryptoTransformStreamFactory : IStreamFactory
    {
        private readonly IStreamFactory _sourceFactory;

        protected CryptoTransformStreamFactory(IStreamFactory sourceFactory)
        {
            _sourceFactory = sourceFactory;
        }

        protected abstract ICryptoTransform CreateTransform();

        Stream IStreamFactory.CreateStream()
        {
            return new CryptoStreamWithPosition(_sourceFactory.CreateStream(), CreateTransform());
        }
    }


    /// <summary>
    /// This stream factory encodes the raw binary data obtained from
    /// the source stream factory using base 64 encoding.
    /// </summary>
    public class Base64EncoderStreamFactory : CryptoTransformStreamFactory
    {
        public Base64EncoderStreamFactory(IStreamFactory source)
            : base(source)
        {
        }

        protected override ICryptoTransform CreateTransform()
        {
            return new ToBase64Transform();
        }
    }

    /// <summary>
    /// This stream factory assumes that the source stream factory
    /// supplies streams which are base 64 encoded. It applies decoding
    /// and provides the original raw binary data.
    /// </summary>
    public class Base64DecoderStreamFactory : CryptoTransformStreamFactory
    {
        public Base64DecoderStreamFactory(IStreamFactory source)
            : base(source)
        {
        }

        protected override ICryptoTransform CreateTransform()
        {
            return new FromBase64Transform();
        }
    }


    /// <summary>
    /// This stream factory provides streams which are a concatenation
    /// of the streams provided by a collection of input stream factories.
    /// It is used to aggregate stream data from multiple sources.
    /// </summary>
    public class MultiSourceStreamFactory : IStreamFactory
    {
        private readonly IEnumerable<IStreamFactory> _sources;

        public MultiSourceStreamFactory(IEnumerable<IStreamFactory> sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");
            var tempSources = sources.ToList();
            if (tempSources.Any(t => t == null))
            {
                throw new ArgumentException("Sources contains elements with value null", "sources");
            }

            _sources = tempSources;
        }

        Stream IStreamFactory.CreateStream()
        {
            return new MultiSourceStream(_sources);
        }

        /// <summary>
        /// Private aggregated stream implementation. The bulk of the
        /// work is in the Read method.
        /// </summary>
        private class MultiSourceStream : Stream
        {
            private int _position;
            private readonly IEnumerable<IStreamFactory> _sources;

            internal MultiSourceStream(IEnumerable<IStreamFactory> sources)
            {
                _sources = sources;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (null == buffer) throw new ArgumentNullException("buffer");
                if (offset < 0) throw new ArgumentException("offset is less than zero", "offset");
                if (count < 0) throw new ArgumentException("count is less than zero", "count");
                if (offset + count > buffer.Length)
                    throw new ArgumentException("offset + count is larger than the size of the bytearray");

                int initialCount = count;
                foreach (var streamFactory in _sources)
                {
                    using (var currentSource = streamFactory.CreateStream())
                    {
                        // Have we read enough?
                        while (count > 0)
                        {
                            int readBytes = currentSource.Read(buffer, offset, count);
                            offset += readBytes;
                            count -= readBytes;
                        }

                    }
                }
                int totalRead = initialCount - count;
                _position += totalRead;
                return totalRead;
            }

            public override int ReadByte()
            {
                //
                // @undone Hope nobody is using ReadByte (frequently)
                // because this impl is sub optimal.
                //
                byte[] buff = new byte[1];
                int bytesRead = Read(buff, 0, 1);
                return bytesRead == 0 ? -1 : buff[0];
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { return _position; }
                set
                {
                    if (_position != value)
                        throw new NotSupportedException();
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        Position = offset;
                        break;

                    case SeekOrigin.Current:
                        Position = Position + offset;
                        break;

                    case SeekOrigin.End:
                        throw new NotSupportedException();
                }
                return Position;
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override void WriteByte(byte v)
            {
                throw new NotSupportedException();
            }
        }
    }


    //-----------------------------------------------------------------

    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    internal class Class1
    {
        private static void Main1(string[] args)
        {
            string headerStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Root><From></From><To></To><binaryData>";
            byte[] header = Encoding.UTF8.GetBytes(headerStr);

            string trailerStr = "</binaryData></Root>";
            byte[] trailer = Encoding.UTF8.GetBytes(trailerStr);

            byte[] binaryData = Encoding.UTF8.GetBytes("binary data, sort of");

            var sources = new List<IStreamFactory>
            {
                new ByteArrayStreamFactory(header, 0, header.Length),
                new Base64EncoderStreamFactory(new ByteArrayStreamFactory(binaryData, 0, binaryData.Length)),
                new ByteArrayStreamFactory(trailer, 0, trailer.Length)
            };

            IStreamFactory multiSourceFactory = new MultiSourceStreamFactory(sources);

            using (Stream src = multiSourceFactory.CreateStream())
            {
                using (FileStream fs = new FileStream("c:\\test.xml", FileMode.OpenOrCreate))
                {
                    byte[] buff = new byte[1024];
                    for (;;)
                    {
                        int nread = src.Read(buff, 0, buff.Length);
                        if (nread == 0) break;
                        fs.Write(buff, 0, nread);
                    }
                }
            }
        }

        /*
        static void Main(string[] args)
        {
            IStreamFactory fileSf = new FileStreamFactory( "\\test.xml" );
            IStreamFactory extractorSf =  new Base64EncodedDataExtractorStreamFactory( fileSf );
            IStreamFactory base64DecoderSf = new Base64DecoderStreamFactory( extractorSf );

            Stream binaryData = base64DecoderSf.CreateStream();
            StreamReader sr = new StreamReader( binaryData, Encoding.UTF8 );
            string s = sr.ReadToEnd();
            Console.WriteLine( s );
        }
        */
    }
}

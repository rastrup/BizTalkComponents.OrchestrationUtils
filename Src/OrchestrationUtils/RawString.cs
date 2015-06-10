using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.XLANGs.BaseTypes;

namespace BizTalkComponents.OrchestrationUtils
{
    /// <summary>
    /// Credit:
    /// https://msdn.microsoft.com/en-us/library/ee253435%28v=bts.10%29.aspx
    /// </summary>
    public abstract class BaseFormatter : IFormatter
    {
        public virtual SerializationBinder Binder
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public virtual StreamingContext Context
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public virtual ISurrogateSelector SurrogateSelector
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public abstract void Serialize(Stream serializationStream, object graph);
        public abstract object Deserialize(Stream serializationStream);
    }

    public class RawStringFormatter : BaseFormatter
    {
        public override void Serialize(Stream serializationStream, object graph)
        {
            StreamWriter writer = new StreamWriter(serializationStream);
            writer.Write(((RawString)graph).ToString());
        }

        public override object Deserialize(Stream serializationStream)
        {
            StreamReader reader = new StreamReader(serializationStream, true);
            return new RawString(reader.ReadToEnd());
        }
    }

    [CustomFormatter(typeof(RawStringFormatter))]
    [Serializable]
    public class RawString
    {
        [XmlIgnore] readonly string _content;

        public RawString(string content)
        {
            if (null == content)
                throw new ArgumentNullException("content");
            _content = content;
        }

        public RawString()
        {
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(_content);
        }

        public override string ToString()
        {
            return _content;
        }
    }
}
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;
using Microsoft.XLANGs.RuntimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace BizTalkComponents.OrchestrationUtils.UnitTests
{
    internal class MockXLANGPart : XLANGPart, IDisposable
    {
        private readonly IDictionary<Type, object> _properties = new Dictionary<Type, object>();
        private Value _value;

        public override string Name
        {
            get { return "MockXLANGPart"; }
        }

        public override XmlSchema XmlSchema
        {
            get { throw new NotImplementedException(); }
        }

        public override XmlSchemaCollection XmlSchemaCollection
        {
            get { throw new NotImplementedException(); }
        }

        public override void Dispose()
        {
            if (_value != null) _value.Dispose();
        }

        public override object GetPartProperty(Type propType)
        {
            return _properties[propType];
        }

        public override Type GetPartType()
        {
            throw new NotImplementedException();
        }

        public override string GetXPathValue(string xpath)
        {
            return _value.GetXPath(xpath, true).ToString();
        }

        public override void LoadFrom(object source)
        {
            if (_value != null) _value.Dispose();
            _value = !(source is XmlDocument)
                ? (!(source is XmlNode)
                    ? (!(source is Stream)
                        ? (!(source is XmlReader)
                            ? (!(source is IStreamFactory)
                                ? (!(source is IXmlReaderFactory)
                                    ? new Value(source)
                                    : new Value((IXmlReaderFactory) source))
                                : new Value((IStreamFactory) source))
                            : new Value((XmlReader) source))
                        : new Value((Stream) source))
                    : new Value((XmlNode) source))
                : new Value((XmlDocument) source);
        }

        public override void PrefetchXPathValue(string xpath)
        {
            _value.PrefetchXPath(xpath);
        }

        public override object RetrieveAs(Type t)
        {
            return _value.RetrieveAs(t);
        }

        public override void SetPartProperty(Type propType, object value)
        {
            _properties[propType] = value;
        }
    }
}
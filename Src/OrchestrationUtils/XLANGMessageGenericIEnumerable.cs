using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.XLANGs.BaseTypes;

namespace BizTalkComponents.OrchestrationUtils
{
    public class XLANGMessageGenericIEnumerable : XLANGMessage, IEnumerable<XLANGPart>, IDisposable
    {
        private readonly XLANGMessage _xlangMessage;

        public XLANGMessageGenericIEnumerable(XLANGMessage xlangMessage)
        {
            _xlangMessage = xlangMessage;
        }

        public override void AddPart(XLANGPart part)
        {
            _xlangMessage.AddPart(part);
        }

        public override void AddPart(XLANGPart part, string partName)
        {
            _xlangMessage.AddPart(part, partName);
        }

        public override void AddPart(object part, string partName)
        {
            _xlangMessage.AddPart(part, partName);
        }

        public override object GetPropertyValue(Type propType)
        {
            return _xlangMessage.GetPropertyValue(propType);
        }

        public override void SetPropertyValue(Type propType, object value)
        {
            _xlangMessage.SetPropertyValue(propType, value);
        }

        IEnumerator<XLANGPart> IEnumerable<XLANGPart>.GetEnumerator()
        {
            return _xlangMessage.Cast<XLANGPart>().GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            return _xlangMessage.GetEnumerator();
        }

        // Flag: Has Dispose already been called? 
        bool _disposed;
        // Public implementation of Dispose pattern callable by consumers. 
        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _xlangMessage.Dispose();
            }
            _disposed = true;
        }

        public override string Name
        {
            get { return _xlangMessage.Name; }
        }

        public override int Count
        {
            get { return _xlangMessage.Count; }
        }

        public override XLANGPart this[string partName]
        {
            get { return _xlangMessage[partName]; }
        }

        public override XLANGPart this[int partIndex]
        {
            get { return _xlangMessage[partIndex]; }
        }
    }
}

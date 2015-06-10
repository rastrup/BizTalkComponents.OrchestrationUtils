using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BizTalkComponents.OrchestrationUtils
{
    [Serializable]
    public class XLANGMessageGenericIEnumerable : XLANGMessage, IEnumerable<XLANGPart>, IDisposable
    {
        private readonly XLANGMessage _xlangMessage;

        // Flag: Has Dispose already been called?
        private bool _disposed;

        public XLANGMessageGenericIEnumerable(XLANGMessage xlangMessage)
        {
            if (xlangMessage == null) throw new ArgumentNullException("xlangMessage");
            _xlangMessage = xlangMessage;
        }

        public override string Name
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _xlangMessage.Name;
            }
        }

        public override int Count
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _xlangMessage.Count;
            }
        }

        public override XLANGPart this[string partName]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _xlangMessage[partName];
            }
        }

        public override XLANGPart this[int partIndex]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _xlangMessage[partIndex];
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        public override sealed void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        IEnumerator<XLANGPart> IEnumerable<XLANGPart>.GetEnumerator()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return _xlangMessage.Cast<XLANGPart>().GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return _xlangMessage.GetEnumerator();
        }

        public override void AddPart(XLANGPart part)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _xlangMessage.AddPart(part);
        }

        public override void AddPart(XLANGPart part, string partName)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _xlangMessage.AddPart(part, partName);
        }

        public override void AddPart(object part, string partName)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _xlangMessage.AddPart(part, partName);
        }

        public override object GetPropertyValue(Type propType)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return _xlangMessage.GetPropertyValue(propType);
        }

        public override void SetPropertyValue(Type propType, object value)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _xlangMessage.SetPropertyValue(propType, value);
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
    }
}
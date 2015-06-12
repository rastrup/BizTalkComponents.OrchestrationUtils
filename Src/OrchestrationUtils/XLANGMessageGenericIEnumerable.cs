using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BizTalkComponents.OrchestrationUtils
{
    [Serializable]
    public class XLANGMessageGenericIEnumerable : XLANGMessage, IDisposable, IReadOnlyList<XLANGPart>
    {
        private readonly XLANGMessage _message;

        // Flag: Has Dispose already been called?
        private bool _disposed;

        public XLANGMessageGenericIEnumerable(XLANGMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            _message = message;
        }

        public override string Name
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _message.Name;
            }
        }

        public override int Count
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _message.Count;
            }
        }

        public override XLANGPart this[string partName]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _message[partName];
            }
        }

        public override XLANGPart this[int partIndex]
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _message[partIndex];
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
            return _message.Cast<XLANGPart>().GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return _message.GetEnumerator();
        }

        public override void AddPart(XLANGPart part)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _message.AddPart(part);
        }

        public override void AddPart(XLANGPart part, string partName)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _message.AddPart(part, partName);
        }

        public override void AddPart(object part, string partName)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _message.AddPart(part, partName);
        }

        public override object GetPropertyValue(Type propType)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return _message.GetPropertyValue(propType);
        }

        public override void SetPropertyValue(Type propType, object value)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _message.SetPropertyValue(propType, value);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _message.Dispose();
            }
            _disposed = true;
        }
    }
}
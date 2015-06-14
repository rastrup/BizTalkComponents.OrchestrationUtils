using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BizTalkComponents.OrchestrationUtils
{
    [Serializable]
    public class XLANGMessageCollectionWrapper : XLANGMessageGenericIEnumerable, ICollection<XLANGPart>
    {
        public XLANGMessageCollectionWrapper(XLANGMessage message)
            : base(message)
        {
        }

        public void Add(XLANGPart item)
        {
            AddPart(item);
        }

        public void Clear()
        {
            throw new NotSupportedException("XLANGMessage supports addition of parts only, not removal.");
        }

        public bool Contains(XLANGPart item)
        {
            IEnumerable<XLANGPart> generic = this;
            return generic.Any(part => part == item);
        }

        public void CopyTo(XLANGPart[] array, int arrayIndex)
        {
            IEnumerable<XLANGPart> generic = this;
            generic.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(XLANGPart item)
        {
            throw new NotSupportedException("XLANGMessage supports addition of parts only, not removal.");
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
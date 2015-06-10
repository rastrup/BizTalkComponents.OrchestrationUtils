using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace BizTalkComponents.OrchestrationUtils
{
    /// <summary>
    /// With inspiration from 
    /// http://blogs.msdn.com/b/paolos/archive/2009/09/10/4-different-ways-to-process-an-xlangmessage-within-an-helper-component-invoked-by-an-orchestration.aspx
    /// </summary>
    [Serializable]
    public sealed class CustomBTXMessage : BTXMessage, IDisposable, IEnumerable<XLANGPart>
    {
        public CustomBTXMessage(string messageName, Context context)
            : base(messageName, context)
        {
            context.RefMessage(this);
        }

        IEnumerator<XLANGPart> IEnumerable<XLANGPart>.GetEnumerator()
        {
            return this.Cast<XLANGPart>().GetEnumerator();
        }
    }
}

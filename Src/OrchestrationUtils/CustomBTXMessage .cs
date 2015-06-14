using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BizTalkComponents.OrchestrationUtils
{
    /// <summary>
    ///     With inspiration from
    ///     http://blogs.msdn.com/b/paolos/archive/2009/09/10/4-different-ways-to-process-an-xlangmessage-within-an-helper-component-invoked-by-an-orchestration.aspx
    /// </summary>
    [Serializable]
    public sealed class CustomBTXMessage : BTXMessage, IDisposable, IReadOnlyList<XLANGPart>
    {
        /// <summary>
        /// </summary>
        /// <param name="msgName">The name of the Message</param>
        /// <param name="owningContext">use Service.RootService.XlangStore.OwningContext</param>
        public CustomBTXMessage(string msgName, Context owningContext)
            : base(msgName, owningContext)
        {
            owningContext.RefMessage(this);
        }

        /// <summary>
        /// </summary>
        /// <returns>An XLANGPart generic enumerator</returns>
        IEnumerator<XLANGPart> IEnumerable<XLANGPart>.GetEnumerator()
        {
            return this.Cast<XLANGPart>().GetEnumerator();
        }
    }
}
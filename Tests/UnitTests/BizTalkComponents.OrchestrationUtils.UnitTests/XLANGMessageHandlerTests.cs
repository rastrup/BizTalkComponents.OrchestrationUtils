using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizTalkComponents.OrchestrationUtils.UnitTests
{
    [TestClass()]
    public class XLANGMessageHandlerTests
    {
        private XLANGMessageHandler _messageHandler = new XLANGMessageHandler(new MockXLANGMessage());
        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod()]
        public void LoadFromStringTest()
        {
            _messageHandler.LoadFromString(Properties.Resources.XmlExample);
            var rootNodeName = _messageHandler.GetRootNodeName();
            Assert.AreEqual("library", rootNodeName);
            var feedback = _messageHandler.RetrieveAsString();
            Assert.AreEqual(Properties.Resources.XmlExample, feedback);
        }
    
        [TestMethod()]
        public void LoadFromStreamTest()
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(Properties.Resources.XmlExample);
            writer.Flush();
            _messageHandler.LoadFromStream(memoryStream);
            var rootNodeName = _messageHandler.GetRootNodeName();
            Assert.AreEqual("library", rootNodeName);
            var feedback = _messageHandler.RetrieveAsString();
            Assert.AreEqual(Properties.Resources.XmlExample, feedback);
        }
    }
}

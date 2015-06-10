using System;
using System.IO;
using System.Text;
using BizTalkComponents.OrchestrationUtils.UnitTests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizTalkComponents.OrchestrationUtils.UnitTests
{
    [TestClass()]
    public class XLANGMessageHandlerTests
    {
        private readonly XLANGMessageHandler _messageHandler = new XLANGMessageHandler(new MockXLANGMessage());

        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod()]
        public void LoadFromStringTest()
        {
            _messageHandler.LoadFromString(Resources.XmlExample);
            var rootNodeName = _messageHandler.GetRootNodeName();
            Assert.AreEqual("library", rootNodeName);
            var feedback = _messageHandler.RetrieveAsString();
            Assert.AreEqual(Resources.XmlExample, feedback);
        }
    
        [TestMethod]
        public void LoadFromStreamTest()
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(Resources.XmlExample);
            writer.Flush();
            _messageHandler.LoadFromStream(memoryStream);
            var rootNodeName = _messageHandler.GetRootNodeName();
            Assert.AreEqual("library", rootNodeName);
            var feedback = _messageHandler.RetrieveAsString();
            Assert.AreEqual(Resources.XmlExample, feedback);
        }

        [TestMethod]
        public void LoadFromBase64Test()
        {
            _messageHandler.LoadFromBase64(GetBase64Example());
            var rootNodeName = _messageHandler.GetRootNodeName();
            Assert.AreEqual("library", rootNodeName);
            var feedback = _messageHandler.RetrieveAsString();
            Assert.AreEqual(Resources.XmlExample, feedback);
            var base64String = _messageHandler.RetrieveAsBase64();
            Assert.AreEqual(GetBase64Example(), base64String);
        }

        public static string GetBase64Example()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Resources.XmlExample));
        }
    }
}

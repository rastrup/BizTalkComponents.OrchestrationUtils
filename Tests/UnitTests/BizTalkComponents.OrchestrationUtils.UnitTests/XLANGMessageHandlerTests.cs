using BizTalkComponents.OrchestrationUtils.UnitTests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace BizTalkComponents.OrchestrationUtils.UnitTests
{
    [TestClass]
    public class XLANGMessageHandlerTests
    {
        private readonly XLANGMessageHandler _messageHandler = new XLANGMessageHandler(new MockXLANGMessage());

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
            _messageHandler.Dispose();
        }

        [TestMethod]
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

        [TestMethod]
        public void LoadFromBase64BinaryTest()
        {
            byte[] byteArray;
            using (var image = Resources.BizTalkServer2013R2)
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    stream.Flush();
                    byteArray = stream.ToArray();
                }
            }
            _messageHandler.LoadFromBase64(Convert.ToBase64String(byteArray));
            var feedback = _messageHandler.RetrieveAsBase64();
        }

        private static string GetBase64Example()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Resources.XmlExample));
        }
    }
}
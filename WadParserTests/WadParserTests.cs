using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace nz.doom.WadParser.Tests
{
    [TestClass, DeploymentItem(@"TestResources\TestBytes.txt", "TestResources")]
    public class WadParserTests
    {

        private readonly byte[] BYTE_ARRAY = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39 };

        [TestMethod]
        public void ReadAllBytesTest()
        {
            using (var fileStream = File.OpenRead(@"TestResources\TestBytes.txt"))
            {
                
                byte[] bytes = WadParser.ReadBytesAtPosition(fileStream,0, BYTE_ARRAY.Length);

                for (int i = 0; i < bytes.Length; i++)
                {
                    Assert.AreEqual(BYTE_ARRAY[i], bytes[i]);
                }
                
            }
        }

        [TestMethod]
        public void ReadSomeBytesTest()
        {
            using (var fileStream = File.OpenRead(@"TestResources\TestBytes.txt"))
            {
                
                byte[] bytes = WadParser.ReadBytesAtPosition(fileStream,0, 2);

                for (int i = 0; i < bytes.Length; i++)
                {
                    Assert.AreEqual(BYTE_ARRAY[i], bytes[i]);
                }
                
            }
        }

        [TestMethod]
        public void ReadSomeBytesAtPositionTest()
        {
            using (var fileStream = File.OpenRead(@"TestResources\TestBytes.txt"))
            {
                
                byte[] bytes = WadParser.ReadBytesAtPosition(fileStream,2, 2);

                for (int i = 2; i < bytes.Length; i++)
                {
                    Assert.AreEqual(BYTE_ARRAY[i], bytes[i]);
                }
                
            }
        }

        [TestMethod]
        public void ReadNoBytesTest()
        {
            using (var fileStream = File.OpenRead(@"TestResources\TestBytes.txt"))
            {
                
                byte[] bytes = WadParser.ReadBytesAtPosition(fileStream,0, 0);

                Assert.AreEqual(0, bytes.Length);
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ReadNegativeBytesTest()
        {
            using (var fileStream = File.OpenRead(@"TestResources\TestBytes.txt"))
            {                
                byte[] bytes = WadParser.ReadBytesAtPosition(fileStream,0, -5);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativePostionBytesTest()
        {
            using (var fileStream = File.OpenRead(@"TestResources\TestBytes.txt"))
            {                
                byte[] bytes = WadParser.ReadBytesAtPosition(fileStream,-3, 1);                
            }
        }
    }
}

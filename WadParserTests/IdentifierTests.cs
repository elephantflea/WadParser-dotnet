using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace nz.doom.WadParser.Tests
{
    [TestClass]
    public class IdentifierTests
    {
        [TestMethod, DeploymentItem(@"TestResources\doom1.wad", "TestResources")]
        public void ReadFromFileTest()
        {
            var type = Identifier.GetWadType(@"TestResources\doom1.wad");

            Assert.AreEqual("IWAD", type);
        }
        
        [DataTestMethod]
        [DataRow("IWAD","IWAD")]
        [DataRow("PWAD", "PWAD")]
        [DataRow("WAD2", "WAD2")]
        [DataRow("WAD3", "WAD3")]
        [DataRow("BLAHBLAH", "UNKNOWN")]
        [DataRow("", "UNKNOWN")]
        public void ReadFromBytesTest(string byteString, string expected)
        {
            var type = Identifier.GetWadType(Encoding.UTF8.GetBytes(byteString));

            Assert.AreEqual(expected, type);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullTest()
        {
            string blah = null;
            Identifier.GetWadType(blah);
        }
        
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FileNotFoundTest()
        {
            Identifier.GetWadType("blah");
        }

        [DataTestMethod]
        [DataRow(new byte[] { }, new byte[] { })]
        [DataRow(new byte[] { }, new byte[] { 1 })]
        [DataRow(new byte[] { 1,2 }, new byte[] { 1,2 })]
        [DataRow(new byte[] { 1,2 }, new byte[] { 1,2,3,4 })]
        public void ArraysStartWith_Test(byte[] expected, byte[] toCheck)
        {
            Assert.IsTrue(Identifier.ArrayStartsWith(expected, toCheck));
        }

        [DataTestMethod]
        [DataRow(new byte[] { 1,2}, new byte[] { })]        
        [DataRow(new byte[] { 1,2 }, new byte[] { 1 })]
        [DataRow(new byte[] { 1,2 }, new byte[] { 1,3 })]
        [DataRow(new byte[] { 1,2 }, new byte[] { 1,3,1,2 })]
        [DataRow(new byte[] { 1,2,3 }, new byte[] { 1,3,2 })]
        public void ArraysNotStartWith_Test(byte[] expected, byte[] toCheck)
        {
            Assert.IsFalse(Identifier.ArrayStartsWith(expected, toCheck));
        }
    }
}

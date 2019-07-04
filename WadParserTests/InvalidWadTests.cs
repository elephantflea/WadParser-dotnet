using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace nz.doom.WadParser.Tests
{
    [TestClass]
    public class InvalidWadTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullWadPathTest()
        {
            WadParser.Parse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FileNotFoundTest()
        {
            WadParser.Parse("somefile");
        }

        [TestMethod]
        [ExpectedException(typeof(WadParseException))]
        public void InvalidWadTest()
        {
            WadParser.Parse(@"TestResources\UNKNOWN.txt");
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\TooManyLumps.wad","TestResources")]
        [ExpectedException(typeof(WadParseException))]
        public void TooManyLumpsTest()
        {
            WadParser.Parse(@"TestResources\TooManyLumps.wad");
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\BadDirectoryOffset.wad", "TestResources")]
        [ExpectedException(typeof(WadParseException))]
        public void BadDirectoryOffsetTest()
        {
            WadParser.Parse(@"TestResources\BadDirectoryOffset.wad");
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\BadDirectoryLength.wad", "TestResources")]
        [ExpectedException(typeof(WadParseException))]
        public void BadDirectoryLengthTest()
        {
            WadParser.Parse(@"TestResources\BadDirectoryLength.wad");
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\UNKNOWN.txt","TestResources")]
        public void ReadBytesAtPosition_OffsetTest()
        {

            using (FileStream fileStream = File.OpenRead(@"TestResources\UNKNOWN.txt"))
            {
                var bytes = WadParser.ReadBytesAtPosition(fileStream, 6, 5);

                Assert.AreEqual(5, bytes.Length);

                var readString = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                Assert.AreEqual("valid", readString);

            }
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\UNKNOWN.txt","TestResources")]
        public void ReadBytesAtPosition_StartTest()
        {

            using (FileStream fileStream = File.OpenRead(@"TestResources\UNKNOWN.txt"))
            {
                var bytes = WadParser.ReadBytesAtPosition(fileStream, 0, 3);

                Assert.AreEqual(3, bytes.Length);

                var readString = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                Assert.AreEqual("Not", readString);

            }
        }
    }
}
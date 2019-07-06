using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace nz.doom.WadParser.Tests
{
    [TestClass]
    public class Doom1SharewareAsStreamTests
    {

        private static Wad _wad = null;
        
        [ClassInitialize, DeploymentItem(@"TestResources\doom1.wad", "TestResources")]
        public static void TestInit(TestContext testContext)
        {
            _wad = WadParser.Parse(@"TestResources\doom1.wad", false);
        }

        [TestMethod]
        public void WadAttributesTest()
        {
            Assert.AreEqual("IWAD", _wad.WadType);
            Assert.AreEqual(4274218, _wad.WadSize);
            Assert.AreEqual(1270, _wad.LumpCount);
            Assert.AreEqual("doom1.wad", _wad.Filename);
        }

        [TestMethod]
        public void GetLumpByNameTest()
        {
            var lump = _wad.GetLumpByName("ENDOOM");
            
            Assert.IsNotNull(lump);
            Assert.AreEqual("ENDOOM", lump.Name);
            Assert.AreEqual(4000, lump.Size);
            Assert.AreEqual(2, lump.Position);
            Assert.AreEqual(-1, lump.LumpType);
            Assert.IsFalse(lump.IsCompressed);
            Assert.IsFalse(lump.IsCorrupt);

            /*Assert.AreEqual(0x20, lump.Bytes[0]);
            Assert.AreEqual(0x07, lump.Bytes[3999]);
            Assert.AreEqual(0x4F, lump.Bytes[3381]);*/
        }

        [TestMethod]
        public void ReadBytesFromLumpTest()
        {
            var lump = _wad.GetLumpByName("ENDOOM");

            Assert.IsNull(lump.Bytes);

            WadParser.ReadBytesIntoLump(@"TestResources\doom1.wad", lump);

            Assert.IsNotNull(lump.Bytes);

            Assert.AreEqual(0x20, lump.Bytes[0]);
            Assert.AreEqual(0x07, lump.Bytes[3999]);
            Assert.AreEqual(0x4F, lump.Bytes[3381]);
        }

        [TestMethod]
        public void ReadBytesFromLumpStreamTest()
        {
            var lump = _wad.GetLumpByName("PLAYPAL");

            Assert.IsNull(lump.Bytes);

            FileStream wadFileStream = File.OpenRead(@"TestResources\doom1.wad");

            WadParser.ReadBytesIntoLump(wadFileStream, lump);

            wadFileStream.Close();

            Assert.IsNotNull(lump.Bytes);

            Assert.AreEqual(0x00, lump.Bytes[0]);
            Assert.AreEqual(0x5E, lump.Bytes[10751]);
            Assert.AreEqual(0x22, lump.Bytes[9752]);
        }

        [TestMethod]
        public void GetLumpByName_InvalidNameTest()
        {
            var lump = _wad.GetLumpByName("INVALID NAME");

            Assert.IsNull(lump);
        }

        [TestMethod]
        public void GetLumpsByNameTest()
        {
            var lumps = _wad.GetLumpsByName("THINGS");

            Assert.IsNotNull(lumps);
            Assert.AreEqual(9, lumps.Count());
        }

        [TestMethod]
        public void GetLumpsByName_InvalidNameTest()
        {
            var lumps = _wad.GetLumpsByName("INVALID NAME");

            Assert.IsNotNull(lumps);
            Assert.AreEqual(0, lumps.Count());
        }

        [TestMethod]
        public void GetLumpByPositionTest()
        {
            var lump = _wad.GetLumpAtPosition(3);

            Assert.IsNotNull(lump);
            Assert.AreEqual("DEMO1", lump.Name);
        }

        [TestMethod]
        public void CorruptAndCompressedLumpCheckTest()
        {
            foreach (var lump in _wad.Lumps) {
                Assert.IsNotNull(lump);
                Assert.IsFalse(lump.IsCorrupt);
                Assert.IsFalse(lump.IsCompressed);
            }
        }
    }
}

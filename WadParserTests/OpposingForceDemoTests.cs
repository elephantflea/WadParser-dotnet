using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace nz.doom.WadParser.Tests
{
    [TestClass]
    public class OpposingForceDemoTests
    {
        private static Wad _wad = null;

        [ClassInitialize, DeploymentItem(@"TestResources\opposing-force-spraypaint.wad", "TestResources")]
        public static void TestInit(TestContext testContext)
        {
            _wad = WadParser.Parse(@"TestResources\opposing-force-spraypaint.wad");
        }

        [TestMethod]
        public void WadAttributesTest()
        {
            Assert.AreEqual("WAD3", _wad.WadType);
            Assert.AreEqual(87988, _wad.WadSize);
            Assert.AreEqual(14, _wad.LumpCount);
            Assert.AreEqual("opposing-force-spraypaint.wad", _wad.Filename);
        }

        [TestMethod]
        public void GetLumpByNameTest()
        {
            var lump = _wad.GetLumpByName("LAMBDA");

            Assert.IsNotNull(lump);
            Assert.AreEqual("LAMBDA", lump.Name);
            Assert.AreEqual(6252, lump.Size);
            Assert.AreEqual(7, lump.Position);
            Assert.AreEqual(0x43, lump.LumpType);
            Assert.IsFalse(lump.IsCompressed);
            Assert.IsFalse(lump.IsCorrupt);

            Assert.AreEqual(0x6C, lump.Bytes[0]);
            Assert.AreEqual(0x00, lump.Bytes[6251]);
            Assert.AreEqual(0x29, lump.Bytes[5607]);
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
            var lumps = _wad.GetLumpsByName("LAMBDA");

            Assert.IsNotNull(lumps);
            Assert.AreEqual(1, lumps.Count());
        }

        [TestMethod]
        public void GetLumpsByName_InvalidNameTest()
        {
            var lumps = _wad.GetLumpsByName("INVALID NAME");

            Assert.IsNotNull(lumps);
            Assert.AreEqual(0, lumps.Count());
        }

        [TestMethod]
        public void CorruptAndCompressedLumpCheckTest()
        {
            foreach (var lump in _wad.Lumps)
            {
                Assert.IsNotNull(lump);
                Assert.IsFalse(lump.IsCorrupt);
                Assert.IsFalse(lump.IsCompressed);
            }
        }
    }
}

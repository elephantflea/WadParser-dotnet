using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace nz.doom.WadParser.Tests
{
    [TestClass]
    public class QuakeSharewareTests
    {
        private static Wad _wad = null;

        [ClassInitialize, DeploymentItem(@"TestResources\quake-gfx.wad", "TestResources")]
        public static void TestInit(TestContext testContext)
        {
            _wad = WadParser.Parse(@"TestResources\quake-gfx.wad");
        }

        [TestMethod]
        public void WadAttributesTest()
        {
            Assert.AreEqual("WAD2", _wad.WadType);
            Assert.AreEqual(112828, _wad.WadSize);
            Assert.AreEqual(163, _wad.LumpCount);
            Assert.AreEqual("quake-gfx.wad", _wad.Filename);
        }

        [TestMethod]
        public void GetLumpByNameTest()
        {
            var lump = _wad.GetLumpByName("SB_SIGIL4");

            Assert.IsNotNull(lump);
            Assert.AreEqual("SB_SIGIL4", lump.Name);
            Assert.AreEqual(136, lump.Size);
            Assert.AreEqual(83, lump.Position);
            Assert.AreEqual(0x42, lump.LumpType);
            Assert.IsFalse(lump.IsCompressed);
            Assert.IsFalse(lump.IsCorrupt);

            Assert.AreEqual(0x08, lump.Bytes[0]);
            Assert.AreEqual(0x11, lump.Bytes[135]);
            Assert.AreEqual(0xAE, lump.Bytes[38]);
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
            var lumps = _wad.GetLumpsByName("FACE1");

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

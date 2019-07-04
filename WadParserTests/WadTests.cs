using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nz.doom.WadParser.Tests
{
    [TestClass]
    public class WadTests
    {
        [TestMethod]
        public void ConstructorFullParametersTests()
        {
            var wad = new Wad("dummy", 10, "filename.wad");

            Assert.AreEqual(10, wad.WadSize);
            Assert.AreEqual("dummy", wad.Path);
            Assert.AreEqual("filename.wad", wad.Filename);
            Assert.AreEqual("filename.wad", wad.ToString());
        }

        [TestMethod, DeploymentItem(@"TestResources\doom1.wad", "TestResources")]
        public void ConstructorMinimalParametersTests()
        {
            var wad = new Wad(@"TestResources\doom1.wad");

            Assert.AreEqual(4274218, wad.WadSize);
            Assert.AreEqual(@"TestResources\doom1.wad", wad.Path);
            Assert.AreEqual("doom1.wad", wad.Filename);
        }

        [TestMethod]
        public void AddLumpTest()
        {
            var wad = new Wad("dummy", 1, "filename.wad");

            Lump lump = new Lump(); {
                lump.Name = "NEWLUMP";
            }

            wad.AddLump(lump);

            Assert.AreEqual(1, wad.LumpCount);
            Assert.AreEqual(lump, wad.GetLumpByName("NEWLUMP"));
        }

        [TestMethod]
        public void AddLumpAtPositionTest()
        {
            var wad = new Wad("dummy",1,"filename.wad");

            for (int i = 0; i < 5; i++)
            {
                Lump lump = new Lump();
                lump.Name = $"NEWLUMP{i}";
                lump.Position = i;
                wad.AddLump(lump);
            }

            Assert.AreEqual(5, wad.LumpCount);

            Lump insertedLump = new Lump();
            insertedLump.Position = 100;
            insertedLump.Name = "INSERED_LUMP";

            wad.AddLumpAtPosition(insertedLump, 3);

            Assert.AreEqual(6, wad.LumpCount);
            Assert.AreEqual(insertedLump, wad.GetLumpAtPosition(3));
            Assert.AreEqual(3, insertedLump.Position);

            Lump lastLump = wad.GetLumpAtPosition(wad.LumpCount - 1);

            Assert.AreEqual(5, lastLump.Position);
            Assert.AreEqual("NEWLUMP4", lastLump.Name);

        }
    }
}

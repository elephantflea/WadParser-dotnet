using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nz.doom.WadParser
{
    /// <summary>
    /// Container class that represents a WAD file and it's attributes and <seealso cref="Lump"/>s.
    /// </summary>
    public class Wad
    {
        private readonly List<Lump> lumps = new List<Lump>();

        /// <summary>
        /// The file size of this WAD in bytes
        /// </summary>
        public int WadSize { get; private set; }

        /// <summary>
        /// The file system location of the WAD
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The filename (excluding the path) of this WAD
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// This WAD's list of <see cref="Lump"/>s.
        /// </summary>
        public List<Lump> Lumps { get => lumps; }

        /// <summary>
        /// What type of WAD this is. See <see cref="Identifier"/> for valid WAD types
        /// </summary>
        public string WadType { get; set; }

        /// <summary>
        /// How many lumps this WAD contains
        /// </summary>
        public int LumpCount { get => Lumps.Count; }

        /// <summary>
        /// Intialises the WAD with the given <paramref name="wadPath"/>. Will not parse the WAD but will set the
        /// filename and filesize. See also <seealso cref="Wad(string, int, string)"/>.
        /// </summary>
        /// <param name="wadPath">The path of the WAD file.</param>
        public Wad(string wadPath)
        {
            Path = wadPath;
            var fi = new FileInfo(wadPath);

            WadSize = (int)fi.Length;
            Filename = fi.Name;
        }

        /// <summary>
        /// Initialises the WAD with the given parameters. Will not parse or validate the values given. See also <seealso cref="Wad(string)"/>
        /// </summary>
        /// <param name="wadPath">The path of the WAD file.</param>
        /// <param name="fileSize">The filesize of the WAD in bytes.</param>
        /// <param name="filename">The filename of this WAD without the path.</param>
        public Wad(string wadPath, int fileSize, string filename)
        {
            Path = wadPath;
            WadSize = fileSize;
            Filename = filename;
        }

        /// <summary>
        /// Add the <paramref name="lump"/> to this WAD. Adds to the end of the lump list.
        /// </summary>
        /// <param name="lump">The lump to add</param>
        /// <returns>Returns the current instance of <see cref="WAD"/></returns>
        public Wad AddLump(Lump lump)
        {
            Lumps.Add(lump);
            return this;
        }

        /// <summary>
        /// Add the <paramref name="lump"/> to this WAD at the given position <paramref name="position"/>. Will update the <see cref="Lump.Position"/>
        /// for each Lump in the Lump list.
        /// </summary>
        /// <param name="lump">The lump to insert</param>
        /// <param name="position">Where to insert the <paramref name="lump"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="position"/> is greater than the current number of lumps</exception>
        /// <returns>Returns the current instance of <see cref="WAD"/></returns>
        public Wad AddLumpAtPosition(Lump lump, int position)
        {
            Lumps.Insert(position, lump);

            for (int i = position; i < Lumps.Count; i++)
            {
                Lumps[i].Position = i;
            }
            return this;
        }

        /// <summary>
        /// Returns all lumps with the name passed in by <paramref name="name"/>. Will return an empty list if no
        /// lumps match the given name.
        /// </summary>
        /// <param name="name">The name of the lumps to find</param>
        /// <returns>A list of <see cref="Lump"/>. Empty if none are found.</returns>
        public IEnumerable<Lump> GetLumpsByName(string name)
        {
            return Lumps.Where(lump => lump.Name.Equals(name));
        }

        /// <summary>
        /// Returns the first <see cref="Lump"/> found in the WAD that as the name passed in from <paramref name="name"/>. If no lumps are 
        /// found then <c>null</c> is returned. Calls <see cref="GetLumpsByName(string)/>.
        /// </summary>
        /// <param name="name">The name of the lump to find</param>
        /// <returns>The first lump called <paramref name="name"/> or <c>null</c> if not found</returns>
        public Lump GetLumpByName(string name)
        {
            return GetLumpsByName(name).FirstOrDefault();
        }

        /// <summary>
        /// Returns the <see cref="Lump"/> at the specified <paramref name="position"/>. Zero based. Note this is not the byte
        /// offset from within the WAD but directory position. See also <seealso cref="Lump.Position"/>.
        /// </summary>
        /// <param name="position">Zero based position of the lump to find</param>
        /// <returns>The <see cref="Lump"/> found at <paramref name="position"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> falls outside the number of lumps in this WAD</exception>
        public Lump GetLumpAtPosition(int position)
        {
            if (position < 0 || position >= Lumps.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return Lumps[position];
        }

        /// <summary>
        /// Alias of <see cref="Filename"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Filename;
    }
}

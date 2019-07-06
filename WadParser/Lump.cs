using System;

namespace nz.doom.WadParser
{
    /// <summary>
    /// Generic container class that represents an individual Lump and it's attributes. See <a href="https://doomwiki.org/wiki/Lump">doomwiki.org</a> for more details.
    /// </summary>
    public class Lump : IComparable<Lump>
    {

        private int offset = -1;
        private int position = -1;
        private int lumpType = -1;

        /// <summary>
        /// The lumps size in bytes
        /// </summary>
        public int Size { get; set; }
        
        /// <summary>
        /// The byte offset within the WAD file this lump is located
        /// </summary>
        public int Offset { get => offset; set => offset = value; }
        
        /// <summary>
        /// The position within the WAD directory this lump resides
        /// </summary>
        public int Position { get => position; set => position = value; }

        /// <summary>
        /// What type of lump this is. Only applicable to the <see cref="Identifier.WAD2"/> and <see cref="Identifier.WAD3"/> formats. Known
        /// types are:<br/>
        /// WAD2:<br/>
        /// <list type="table">
        /// <listheader>
        ///     <term>Hex</term>
        ///     <term>String</term>
        ///     <term>Description</term>
        /// </listheader>
        /// <item>
        ///     <term><c>0x40</c></term>
        ///     <term><c>@</c></term>
        ///     <term>Colour palette</term>
        /// </item>
        /// <item>
        ///     <term><c>0x42</c></term>
        ///     <term><c>B</c></term>
        ///     <term>Status bar images</term>
        /// </item>
        /// <item>
        ///     <term><c>0x44</c></term>
        ///     <term><c>D</c></term>
        ///     <term>Mip textures</term>
        /// </item>
        /// <item>
        ///     <term><c>0x45</c></term>
        ///     <term><c>E</c></term>
        ///     <term>Console picture (flat)</term>
        /// </item>
        /// </list>
        /// WAD3:<br/>
        /// <list type="table">
        /// <listheader>
        ///     <term>Hex</term>
        ///     <term>String</term>
        ///     <term>Description</term>
        /// </listheader>
        /// <item>
        ///     <term><c>0x44</c></term>
        ///     <term><c>D</c></term>
        ///     <term>Mip texture</term>
        /// </item>
        /// <item>
        ///     <term><c>0x42</c></term>
        ///     <term><c>B</c></term>
        ///     <term><c>qpic</c></term>
        /// </item>
        /// <item>
        ///     <term><c>0x45</c></term>
        ///     <term><c>E</c></term>
        ///     <term>Font</term>
        /// </item>
        /// </list>
        /// </summary>
        public int LumpType { get => lumpType; set => lumpType = value; }

        /// <summary>
        /// The lumps name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The lump bytes. Can be empty
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Whether or not this lump is compressed. Only applicable to <see cref="Identifier.WAD2"/> and <see cref="Identifier.WAD3"/> WADs
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Whether or not this lump has been determined to have been corrupted (either name or bytes are corrupt).
        /// </summary>
        public bool IsCorrupt { get; set; }

        /// <summary>
        /// Whether or not this lump's bytes are corrupt
        /// </summary>
        public bool IsCorruptBytes { get; set; }

        /// <summary>
        /// Alias of <see cref="Name"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Compares the current lump's <see cref="Size"/>, <see cref="Offset"/> and <see cref="Name"/> against the same 
        /// values in <paramref name="obj"/> if <paramref name="obj"/> is an instance of <see cref="Lump"/>. Does not
        /// compare the bytes of each <see cref="Lump"/>.
        /// </summary>
        /// <param name="obj">The object to compare the lump against</param>
        /// <returns>True if both lumps have the same size, offset and name</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Lump lump = (Lump)obj;

            return Size == lump.Size && Offset == lump.Offset && Name.Equals(lump.Name);
        }

        /// <summary>
        /// Generates the lumps hash code based on <see cref="Lump.Size"/>, <see cref="Lump.Offset"/> and <see cref="Lump.Name"/>.
        /// </summary>
        /// <returns>The lumps hash code</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash *= 23 + Size.GetHashCode();
            hash *= 23 + Offset.GetHashCode();
            hash *= 23 + Name.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Compares <paramref name="other"/> to see if it preceeds, follows or appears in the same position as
        /// the current <see cref="Lump"/>. Uses <see cref="Name"/>, <see cref="Position"/> and <see cref="Size"/> 
        /// in it's comparison. If <paramref name="other"/> is <c>null</c> then it will the current instance will
        /// preceed <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The lump to compare against</param>
        /// <returns>A negative integer if this instance should preceed <paramref name="other"/>, positive if it should follow or 0 if they both occupy the same position</returns>
        public int CompareTo(Lump other)
        {
            if (other == null)
            {
                return -1;
            }

            if (!Name.Equals(other.Name))
            {
                return Name.CompareTo(other.Name);
            }

            if (Position != other.Position)
            {
                return Position.CompareTo(other.Position);
            }

            return Size.CompareTo(other.Size);
        }
    }
}

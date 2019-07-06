using System;
using System.IO;
using System.Text;

namespace nz.doom.WadParser
{
    /// <summary>
    /// Parse IWAD, PWAD, WAD2 and WAD3 files into a <see cref="Wad"/> object.
    /// </summary>
    public class WadParser
    {
        /// <summary>
        /// The directory size for <see cref="Identifier.IWAD"/> and <see cref="Identifier.PWAD"/> <see cref="Lump"/>s. Each directory contains an offset, size and name for an individual lump.
        /// One directory entry for each Lump in the WAD. See also <seealso cref="WAD2_DIRECTORY_SIZE"/>.
        /// </summary>
        public static readonly int WAD_DIRECTORY_SIZE = 16;

        /// <summary>
        /// The directory size for <see cref="Identifier.WAD2"/> and <see cref="Identifier.WAD3"/> <see cref="Lump"/>s. Each directory contains an offset, size and name for an individual lump.
        /// One directory entry for each Lump in the WAD. See also <seealso cref="WAD_DIRECTORY_SIZE"/>.
        /// </summary>
        public static readonly int WAD2_DIRECTORY_SIZE = 32;

        /// <summary>
        /// The position in the directory where the <see cref="Lump"/> name is stored. Used for <see cref="Identifier.IWAD"/> and <see cref="Identifier.PWAD"/>. See also <seealso cref="WAD2_NAME_OFFSET"/>.
        /// </summary>
        public static readonly int WAD_NAME_OFFSET = 8;

        /// <summary>
        /// The maximum size of a <see cref="Lump"/> name in <see cref="Identifier.IWAD"/> and <see cref="Identifier.PWAD"/>. See also <seealso cref="WAD2_NAME_SIZE"/>.
        /// </summary>
        public static readonly int WAD_NAME_SIZE = 8;

        /// <summary>
        /// The position in the directory where the <see cref="Lump"/> name is stored. Used for <see cref="Identifier.WAD2"/> and <see cref="Identifier.WAD3"/>. See also <seealso cref="WAD_NAME_OFFSET"/>.
        /// </summary
        public static readonly int WAD2_NAME_OFFSET = 16;

        /// <summary>
        /// The maximum size of a <see cref="Lump"/> name in <see cref="Identifier.WAD2"/> and <see cref="Identifier.WAD3"/>. See also <seealso cref="WAD_NAME_SIZE"/>.
        /// </summary>
        public static readonly int WAD2_NAME_SIZE = 16;

        /// <summary>
        /// The size of the WAD header than contains the WAD type magic bytes, the number of <see cref="Lump"/>s and the directory offset
        /// </summary>
        public static readonly int HEADER_SIZE = 12;

        /// <summary>
        /// Parse the given WAD file located at <paramref name="wadFileStream"/> into a <see cref="Wad"/> object. If <paramref name="readBytes"/> is false
        /// then the bytes in each lump can be read using <see cref="ReadBytesIntoLump(FileStream, Lump)"/>.
        /// </summary>
        /// <param name="wadFileStream">The WAD FileStream</param>
        /// <param name="readBytes">Should the bytes be parsed into the lump object or not. Defaults to <c>true</c></param>
        /// <exception cref="WadParseException">Thrown if the WAD is in an invalid format</exception>
        /// <returns>The parsed WAD file with each of it's <see cref="Lump"/>s</returns>
        public static Wad Parse(FileStream wadFileStream, bool readBytes = true)
        {
            // read first 4 bytes to determine the WAD type
            var wadTypeBytes = ReadBytesAtPosition(wadFileStream, 0, 4);
            string wadType = Identifier.GetWadType(wadTypeBytes);

            int dirSize = WAD_DIRECTORY_SIZE;
            int nameOffset = WAD_NAME_OFFSET;
            int nameSize = WAD_NAME_SIZE;

            switch (wadType)
            {
                case Identifier.PWAD:
                case Identifier.IWAD:
                    break;
                case Identifier.WAD2:
                case Identifier.WAD3:
                    dirSize = WAD2_DIRECTORY_SIZE;
                    nameOffset = WAD2_NAME_OFFSET;
                    nameSize = WAD2_NAME_SIZE;
                    break;
                default:
                    throw new WadParseException($"Invalid WAD type '{wadType}'. Type must be one of: PWAD, IWAD, WAD2 or WAD3");
            }

            int lumpCount = BitConverter.ToInt32(ReadBytesAtPosition(wadFileStream, 4, 4));

            if (lumpCount > 65536 || lumpCount < 0)
            {
                throw new WadParseException($"Invalid number of lumps listed in header. Max supported 65536, found {lumpCount}");
            }

            int directoryOffset = BitConverter.ToInt32(ReadBytesAtPosition(wadFileStream, 8, 4));

            if (directoryOffset > wadFileStream.Length)
            {
                throw new WadParseException($"Directory offset {directoryOffset} is beyond the WAD filesize of {wadFileStream.Length}");
            }

            if (directoryOffset < HEADER_SIZE)
            {
                throw new WadParseException($"Directory offset {directoryOffset} is before the WAD header");
            }

            if ((directoryOffset + (dirSize * lumpCount)) > wadFileStream.Length)
            {
                throw new WadParseException($"Directory goes off the end of the WAD file");
            }

            var lumpEntryNumber = -1;
            var maxLumpSize = wadFileStream.Length - (dirSize * lumpCount);

            var wad = new Wad(wadFileStream.Name, (int)wadFileStream.Length, Path.GetFileName(wadFileStream.Name));
            wad.WadType = wadType;

            for (int position = directoryOffset; position < (directoryOffset + (lumpCount * dirSize)); position += dirSize)
            {
                int lumpOffset = BitConverter.ToInt32(ReadBytesAtPosition(wadFileStream, position, 4));
                lumpEntryNumber++;

                byte[] lumpNameBytes = ReadBytesAtPosition(wadFileStream, position + nameOffset, nameSize);

                string lumpName = Encoding.ASCII.GetString(lumpNameBytes, 0, lumpNameBytes.Length);

                var nullOffset = lumpName.IndexOf("\0");
                if (nullOffset > -1)
                {
                    lumpName = lumpName.Substring(0, nullOffset);
                }

                Lump lump = new Lump
                {
                    Offset = lumpOffset,
                    Position = lumpEntryNumber,
                    Size = BitConverter.ToInt32(ReadBytesAtPosition(wadFileStream, position + 4, 4))
                };

                if (String.IsNullOrEmpty(lumpName))
                {
                    lump.Name = "<CORRUPT NAME>";
                    lump.IsCorrupt = true;
                }
                else
                {
                    lump.Name = lumpName;
                }

                switch (wadType)
                {
                    case "WAD2":
                    case "WAD3":
                        lump.LumpType = BitConverter.ToInt16(ReadBytesAtPosition(wadFileStream, position + 12, 2));
                        lump.IsCompressed = BitConverter.ToInt16(ReadBytesAtPosition(wadFileStream, position + 14, 2)) == 1;
                        break;
                }

                if (lump.Size == 0)
                {
                    lump.Bytes = new byte[0];
                }
                else if (lump.Size < 0 || lump.Size > maxLumpSize || lumpOffset < HEADER_SIZE)
                {
                    lump.IsCorrupt = true;
                    lump.Bytes = new byte[0];
                    lump.IsCorruptBytes = true;
                }
                else if (readBytes)
                {
                    lump.Bytes = ReadBytesAtPosition(wadFileStream, lumpOffset, lump.Size);                    
                }

                wad.AddLump(lump);
            }

            return wad;
        }

        /// <summary>
        /// Parse the given WAD file located at <paramref name="wadPath"/> into a <see cref="Wad"/> object. If <paramref name="readBytes"/> is false
        /// then the bytes in each lump can be read using <see cref="ReadBytesIntoLump(string, Lump)"/>.
        /// </summary>
        /// <param name="wadPath">The location of the WAD file to parse</param>
        /// <param name="readBytes">Should the bytes be parsed into the lump object or not. Defaults to <c>true</c></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="wadPath"/> is null</exception>
        /// <exception cref="WadParseException">Thrown if the WAD is in an invalid format</exception>
        /// <returns>The parsed WAD file with each of it's <see cref="Lump"/>s</returns>
        public static Wad Parse(string wadPath, bool readBytes = true)
        {
            if (wadPath == null)
            {
                throw new ArgumentNullException(nameof(wadPath));
            }

            using (FileStream wadFileStream = File.OpenRead(wadPath))
            {
                return Parse(wadFileStream, readBytes);
            }
        }

        /// <summary>
        /// Reads <paramref name="length"/> number of bytes from the <paramref name="fileStream"/> starting at <paramref name="position"/>.
        /// </summary>
        /// <param name="fileStream">The file stream to read the bytes from</param>
        /// <param name="position">Where in the stream should the bytes be read from</param>
        /// <param name="length">How many bytes to read</param>
        /// <returns>The array of bytes read from <paramref name="fileStream"/></returns>
        public static byte[] ReadBytesAtPosition(FileStream fileStream,int position = 0,int length = 1)
        {
            var bytes = new byte[length];

            fileStream.Position = position;
            int offset = 0;

            int read;
            while ((read = fileStream.Read(bytes, offset, bytes.Length - offset)) > 0 && offset < length)
            {
                offset += read;
            }

            return bytes;
        }

        /// <summary>
        /// Read the bytes for the <paramref name="lumpToRead"/> passed in from <paramref name="wadFileStream"/>.
        /// </summary>
        /// <param name="wadFileStream">The WAD file stream to read the bytes from.</param>
        /// <param name="lumpToRead">Which lump should be read from <paramref name="wadFileStream"/>.</param>
        /// <returns><paramref name="lumpToRead"/> with the <see cref="Lump.Bytes"/> read in</returns>
        public static Lump ReadBytesIntoLump(FileStream wadFileStream, Lump lumpToRead)
        {
            if ((lumpToRead.Bytes == null && lumpToRead.IsCorruptBytes) || lumpToRead.Size == 0)
            {
                lumpToRead.Bytes = new byte[] { };
                return lumpToRead;
            }

            byte[] bytes = ReadBytesAtPosition(wadFileStream, lumpToRead.Offset, lumpToRead.Size);

            lumpToRead.Bytes = bytes;

            return lumpToRead;
        }

        /// <summary>
        /// Read the bytes for the <paramref name="lumpToRead"/> passed in from <paramref name="wadPath"/>.
        /// </summary>
        /// <param name="wadPath">The WAD file to read the bytes from.</param>
        /// <param name="lumpToRead">Which lump should be read from <paramref name="wadPath"/>.</param>
        /// <returns><paramref name="lumpToRead"/> with the <see cref="Lump.Bytes"/> read in</returns>
        public static Lump ReadBytesIntoLump(string wadPath, Lump lumpToRead)
        {
            using (FileStream wadFileStream = File.OpenRead(wadPath))
            {
                return ReadBytesIntoLump(wadFileStream, lumpToRead);
            }
        }
        
    }
}

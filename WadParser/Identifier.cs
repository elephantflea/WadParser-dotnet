using System;
using System.IO;

namespace nz.doom.WadParser
{
    /// <summary>
    /// Helper class used to determine the type of WAD a given file is.
    /// </summary>
    public class Identifier
    {
        /// <summary>
        /// Magic bytes for <c>IWAD</c> files. See <seealso cref="IWAD"/>
        /// </summary>
        public static readonly byte[] MAGIC_IWAD = { 0x49, 0x57, 0x41, 0x44 };
        /// <summary>
        /// Magic bytes for <c>PWAD</c> files. See <seealso cref="PWAD"/>
        /// </summary>
        public static readonly byte[] MAGIC_PWAD = { 0x50, 0x57, 0x41, 0x44 };
        /// <summary>
        /// Magic bytes for <c>WAD2</c> files. See <seealso cref="WAD2"/>
        /// </summary>
        public static readonly byte[] MAGIC_WAD2 = { 0x57, 0x41, 0x44, 0x32 };
        /// <summary>
        /// Magic bytes for <c>WAD3</c> files. See <seealso cref="WAD3"/>
        /// </summary>
        public static readonly byte[] MAGIC_WAD3 = { 0x57, 0x41, 0x44, 0x33 };

        /// <summary>
        /// <c>Internal WAD</c>. Used for commercial Doom engine WADs. See <a href="https://doomwikig.org/wiki/IWAD">DoomWiki</a> for more details.
        /// </summary>
        public const string IWAD = "IWAD";
        /// <summary>
        /// <c>Patch WAD</c>. Used for user generated (mods) Doom engine WADs. See <a href="https://doomwikig.org/wiki/PWAD">DoomWiki</a> for more details.
        /// </summary>
        public const string PWAD = "PWAD";
        /// <summary>
        /// Used for <c>Quake Engine</c> based WAD files. See <a href="http://www.gamers.org/dEngine/quake/spec/quake-spec34/qkspec_7.htm">Gamers.org</a> for more details.
        /// </summary>
        public const string WAD2 = "WAD2";
        /// <summary>
        /// Used for <c>GoldSrc</c> (<c>Half-Life</c>, <c>Opposing Force</c> etc..) based WAD files. See <a href="http://hlbsp.sourceforge.net/index.php?content=waddef">hlbsp project</a> for more details.
        /// </summary>
        public const string WAD3 = "WAD3";
        /// <summary>
        /// Used for WAD files that cannot be identified.
        /// </summary>
        public const string UNKNOWN = "UNKNOWN";

        /// <summary>
        /// Checks the first 4 bytes of the byte array <paramref name="fileBytes"/> to see if it matches one
        /// of the known <c>WAD</c> formats. See also <seealso cref="GetWadType(string)"/>.
        /// </summary>
        /// <param name="fileBytes">The WAD bytes to check</param>
        /// <returns>One of the WAD constants or <see cref="UNKNOWN"/> if <paramref name="fileBytes"/> cannot be identified</returns>
        public static string GetWadType(byte[] fileBytes)
        {
            if(fileBytes.Length < 4)
            {
                return UNKNOWN;
            }

            if (ArrayStartsWith(MAGIC_PWAD, fileBytes))
            {
                return PWAD;
            }

            if (ArrayStartsWith(MAGIC_IWAD, fileBytes))
            {
                return IWAD;
            }

            if (ArrayStartsWith(MAGIC_WAD2, fileBytes))
            {
                return WAD2;
            }            

            if (ArrayStartsWith(MAGIC_WAD3, fileBytes))
            {
                return WAD3;
            }


            return UNKNOWN;
        }

        /// <summary>
        /// Will read the first 4 bytes of the file represented by <paramref name="path"/> to determine it's WAD 
        /// format. See also <seealso cref="GetWadType(byte[])"/>.
        /// </summary>
        /// <param name="path">The string representing the file path to check</param>
        /// <returns>One of the WAD constants or <see cref="UNKNOWN"/> if <paramref name="fileBytes"/> cannot be identified</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Thrown if the file found at <paramref name="path"/> does not exist</exception>
        public static string GetWadType(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            using (FileStream wadFileStream = File.OpenRead(path))
            {
                var headerBytes = new byte[4];

                wadFileStream.Read(headerBytes, 0, headerBytes.Length);

                return GetWadType(headerBytes);
            }
        }

        /// <summary>
        /// Checks to see if the contents of <paramref name="startsWith"/> is found at the begining of
        /// <paramref name="toCheck"/>
        /// </summary>
        /// <param name="startsWith">The array of bytes to look for</param>
        /// <param name="toCheck">The array of bytes to check the presence of <paramref name="startsWith"/> at position 0</param>
        /// <returns>True if <paramref name="startsWith"/> is found at the start of <paramref name="toCheck"/></returns>
        public static bool ArrayStartsWith(byte[] startsWith, byte[] toCheck)
        {
            if(startsWith.Length > toCheck.Length)
            {
                return false;
            }

            for (int i = 0; i < startsWith.Length; i++)
            {
                if(startsWith[i] != toCheck[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

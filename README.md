# WadParser-dotnet
A C# .NET Core (2.1) helper library to parse WAD files and extract lumps contained in Doom, Quake and Half-life WAD files.

Based on:
* https://doomwiki.org/wiki/WAD
* http://www.gamers.org/dEngine/quake/spec/quake-spec34/qkspec_7.htm
* http://hlbsp.sourceforge.net/index.php?content=waddef

## Supported Formats
* IWAD/PWAD
  * Doom, Heretic, Hexen and Strife
* WAD2
  * Quake
* WAD3
  * Half-Life, Opposing Force, Blue Shift
  
## Usage
```csharp
using System;
using nz.doom.WadParser;

namespace WadReader
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args == null || args.Length != 1)
            {
                Console.Error.WriteLine("WAD file required");
                return 2;
            }

            Wad wad = WadParser.Parse(args[0]);

            foreach(var lump in wad.Lumps)
            {
                Console.WriteLine(lump.Name)
            }

            return 0;
        }
    }
}
```
### See also
Also available for Java https://github.com/elephantflea/WadParser
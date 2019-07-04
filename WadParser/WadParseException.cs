using System;
using System.Collections.Generic;
using System.Text;

namespace nz.doom.WadParser
{
    /// <summary>
    /// Exception that is thrown when a WAD file cannot be parsed due to the WAD being in an invalid format.
    /// </summary>
    public class WadParseException : Exception
    {
        public WadParseException()
        {

        }

        public WadParseException(string message) : base(message)
        {

        }

        public WadParseException(string message, Exception inner) : base(message,inner)
        {

        }
    }
}

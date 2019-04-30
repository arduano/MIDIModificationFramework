using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    class CorruptChunkHeadersException : Exception
    {
        public CorruptChunkHeadersException() : base() { }
        public CorruptChunkHeadersException(string message) : base(message) { }
    }
}

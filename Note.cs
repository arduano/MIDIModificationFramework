using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class Note
    {
        ulong start;
        public ulong Start { get => start; set => start = value; }
        public byte Key { get; set; }
        public byte Velocity { get; set; }
        ulong end;
        public ulong End
        {
            get => end;
            set
            {
                if (value < start) throw new ArgumentException("Note end can not be less than start");
                end = value;
            }
        }

        public ulong Length
        {
            get => end - start;
            set
            {
                end = start + value;
            }
        }

        public Note(byte key, byte vel, ulong start, ulong end)
        {
            this.start = start;
            this.end = end;
            this.Key = key;
            this.Velocity = vel;
        }
    }
}

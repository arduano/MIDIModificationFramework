using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public class Note
    {
        double start;
        public double Start { get => start; set => start = value; }
        public byte Channel { get; set; }
        public byte Key { get; set; }
        public byte Velocity { get; set; }
        double end;
        public double End
        {
            get => end;
            set
            {
                if (value < start) throw new ArgumentException("Note end can not be less than start");
                end = value;
            }
        }

        public double Length
        {
            get => end - start;
            set
            {
                end = start + value;
            }
        }

        public Note(byte channel, byte key, byte vel, double start, double end)
        {
            Channel = channel;
            this.start = start;
            this.end = end;
            this.Key = key;
            this.Velocity = vel;
        }
    }
}

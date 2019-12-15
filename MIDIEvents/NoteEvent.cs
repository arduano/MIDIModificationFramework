using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDIEvents
{
    public abstract class NoteEvent : MIDIEvent
    {
        byte channel;
        public byte Channel
        {
            get { return channel; }
            set
            {
                channel = (byte)(value & 0x0F);
            }
        }
        public byte Key { get; set; }

        public NoteEvent(double delta, byte key) : base(delta)
        {
            Channel = channel;
            Key = key;
        }
    }
}

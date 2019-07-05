using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.MIDIEvents
{
    public class ChannelPressureEvent :MIDIEvent
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
        public byte Pressure { get; set; }

        public ChannelPressureEvent(uint delta, byte channel, byte pressure) : base(delta)
        {
            Channel = channel;
            Pressure = pressure;
        }

        public override MIDIEvent Clone()
        {
            return new ChannelPressureEvent(DeltaTime, Channel, Pressure);
        }

        public override byte[] GetData()
        {
            return new byte[]
            {
                (byte)(0b11010000 | Channel),
                Pressure
            };
        }
    }
}

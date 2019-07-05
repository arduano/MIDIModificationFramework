using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework.Interfaces
{
    interface IEventWriter
    {
        void WriteEvent(MIDIEvent e);
    }
}

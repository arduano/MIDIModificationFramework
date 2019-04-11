using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIModificationFramework
{
    public static class XZ
    {
        public static Stream RemoveXZCompressionLayer(Stream input, int threads = 0)
        {
            Process xz = new Process();
            xz.StartInfo = new ProcessStartInfo("xz", "-dc --threads=" + threads)
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            xz.Start();
            Task.Run(() =>
            {
                input.CopyTo(xz.StandardInput.BaseStream);
                xz.StandardInput.Close();
            });
            return xz.StandardOutput.BaseStream;
        }

        public static Stream AddXZCompressionLayer(Stream input, int threads = 0)
        {
            Process xz = new Process();
            xz.StartInfo = new ProcessStartInfo("xz", "-zc --threads=" + threads)
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            xz.Start();
            Task.Run(() =>
            {
                input.CopyTo(xz.StandardInput.BaseStream);
                xz.StandardInput.Close();
            });
            return xz.StandardOutput.BaseStream;
        }
    }
}

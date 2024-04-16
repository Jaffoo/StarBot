using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBot.DeskServer
{
    public class SizeChangeEventArgs : EventArgs
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

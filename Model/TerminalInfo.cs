using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TerminalInfo
    {
        public Socket socket { get; set; }
        public NetworkStream networkStream { get; set; }
    }
}

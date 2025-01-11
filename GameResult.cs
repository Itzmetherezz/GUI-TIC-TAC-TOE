using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TIC_TAC_TOE
{
    public class GameResult
    {
        public Player Winner { get; set; }
        public required WinInfo WinInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCSharp
{
    public class VisitorEventArgs : EventArgs
    {
        public bool Exclude { get; set; }
        public bool Terminate { get; set; }
    }
}

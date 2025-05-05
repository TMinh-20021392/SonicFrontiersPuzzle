using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicFrontiersPuzzle.Events
{
    public class NodeValueChangedEventArgs : EventArgs
    {
        public int NodeIndex { get; }
        public int Value { get; }

        public NodeValueChangedEventArgs(int nodeIndex, int value)
        {
            NodeIndex = nodeIndex;
            Value = value;
        }
    }
}

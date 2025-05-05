namespace DirectedGraphPuzzleSolver
{
    public partial class DirectedGraphPuzzleForm
    {
        public class Edge
        {
            public int From { get; set; }
            public int To { get; set; }

            public Edge(int from, int to)
            {
                From = from;
                To = to;
            }

            public override bool Equals(object obj)
            {
                if (obj is Edge other)
                {
                    return From == other.From && To == other.To;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(From, To);
            }
        }
    }
}
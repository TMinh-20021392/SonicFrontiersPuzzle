namespace DirectedGraphPuzzleSolver
{
    public partial class DirectedGraphPuzzleForm
    {
        // Core graph and puzzle solving logic
        #region PuzzleSolver

        public class Edge
        {
            public int From { get; set; }
            public int To { get; set; }

            public Edge(int from, int to)
            {
                From = from;
                To = to;
            }
        }

        #endregion
    }
}
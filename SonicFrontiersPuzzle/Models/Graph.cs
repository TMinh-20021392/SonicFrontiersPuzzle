namespace DirectedGraphPuzzleSolver
{
    public partial class DirectedGraphPuzzleForm
    {
        public class Graph
        {
            public int NodeCount { get; private set; }
            public int Modulo { get; private set; }
            public List<List<int>> Adjacency { get; private set; }

            public Graph(int nodeCount, int modulo, List<Edge> edges)
            {
                NodeCount = nodeCount;
                Modulo = modulo;
                Adjacency = new List<List<int>>();

                // Initialize adjacency lists for each node
                for (int i = 0; i < nodeCount; i++)
                {
                    Adjacency.Add(new List<int>());
                }

                // Add directed edges
                foreach (var edge in edges)
                {
                    Adjacency[edge.From].Add(edge.To);
                }
            }

            // Increment the value of a specific node and all nodes it points to
            public void Increment(int[] values, int nodeIndex)
            {
                // Increment all nodes that this node points to (including self if there's a self-loop)
                foreach (var target in Adjacency[nodeIndex])
                {
                    values[target] = (values[target] + 1) % Modulo;
                }
            }

            // Check if the current state matches the goal state
            public bool IsGoalReached(int[] currentValues, int[] goalValues)
            {
                for (int i = 0; i < NodeCount; i++)
                {
                    if (currentValues[i] != goalValues[i])
                        return false;
                }
                return true;
            }
        }
    }
}
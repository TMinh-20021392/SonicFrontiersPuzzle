using System;
using System.Collections.Generic;
using System.Linq;

namespace DirectedGraphPuzzleSolver
{
    public class Program
    {
        // Represents a directed edge from one node to another
        public class Arrow
        {
            public int From { get; set; }
            public int To { get; set; }

            public Arrow(int from, int to)
            {
                From = from;
                To = to;
            }
        }

        // Graph representation where each node has outgoing edges
        public class Graph
        {
            public int NodeCount { get; private set; }
            public int Modulo { get; private set; }
            public List<List<int>> Adjacency { get; private set; }

            public Graph(int nodeCount, int modulo, List<Arrow> arrows)
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
                foreach (var arrow in arrows)
                {
                    Adjacency[arrow.From].Add(arrow.To);
                }
            }

            // Increment the value of a specific node and all nodes it points to
            public void Increment(int[] values, int nodeIndex)
            {
                // Increment the selected node
                values[nodeIndex] = (values[nodeIndex] + 1) % Modulo;
                
                // Increment all nodes that this node points to
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

        // Using breadth-first search to find the shortest solution
        public static List<int> SolvePuzzle(Graph graph, int[] initialValues, int[] goalValues)
        {
            // Queue for BFS
            Queue<(int[] state, List<int> moves)> queue = new Queue<(int[] state, List<int> moves)>();
            
            // Store visited states to avoid cycles
            HashSet<string> visited = new HashSet<string>();
            
            // Start with initial state and empty moves list
            queue.Enqueue((initialValues.ToArray(), new List<int>()));
            visited.Add(GetStateKey(initialValues));

            while (queue.Count > 0)
            {
                var (currentState, currentMoves) = queue.Dequeue();
                
                // Check if we've reached the goal
                if (graph.IsGoalReached(currentState, goalValues))
                    return currentMoves;
                
                // Try incrementing each node
                for (int nodeIndex = 0; nodeIndex < graph.NodeCount; nodeIndex++)
                {
                    // Create a copy of the current state
                    int[] newState = currentState.ToArray();
                    
                    // Apply the move
                    graph.Increment(newState, nodeIndex);
                    
                    // Generate a unique key for this state
                    string stateKey = GetStateKey(newState);
                    
                    // If we haven't seen this state before, add it to the queue
                    if (!visited.Contains(stateKey))
                    {
                        var newMoves = new List<int>(currentMoves) { nodeIndex };
                        queue.Enqueue((newState, newMoves));
                        visited.Add(stateKey);
                    }
                }
            }
            
            // If no solution is found
            return null;
        }

        // Helper method to create a unique string key for a state
        private static string GetStateKey(int[] state)
        {
            return string.Join(",", state);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Directed Graph Puzzle Solver");
            Console.WriteLine("===========================");

            // Get number of nodes
            Console.Write("Enter the number of nodes: ");
            int nodeCount = int.Parse(Console.ReadLine());

            // Get modulo value
            Console.Write("Enter the modulo value: ");
            int modulo = int.Parse(Console.ReadLine());

            // Get initial state
            Console.WriteLine($"Enter the initial values for {nodeCount} nodes (space-separated):");
            int[] initialValues = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();

            // Get goal state
            Console.WriteLine($"Enter the goal values for {nodeCount} nodes (space-separated):");
            int[] goalValues = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();

            // Get the number of arrows
            Console.Write("Enter the number of directed arrows: ");
            int arrowCount = int.Parse(Console.ReadLine());

            List<Arrow> arrows = new List<Arrow>();
            Console.WriteLine("Enter each arrow as 'fromNode toNode' (0-indexed):");
            
            for (int i = 0; i < arrowCount; i++)
            {
                string[] input = Console.ReadLine().Split(' ');
                int from = int.Parse(input[0]);
                int to = int.Parse(input[1]);
                arrows.Add(new Arrow(from, to));
            }

            // Add self-loops (each node increments itself)
            for (int i = 0; i < nodeCount; i++)
            {
                arrows.Add(new Arrow(i, i));
            }

            // Create the graph
            Graph graph = new Graph(nodeCount, modulo, arrows);

            // Find solution
            Console.WriteLine("\nSolving the puzzle...");
            List<int> solution = SolvePuzzle(graph, initialValues, goalValues);

            // Display result
            if (solution != null)
            {
                Console.WriteLine($"Solution found! Moves required: {solution.Count}");
                Console.WriteLine("Sequence of nodes to increment:");
                Console.WriteLine(string.Join(" ", solution));
                
                // Optional: Show the step-by-step transformation
                Console.WriteLine("\nStep-by-step transformation:");
                int[] currentState = initialValues.ToArray();
                Console.WriteLine($"Initial state: {string.Join(" ", currentState)}");
                
                for (int i = 0; i < solution.Count; i++)
                {
                    int node = solution[i];
                    graph.Increment(currentState, node);
                    Console.WriteLine($"After incrementing node {node}: {string.Join(" ", currentState)}");
                }
            }
            else
            {
                Console.WriteLine("No solution found for this puzzle configuration.");
            }
        }
    }
}

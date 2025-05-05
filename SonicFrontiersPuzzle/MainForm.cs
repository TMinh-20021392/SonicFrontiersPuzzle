using SonicFrontiersPuzzle.Events;
using System;
using System.Drawing;
using System.Windows.Forms;
using static DirectedGraphPuzzleSolver.DirectedGraphPuzzleForm;

namespace DirectedGraphPuzzleSolver
{
    public partial class MainForm : Form
    {
        // Constants for visual layout
        public const int NodeSize = 60;
        public const int NodeMargin = 30;
        public const int DefaultModulo = 8;
        public const int DefaultNodeCount = 4;

        // Core data structures
        private int nodeCount;
        private int modulo;

        // UI panels
        private ConfigurationPanel configPanel;
        private InitialGraphPanel initialGraphPanel;
        private TargetGraphPanel targetGraphPanel;
        private SolutionPanel solutionPanel;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScroll = true;
            ClientSize = new Size(800, 1200);
            Name = "MainForm";
            Text = "Directed Graph Puzzle Solver";

            ResumeLayout(false);
        }

        private void InitializeUI()
        {
            // Set initial values
            nodeCount = DefaultNodeCount;
            modulo = DefaultModulo;

            // Configuration panel
            configPanel = new ConfigurationPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(configPanel);

            // Initial graph panel
            GroupBox initialGraphGroupBox = new()
            {
                Text = "Initial Graph",
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };
            Controls.Add(initialGraphGroupBox);

            initialGraphPanel = new InitialGraphPanel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            initialGraphGroupBox.Controls.Add(initialGraphPanel);

            // Target graph panel
            GroupBox targetGraphGroupBox = new()
            {
                Text = "Target Graph (Set target values)",
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };
            Controls.Add(targetGraphGroupBox);

            targetGraphPanel = new TargetGraphPanel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            targetGraphGroupBox.Controls.Add(targetGraphPanel);

            // Solution panel
            GroupBox solutionGroupBox = new()
            {
                Text = "Solution",
                Dock = DockStyle.Top,
                Height = 300,
                Padding = new Padding(10)
            };
            Controls.Add(solutionGroupBox);

            solutionPanel = new SolutionPanel
            {
                Dock = DockStyle.Fill
            };
            solutionGroupBox.Controls.Add(solutionPanel);

            // Connect the panels and set up event handlers
            configPanel.SetupButtonClicked += (sender, e) =>
            {
                if (configPanel.TryGetConfiguration(out int count, out int mod))
                {
                    nodeCount = count;
                    modulo = mod;
                    initialGraphPanel.SetupGraph(nodeCount, modulo);
                    targetGraphPanel.SetupGraph(nodeCount, modulo);
                    initialGraphPanel.NodeValuesChanged += UpdateTargetGraph;
                    solutionPanel.EnableSolutionGeneration(true);
                }
            };

            initialGraphPanel.EdgesChanged += (sender, e) =>
            {
                targetGraphPanel.UpdateEdges(initialGraphPanel.Edges);
            };

            solutionPanel.GenerateSolutionClicked += (sender, e) =>
            {
                GenerateSolution();
            };
        }

        private void UpdateTargetGraph(object sender, NodeValueChangedEventArgs e)
        {
            targetGraphPanel.UpdateNodeValue(e.NodeIndex, e.Value);
        }

        private void GenerateSolution()
        {
            try
            {
                solutionPanel.SetStatus("Generating solution...");

                // Get initial and target values
                int[] initialValues = initialGraphPanel.GetNodeValues();
                int[] targetValues = targetGraphPanel.GetNodeValues();

                // Add self-loops if they don't exist
                initialGraphPanel.AddSelfLoopsIfNeeded();

                // Create graph and solve puzzle
                Graph graph = new(nodeCount, modulo, initialGraphPanel.Edges);
                List<int> solution = SolvePuzzle(graph, initialValues, targetValues);

                if (solution != null)
                {
                    solutionPanel.DisplaySolution(solution, graph, initialValues);
                }
                else
                {
                    solutionPanel.SetNoSolutionFound();
                }
            }
            catch (Exception ex)
            {
                solutionPanel.SetError($"Error generating solution: {ex.Message}");
            }
        }

        // Using breadth-first search to find the shortest solution
        private List<int> SolvePuzzle(Graph graph, int[] initialValues, int[] goalValues)
        {
            // Queue for BFS
            Queue<(int[] state, List<int> moves)> queue = new();

            // Store visited states to avoid cycles
            HashSet<string> visited = new();

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

        private void ResetApplication()
        {
            // Reset data structures
            nodeCount = DefaultNodeCount;
            modulo = DefaultModulo;

            // Reset UI components
            initialGraphPanel.ResetPanel();
            targetGraphPanel.ResetPanel();
            solutionPanel.ResetPanel();

            // Reset configuration panel
            configPanel.ResetPanel();

            // Update the UI
            Invalidate();
        }

        // Helper method to create a unique string key for a state
        private static string GetStateKey(int[] state)
        {
            return string.Join(",", state);
        }
    }
}
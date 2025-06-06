﻿using SonicFrontiersPuzzle.Events;
using SonicFrontiersPuzzle.Extensions;
using SonicFrontiersPuzzle.Panels;
using static DirectedGraphPuzzleSolver.DirectedGraphPuzzleForm;

namespace SonicFrontiersPuzzle
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
            DebugLogger.Log("MainForm constructor called");
            InitializeComponent();

            // Initialize non-nullable fields to default instances
            configPanel = new ConfigurationPanel();
            initialGraphPanel = new InitialGraphPanel();
            targetGraphPanel = new TargetGraphPanel();
            solutionPanel = new SolutionPanel();

            InitializeUI();
            DebugLogger.Log("MainForm initialized");
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScroll = true;
            ClientSize = new Size(800, 1200);
            Name = "MainForm";
            Text = "Directed Graph Puzzle Solver";

            ResumeLayout(false);
            DebugLogger.Log("MainForm components initialized");
        }

        private void InitializeUI()
        {
            // Set initial values
            nodeCount = DefaultNodeCount;
            modulo = DefaultModulo;
            DebugLogger.Log($"Initial nodeCount={nodeCount}, modulo={modulo}");

            // Create all panels first but don't add them to Controls yet

            // Configuration panel
            configPanel = new ConfigurationPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Solution panel
            GroupBox solutionGroupBox = new()
            {
                Text = "Solution",
                Dock = DockStyle.Top,
                Height = 300,
                Padding = new Padding(10)
            };

            solutionPanel = new SolutionPanel
            {
                Dock = DockStyle.Fill
            };
            solutionGroupBox.Controls.Add(solutionPanel);

            // Target graph panel
            GroupBox targetGraphGroupBox = new()
            {
                Text = "Target Graph (Set target values)",
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };

            targetGraphPanel = new TargetGraphPanel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            targetGraphGroupBox.Controls.Add(targetGraphPanel);

            // Initial graph panel
            GroupBox initialGraphGroupBox = new()
            {
                Text = "Initial Graph",
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };

            initialGraphPanel = new InitialGraphPanel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            initialGraphGroupBox.Controls.Add(initialGraphPanel);

            // Now add them to Controls in reverse order because of DockStyle.Top
            Controls.Add(solutionGroupBox);
            DebugLogger.Log("Solution panel added");

            Controls.Add(targetGraphGroupBox);
            DebugLogger.Log("Target graph panel added");

            Controls.Add(initialGraphGroupBox);
            DebugLogger.Log("Initial graph panel added");

            Controls.Add(configPanel);
            DebugLogger.Log("Configuration panel added");

            // Connect the panels and set up event handlers
            configPanel.SetupButtonClicked += (sender, e) =>
            {
                DebugLogger.Log("Setup button clicked");
                if (configPanel.TryGetConfiguration(out int count, out int mod))
                {
                    DebugLogger.Log($"Configuration valid: nodeCount={count}, modulo={mod}");
                    nodeCount = count;
                    modulo = mod;

                    // Set up graph panels
                    initialGraphPanel.SetupGraph(nodeCount, modulo);
                    targetGraphPanel.SetupGraph(nodeCount, modulo);

                    initialGraphPanel.NodeValuesChanged -= UpdateTargetGraph;
                    DebugLogger.Log("Connected NodeValuesChanged event to UpdateTargetGraph");

                    solutionPanel.EnableSolutionGeneration(true);
                }
                else
                {
                    DebugLogger.Log("Invalid configuration");
                }
            };

            initialGraphPanel.EdgesChanged += (sender, e) =>
            {
                DebugLogger.Log("Edges changed in initial graph, updating target graph");
                targetGraphPanel.UpdateEdges(initialGraphPanel.Edges);
            };

            solutionPanel.GenerateSolutionClicked += (sender, e) =>
            {
                DebugLogger.Log("Generate solution button clicked");
                GenerateSolution();
            };
        }

        private void UpdateTargetGraph(object? sender, NodeValueChangedEventArgs e)
        {
            DebugLogger.Log($"UpdateTargetGraph called: Node {e.NodeIndex} value changed to {e.Value}");
            targetGraphPanel.UpdateNodeValue(e.NodeIndex, e.Value);
            targetGraphPanel.Invalidate();
            targetGraphPanel.Refresh();
        }

        private void GenerateSolution()
        {
            try
            {
                DebugLogger.Log("Generating solution...");
                solutionPanel.SetStatus("Generating solution...");

                // Get initial and target values
                int[] initialValues = initialGraphPanel.GetNodeValues();
                int[] targetValues = targetGraphPanel.GetNodeValues();

                DebugLogger.Log($"Initial values: [{string.Join(", ", initialValues)}]");
                DebugLogger.Log($"Target values: [{string.Join(", ", targetValues)}]");

                // Add self-loops if they don't exist
                initialGraphPanel.AddSelfLoopsIfNeeded();

                // Create graph and solve puzzle
                Graph graph = new(nodeCount, modulo, initialGraphPanel.Edges);
                DebugLogger.Log("Created graph for solving");

                List<int>? solution = SolvePuzzle(graph, initialValues, targetValues);

                if (solution != null)
                {
                    DebugLogger.Log($"Solution found with {solution.Count} steps");
                    solutionPanel.DisplaySolution(solution, graph, initialValues);
                }
                else
                {
                    DebugLogger.Log("No solution found");
                    solutionPanel.SetNoSolutionFound();
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"Error generating solution: {ex.Message}\n{ex.StackTrace}");
                solutionPanel.SetError($"Error generating solution: {ex.Message}");
            }
        }

        // Using breadth-first search to find the shortest solution
        private static List<int>? SolvePuzzle(Graph graph, int[] initialValues, int[] goalValues)
        {
            DebugLogger.Log("Starting BFS to solve puzzle");

            // Queue for BFS
            Queue<(int[] state, List<int> moves)> queue = new();

            // Store visited states to avoid cycles
            HashSet<string> visited = new();

            // Start with initial state and empty moves list
            queue.Enqueue((initialValues.ToArray(), new List<int>()));
            visited.Add(GetStateKey(initialValues));

            DebugLogger.Log($"Initial state: {GetStateKey(initialValues)}");

            int iterations = 0;
            const int MaxIterations = 10000;

            while (queue.Count > 0 && iterations < MaxIterations)
            {
                iterations++;

                var (currentState, currentMoves) = queue.Dequeue();

                // Check if we've reached the goal
                if (graph.IsGoalReached(currentState, goalValues))
                {
                    DebugLogger.Log($"Goal reached after {iterations} iterations, solution has {currentMoves.Count} moves");
                    return currentMoves;
                }

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

            if (iterations % 1000 == 0)
            {
                DebugLogger.Log($"BFS iteration {iterations}, queue size: {queue.Count}");
            }

            DebugLogger.Log("Max iterations reached or no solution found");
            return null;
        }

        private void ResetApplication()
        {
            DebugLogger.Log("Resetting application to default state");

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

            DebugLogger.Log("Application reset complete");
        }

        // Helper method to create a unique string key for a state
        private static string GetStateKey(int[] state)
        {
            return string.Join(",", state);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DirectedGraphPuzzleSolver
{
    public partial class DirectedGraphPuzzleForm : Form
    {
        // Constants for visual layout
        private const int NodeSize = 60;
        private const int NodeMargin = 30;
        private const int DefaultModulo = 3;
        private const int DefaultNodeCount = 4;

        // Core data structures
        private int nodeCount;
        private int modulo;
        private List<Edge> edges = new();
        private List<NodeTextBox> initialGraphNodes = new();
        private List<NodeTextBox> targetGraphNodes = new();

        // UI Elements
        private TextBox nodeCountTextBox;
        private TextBox moduloTextBox;
        private Button setupButton;
        private Panel initialGraphPanel;
        private Panel targetGraphPanel;
        private Panel solutionPanel;
        private Button generateSolutionButton;
        private RichTextBox solutionTextBox;
        private Label statusLabel;

        // State tracking for arrow drawing
        private NodeTextBox sourceNode = null;
        private NodeTextBox currentLineEndPoint = null;
        private bool isDrawingArrow = false;

        public DirectedGraphPuzzleForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScroll = true;
            ClientSize = new Size(800, 1000);
            Name = "DirectedGraphPuzzleForm";
            Text = "Directed Graph Puzzle Solver";
            Paint += new PaintEventHandler(Form_Paint);

            ResumeLayout(false);
        }

        private void InitializeUI()
        {
            // Configuration panel
            Panel configPanel = new()
            {
                Dock = DockStyle.Top,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(configPanel);

            // Node count label and textbox
            Label nodeCountLabel = new()
            {
                Text = "Number of Nodes:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            configPanel.Controls.Add(nodeCountLabel);

            nodeCountTextBox = new TextBox
            {
                Location = new Point(150, 20),
                Size = new Size(50, 20),
                Text = DefaultNodeCount.ToString()
            };
            configPanel.Controls.Add(nodeCountTextBox);

            // Modulo label and textbox
            Label moduloLabel = new()
            {
                Text = "Modulo Value:",
                Location = new Point(250, 20),
                AutoSize = true
            };
            configPanel.Controls.Add(moduloLabel);

            moduloTextBox = new TextBox
            {
                Location = new Point(350, 20),
                Size = new Size(50, 20),
                Text = DefaultModulo.ToString()
            };
            configPanel.Controls.Add(moduloTextBox);

            // Setup button
            setupButton = new Button
            {
                Text = "Setup Graph",
                Location = new Point(450, 20),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };
            setupButton.Click += new EventHandler(SetupButton_Click);
            configPanel.Controls.Add(setupButton);

            // Initial graph panel
            GroupBox initialGraphGroupBox = new()
            {
                Text = "Initial Graph (Create edges by dragging between nodes)",
                Dock = DockStyle.Top,
                Height = 300,
                Padding = new Padding(10)
            };
            Controls.Add(initialGraphGroupBox);

            initialGraphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            initialGraphPanel.Paint += new PaintEventHandler(InitialGraphPanel_Paint);
            initialGraphGroupBox.Controls.Add(initialGraphPanel);

            // Target graph panel
            GroupBox targetGraphGroupBox = new()
            {
                Text = "Target Graph (Set target values)",
                Dock = DockStyle.Top,
                Height = 300,
                Padding = new Padding(10)
            };
            Controls.Add(targetGraphGroupBox);

            targetGraphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            targetGraphPanel.Paint += new PaintEventHandler(TargetGraphPanel_Paint);
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

            solutionPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            solutionGroupBox.Controls.Add(solutionPanel);

            // Generate solution button
            generateSolutionButton = new Button
            {
                Text = "Generate Solution",
                Location = new Point(20, 20),
                Size = new Size(150, 30),
                UseVisualStyleBackColor = true,
                Enabled = false
            };
            generateSolutionButton.Click += new EventHandler(GenerateSolutionButton_Click);
            solutionPanel.Controls.Add(generateSolutionButton);

            // Status label
            statusLabel = new Label
            {
                Location = new Point(200, 25),
                AutoSize = true,
                Text = "Configure and set up your graph to start."
            };
            solutionPanel.Controls.Add(statusLabel);

            // Solution text box
            solutionTextBox = new RichTextBox
            {
                Location = new Point(20, 60),
                Size = new Size(700, 200),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            solutionPanel.Controls.Add(solutionTextBox);

            // Set initial values
            nodeCount = DefaultNodeCount;
            modulo = DefaultModulo;
        }

        private void SetupButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(nodeCountTextBox.Text, out nodeCount) || nodeCount <= 0)
            {
                MessageBox.Show("Please enter a valid number of nodes.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(moduloTextBox.Text, out modulo) || modulo <= 0)
            {
                MessageBox.Show("Please enter a valid modulo value.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Clear existing nodes and edges
            initialGraphNodes.Clear();
            targetGraphNodes.Clear();
            edges.Clear();
            initialGraphPanel.Controls.Clear();
            targetGraphPanel.Controls.Clear();

            // Calculate layout for nodes
            int columns = (int)Math.Ceiling(Math.Sqrt(nodeCount));
            int xOffset = (initialGraphPanel.Width - (columns * (NodeSize + NodeMargin))) / 2 + NodeSize / 2;
            int yOffset = 50;

            // Create nodes for initial graph
            for (int i = 0; i < nodeCount; i++)
            {
                int row = i / columns;
                int col = i % columns;
                int x = xOffset + col * (NodeSize + NodeMargin);
                int y = yOffset + row * (NodeSize + NodeMargin);

                NodeTextBox nodeTextBox = new(i, new Point(x, y), NodeSize, modulo);
                initialGraphPanel.Controls.Add(nodeTextBox);
                initialGraphNodes.Add(nodeTextBox);

                // Add event handlers for drawing edges
                nodeTextBox.MouseDown += new MouseEventHandler(Node_MouseDown);
                nodeTextBox.MouseUp += new MouseEventHandler(Node_MouseUp);
                nodeTextBox.MouseMove += new MouseEventHandler(Node_MouseMove);
            }

            // Create nodes for target graph (same layout)
            for (int i = 0; i < nodeCount; i++)
            {
                NodeTextBox sourceNode = initialGraphNodes[i];
                NodeTextBox nodeTextBox = new(i, sourceNode.NodeCenter, NodeSize, modulo);
                targetGraphPanel.Controls.Add(nodeTextBox);
                targetGraphNodes.Add(nodeTextBox);

                // Sync values between initial and target graphs
                sourceNode.TextChanged += (s, args) =>
                {
                    if (int.TryParse(sourceNode.Text, out int value) && value >= 0 && value < modulo)
                    {
                        nodeTextBox.Text = sourceNode.Text;
                    }
                };
            }

            statusLabel.Text = "Graph set up. Create edges by dragging between nodes in the Initial Graph.";
            generateSolutionButton.Enabled = true;
            solutionTextBox.Visible = false;

            initialGraphPanel.Invalidate();
            targetGraphPanel.Invalidate();
        }

        private void Node_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                sourceNode = sender as NodeTextBox;
                isDrawingArrow = true;
                currentLineEndPoint = sourceNode;
                initialGraphPanel.Invalidate();
            }
        }

        private void Node_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingArrow && sourceNode != null)
            {
                Point screenPoint = (sender as Control).PointToScreen(e.Location);
                Point formPoint = initialGraphPanel.PointToClient(screenPoint);
                currentLineEndPoint = null;

                initialGraphPanel.Invalidate();
            }
        }

        private void Node_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawingArrow && sourceNode != null)
            {
                NodeTextBox targetNode = sender as NodeTextBox;
                if (targetNode != null && sourceNode != targetNode)
                {
                    // Check if the edge already exists
                    bool edgeExists = edges.Any(edge => edge.From == sourceNode.NodeIndex && edge.To == targetNode.NodeIndex);
                    if (!edgeExists)
                    {
                        edges.Add(new Edge(sourceNode.NodeIndex, targetNode.NodeIndex));
                        statusLabel.Text = $"Added edge from Node {sourceNode.NodeIndex} to Node {targetNode.NodeIndex}";
                    }
                }

                isDrawingArrow = false;
                sourceNode = null;
                currentLineEndPoint = null;
                initialGraphPanel.Invalidate();
                targetGraphPanel.Invalidate();
            }
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            // This method handles any global form painting if needed
        }

        private void InitialGraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw all edges
            foreach (var edge in edges)
            {
                if (edge.From < initialGraphNodes.Count && edge.To < initialGraphNodes.Count)
                {
                    DrawArrow(g, initialGraphNodes[edge.From].NodeCenter, initialGraphNodes[edge.To].NodeCenter, Color.DarkBlue);
                }
            }

            // Draw the edge being created (if any)
            if (isDrawingArrow && sourceNode != null && currentLineEndPoint != null)
            {
                DrawArrow(g, sourceNode.NodeCenter, currentLineEndPoint.NodeCenter, Color.Red);
            }
        }

        private void TargetGraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw all edges (same as initial graph)
            foreach (var edge in edges)
            {
                if (edge.From < targetGraphNodes.Count && edge.To < targetGraphNodes.Count)
                {
                    DrawArrow(g, targetGraphNodes[edge.From].NodeCenter, targetGraphNodes[edge.To].NodeCenter, Color.DarkBlue);
                }
            }
        }

        private void DrawArrow(Graphics g, Point start, Point end, Color color)
        {
            // Calculate offset to make arrow start and end at the node boundaries
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            int nodeRadius = NodeSize / 2;

            // Adjust start and end points to be on the circumference of the nodes
            Point adjustedStart = new(
                start.X + (int)(nodeRadius * Math.Cos(angle)),
                start.Y + (int)(nodeRadius * Math.Sin(angle))
            );

            Point adjustedEnd = new(
                end.X - (int)(nodeRadius * Math.Cos(angle)),
                end.Y - (int)(nodeRadius * Math.Sin(angle))
            );

            // Draw the line
            using (Pen pen = new(color, 2))
            {
                g.DrawLine(pen, adjustedStart, adjustedEnd);
            }

            // Draw arrow head
            DrawArrowHead(g, adjustedStart, adjustedEnd, 15, 8, color);
        }

        private void DrawArrowHead(Graphics g, Point start, Point end, float size, float width, Color color)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            // Normalize the direction vector
            dx /= length;
            dy /= length;

            // Calculate the arrowhead points
            PointF[] arrowHead = new PointF[3];
            arrowHead[0] = end;
            arrowHead[1] = new PointF(
                end.X - size * dx + width * dy,
                end.Y - size * dy - width * dx
            );
            arrowHead[2] = new PointF(
                end.X - size * dx - width * dy,
                end.Y - size * dy + width * dx
            );

            using (SolidBrush brush = new(color))
            {
                g.FillPolygon(brush, arrowHead);
            }
        }

        private void GenerateSolutionButton_Click(object sender, EventArgs e)
        {
            try
            {
                statusLabel.Text = "Generating solution...";
                solutionTextBox.Clear();
                solutionTextBox.Visible = true;

                // Get initial and target values
                int[] initialValues = initialGraphNodes.Select(n => int.Parse(n.Text)).ToArray();
                int[] targetValues = targetGraphNodes.Select(n => int.Parse(n.Text)).ToArray();

                // Add self-loops if they don't exist
                for (int i = 0; i < nodeCount; i++)
                {
                    bool selfLoopExists = edges.Any(edge => edge.From == i && edge.To == i);
                    if (!selfLoopExists)
                    {
                        edges.Add(new Edge(i, i));
                        statusLabel.Text = "Added self-loops (each node increments itself).";
                        initialGraphPanel.Invalidate();
                        targetGraphPanel.Invalidate();
                    }
                }

                // Create graph and solve puzzle
                Graph graph = new(nodeCount, modulo, edges);
                List<int> solution = SolvePuzzle(graph, initialValues, targetValues);

                if (solution != null)
                {
                    solutionTextBox.AppendText($"Solution found! Moves required: {solution.Count}\n\n");
                    solutionTextBox.AppendText("Sequence of nodes to increment:\n");
                    solutionTextBox.AppendText(string.Join(" ", solution) + "\n\n");

                    // Show step-by-step transformation
                    solutionTextBox.AppendText("Step-by-step transformation:\n");
                    int[] currentState = initialValues.ToArray();
                    solutionTextBox.AppendText($"Initial state: {string.Join(" ", currentState)}\n");

                    for (int i = 0; i < solution.Count; i++)
                    {
                        int node = solution[i];
                        graph.Increment(currentState, node);
                        solutionTextBox.AppendText($"After incrementing node {node}: {string.Join(" ", currentState)}\n");
                    }

                    statusLabel.Text = "Solution generated successfully!";
                }
                else
                {
                    solutionTextBox.AppendText("No solution found for this puzzle configuration.");
                    statusLabel.Text = "No solution found.";
                }
            }
            catch (Exception ex)
            {
                solutionTextBox.AppendText($"Error generating solution: {ex.Message}");
                statusLabel.Text = "Error generating solution.";
            }
        }

        #region PuzzleSolver

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

        // Helper method to create a unique string key for a state
        private static string GetStateKey(int[] state)
        {
            return string.Join(",", state);
        }

        #endregion
    }
}
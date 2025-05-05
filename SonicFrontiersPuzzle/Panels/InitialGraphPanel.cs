using SonicFrontiersPuzzle;
using SonicFrontiersPuzzle.Events;
using SonicFrontiersPuzzle.Extensions;
using System.Drawing.Drawing2D;
using static DirectedGraphPuzzleSolver.DirectedGraphPuzzleForm;

namespace SonicFrontiersPuzzle.Panels
{
    public class InitialGraphPanel : Panel
    {
        // Constants for visual layout
        private const int NodeSize = MainForm.NodeSize;
        private const int NodeMargin = MainForm.NodeMargin;

        // Core data structures
        private int nodeCount;
        private int modulo;
        private List<NodeTextBox> nodes = new();
        public List<Edge> Edges { get; private set; } = new();

        // Mode tracking
        private enum EditMode { EditValues, AddEdges }
        private EditMode currentMode = EditMode.EditValues;

        // Edge creation state
        private NodeTextBox? sourceNode = null;

        // UI elements
        private Button editValuesButton;
        private Button addEdgesButton;
        private Button clearEdgesButton;

        // Events
        public event EventHandler EdgesChanged;
        public event EventHandler<NodeValueChangedEventArgs> NodeValuesChanged;

        public InitialGraphPanel()
        {
            InitializeUI();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            Paint += InitialGraphPanel_Paint;

            DebugLogger.Log("InitialGraphPanel initialized");
        }

        private void InitializeUI()
        {
            // Edit Values button
            editValuesButton = new Button
            {
                Text = "Edit Values",
                Location = new Point(10, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true,
                BackColor = SystemColors.Highlight
            };
            editValuesButton.Click += (sender, e) => SetMode(EditMode.EditValues);
            Controls.Add(editValuesButton);

            // Add Edges button
            addEdgesButton = new Button
            {
                Text = "Add Edges",
                Location = new Point(120, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };
            addEdgesButton.Click += (sender, e) => SetMode(EditMode.AddEdges);
            Controls.Add(addEdgesButton);

            // Clear Edges button
            clearEdgesButton = new Button
            {
                Text = "Clear Edges",
                Location = new Point(230, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };
            clearEdgesButton.Click += ClearEdgesButton_Click;
            Controls.Add(clearEdgesButton);

            DebugLogger.Log("InitialGraphPanel UI initialized");
        }

        public void SetupGraph(int nodeCount, int modulo)
        {
            DebugLogger.Log($"Setting up graph with {nodeCount} nodes and modulo {modulo}");
            this.nodeCount = nodeCount;
            this.modulo = modulo;

            // Clear existing nodes and edges
            ClearGraph();

            // Calculate layout for nodes
            int columns = (int)Math.Ceiling(Math.Sqrt(nodeCount));
            int xOffset = (Width - columns * (NodeSize + NodeMargin)) / 2 + NodeSize / 2;
            int yOffset = 50;

            // Create nodes for initial graph
            for (int i = 0; i < nodeCount; i++)
            {
                int row = i / columns;
                int col = i % columns;
                int x = xOffset + col * (NodeSize + NodeMargin);
                int y = yOffset + row * (NodeSize + NodeMargin);

                NodeTextBox nodeTextBox = new(i, new Point(x, y), NodeSize, modulo);
                Controls.Add(nodeTextBox);
                nodeTextBox.AddLabelToPanel(this);
                nodes.Add(nodeTextBox);

                // Forward node value changes to listeners
                nodeTextBox.ValueChanged += Node_ValueChanged;

                // Add event handlers for node selection (adding edges)
                nodeTextBox.Click += Node_Click;

                DebugLogger.Log($"Created node {i} at position ({x}, {y})");
            }

            // Set initial mode
            SetMode(EditMode.EditValues);
        }

        private void Node_ValueChanged(object? sender, NodeValueChangedEventArgs e)
        {
            DebugLogger.Log($"Node {e.NodeIndex} value changed to {e.Value}, forwarding event");
            NodeValuesChanged?.Invoke(this, e);
        }

        private void ClearGraph()
        {
            DebugLogger.Log("Clearing graph");
            foreach (var node in nodes)
            {
                Controls.Remove(node);
            }
            nodes.Clear();
            Edges.Clear();
            sourceNode = null;
            Invalidate();
        }

        public void ResetPanel()
        {
            DebugLogger.Log("Resetting panel");
            // Clear all nodes and edges
            foreach (var node in nodes)
            {
                Controls.Remove(node);
            }
            nodes.Clear();
            Edges.Clear();
            sourceNode = null;
            SetMode(EditMode.EditValues);
            Invalidate();
        }

        private void ClearEdgesButton_Click(object? sender, EventArgs e)
        {
            DebugLogger.Log("Clear edges button clicked");
            Edges.Clear();
            EdgesChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        private void SetMode(EditMode mode)
        {
            DebugLogger.Log($"Setting mode to {mode}");
            currentMode = mode;
            sourceNode = null;

            // Update UI to reflect current mode
            editValuesButton.BackColor = mode == EditMode.EditValues ? SystemColors.Highlight : SystemColors.Control;
            addEdgesButton.BackColor = mode == EditMode.AddEdges ? SystemColors.Highlight : SystemColors.Control;

            // Enable/disable node editing based on mode
            foreach (var node in nodes)
            {
                node.SetEnabled(mode == EditMode.EditValues);
            }

            Invalidate();
        }

        private void Node_Click(object? sender, EventArgs e)
        {
            if (sender is not NodeTextBox clickedNode) return;

            DebugLogger.Log($"Node {clickedNode.NodeIndex} clicked, current mode: {currentMode}");

            if (currentMode == EditMode.AddEdges)
            {
                if (sourceNode == null)
                {
                    // First node selection for edge creation
                    sourceNode = clickedNode;
                    sourceNode.BackColor = Color.Yellow;
                    DebugLogger.Log($"Selected source node {sourceNode.NodeIndex} for edge creation");
                }
                else
                {
                    // Second node selection - create edge
                    if (sourceNode != clickedNode)
                    {
                        // Check if the edge already exists
                        Edge newEdge = new(sourceNode.NodeIndex, clickedNode.NodeIndex);
                        if (!Edges.Any(edge => edge.From == newEdge.From && edge.To == newEdge.To))
                        {
                            Edges.Add(newEdge);
                            DebugLogger.Log($"Added edge from node {sourceNode.NodeIndex} to node {clickedNode.NodeIndex}");
                            EdgesChanged?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            DebugLogger.Log($"Edge from node {sourceNode.NodeIndex} to node {clickedNode.NodeIndex} already exists");
                        }
                    }
                    else
                    {
                        DebugLogger.Log($"Self edge not created through click (source and target are the same node {clickedNode.NodeIndex})");
                    }

                    // Reset source node
                    sourceNode.BackColor = Color.LightCyan;
                    sourceNode = null;
                    Invalidate();
                }
            }
            else
            {
                DebugLogger.Log($"Node clicked in EditValues mode, node should handle its own interaction");
            }
        }

        public void AddSelfLoopsIfNeeded()
        {
            DebugLogger.Log("Adding self-loops if needed");
            bool edgesAdded = false;
            for (int i = 0; i < nodeCount; i++)
            {
                Edge selfLoop = new(i, i);
                if (!Edges.Any(edge => edge.From == i && edge.To == i))
                {
                    Edges.Add(selfLoop);
                    DebugLogger.Log($"Added self-loop for node {i}");
                    edgesAdded = true;
                }
            }

            if (edgesAdded)
            {
                DebugLogger.Log("Self-loops added, firing EdgesChanged event");
                EdgesChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        public int[] GetNodeValues()
        {
            int[] values = nodes.Select(n => n.GetValue()).ToArray();
            DebugLogger.Log($"GetNodeValues returned: [{string.Join(", ", values)}]");
            return values;
        }

        private void InitialGraphPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw all edges
            foreach (var edge in Edges)
            {
                if (edge.From < nodes.Count && edge.To < nodes.Count)
                {
                    DrawArrow(g, nodes[edge.From].NodeCenter, nodes[edge.To].NodeCenter, Color.DarkBlue);
                }
            }

            // Draw the edge being created (if any)
            if (currentMode == EditMode.AddEdges && sourceNode != null)
            {
                // Highlight the source node
                using SolidBrush brush = new(Color.FromArgb(100, Color.Yellow));
                Rectangle rect = new(
                    sourceNode.NodeCenter.X - NodeSize / 2,
                    sourceNode.NodeCenter.Y - NodeSize / 2,
                    NodeSize,
                    NodeSize
                );
                g.FillEllipse(brush, rect);
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

        private static void DrawArrowHead(Graphics g, Point start, Point end, float size, float width, Color color)
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

            using SolidBrush brush = new(color);
            g.FillPolygon(brush, arrowHead);
        }
    }
}
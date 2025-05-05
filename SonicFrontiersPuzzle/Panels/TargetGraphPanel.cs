using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static DirectedGraphPuzzleSolver.DirectedGraphPuzzleForm;

namespace DirectedGraphPuzzleSolver
{
    public class TargetGraphPanel : Panel
    {
        // Constants for visual layout
        private const int NodeSize = MainForm.NodeSize;
        private const int NodeMargin = MainForm.NodeMargin;

        // Core data structures
        private int nodeCount;
        private int modulo;
        private List<NodeTextBox> nodes = new();
        private List<Edge> edges = new();

        // UI elements
        private Button clearValuesButton;

        public TargetGraphPanel()
        {
            InitializeUI();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            Paint += TargetGraphPanel_Paint;
        }

        private void InitializeUI()
        {
            // Clear Values button
            clearValuesButton = new Button
            {
                Text = "Clear Values",
                Location = new Point(10, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };
            clearValuesButton.Click += ClearValuesButton_Click;
            Controls.Add(clearValuesButton);
        }

        public void SetupGraph(int nodeCount, int modulo)
        {
            this.nodeCount = nodeCount;
            this.modulo = modulo;

            // Clear existing nodes
            ClearGraph();

            // Calculate layout for nodes
            int columns = (int)Math.Ceiling(Math.Sqrt(nodeCount));
            int xOffset = (Width - (columns * (NodeSize + NodeMargin))) / 2 + NodeSize / 2;
            int yOffset = 50;

            // Create nodes for target graph
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
            }

            Invalidate();
        }

        private void ClearGraph()
        {
            foreach (var node in nodes)
            {
                Controls.Remove(node);
            }
            nodes.Clear();
            Invalidate();
        }

        private void ClearValuesButton_Click(object sender, EventArgs e)
        {
            foreach (var node in nodes)
            {
                node.SetValue(0);
            }
        }

        public void UpdateEdges(List<Edge> newEdges)
        {
            edges = new List<Edge>(newEdges);
            Invalidate();
        }

        public void UpdateNodeValue(int nodeIndex, int value)
        {
            if (nodeIndex >= 0 && nodeIndex < nodes.Count)
            {
                nodes[nodeIndex].SetValue(value);
            }
        }

        public int[] GetNodeValues()
        {
            return nodes.Select(n => n.GetValue()).ToArray();
        }

        private void TargetGraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw all edges
            foreach (var edge in edges)
            {
                if (edge.From < nodes.Count && edge.To < nodes.Count)
                {
                    DrawArrow(g, nodes[edge.From].NodeCenter, nodes[edge.To].NodeCenter, Color.DarkBlue);
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
    }
}
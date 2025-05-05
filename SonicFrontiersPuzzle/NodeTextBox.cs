using SonicFrontiersPuzzle.Events;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DirectedGraphPuzzleSolver
{
    // Custom TextBox for graph nodes
    public class NodeTextBox : TextBox
    {
        public int NodeIndex { get; private set; }
        public Point NodeCenter { get; private set; }
        private int modulo;
        private Label indexLabel;

        public event EventHandler<NodeValueChangedEventArgs> ValueChanged;

        public NodeTextBox(int index, Point center, int size, int modValue)
        {
            NodeIndex = index;
            NodeCenter = center;
            modulo = modValue;

            Size = new Size(size, size);
            Location = new Point(center.X - size / 2, center.Y - size / 2);
            TextAlign = HorizontalAlignment.Center;
            Font = new Font("Arial", 16, FontStyle.Bold);
            Text = "0";
            MaxLength = 1;
            Multiline = true;
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.LightCyan;

            // Add a label for the node index
            indexLabel = new Label
            {
                Text = index.ToString(),
                AutoSize = true,
                Location = new Point(center.X - 8, center.Y - size / 2 - 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            // Validate input (only allow digits 0 to modulo-1)
            KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) &&
                    (!char.IsDigit(e.KeyChar) || int.Parse(e.KeyChar.ToString()) >= modulo))
                {
                    e.Handled = true;
                }
            };

            // Ensure value is between 0 and modulo-1
            TextChanged += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(Text) && int.TryParse(Text, out int value))
                {
                    if (value >= modulo)
                    {
                        Text = "0";
                        value = 0;
                    }
                    ValueChanged?.Invoke(this, new NodeValueChangedEventArgs(NodeIndex, value));
                }
                else if (string.IsNullOrEmpty(Text))
                {
                    Text = "0";
                    ValueChanged?.Invoke(this, new NodeValueChangedEventArgs(NodeIndex, 0));
                }
            };
        }

        public void AddLabelToPanel(Panel panel)
        {
            panel.Controls.Add(indexLabel);
        }

        public int GetValue()
        {
            if (int.TryParse(Text, out int value))
                return value;
            return 0;
        }

        public void SetValue(int value)
        {
            if (value >= 0 && value < modulo)
            {
                Text = value.ToString();
            }
        }

        public void SetEnabled(bool enabled)
        {
            ReadOnly = !enabled;
            if (enabled)
            {
                BackColor = Color.LightCyan;
            }
            else
            {
                BackColor = SystemColors.Control;
            }
        }
    }
}
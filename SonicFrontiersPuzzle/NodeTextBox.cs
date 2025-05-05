using SonicFrontiersPuzzle.Events;
using SonicFrontiersPuzzle.Extensions;
using System.ComponentModel;

namespace SonicFrontiersPuzzle
{
    // Custom TextBox for graph nodes
    public class NodeTextBox : TextBox
    {
        public int NodeIndex { get; private set; }
        public Point NodeCenter { get; private set; }
        private int modulo;
        private Label indexLabel;
        private bool isEnabled = true;

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
            TabStop = true;  // Ensure the textbox can receive focus

            DebugLogger.Log($"Created NodeTextBox {index} with modulo {modValue}");

            // Add a label for the node index
            indexLabel = new Label
            {
                Text = index.ToString(),
                AutoSize = true,
                Location = new Point(center.X - 8, center.Y - size / 2 - 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            // Validate input (only allow digits 0 to modulo-1)
            KeyPress += NodeTextBox_KeyPress;

            this.Validating += NodeTextBox_Validating;

            // Focus handling for better user interaction
            GotFocus += (s, e) => {
                DebugLogger.Log($"Node {NodeIndex} got focus");
                SelectAll();
            };
        }

        private void NodeTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(Text, out int value) || value < 0 || value >= modulo)
            {
                // invalid or out of bounds
                MessageBox.Show($"Please enter a number between 0 and {modulo - 1}.");
                e.Cancel = true;            // keep focus here
            }
            else
            {
                // only fire once, when losing focus
                ValueChanged?.Invoke(this, new NodeValueChangedEventArgs(NodeIndex, value));
            }
        }

        private void NodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            DebugLogger.Log($"Node {NodeIndex} KeyPress: '{e.KeyChar}' (ASCII: {(int)e.KeyChar})");

            if (!char.IsControl(e.KeyChar) &&
                (!char.IsDigit(e.KeyChar) || char.IsDigit(e.KeyChar) && int.Parse(e.KeyChar.ToString()) >= modulo))
            {
                DebugLogger.Log($"Node {NodeIndex} invalid input rejected: '{e.KeyChar}'");
                e.Handled = true;
            }
        }

        public void AddLabelToPanel(Panel panel)
        {
            panel.Controls.Add(indexLabel);
            DebugLogger.Log($"Added label for Node {NodeIndex} to panel");
        }

        public int GetValue()
        {
            if (int.TryParse(Text, out int value))
                return value;
            return 0;
        }

        public void SetValue(int value)
        {
            DebugLogger.Log($"Setting Node {NodeIndex} value to {value}");
            if (value >= 0 && value < modulo)
            {
                Text = value.ToString();
                DebugLogger.Log($"Node {NodeIndex} value set to {value}");
            }
            else
            {
                DebugLogger.Log($"Attempted to set Node {NodeIndex} to invalid value {value}");
            }
        }

        public void SetEnabled(bool enabled)
        {
            DebugLogger.Log($"Setting Node {NodeIndex} enabled = {enabled}");
            isEnabled = enabled;
            ReadOnly = !enabled;
            if (enabled)
            {
                BackColor = Color.LightCyan;
                Cursor = Cursors.IBeam;  // Set appropriate cursor for editing
            }
            else
            {
                BackColor = SystemColors.Control;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            DebugLogger.Log($"Node {NodeIndex} clicked, isEnabled = {isEnabled}");
            if (isEnabled)
            {
                Focus();  // Ensure the textbox gets focus when clicked
                SelectAll(); // Select all text for easy replacement
                DebugLogger.Log($"Node {NodeIndex} focused and text selected");
            }
        }

        // Override OnKeyDown to log key presses
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            DebugLogger.Log($"Node {NodeIndex} KeyDown: {e.KeyCode}, ReadOnly = {ReadOnly}, isEnabled = {isEnabled}");
        }

        // Override OnTextChanged to provide more debugging
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            DebugLogger.Log($"Node {NodeIndex} OnTextChanged override: Text = '{Text}'");
        }
    }
}
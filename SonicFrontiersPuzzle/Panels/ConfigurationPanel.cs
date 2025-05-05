using System;
using System.Drawing;
using System.Windows.Forms;

namespace DirectedGraphPuzzleSolver
{
    public class ConfigurationPanel : Panel
    {
        private TextBox nodeCountTextBox;
        private TextBox moduloTextBox;
        private Button setupButton;

        public event EventHandler SetupButtonClicked;

        public ConfigurationPanel()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Node count label and textbox
            Label nodeCountLabel = new()
            {
                Text = "Number of Nodes:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(nodeCountLabel);

            nodeCountTextBox = new TextBox
            {
                Location = new Point(150, 20),
                Size = new Size(50, 20),
                Text = MainForm.DefaultNodeCount.ToString()
            };
            Controls.Add(nodeCountTextBox);

            // Modulo label and textbox
            Label moduloLabel = new()
            {
                Text = "Modulo Value:",
                Location = new Point(250, 20),
                AutoSize = true
            };
            Controls.Add(moduloLabel);

            moduloTextBox = new TextBox
            {
                Location = new Point(350, 20),
                Size = new Size(50, 20),
                Text = MainForm.DefaultModulo.ToString()
            };
            Controls.Add(moduloTextBox);

            // Setup button
            setupButton = new Button
            {
                Text = "Setup Graph",
                Location = new Point(450, 20),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };
            setupButton.Click += (sender, e) => SetupButtonClicked?.Invoke(this, EventArgs.Empty);
            Controls.Add(setupButton);
        }

        public bool TryGetConfiguration(out int nodeCount, out int modulo)
        {
            nodeCount = 0;
            modulo = 0;

            if (!int.TryParse(nodeCountTextBox.Text, out nodeCount) || nodeCount <= 0)
            {
                MessageBox.Show("Please enter a valid number of nodes.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(moduloTextBox.Text, out modulo) || modulo <= 0)
            {
                MessageBox.Show("Please enter a valid modulo value.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public void ResetPanel()
        {
            nodeCountTextBox.Text = MainForm.DefaultNodeCount.ToString();
            moduloTextBox.Text = MainForm.DefaultModulo.ToString();
        }
    }
}
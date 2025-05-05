using static DirectedGraphPuzzleSolver.DirectedGraphPuzzleForm;

namespace SonicFrontiersPuzzle.Panels
{
    public class SolutionPanel : Panel
    {
        private Button generateSolutionButton;
        private Label statusLabel;
        private RichTextBox solutionTextBox;

        public event EventHandler GenerateSolutionClicked;

        public SolutionPanel()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Generate solution button
            generateSolutionButton = new Button
            {
                Text = "Generate Solution",
                Location = new Point(20, 20),
                Size = new Size(150, 30),
                UseVisualStyleBackColor = true,
                Enabled = false
            };
            generateSolutionButton.Click += (sender, e) => GenerateSolutionClicked?.Invoke(this, EventArgs.Empty);
            Controls.Add(generateSolutionButton);

            // Status label
            statusLabel = new Label
            {
                Location = new Point(200, 25),
                AutoSize = true,
                Text = "Configure and set up your graph to start."
            };
            Controls.Add(statusLabel);

            // Solution text box
            solutionTextBox = new RichTextBox
            {
                Location = new Point(20, 60),
                Size = new Size(700, 200),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            Controls.Add(solutionTextBox);
        }

        public void EnableSolutionGeneration(bool enable)
        {
            generateSolutionButton.Enabled = enable;
            if (enable)
            {
                statusLabel.Text = "Ready to generate solution. Make sure initial and target values are set.";
            }
            else
            {
                statusLabel.Text = "Configure and set up your graph to start.";
                solutionTextBox.Visible = false;
            }
        }

        public void SetStatus(string status)
        {
            statusLabel.Text = status;
            solutionTextBox.Clear();
            solutionTextBox.Visible = true;
        }

        public void SetNoSolutionFound()
        {
            solutionTextBox.Clear();
            solutionTextBox.AppendText("No solution found for this puzzle configuration.");
            statusLabel.Text = "No solution found.";
        }

        public void SetError(string errorMessage)
        {
            solutionTextBox.Clear();
            solutionTextBox.AppendText(errorMessage);
            statusLabel.Text = "Error generating solution.";
        }

        public void DisplaySolution(List<int> solution, Graph graph, int[] initialValues)
        {
            solutionTextBox.Clear();
            solutionTextBox.Visible = true;

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

        public void ResetPanel()
        {
            solutionTextBox.Clear();
            solutionTextBox.Visible = false;
            statusLabel.Text = "Configure and set up your graph to start.";
            EnableSolutionGeneration(false);
        }
    }
}
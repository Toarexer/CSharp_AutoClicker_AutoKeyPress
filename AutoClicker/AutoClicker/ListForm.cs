using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AutoClicker
{
    class ListForm : Form
    {
        ListBox processListBox;
        Process[] processes;

        public ListForm()
        {
            Size = new Size(400, 528);  // +15 Width because of the ScrollBar, +1 because of the ProcessListBox Location
            Text = "Choose a process";
            Icon = SystemIcons.Application;
            Resize += OnResized;

            // Initialize Components

            Font font = new Font("Consolas", 8f);
            Font fontb = new Font(font, FontStyle.Bold);

            Label processListLabel = new Label();
            processListLabel.Size = new Size(128, 20);
            processListLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            processListLabel.Text = "PID     |    Name";
            processListLabel.TextAlign = ContentAlignment.MiddleLeft;
            processListLabel.Font = fontb;

            TextBox processSearchTextBox = new TextBox();
            processSearchTextBox.Size = new Size(192, 16);
            processSearchTextBox.Dock = DockStyle.Right;
            processSearchTextBox.TabIndex = 2;
            processSearchTextBox.Text = "Search";
            processSearchTextBox.ForeColor = Color.Gray;
            processSearchTextBox.KeyDown += ListMatchingElements;
            processSearchTextBox.KeyPress += (s, e) => FormatProcessSearchTextBox(s, true);
            processSearchTextBox.LostFocus += (s, e) => FormatProcessSearchTextBox(s, false);

            processListBox = new ListBox();
            processListBox.IntegralHeight = true;
            processListBox.Size = new Size(Width - 16, Height - 56);
            processListBox.Location = new Point(1, 20);
            processListBox.TabIndex = 1;
            processListBox.ScrollAlwaysVisible = true;
            processListBox.SelectionMode = SelectionMode.One;
            processListBox.MouseDoubleClick += OnSelected;
            processListBox.KeyDown += OnSelectedByKey;
            processListBox.Font = font;

            Controls.Add(processListLabel);
            Controls.Add(processSearchTextBox);
            Controls.Add(processListBox);

            // Fill up the ListBox

            processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
                processListBox.Items.Add($"{processes[i].Id}\t|    {processes[i].ProcessName}");
        }

        void OnResized(object sender, EventArgs e)
        {
            processListBox.Width = Width - 16;
            processListBox.Height = Height - 56;
        }

        void OnSelected(object sender, EventArgs e)
        {
            if (processListBox.SelectedIndex > 0)
            {
                Program.targetProcess = processListBox.SelectedIndex < processes.Length
                    ? processes[processListBox.SelectedIndex]
                    : processes[processes.Length];
                Console.WriteLine("PID: {0}\tName: {1}", Program.targetProcess.Id, Program.targetProcess.ProcessName);
                Close();
            }
        }

        void OnSelectedByKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OnSelected(sender, e);
        }

        void ListMatchingElements(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || ((TextBox)sender).ForeColor == Color.Gray)
                return;

            processListBox.Items.Clear();
            processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
                if (processes[i].ProcessName.ToLower().Contains(((TextBox)sender).Text.ToLower()))
                    processListBox.Items.Add($"{processes[i].Id}\t|    {processes[i].ProcessName}");
        }

        void FormatProcessSearchTextBox(object sender, bool focused)
        {
            if (focused)
            {
                if (((TextBox)sender).Text == "Search")
                {
                    ((TextBox)sender).ForeColor = Color.Black;
                    ((TextBox)sender).Text = "";
                }
            }
            else if (((TextBox)sender).Text.Length == 0)
            {
                ((TextBox)sender).ForeColor = Color.Gray;
                ((TextBox)sender).Text = "Search";
            }
        }
    }
}

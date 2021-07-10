using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AutoClicker
{
    class ProcessListBox : ListBox
    {
        // hidden scrollbar exaple form https://stackoverflow.com/questions/13169900/hide-vertical-scroll-bar-in-listbox-control
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                //createParams.Style &= ~0x200000;
                return createParams;
            }
        }
    }

    class ListForm : Form
    {
        ProcessListBox processListBox;
        Process[] processes;

        public ListForm()
        {
            Size = new Size(400, 528);  // +15 Width because of the ScrollBar, +1 because of the ProcessListBox Location
            Text = "Choose a process";
            Icon = SystemIcons.Application;
            Resize += OnResized;

            // Initialize Components

            //Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Bold);
            Font font = new Font("Consolas", 8f);
            Font fontb = new Font("Consolas", 8f, FontStyle.Bold);

            Label processListLabel = new Label();
            processListLabel.Size = new Size(Width - 34, 16);
            processListLabel.Location = new Point(0, 0);
            processListLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            processListLabel.Text = "PID     |    Name";
            processListLabel.TextAlign = ContentAlignment.MiddleLeft;
            processListLabel.Font = fontb;

            processListBox = new ProcessListBox();
            processListBox.IntegralHeight = true;
            processListBox.Size = new Size(Width - 16, Height - 48);
            processListBox.Location = new Point(1, 16);
            processListBox.ScrollAlwaysVisible = true;
            processListBox.SelectionMode = SelectionMode.One;
            processListBox.MouseDoubleClick += OnSelected;
            processListBox.KeyDown += OnSelectedByKey;
            processListBox.Font = font;

            Controls.Add(processListLabel);
            Controls.Add(processListBox);
            
            // Fill up the ListBox

            processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
                processListBox.Items.Add($"{processes[i].Id}\t|    {processes[i].ProcessName}");
        }

        void OnResized(object sender, EventArgs e)
        {   
            processListBox.Width = Width - 16;
            processListBox.Height = Height - 48;
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
    }
}

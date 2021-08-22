using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AutoClicker
{
    class AppForm : Form
    {
        TextBox processTextBox;
        Label processLabel;
        public NumericUpDown intervalNumericUpDown;

        public AppForm()
        {
            Size = new Size(384, 264);
            Text = "C# Auto Clicker";
            Icon = SystemIcons.Application;

            // Initialize Components

            Button chooseProcessButton = new Button();
            chooseProcessButton.Size = new Size(144, 24);   // (75, 23)
            chooseProcessButton.Location = new Point(16, 16);
            chooseProcessButton.Text = "Choose Process";
            chooseProcessButton.Click += ChooseProcess;

            processTextBox = new TextBox();
            processTextBox.Width = 64;
            processTextBox.Location = new Point(168, 17);
            processTextBox.TabStop = false;
            processTextBox.ForeColor = Color.Gray;
            processTextBox.Font = new Font(processTextBox.Font.FontFamily, 10f);
            processTextBox.Text = "PID";
            processTextBox.TextAlign = HorizontalAlignment.Center;
            processTextBox.KeyDown -= default;
            processTextBox.TextChanged += OnPTBTextChanged;
            processTextBox.KeyPress += OnPTBKeyPressed;
            processTextBox.LostFocus += (s, e) => FormatProcessTextBox(false);

            processLabel = new Label();
            processLabel.Size = new Size(256, 16);
            processLabel.Location = new Point(240, 21);

            Button startButton = new Button();
            startButton.Size = new Size(64, 24);
            startButton.Location = new Point(16, 48);
            startButton.Text = "Start";
            startButton.Click += (s, e) => Program.suspendClickingThread = false;

            Button stopButton = new Button();
            stopButton.Size = new Size(64, 24);
            stopButton.Location = new Point(96, 48);
            stopButton.Text = "Stop";
            stopButton.Click += (s, e) => Program.suspendClickingThread = true;

            Label settingsLabel = new Label();
            settingsLabel.Size = new Size(256, 16);
            settingsLabel.Location = new Point(14, 96);
            settingsLabel.Text = "Settings";

            Label intervalLabel = new Label();
            intervalLabel.Size = new Size(48, 16);
            intervalLabel.Location = new Point(14, 128);
            intervalLabel.Text = "Interval:";

            intervalNumericUpDown = new NumericUpDown();
            intervalNumericUpDown.Size = new Size(128, 24);
            intervalNumericUpDown.Location = new Point(65, 126);
            intervalNumericUpDown.Value = 100m;
            intervalNumericUpDown.Maximum = 10000m;
            intervalNumericUpDown.Minimum = 1m;
            intervalNumericUpDown.DecimalPlaces = 0;

            Label millisecondsLabel = new Label();
            millisecondsLabel.Size = new Size(64, 16);
            millisecondsLabel.Location = new Point(192, 128);
            millisecondsLabel.Text = "milliseconds";

            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();

            Label keyToSendLabel = new Label();
            keyToSendLabel.Size = new Size(48, 16);
            keyToSendLabel.Location = new Point(14, 160);
            keyToSendLabel.Text = "Key:";

            ComboBox keyToSendComboBox = new ComboBox();
            keyToSendComboBox.Size = new Size(128, 20);
            keyToSendComboBox.Location = new Point(65, 157);
            keyToSendComboBox.DropDownWidth = 240;
            keyToSendComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            keyToSendComboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            keyToSendComboBox.AutoCompleteCustomSource = acsc;
            keyToSendComboBox.TextChanged += ChooseVirtualKey;
            keyToSendComboBox.SelectedIndexChanged += (s, e) => Program.KeyToSend = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), ((ComboBox)s).Text.Split(' ')[1]);
            keyToSendComboBox.Name = "ktsend";

            Label keyToScanLabel = new Label();
            keyToScanLabel.Size = new Size(48, 16);
            keyToScanLabel.Location = new Point(14, 192);
            keyToScanLabel.Text = "Toggle:";

            ComboBox keyToScanComboBox = new ComboBox();
            keyToScanComboBox.Size = new Size(128, 20);
            keyToScanComboBox.Location = new Point(65, 189);
            keyToScanComboBox.DropDownWidth = 240;
            keyToScanComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            keyToScanComboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            keyToScanComboBox.AutoCompleteCustomSource = acsc;
            keyToScanComboBox.TextChanged += ChooseVirtualKey;
            keyToScanComboBox.SelectedIndexChanged += (s, e) => Program.KeyToScan = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), ((ComboBox)s).Text.Split(' ')[1]);
            keyToScanComboBox.Name = "ktscan";

            foreach (byte value in Enum.GetValues(typeof(VirtualKeys)))
            {
                string name = Enum.GetName(typeof(VirtualKeys), value);
                acsc.Add(name);
                keyToSendComboBox.Items.Add(string.Format("0x{0:X2}: {1}", value, name));
                keyToScanComboBox.Items.Add(string.Format("0x{0:X2}: {1}", value, name));
            }
            if (keyToSendComboBox.Items.Count > 0) keyToSendComboBox.SelectedItem = "0x01: VK_LBUTTON";
            if (keyToScanComboBox.Items.Count > 0) keyToScanComboBox.SelectedItem = "0x23: VK_END";

            // Add Controls

            Controls.Add(chooseProcessButton);
            Controls.Add(processTextBox);
            Controls.Add(processLabel);
            Controls.Add(startButton);
            Controls.Add(stopButton);
            Controls.Add(settingsLabel);
            Controls.Add(intervalLabel);
            Controls.Add(intervalNumericUpDown);
            Controls.Add(millisecondsLabel);
            Controls.Add(keyToSendLabel);
            Controls.Add(keyToSendComboBox);
            Controls.Add(keyToScanLabel);
            Controls.Add(keyToScanComboBox);
        }

        void ChooseProcess(object sender, EventArgs e)
        {
            new ListForm().ShowDialog();
            if (Program.targetProcess != null)
            {
                processTextBox.ForeColor = Color.Black;
                processTextBox.Text = Program.targetProcess.Id.ToString();
                processLabel.Text = Program.targetProcess.ProcessName;
            }
        }

        void OnPTBTextChanged(object sender, EventArgs e)
        {
            int pid;
            Process proc = null;
            if (int.TryParse(processTextBox.Text, out pid))
                try { proc = Process.GetProcessById(pid); }
                catch { }
            if (proc != null)
                processLabel.Text = proc.ProcessName;
            else
                processLabel.Text = "";
        }

        void FormatProcessTextBox(bool focused)
        {
            if (focused)
            {
                if (processTextBox.Text == "PID")
                {
                    processTextBox.ForeColor = Color.Black;
                    processTextBox.Text = "";
                }
            }
            else if (processTextBox.Text.Length == 0)
            {
                processTextBox.ForeColor = Color.Gray;
                processTextBox.Text = "PID";
            }
        }

        void OnPTBKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyPress(e);
            FormatProcessTextBox(true);
        }

        void ChooseVirtualKey(object sender, EventArgs e)
        {
            string text = ((ComboBox)sender).Text;
            if (!Regex.IsMatch(text, @"^0x\d{2}: .*"))
                if (Enum.TryParse(text, out VirtualKeys value))
                {
                    ((ComboBox)sender).SelectedItem = $"0x{(int)value:X2}: {Enum.GetName(typeof(VirtualKeys), value)}";
                    switch (((ComboBox)sender).Name)
                    {
                        case "ktsend":
                            Program.KeyToSend = value;
                            break;
                        case "ktscan":
                            Program.KeyToScan = value;
                            break;
                        default:
                            throw new Exception($"void ChooseVirtualKey(object sender, EventArgs e): Invalid sender ({nameof(sender)})");
                    }
                }
        }
    }
}

using GHelper.Mode;
using GHelper.UI;
using System.Diagnostics;

namespace GHelper
{
    public partial class Routines : RForm
    {
        List<Routine> routines = RoutineControl.GetRoutines();
        List<int> modes = Modes.GetList();

        // Apps that run many different programs, so window title is needed to tell them apart
        static readonly string[] launchers = { "javaw", "java", "python", "pythonw", "node", "electron" };

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 2000 };

        public Routines()
        {
            InitializeComponent();
            InitTheme(true);

            Text = labelLockTitle.Text = Properties.Strings.Routines;
            labelHint.Text = Properties.Strings.RoutineHint;
            labelLock.Text = Properties.Strings.RoutineLock;
            labelApps.Text = Properties.Strings.RoutineApps;
            labelMode.Text = Properties.Strings.PerformanceMode;
            labelTitle.Text = Properties.Strings.RoutineTitle;

            comboLock.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLock.Items.Add(Properties.Strings.Off);
            foreach (int mode in modes) comboLock.Items.Add(Modes.GetName(mode));
            comboLock.SelectedIndex = modes.IndexOf(AppConfig.Get("routine_lock")) + 1;
            comboLock.SelectionChangeCommitted += ComboLock_SelectionChangeCommitted;

            comboMode.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (int mode in modes) comboMode.Items.Add(Modes.GetName(mode));
            comboMode.SelectionChangeCommitted += ComboMode_SelectionChangeCommitted;

            comboApps.DropDownStyle = ComboBoxStyle.DropDownList;
            comboApps.SelectedIndexChanged += ComboApps_SelectedIndexChanged;

            textTitle.Leave += TextTitle_Changed;
            textTitle.KeyDown += TextTitle_KeyDown;

            buttonAdd.Click += ButtonAdd_Click;
            buttonRemove.Click += ButtonRemove_Click;

            timer.Tick += (sender, e) => VisualiseStatus();
            Shown += Routines_Shown;
            FormClosed += (sender, e) => timer.Dispose();

            FillApps();
        }

        private void Routines_Shown(object? sender, EventArgs e)
        {
            VisualiseStatus();
            timer.Start();
        }

        public void FormPosition()
        {
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }

        private void FillApps(int index = 0)
        {
            comboApps.Items.Clear();
            foreach (var routine in routines) comboApps.Items.Add(routine.Name);

            bool any = routines.Count > 0;
            comboApps.Enabled = comboMode.Enabled = textTitle.Enabled = buttonRemove.Enabled = any;

            if (any)
            {
                comboApps.SelectedIndex = Math.Min(index, routines.Count - 1);
            }
            else
            {
                comboMode.SelectedIndex = -1;
                textTitle.Text = "";
            }
        }

        private void ComboApps_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (comboApps.SelectedIndex < 0) return;
            var routine = routines[comboApps.SelectedIndex];
            comboMode.SelectedIndex = modes.IndexOf(routine.Mode);
            textTitle.Text = routine.Title;
        }

        private void ComboMode_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            if (comboApps.SelectedIndex < 0 || comboMode.SelectedIndex < 0) return;
            routines[comboApps.SelectedIndex].Mode = modes[comboMode.SelectedIndex];
            Save();
        }

        private void ComboLock_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            AppConfig.Set("routine_lock", comboLock.SelectedIndex > 0 ? modes[comboLock.SelectedIndex - 1] : -1);
            Program.routineControl.Init();
        }

        private void TextTitle_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) TextTitle_Changed(sender, e);
        }

        private void TextTitle_Changed(object? sender, EventArgs e)
        {
            int index = comboApps.SelectedIndex;
            if (index < 0 || routines[index].Title == textTitle.Text.Trim()) return;

            routines[index].Title = textTitle.Text.Trim();
            Save();
            FillApps(index);
        }

        private void ButtonRemove_Click(object? sender, EventArgs e)
        {
            int index = comboApps.SelectedIndex;
            if (index < 0) return;

            routines.RemoveAt(index);
            Save();
            FillApps(index);
        }

        private void ButtonAdd_Click(object? sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();

            var processes = Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle.Length > 0 && p.Id != Environment.ProcessId)
                .OrderBy(p => p.MainWindowTitle);

            foreach (var process in processes)
            {
                string name = process.ProcessName;
                string title = process.MainWindowTitle;

                var item = new ToolStripMenuItem((title.Length > 40 ? title.Substring(0, 40) + "…" : title) + "  (" + name + ")");

                try
                {
                    item.Image = Icon.ExtractAssociatedIcon(process.MainModule!.FileName)?.ToBitmap();
                }
                catch
                {
                }

                // Keep first word of the title, "Minecraft* 1.21" -> "Minecraft"
                if (launchers.Contains(name.ToLower()))
                    title = new string(title.TakeWhile(c => char.IsLetter(c) || c == ' ').ToArray()).Trim();
                else
                    title = "";

                item.Click += (s, a) => AddApp(name, title);
                menu.Items.Add(item);
            }

            if (menu.Items.Count == 0) menu.Items.Add(new ToolStripMenuItem(Properties.Strings.RoutineNoApps) { Enabled = false });

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(Properties.Strings.Browse, null, (s, a) => BrowseApp());

            menu.Show(buttonAdd, new Point(0, buttonAdd.Height));
        }

        private void BrowseApp()
        {
            using var dialog = new OpenFileDialog { Filter = "Apps (*.exe)|*.exe" };
            if (dialog.ShowDialog(this) == DialogResult.OK) AddApp(Path.GetFileNameWithoutExtension(dialog.FileName), "");
        }

        private void AddApp(string name, string title)
        {
            int index = routines.FindIndex(r => r.Process.Equals(name, StringComparison.OrdinalIgnoreCase) && r.Title == title);

            if (index < 0)
            {
                // Default to custom mode based on Turbo (like "Full Speed") if there is one
                int mode = modes.FirstOrDefault(m => m > 2 && Modes.GetBase(m) == AsusACPI.PerformanceTurbo, AsusACPI.PerformanceTurbo);
                routines.Add(new Routine { Process = name, Title = title, Mode = mode });
                index = routines.Count - 1;
                Save();
            }

            FillApps(index);
        }

        private void Save()
        {
            RoutineControl.SetRoutines(routines);
            Program.routineControl.Init();
        }

        private void VisualiseStatus()
        {
            string status = Program.routineControl.Status;
            labelStatus.Text = status.Length > 0 ? status + " → " + Modes.GetCurrentName() : "";
        }
    }
}

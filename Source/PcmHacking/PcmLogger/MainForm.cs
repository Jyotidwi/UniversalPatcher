﻿//#define Vpw4x

using PcmLibraryWindowsForms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PcmHacking
{
    public partial class MainForm : MainFormBase
    {
        private bool saving;
        private object loggingLock = new object();
        private bool logStopRequested;
        private TaskScheduler uiThreadScheduler;
        private uint osid;

        private const string appName = "PCM Logger";
        private const string defaultFileName = "New Profile";
        private string fileName = defaultFileName;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #region MainFormBase override methods

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="message"></param>
        public override void AddUserMessage(string message)
        {
        }

        /// <summary>
        /// Add a message to the debug pane of the main window.
        /// </summary>
        public override void AddDebugMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("hh:mm:ss:fff");

            Task foreground = Task.Factory.StartNew(
                delegate ()
                {
                    try
                    {
                        if (enableDebugCheckbox.Checked)
                            this.debugLog.AppendText("[" + timestamp + "]  " + message + Environment.NewLine);
                    }
                    catch (ObjectDisposedException)
                    {
                        // This will happen if the window is closing. Just ignore it.
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                uiThreadScheduler);
        }

        public override void ResetLogs()
        {
            this.debugLog.Clear();
        }

        public override string GetAppNameAndVersion()
        {
            return "PCM Logger";
        }

        protected override void DisableUserInput()
        {
            this.EnableProfileButtons(false);
            this.profileList.Enabled = false;
            this.parameterGrid.Enabled = false;
            this.parameterSearch.Enabled = false;
            this.selectButton.Enabled = false;
            this.startStopSaving.Enabled = false;
        }

        protected override void EnableInterfaceSelection()
        {
            this.selectButton.Enabled = true;
        }

        protected override void EnableUserInput()
        {
            this.EnableProfileButtons(true);
            this.profileList.Enabled = true;
            this.parameterGrid.Enabled = true;
            this.parameterSearch.Enabled = true;
            this.selectButton.Enabled = true;
            this.startStopSaving.Enabled = true;
            this.startStopSaving.Focus();
        }

        protected override void NoDeviceSelected()
        {
            this.selectButton.Enabled = true;
            this.deviceDescription.Text = "No device selected";

            // This or ValidDeviceSelected will be called after the form loads.
            // Either way, we should let users manipulate profiles.
            this.EnableProfileButtons(true);
        }

        protected override void SetSelectedDeviceText(string message)
        {
            this.deviceDescription.Text = message;
        }

        /// <summary>
        /// This is invoked from within the call to base.ResetDevice().
        /// </summary>
        protected override void ValidDeviceSelectedAsync(string deviceName)
        {
            this.AddDebugMessage("ValidDeviceSelectedAsync started.");
            Response<uint> response = this.Vehicle.QueryOperatingSystemId(new CancellationToken());
            if (response.Status != ResponseStatus.Success)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.deviceDescription.Text = deviceName + " is unable to connect to the PCM";
                });

                return;
            }

            // This must be assigned prior to calling FillParameterGrid(), 
            // otherwise the RAM parameters will not appear in the grid.
            this.osid = response.Value;
            
            this.Invoke((MethodInvoker)delegate ()
            {
                this.deviceDescription.Text = deviceName + " " + osid.ToString();
                this.startStopSaving.Enabled = true;
                this.parameterGrid.Enabled = true;
                this.EnableProfileButtons(true);
                this.FillParameterGrid();
            });

            string lastProfile = Properties.Settings.Default.LastProfile;
            if (!string.IsNullOrEmpty(lastProfile) && File.Exists(lastProfile))
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.OpenProfile(lastProfile);
                });
            }

            // Start pulling data from the PCM
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoggingThread), null);

            this.AddDebugMessage("ValidDeviceSelectedAsync ended.");
        }

        #endregion

        #region Open / Close

        /// <summary>
        /// Open the most-recently-used device, if possible.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Order matters - the scheduler must be set before adding messages.
            this.uiThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.AddDebugMessage("MainForm_Load started.");
                        
            string logDirectory = Properties.Settings.Default.LogDirectory;
            if (string.IsNullOrWhiteSpace(logDirectory))
            {
                logDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Properties.Settings.Default.LogDirectory = logDirectory;
                Properties.Settings.Default.Save();
            }

            // This just saves the trouble of having to keep a const string in 
            // sync with whatever window text is entered in the designer view.
            this.Text = appName;

            this.EnableProfileButtons(false);

            this.LoadProfileHistory();

            enablePassiveModeCheckBox.Checked = DeviceConfiguration.Settings.EnablePassiveMode;

            ThreadPool.QueueUserWorkItem(BackgroundInitialization);

            this.logFilePath.Text = logDirectory;

            this.AddDebugMessage("MainForm_Load ended.");
        }
        
        private void BackgroundInitialization(object unused)
        {
            try
            {
                this.AddDebugMessage("Device reset started.");

                // This will cause the ValidDeviceSelectedAsync callback to be invoked.
                this.ResetDevice();

                this.AddDebugMessage("Device reset completed.");
            }
            catch (Exception exception)
            {
                // Don't try to log messages during shutdown, that doesn't end well
                // because the window handle is no longer valid.
                //
                // There is still a race condition around using logStopRequested for
                // this, but the only deterministic solution involves cross-thread 
                // access to the Form object, which isn't allowed.
                if (!this.logStopRequested) 
                {
                    this.Invoke(
                        (MethodInvoker)
                        delegate ()
                        {
                            if (!this.logStopRequested)
                            {
                                this.AddDebugMessage("BackgroundInitialization: " + exception.ToString());
                            }
                        });
                }
            }
        }

        private void EnableProfileButtons(bool enable)
        {
            this.newButton.Enabled = enable;
            this.openButton.Enabled = enable;
            this.saveButton.Enabled = enable;
            this.saveAsButton.Enabled = enable;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.currentProfileIsDirty)
            {
                if (this.SaveIfNecessary() == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            this.logStopRequested = true;

            this.SaveProfileHistory();

            // It turns out that WaitAll is not supported on an STA thread.
            // WaitHandle.WaitAll(new WaitHandle[] { loggerThreadEnded, writerThreadEnded });
            loggerThreadEnded.WaitOne(1000);
            writerThreadEnded.WaitOne(1000);
        }

        #endregion

        #region Button clicks

        /// <summary>
        /// Select which interface device to use. This opens the Device-Picker dialog box.
        /// </summary>
        protected void selectButton_Click(object sender, EventArgs e)
        {
            base.HandleSelectButtonClick();
        }

        /// <summary>
        /// Choose which directory to create log files in.
        /// </summary>
        private void setDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Properties.Settings.Default.LogDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LogDirectory = dialog.SelectedPath;
                Properties.Settings.Default.Save();
                this.logFilePath.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Open a File Explorer window in the log directory.
        /// </summary>
        private void openDirectory_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.LogDirectory);
        }

        /// <summary>
        /// Start or stop logging.
        /// </summary>
        private void startStopSaving_Click(object sender, EventArgs e)
        {
            if (saving)
            {
                this.saving = false;
                this.startStopSaving.Text = "Start &Recording";
                this.loggerProgress.MarqueeAnimationSpeed = 0;
                this.loggerProgress.Visible = false;
                this.logState = LogState.StopSaving;
            }
            else
            {
                this.saving = true;
                this.startStopSaving.Text = "Stop &Recording";
                this.loggerProgress.MarqueeAnimationSpeed = 100;
                this.loggerProgress.Visible = true;
                this.logState = LogState.StartSaving;
            }
        }

        #endregion


        private void enablePassiveModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeviceConfiguration.Settings.EnablePassiveMode = enablePassiveModeCheckBox.Checked;
            DeviceConfiguration.Settings.Save();
            this.ResetDevice();
        }
    }
}

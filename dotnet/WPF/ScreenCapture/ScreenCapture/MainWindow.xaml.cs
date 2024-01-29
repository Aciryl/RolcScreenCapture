//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using CaptureSampleCore;
using Composition.WindowsRuntimeHelpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Windows.Foundation.Metadata;
using Windows.Graphics.Capture;
using Windows.UI.Composition;

namespace WPFCaptureSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr hwnd;
        private Compositor compositor;
        private CompositionTarget target;
        private ContainerVisual root;

        private BasicSampleApplication sample;
        private ObservableCollection<ComboBoxItem> processes;
        private ObservableCollection<MonitorInfo> monitors;

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            // Force graphicscapture.dll to load.
            var picker = new GraphicsCapturePicker();
#endif
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var interopWindow = new WindowInteropHelper(this);
            hwnd = interopWindow.Handle;

            var presentationSource = PresentationSource.FromVisual(this);
            double dpiX = 1.0;
            double dpiY = 1.0;
            if (presentationSource != null)
            {
                dpiX = presentationSource.CompositionTarget.TransformToDevice.M11;
                dpiY = presentationSource.CompositionTarget.TransformToDevice.M22;
            }
            var controlsWidth = (float)(ControlsGrid.ActualWidth * dpiX);

            InitComposition(controlsWidth);
            InitWindowList();
            //InitMonitorList();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < BasicSampleApplication.ScreenCount; ++i)
                StopCapture(i);
            WindowComboBox1.SelectedIndex = -1;
            WindowComboBox2.SelectedIndex = -1;
            WindowComboBox3.SelectedIndex = -1;
            //MonitorComboBox.SelectedIndex = -1;
        }

        private void PickerButton1_Click(object sender, RoutedEventArgs e)
            => PickerButton_Click(sender, e, 0);

        private void PickerButton2_Click(object sender, RoutedEventArgs e)
            => PickerButton_Click(sender, e, 1);

        private void PickerButton3_Click(object sender, RoutedEventArgs e)
            => PickerButton_Click(sender, e, 2);

        private async void PickerButton_Click(object sender, RoutedEventArgs e, int i)
        {
            StopCapture(i);

            if (i == 0)
                WindowComboBox1.SelectedIndex = -1;
            else if (i == 1)
                WindowComboBox2.SelectedIndex = -1;
            else if (i == 2)
                WindowComboBox3.SelectedIndex = -1;

            //MonitorComboBox.SelectedIndex = -1;
            await StartPickerCaptureAsync(i);
        }
        /*
        private void PrimaryMonitorButton_Click(object sender, RoutedEventArgs e)
        {
            StopCapture();
            WindowComboBox.SelectedIndex = -1;
            MonitorComboBox.SelectedIndex = -1;
            StartPrimaryMonitorCapture();
        }*/

        private void WindowComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => WindowComboBox_SelectionChanged(sender, e, 0);

        private void WindowComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => WindowComboBox_SelectionChanged(sender, e, 1);

        private void WindowComboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => WindowComboBox_SelectionChanged(sender, e, 2);

        private void WindowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e, int i)
        {
            var comboBox = (ComboBox)sender;
            var selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            var process = selectedItem.Process;

            if (process != null)
            {
                StopCapture(i);
                //MonitorComboBox.SelectedIndex = -1;
                var hwnd = process.MainWindowHandle;
                try
                {
                    StartHwndCapture(hwnd, i);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Hwnd 0x{hwnd.ToInt32():X8} is not valid for capture!");
                    processes.Remove(selectedItem);
                    comboBox.SelectedIndex = -1;
                }
            }
        }
        /*
        private void MonitorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var monitor = (MonitorInfo)comboBox.SelectedItem;

            if (monitor != null)
            {
                StopCapture();
                WindowComboBox.SelectedIndex = -1;
                var hmon = monitor.Hmon;
                try
                {
                    StartHmonCapture(hmon);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Hmon 0x{hmon.ToInt32():X8} is not valid for capture!");
                    monitors.Remove(monitor);
                    comboBox.SelectedIndex = -1;
                }
            }
        }*/

        private void InitComposition(float controlsWidth)
        {
            // Create the compositor.
            compositor = new Compositor();

            // Create a target for the window.
            target = compositor.CreateDesktopWindowTarget(hwnd, true);

            // Attach the root visual.
            root = compositor.CreateContainerVisual();
            root.RelativeSizeAdjustment = Vector2.One;
            root.Size = new Vector2(-controlsWidth, 0);
            root.Offset = new Vector3(controlsWidth, 0, 0);
            target.Root = root;
            

            // Setup the rest of the sample application.
            sample = new BasicSampleApplication(compositor);
            root.Children.InsertAtTop(sample.Visual);
        }

        public class ComboBoxItem
        {
            public Process Process { get; set; }
            public string Title { get; set; }

            public ComboBoxItem(Process p, string title)
            {
                Process = p;
                Title = title;
            }
        }

        private void InitWindowList()
        {
            if (ApiInformation.IsApiContractPresent(typeof(Windows.Foundation.UniversalApiContract).FullName, 8))
            {
                var processesWithWindows = from p in Process.GetProcesses()
                                           where !string.IsNullOrWhiteSpace(p.MainWindowTitle) && WindowEnumerationHelper.IsWindowValidForCapture(p.MainWindowHandle)
                                           where p.MainWindowTitle.Contains("The Ruins Of The Lost Kingdom CHRONICLE")
                                           select new ComboBoxItem(p, $"{p.MainWindowTitle}{Environment.NewLine}{p.MainModule.FileName}");
                processes = new ObservableCollection<ComboBoxItem>(processesWithWindows);
                WindowComboBox1.ItemsSource = processes;
                WindowComboBox2.ItemsSource = processes;
                WindowComboBox3.ItemsSource = processes;
            }
            else
            {
                WindowComboBox1.IsEnabled = false;
                WindowComboBox2.IsEnabled = false;
                WindowComboBox3.IsEnabled = false;
            }
        }
        /*
        private void InitMonitorList()
        {
            if (ApiInformation.IsApiContractPresent(typeof(Windows.Foundation.UniversalApiContract).FullName, 8))
            {
                monitors = new ObservableCollection<MonitorInfo>(MonitorEnumerationHelper.GetMonitors());
                MonitorComboBox.ItemsSource = monitors;
            }
            else
            {
                MonitorComboBox.IsEnabled = false;
                PrimaryMonitorButton.IsEnabled = false;
            }
        }*/

        private async Task StartPickerCaptureAsync(int i)
        {
            var picker = new GraphicsCapturePicker();
            picker.SetWindow(hwnd);
            GraphicsCaptureItem item = await picker.PickSingleItemAsync();

            if (item != null)
            {
                sample.StartCaptureFromItem(item, i);
            }
        }

        private void StartHwndCapture(IntPtr hwnd, int i)
        {
            GraphicsCaptureItem item = CaptureHelper.CreateItemForWindow(hwnd);
            if (item != null)
            {
                sample.StartCaptureFromItem(item, i);
            }
        }
        /*
        private void StartHmonCapture(IntPtr hmon)
        {
            GraphicsCaptureItem item = CaptureHelper.CreateItemForMonitor(hmon);
            if (item != null)
            {
                sample.StartCaptureFromItem(item);
            }
        }*/
        /*
        private void StartPrimaryMonitorCapture()
        {
            MonitorInfo monitor = (from m in MonitorEnumerationHelper.GetMonitors()
                           where m.IsPrimary
                           select m).First();
            StartHmonCapture(monitor.Hmon);
        }*/

        private void StopCapture(int i)
        {
            sample.StopCapture(i);
        }
    }
}

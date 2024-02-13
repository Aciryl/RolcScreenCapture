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
using SharpDX.Direct3D;
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
using Macro;
using System.Windows.Threading;
using System.Threading;
using System.Collections.Generic;
using WindowsInput.Native;
using Macro.Maq;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using Reactive.Bindings;

namespace WPFCaptureSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IntPtr hwnd;
        private Compositor compositor;
        private CompositionTarget target;
        private ContainerVisual root;

        private BasicSampleApplication sample;
        private ObservableCollection<ComboBoxItem> processes;
        private ObservableCollection<MonitorInfo> monitors;

        private WindowActivator wa;
        private SubCharacterKeyMapping sub1;
        private SubCharacterKeyMapping sub2;

        private Dictionary<Key, VirtualKeyCode>[] KeyLists { get; } = new Dictionary<Key, VirtualKeyCode>[BasicSampleApplication.ScreenCount];

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

#if DEBUG
            // Force graphicscapture.dll to load.
            var picker = new GraphicsCapturePicker();
#endif

            KeyLists[0] = new Dictionary<Key, VirtualKeyCode>()
            {
                { Key.Left, VirtualKeyCode.LEFT },
                { Key.Up, VirtualKeyCode.UP },
                { Key.Right, VirtualKeyCode.RIGHT },
                { Key.Down, VirtualKeyCode.DOWN },
                { Key.A, VirtualKeyCode.VK_A },
                { Key.S, VirtualKeyCode.VK_S },
                { Key.Z, VirtualKeyCode.VK_Z },
                { Key.X, VirtualKeyCode.VK_X },
                { Key.C, VirtualKeyCode.VK_C },
                { Key.V, VirtualKeyCode.VK_V },
            };

            KeyLists[1] = new Dictionary<Key, VirtualKeyCode>()
            {
                { Key.Left, VirtualKeyCode.VK_F },
                { Key.Up, VirtualKeyCode.VK_T },
                { Key.Right, VirtualKeyCode.VK_H },
                { Key.Down, VirtualKeyCode.VK_G },
                { Key.A, VirtualKeyCode.VK_1 },
                { Key.S, VirtualKeyCode.VK_2 },
                { Key.Z, VirtualKeyCode.VK_Q },
                { Key.X, VirtualKeyCode.VK_W },
                { Key.C, VirtualKeyCode.VK_E },
                { Key.V, VirtualKeyCode.VK_R },
            };

            KeyLists[2] = new Dictionary<Key, VirtualKeyCode>()
            {
                { Key.Left, VirtualKeyCode.VK_L },
                { Key.Up, VirtualKeyCode.VK_P },
                { Key.Right, VirtualKeyCode.OEM_7 },
                { Key.Down, VirtualKeyCode.OEM_1 },
                { Key.A, VirtualKeyCode.VK_6 },
                { Key.S, VirtualKeyCode.VK_7 },
                { Key.Z, VirtualKeyCode.VK_Y },
                { Key.X, VirtualKeyCode.VK_U },
                { Key.C, VirtualKeyCode.VK_I },
                { Key.V, VirtualKeyCode.VK_O },
            };

            sub1 = new SubCharacterKeyMapping(KeyLists[1]);
            sub2 = new SubCharacterKeyMapping(KeyLists[2]);

            sub1.MappingStart();
            sub2.MappingStart();
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

            // WindowActivation
            wa = new WindowActivator(BasicSampleApplication.ScreenCount);

            if (WindowComboBox1.Items.Count >= 1)
                WindowComboBox1.SelectedItem = WindowComboBox1.Items[0];
            if (WindowComboBox2.Items.Count >= 2)
                WindowComboBox2.SelectedItem = WindowComboBox2.Items[1];
            if (WindowComboBox3.Items.Count >= 3)
                WindowComboBox3.SelectedItem = WindowComboBox3.Items[2];
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
        /*
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
        }*/
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

            var process = selectedItem?.Process;

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

        private void MappingStart_Click(object sender, RoutedEventArgs e)
        {
            sub1.MappingStart();
            sub2.MappingStart();
        }

        private void MappingStop_Click(object sender, RoutedEventArgs e)
        {
            sub1.MappingStop();
            sub2.MappingStop();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButton.IsEnabled = false;
            Thread.Sleep(50);

            Action emptyDelegate = delegate { };
            RefreshButton.Dispatcher.Invoke(emptyDelegate, DispatcherPriority.Render);

            InitWindowList();

            RefreshButton.IsEnabled = true;
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
                                           where p.MainWindowTitle.Equals("The Ruins Of The Lost Kingdom CHRONICLE") ||
                                                 p.MainWindowTitle.Equals("[#] The Ruins Of The Lost Kingdom CHRONICLE [#]")
                                           select new ComboBoxItem(p, $"{p.MainWindowTitle}{Environment.NewLine}{p.MainModule.FileName}");
                processesWithWindows = processesWithWindows.OrderBy(x => x.Process.MainModule.FileName.Replace("_", ""));
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
        /*
        private async Task StartPickerCaptureAsync(int i)
        {
            var picker = new GraphicsCapturePicker();
            picker.SetWindow(hwnd);

            GraphicsCaptureItem item = await picker.PickSingleItemAsync();

            if (item != null)
            {
                sample.StartCaptureFromItem(item, i);
            }
        }*/

        private void StartHwndCapture(IntPtr hwnd, int i)
        {
            wa.Hwnds[i] = hwnd;
            if (i == 1)
                sub1.SetHwnd(hwnd);
            if (i == 2)
                sub2.SetHwnd(hwnd);

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

        private MacroManager MacroManager { get; set; }
        private void MaqMacroButton_Checked(object sender, RoutedEventArgs e)
        {
            if (MacroManager != null)
                return;

            if (wa.Hwnds?[0] != null &&
                wa.Hwnds?[1] != null &&
                wa.Hwnds?[2] != null)
            {
                var hwnds = new[] { wa.Hwnds[0].Value, wa.Hwnds[1].Value, wa.Hwnds[2].Value };
                var keyLists = new[] { KeyLists[0], KeyLists[1], KeyLists[2] };
                var helper = new WindowInteropHelper(this);

                MacroManager = new Macro.Maq.Manager(helper.Handle, hwnds, keyLists, MappingStart, MappingStop, sample.Capture.Select(x => x.ScreenShot).ToArray());
                MacroManager.Start();

                void MappingStart()
                {
                    sub1.MappingStart();
                    sub2.MappingStart();
                    MacroManager = null;
                    MaqMacroButton.IsChecked = false;
                }

                void MappingStop()
                {
                    sub1.MappingStop();
                    sub2.MappingStop();
                }
            }
        }

        private void MaqMacroButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MacroManager != null)
            {
                MacroManager.IsCanceled = true;
                MacroManager.HasError = true;
                MacroManager = null;
            }
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var browser = new CommonOpenFileDialog();

            browser.Title = "フォルダーを選択してください";
            browser.IsFolderPicker = true;
#if DEBUG
            browser.DefaultDirectory = Path.Combine("../", "../", "../", Directory.GetCurrentDirectory(), "ItemImages");
#else
            browser.DefaultDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ItemImages");
#endif
            browser.RestoreDirectory = true;

            if (browser.ShowDialog() == CommonFileDialogResult.Ok)
                ScreenShotFolderName.Text = browser.FileName;
        }

        private void SaveScreenShot_Click(object sender, RoutedEventArgs e)
        {
            if (sample.Capture.Length == 0)
                return;

            if (string.IsNullOrEmpty(ItemName.Text))
                return;

            if (string.IsNullOrEmpty(ScreenShotFolderName.Text))
                return;

            if (!(ScreenShotFree.IsChecked ?? false) &&
                !(ScreenShotShare.IsChecked ?? false) &&
                !(ScreenShotShield.IsChecked ?? false) &&
                !(ScreenShotShareShield.IsChecked ?? false))
                return;

            if (sample.Capture.Length >= 1)
                SaveScreenShot_Do(0);
            if (sample.Capture.Length >= 2)
                SaveScreenShot_Do(1);
            if (sample.Capture.Length >= 3)
                SaveScreenShot_Do(2);
        }

        private void LoadScreenShot_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image file (*.png)|*.png|All file (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "ファイルを選択してください";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                var vecs = Macro.Macro.LoadDataFromImage(openFileDialog.FileNames);

                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    var filename = openFileDialog.FileNames[i];
                    var vec = vecs[i];

                    var itemName = Regex.Match(Path.GetFileName(filename), "(.+)_\\d.png").Result("$1");
                    var vm = DataContext as MainWindowViewModel;
                    vm?.AddItem(itemName, sample.Capture, vec);
                }
            }
        }

        private void SaveScreenShot_Do(int i)
        {
            var screenShot = sample.Capture[i].ScreenShot;

            var colors = new List<(byte r, byte g, byte b, byte a)>();

            if (ScreenShotFree.IsChecked ?? false)
                colors.Add((0xff, 0xff, 0xff, 0xff));
            if (ScreenShotShare.IsChecked ?? false)
                colors.Add((0x40, 0xc0, 0xff, 0xff));
            if (ScreenShotShield.IsChecked ?? false)
                colors.Add((0xc0, 0xff, 0x40, 0xff));
            if (ScreenShotShareShield.IsChecked ?? false)
                colors.Add((0x40, 0xff, 0xc0, 0xff));

            Func<(byte, byte, byte, byte), (byte, byte, byte, byte)> func = ((byte, byte, byte, byte) tuple) =>
            {
                var (r, g, b, a) = tuple;
                
                foreach (var (rr, gg, bb, aa) in colors)
                {
                    if (Near(r, rr) && Near(g, gg) && Near(b, bb) && Near(a, aa))
                    {
                        return (r, g, b, a);
                    }
                }

                return (0, 0, 0, 255);
            };

            screenShot.Save($@"{ScreenShotFolderName.Text}\{ItemName.Text}_{i}.png", func);
        }

        private bool Near(byte c1, byte c2, byte dif = 5)
        {
            return c1 >= c2 - dif && c1 <= c2 + dif;
        }
    }
}

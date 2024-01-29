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

using Composition.WindowsRuntimeHelpers;
using System;
using System.Numerics;
using Windows.ApplicationModel.Contacts;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.UI.Composition;

namespace CaptureSampleCore
{
    public class BasicSampleApplication : IDisposable
    {
        public const int ScreenCount = 3;

        private Compositor compositor;
        private ContainerVisual root;

        private SpriteVisual[] contents = new SpriteVisual[ScreenCount];
        private CompositionSurfaceBrush[] brushes = new CompositionSurfaceBrush[ScreenCount];

        private IDirect3DDevice device;
        private BasicCapture[] capture = new BasicCapture[ScreenCount];

        public BasicSampleApplication(Compositor c)
        {
            compositor = c;
            device = Direct3D11Helper.CreateDevice();

            // Setup the root.
            root = compositor.CreateContainerVisual();
            root.RelativeSizeAdjustment = Vector2.One;

            // Setup the content.
            for (int i = 0; i < ScreenCount; i++)
            {
                brushes[i] = compositor.CreateSurfaceBrush();
                brushes[i].HorizontalAlignmentRatio = 0.5f;
                brushes[i].VerticalAlignmentRatio = 0.5f;
                brushes[i].Stretch = CompositionStretch.Uniform;
                brushes[i].Scale = new Vector2(0.5f, 0.5f);

                var shadow = compositor.CreateDropShadow();
                shadow.Mask = brushes[i];


                contents[i] = compositor.CreateSpriteVisual();
                contents[i].AnchorPoint = new Vector2(i == 0 || i == 2 ? 0.5f + 0.02f : -0.02f,
                                                      i == 0 || i == 1 ? 0.5f + 0.02f / 9 * 16 : -0.02f / 9 * 16);
                contents[i].RelativeOffsetAdjustment = new Vector3(0.5f, 0.5f, 0);
                contents[i].RelativeSizeAdjustment = Vector2.One;
                contents[i].Size = new Vector2(-80, -80);
                contents[i].Brush = brushes[i];
                contents[i].Shadow = shadow;

                root.Children.InsertAtTop(contents[i]);
            }
        }

        public Visual Visual => root;

        public void Dispose()
        {
            for (int i = 0; i < ScreenCount; ++i)
                StopCapture(i);
            compositor = null;
            root.Dispose();
            foreach (var content in contents)
                content.Dispose();
            foreach (var brush in brushes)
                brush.Dispose();
            device.Dispose();
        }

        public void StartCaptureFromItem(GraphicsCaptureItem item, int i)
        {
            if (i < 0 || i >= ScreenCount)
                throw new ArgumentOutOfRangeException(nameof(i));

            StopCapture(i);
            capture[i] = new BasicCapture(device, item);

            var surface = capture[i].CreateSurface(compositor);
            brushes[i].Surface = surface;

            capture[i].StartCapture();
        }

        public void StopCapture(int i)
        {
            if (i < 0 || i >= ScreenCount)
                throw new ArgumentOutOfRangeException(nameof(i));

            capture[i]?.Dispose();
            brushes[i].Surface = null;
        }
    }
}

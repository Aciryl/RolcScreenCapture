using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Windows.ApplicationModel.AppExtensions;
using Windows.UI.Xaml.Controls;

namespace CaptureSampleCore
{
    public class ScreenShot
    {
        private const int Top = 26;
        private const int Left = 1;
        private const int Right = 1;
        private const int Bottom = 1;

        private byte[] colors;
        public int TextureWidth { get; private set; }
        public int TextureHeight { get; private set; }
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }

        public delegate void UpdateEventHandler();
        public event UpdateEventHandler UpdateEvent;

        public void Update(Texture2D texture)
        {
            TextureWidth = Get64Width(texture.Description.Width);
            TextureHeight = texture.Description.Height;
            ClientWidth = texture.Description.Width - Left - Right;
            ClientHeight = TextureHeight - Top - Bottom;
            colors = GetColorDataFromTexture(texture);
            UpdateEvent?.Invoke();
        }

        public (byte r, byte g, byte b, byte a) GetPixels(int x, int y)
        {
            var index = GetIndex(x, y);
            return (colors[index + 2],
                    colors[index + 1],
                    colors[index + 0],
                    colors[index + 3]);
        }

        public void Save(string filename, Func<(byte r, byte g, byte b, byte a), (byte r, byte g, byte b, byte a)> func = null)
        {
            var pixels = new byte[ClientWidth * ClientHeight * 4];

            using (var img = new Bitmap(ClientWidth, ClientHeight, PixelFormat.Format32bppArgb))
            {
                //Bitmapをロックする
                BitmapData bmpData = img.LockBits(
                    new Rectangle(0, 0, img.Width, img.Height),
                    ImageLockMode.ReadWrite,
                    img.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                // データを加工する
                var index2 = 0;
                //for (int y = Top; y < textureHeight - Bottom; ++y)
                for (int y = 0; y < ClientHeight; ++y)
                {
                    //for (int x = Left; x < textureWidth - Right; ++x)
                    for (int x = 0; x < ClientWidth; ++x)
                    {
                        //var index = (y * textureWidth + x) * 4;
                        var index = GetIndex(x, y);
                        var b = colors[index + 0];
                        var g = colors[index + 1];
                        var r = colors[index + 2];
                        var a = colors[index + 3];

                        (byte pr, byte pg, byte pb, byte pa) = func?.Invoke((r, g, b, a)) ?? (r, g, b, a);
                        pixels[index2 + 0] = pb;
                        pixels[index2 + 1] = pg;
                        pixels[index2 + 2] = pr;
                        pixels[index2 + 3] = pa;

                        index2 += 4;
                    }
                }

                //ピクセルデータを Bitmap に反映させる
                Marshal.Copy(pixels, 0, ptr, pixels.Length);

                //ロックを解除する
                img.UnlockBits(bmpData);

                img.Save(filename, ImageFormat.Png);
            }
        }

        private int GetIndex(int x, int y)
        {
            if (x < 0 || x >= ClientWidth)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= ClientHeight)
                throw new ArgumentOutOfRangeException(nameof(y));

            return ((y + Top) * TextureWidth + (x + Left)) * 4;
        }

        private byte[] GetColorDataFromTexture(Texture2D texture)
        {
            // Create our staging texture
            var description = new Texture2DDescription
            {
                Width = texture.Description.Width,
                Height = texture.Description.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                SampleDescription = new SharpDX.DXGI.SampleDescription()
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = ResourceUsage.Staging,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            };

            // Texture2DからDeviceへのアクセスを取得
            //using (var surface = texture.QueryInterface<Surface>())
            using (var device = texture.Device.QueryInterface<Device>())
            using (var deviceContext = device.ImmediateContext)
            using (var stagingTexture = new Texture2D(device, description))
            {
                // Copy to our staging texture
                deviceContext.CopyResource(texture, stagingTexture);
                // Texture2Dをマップ
                var dataBox = deviceContext.MapSubresource(
                    stagingTexture,
                    0,
                    MapMode.Read,
                    MapFlags.None);

                // カラーデータを取得
                int width = TextureWidth;
                int height = TextureHeight;
                int pitch = dataBox.RowPitch;

                // カラーデータを格納する配列
                //byte[,,] colors = new byte[height - top - left, width - left * 2, 4];
                byte[] rawColors = new byte[height * width * 4];

                // ポインタから1次元配列へコピー
                Marshal.Copy(dataBox.DataPointer, rawColors, 0, rawColors.Length);

                // マップ解除
                deviceContext.UnmapSubresource(texture, 0);

                return rawColors;
            }
        }

        private static int Get64Width(int value)
        {
            return (int)Math.Ceiling(value / 64d) * 64;
        }
    }
}

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace dotnet
{
    class Program
    {
        private const int ColorsPerPixel = 6;

        private static int GetIterations(int maxiter, double x, double y)
        {
            double iterX = 0.0;
            double iterY = 0.0;
            double iterX2 = iterX * iterX;
            double iterY2 = iterY * iterY;

            /* iterate the point */
            int k;
            for (k = 1; k < maxiter && ((iterX2 + iterY2) < 4.0); k++) {
                iterY = (2 * iterX * iterY) + y;
                iterX = iterX2 - iterY2 + x;
                iterX2 = iterX * iterX;
                iterY2 = iterY * iterY;
            };

            // Console.WriteLine("x={0}, y={1}, k={2}", x, y, k);            
            return k;
        }

        private static byte[] ComputeSet(double xmin, double xmax, double ymin, double ymax,
        int maxiter, int xres, int yres)
        {
            int colorsLength = yres * xres * ColorsPerPixel;
            byte[] colours = new byte[colorsLength];

            // Calculate width and height
            double pixelWidth = (xmax - xmin)/(double)xres;
            double pixelHeight = (ymax-ymin)/(double)yres;

            double currentX, currentY; // Current point in the complex plane
            int xPixel, yPixel;

            for (yPixel = 0; yPixel < yres; yPixel++)
            {
                currentY = ymax - ((double)yPixel*pixelHeight);
                for (xPixel = 0; xPixel < xres; xPixel++)
                {
                    currentX = xmin + ((double)xPixel*pixelWidth);
                    int iterations = GetIterations(maxiter, currentX, currentY);
                    
                    int colorIndex = (yPixel*xres*ColorsPerPixel)+(xPixel*ColorsPerPixel);
                    if(iterations >= maxiter)
                    {
                        // interior - color=black
                        byte[] black = {(byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0};
                        Array.Copy(black, 0, colours, colorIndex, ColorsPerPixel);
                    }else
                    {
                        byte[] color = new byte[6];
                        color[0] = (byte)(iterations >> 8);
                        color[1] = (byte)(iterations & 255);
                        color[2] = (byte)(iterations >> 8);
                        color[3] = (byte)(iterations & 255);
                        color[4] = (byte)(iterations >> 8);
                        color[5] = (byte)(iterations & 255);

                        Array.Copy(color, 0, colours, colorIndex, ColorsPerPixel);
                    }
                }
            }

            return colours;
        }

        private static void WriteFile(string fileName, string header, byte[] colorBytes)
        {
            using(var textWriter = new StreamWriter(fileName))
            {
                textWriter.Write(header);
                textWriter.Close();
            }
            
            using(var fileStream = File.Open(fileName, FileMode.Append))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    Span<byte> bytes = colorBytes.AsSpan();
                    binaryWriter.Write(bytes);
                }
            }
        }

        public static void Main(string[] args)
        {
            double xmin = -2.0D;
            double xmax = 1.0D;
            double ymin = -1.0D;
            double ymax = 1.0D;
            int maxiter = 1000;
            int xres = 2000;
            string fileName = "pic.ppm";
            int yres = (int)((xres*(ymax-ymin))/(xmax-xmin));

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            var colorBytes = ComputeSet(xmin, xmax, ymin, ymax, maxiter, xres, yres);
            stopWatch.Stop();
            Console.WriteLine("Computation took {0}ms", stopWatch.ElapsedMilliseconds);
            string headerText = $"P6\n# Mandelbrot, xmin={xmin}, xmax={xmax}, ymin={ymin}, ymax={ymax}, maxiter={maxiter}\n{xres}\n{yres}\n{(maxiter < 256 ? 256 : maxiter)}\n";
            WriteFile(fileName, headerText, colorBytes);
        }
    }
}

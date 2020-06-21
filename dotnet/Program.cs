using System;
using System.IO;
using System.Runtime.InteropServices;

namespace dotnet
{
    class Program
    {

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

        private static int[] ComputeSet(double xmin, double xmax, double ymin, double ymax,
        int maxiter, int xres, int yres)
        {
            int colorsLength = yres * xres * 6;
            int[] colours = new int[colorsLength];

            // Calculate width and height
            double pixelWidth = (xmax - xmin)/xres;
            double pixelHeight = (ymax-ymin)/yres;

            double currentX, currentY; // Current point in the complex plane
            int xPixel, yPixel;

            for (yPixel = 0; yPixel < yres; yPixel++)
            {
                currentY = ymax - (yPixel*pixelHeight);
                for (xPixel = 0; xPixel < xres; xPixel++)
                {
                    currentX = xmin + (xPixel*pixelWidth);
                    int iterations = GetIterations(maxiter, currentX, currentY);
                    
                    int colorIndex = (yPixel*xres*6)+(xPixel*6);
                    if(iterations >= maxiter)
                    {
                        // interior - color=black
                        int[] black = {0, 0, 0, 0, 0, 0};
                        Array.Copy(black, 0, colours, colorIndex, 6);
                    }else
                    {
                        int[] color = new int[6];
                        color[0] = iterations >> 8;
                        color[1] = iterations & 255;
                        color[2] = iterations >> 8;
                        color[3] = iterations & 255;
                        color[4] = iterations >> 8;
                        color[5] = iterations & 255;
                        Array.Copy(color, 0, colours, colorIndex, 6);
                    }

                }
            }

            return colours;
        }

        private static void WriteFile(string fileName, string header, int[] colorBytes)
        {
            using(var textWriter = new StreamWriter(fileName))
            {
                textWriter.Write(header);
                textWriter.Close();
            }
            
            using(var fileStream = File.Open(fileName, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    Span<byte> bytes = MemoryMarshal.Cast<int, byte>(colorBytes.AsSpan());
                    binaryWriter.Write(bytes);
                }
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length != 7) {
                Console.WriteLine("Usage:   dotnet run <xmin> <xmax> <ymin> <ymax> <maxiter> <xres> <out.ppm>\n");
                Console.WriteLine("Example: dotnet run 0.27085 0.27100 0.004640 0.004810 1000 1024 pic.ppm\n");
                return;
            }

            // var test = GetIterations(1000, 0.27085004999999995, 0.00481);

            double xmin = Double.Parse(args[0]);
            double xmax = Double.Parse(args[1]);
            double ymin = Double.Parse(args[2]);
            double ymax = Double.Parse(args[3]);
            int maxiter = Int16.Parse(args[4]);
            int xres = Int32.Parse(args[5]);
            string fileName = args[6];
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

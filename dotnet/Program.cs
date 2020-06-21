using System;
// using System.Diagnostics;

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
            for (k = 1; k < maxiter && (iterX2 + iterY2 < 4.0); k++) {
                iterY = 2 * iterX * iterY + y;
                iterX = iterX2 - iterY2 + x;
                iterX2 = iterX * iterX;
                iterY2 = iterY * iterY;
            };

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
                currentY = ymax - yPixel*pixelHeight;
                for (xPixel = 0; xPixel < xres; xPixel++)
                {
                    currentX = xmin + xPixel*pixelWidth;
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


        public static void Main(string[] args)
        {
            if (args.Length != 7) {
                Console.WriteLine("Usage:   dotnet run <xmin> <xmax> <ymin> <ymax> <maxiter> <xres> <out.ppm>\n");
                Console.WriteLine("Example: dotnet run 0.27085 0.27100 0.004640 0.004810 1000 1024 pic.ppm\n");
                return;
            }

            double xmin = Double.Parse(args[0]);
            double xmax = Double.Parse(args[1]);
            double ymin = Double.Parse(args[2]);
            double ymax = Double.Parse(args[3]);
            int maxiter = Int16.Parse(args[4]);
            int xres = Int32.Parse(args[5]);
            int yres = (int)((xres*(ymax-ymin))/(xmax-xmin));

            // var stopWatch = new StopWatch();
            // stopWatch.Start();
            long start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            ComputeSet(xmin, xmax, ymin, ymin, maxiter, xres, yres);
            // stopWatch.Stop();
            long end = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            Console.WriteLine("Computation took {0}ms", end-start);
        }
    }
}

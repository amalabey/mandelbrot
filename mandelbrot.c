#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>

int get_iterations(uint16_t maxiter, double x, double y)
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


void compute_set(double xmin, double xmax,double ymin, double ymax, uint16_t maxiter, 
int xres, int yres, unsigned char* colors)
{
    // Calculate width and height
    double pixelWidth = (xmax - xmin)/xres;
    double pixelHeight = (ymax-ymin)/yres;

    double currentX, currentY; // Current point in the complex plane
    double iterX, iterY; // Currently iterated point
    int xPixel, yPixel;
    int iterCounter;

    for (yPixel = 0; yPixel < yres; yPixel++)
    {
        currentY = ymax - yPixel*pixelHeight;
        for (xPixel = 0; xPixel < xres; xPixel++)
        {
            currentX = xmin + xPixel*pixelWidth;
            int iterations = get_iterations(maxiter, currentX, currentY);
            unsigned char color[6];
            unsigned char* colorIndex = colors + (yPixel*xres*6)+(xPixel*6);
            if(iterations >= maxiter)
            {
                // interior - color=black
                const unsigned char black[] = {0, 0, 0, 0, 0, 0};
                memcpy(colorIndex, black, sizeof(black));
            }else
            {
                unsigned char color[6];
                color[0] = iterations >> 8;
                color[1] = iterations & 255;
                color[2] = iterations >> 8;
                color[3] = iterations & 255;
                color[4] = iterations >> 8;
                color[5] = iterations & 255;
                memcpy(colorIndex, color, sizeof(color));
            }
        }
    }
}

void write_file(const char* file_name, unsigned char* colors, int len, double xmin, double xmax,
double ymin, double ymax, int maxiter, int xres, int yres)
{
    FILE * fp = fopen(file_name,"wb");
    
    // write ASCII header to the file
    fprintf(fp,
            "P6\n# Mandelbrot, xmin=%lf, xmax=%lf, ymin=%lf, ymax=%lf, maxiter=%d\n%d\n%d\n%d\n",
            xmin, xmax, ymin, ymax, maxiter, xres, yres, (maxiter < 256 ? 256 : maxiter));

    fwrite(colors, 1, len, fp);
    fclose(fp);
}

int main(int argc, char* argv[])
{
    /* Parse the command line arguments. */
    if (argc != 8) {
        printf("Usage:   %s <xmin> <xmax> <ymin> <ymax> <maxiter> <xres> <out.ppm>\n", argv[0]);
        printf("Example: %s 0.27085 0.27100 0.004640 0.004810 1000 1024 pic.ppm\n", argv[0]);
        exit(EXIT_FAILURE);
    }

    /* The window in the plane. */
    const double xmin = atof(argv[1]);
    const double xmax = atof(argv[2]);
    const double ymin = atof(argv[3]);
    const double ymax = atof(argv[4]);

    /* Maximum number of iterations, at most 65535. */
    const uint16_t maxiter = (unsigned short)atoi(argv[5]);

    /* Image size, width is given, height is computed. */
    const int xres = atoi(argv[6]);
    const int yres = (xres*(ymax-ymin))/(xmax-xmin);

    // Allocated an array for colors = width * height * 6 byte color code
    int colors_len = yres * xres * 6;
    unsigned char* colors = malloc(colors_len * sizeof(unsigned char));
    compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres, colors);

    // Write resulting colors to file
    write_file("c-mandelbrot.ppm", colors, colors_len, xmin, xmax, ymin, ymax, maxiter, xres, yres);

    // Free up allocated memor
    free(colors);
}
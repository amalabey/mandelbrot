from datetime import datetime
import numpy as np

COLOURS_PER_PIXEL = 6  

def get_iterations(maxiter, x, y):
    iterX = 0.0
    iterY = 0.0
    iterX2 = iterX * iterX
    iterY2 = iterY * iterY

    k = 1
    while k < maxiter and ((iterX2 + iterY2) < 4.0):
        iterY = (2 * iterX * iterY) + y
        iterX = iterX2 - iterY2 + x
        iterX2 = iterX * iterX
        iterY2 = iterY * iterY
        k += 1

    return k

def compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres):
    colorsLength = yres * xres * COLOURS_PER_PIXEL
    colours = np.empty([colorsLength], dtype=np.uint8)

    pixelWidth = (xmax - xmin)/xres
    pixelHeight = (ymax-ymin)/yres

    currentX = 0
    currentY = 0

    for y in range(yres): 
        currentY = ymax - y*pixelHeight
        for x in range(xres):
            currentX = xmin + x*pixelWidth
            k = get_iterations(maxiter, currentX, currentY)
            colourIndex = (y*xres*COLOURS_PER_PIXEL)+(x*COLOURS_PER_PIXEL)
            if k >= maxiter:
                colours[colourIndex:colourIndex+COLOURS_PER_PIXEL] = np.array([0,0,0,0,0,0])
            else:
                colour = np.empty([COLOURS_PER_PIXEL], dtype=np.uint8)
                colour[0] = k >> 8
                colour[1] = k & 255
                colour[2] = k >> 8
                colour[3] = k & 255
                colour[4] = k >> 8
                colour[5] = k & 255
                colours[colourIndex:colourIndex+COLOURS_PER_PIXEL] = colour
    
    return colours

def write_file(file_name, header, colourBytes):
    with open(file_name,"w") as textWriter:
        textWriter.write(header)
        textWriter.close()

    with open(file_name,"ab") as binaryWriter:
        colourBytes.tofile(binaryWriter)


xmin = -2.0
xmax = 1.0
ymin = -1.0
ymax = 1.0
maxiter = 1000
xres = 2000
yres = int((xres*(ymax-ymin))/(xmax-xmin))

start = datetime.now()
colours = compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres)
end = datetime.now()
timediff = (end - start)
print("Computation took: {0}ms".format((timediff.seconds * 1000)+(timediff.microseconds/1000)))

file_name = "pic.ppm"
colour_depth = 256 if maxiter < 256 else maxiter
headerText = "P6\n# Mandelbrot, xmin={0}, xmax={1}, ymin={2}, ymax={3}, maxiter={4}\n{5}\n{6}\n{7}\n".format(xmin, xmax, ymin, ymax, maxiter, xres, yres, colour_depth)
write_file(file_name, headerText, colours)
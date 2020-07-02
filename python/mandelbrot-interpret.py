from datetime import datetime
from PIL import Image 
from numpy import complex, array 
import colorsys 

def get_iterations(maxiter, x, y):
    c0 = complex(x, y) 
    c = 0
    for i in range(1, maxiter): 
        if abs(c) > 4: 
            return i
        c = c * c + c0
    return maxiter

def compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres, img, pixels):
    pixelWidth = (xmax - xmin)/xres
    pixelHeight = (ymax-ymin)/yres

    currentX = 0
    currentY = 0

    for x in range(img.size[0]): 
        currentX = xmin + x*pixelWidth
        for y in range(img.size[1]):
            currentY = ymax - y*pixelHeight
            k = get_iterations(maxiter, currentX, currentY)
            if k >= maxiter:
                pixels[x,y] = (0,0,0)
            else:
                color = 255 * array(colorsys.hsv_to_rgb(k / 255.0, 1.0, 0.5)) 
                pixels[x,y] =  tuple(color.astype(int))


if __name__ == "__main__":
    xmin = -2.0
    xmax = 1.0
    ymin = -1.0
    ymax = 1.0
    maxiter = 1000
    xres = 3000
    yres = int((xres*(ymax-ymin))/(xmax-xmin))

    # creating the new image in RGB mode 
    img = Image.new('RGB', (xres, yres))
    pixels = img.load()

    pixels = img.load() 
    start = datetime.now()
    print("Starting computation")
    # CPython = Computation took: 203966ms
    # PYPY = Computation took: 146862ms
    compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres, img, pixels)
    print("Completed computation")
    end = datetime.now()
    timediff = (end - start)
    print("Computation took: {0}ms".format((timediff.seconds * 1000)+(timediff.microseconds/1000)))
    img.save("pic.png")



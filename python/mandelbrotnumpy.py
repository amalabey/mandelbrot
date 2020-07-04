# original code from: https://tomroelandts.com/articles/how-to-compute-the-mandelbrot-set-using-numpy-array-operations
import numpy as np
from imageio import imwrite
import sys
import time

def compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres):
    x = np.linspace(xmin, xmax, num=xres).reshape((1, xres))
    y = np.linspace(ymin, ymax, num=yres).reshape((yres, 1))
    C = np.tile(x, (yres, 1)) + 1j * np.tile(y, (1, xres))
    
    Z = np.zeros((yres, xres), dtype=complex)
    M = np.full((yres, xres), True, dtype=bool)
    for i in range(maxiter):
        Z[M] = Z[M] * Z[M] + C[M]
        M[np.abs(Z) > 2] = False
    
    return M

def write_file(file_name, M):
    imwrite(file_name, np.uint8(np.flipud(1 - M) * 255))

if __name__ == '__main__':
    if len(sys.argv) != 8:
        print("Usage:   {0} <xmin> <xmax> <ymin> <ymax> <maxiter> <xres> <out.ppm>\n".format(sys.argv[0]))
        print("Example: {0} 0.27085 0.27100 0.004640 0.004810 1000 1024 pic.ppm\n".format(sys.argv[0]))
        exit()
    
    xmin = float(sys.argv[1])
    xmax = float(sys.argv[2])
    ymin = float(sys.argv[3])
    ymax = float(sys.argv[4])
    maxiter = int(sys.argv[5])
    xres = int(sys.argv[6])
    yres = int((xres*(ymax-ymin))/(xmax-xmin))
    file_name = sys.argv[7]

    current_milli_time = lambda: int(round(time.time() * 1000))
    start_time = current_milli_time()
    M = compute_set(xmin, xmax, ymin, ymax, maxiter, xres, yres)
    end_time = current_milli_time()
    print("Computation took {0}ms".format(end_time-start_time))

    write_file(file_name, M)
    



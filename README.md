## C Example

- Compile  
```
gcc -o mandelbrot mandelbrot.c
```

- Run    
```
./mandelbrot 0.27085 0.27100 0.004640 0.004810 1000 1024 c.ppm && convert -normalize c.ppm c.png
```
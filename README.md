# Get started
Refer to the `Dockerfile` for all dependencies and how to build a container with all pre-requsites.

Quick start using Docker:
```
docker run --rm -it amalabey/mandelbrot:v0.1.5 bash
```

# Run Examples
```
.\run
```
Example output:
```
 ./run.sh
----- Running C implementation ------
Computation took 4090 ms
Computation took 4125 ms
Computation took 4104 ms
----- Running C# (dotnetcore 3.1) implementation ------
Computation took 4113ms
Computation took 4165ms
Computation took 4117ms
----- Running Python (PyPy) implementation ------
Computation took: 29640.48ms
Computation took: 30236.367ms
Computation took: 31263.059ms
----- Running Python (Cython) implementation ------
Computation took: 5550.133ms
Computation took: 5592.906ms
Computation took: 5615.564ms
----- Running Python (Numba) implementation ------
Computation took: 2828.969ms
Computation took: 2851.043ms
Computation took: 2826.89ms
----- Running Python (CPython) implementation ------
Computation took: 127458.006ms
Computation took: 127332.571ms
Computation took: 127644.063ms
```

# View Output
Each implementation outputs the generated set as a "ppm" file. Use the below command to convert it to a PNG.

```
convert -normalize pic.ppm pic.png
```
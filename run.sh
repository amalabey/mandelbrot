#!/bin/bash

echo "----- Running C implementation ------"
./c/mandelbrot
./c/mandelbrot
./c/mandelbrot

echo "----- Running C# (dotnetcore 3.1) implementation ------"
dotnet run -p dotnet/dotnet.csproj
dotnet run -p dotnet/dotnet.csproj
dotnet run -p dotnet/dotnet.csproj

echo "----- Running Python (PyPy) implementation ------"
pypy3 python/cpythonimpl.py
pypy3 python/cpythonimpl.py
pypy3 python/cpythonimpl.py

echo "----- Running Python (Cython) implementation ------"
python3 python/run-cython.py
python3 python/run-cython.py
python3 python/run-cython.py

echo "----- Running Python (Numba) implementation ------"
python3 python/numbaimpl.py
python3 python/numbaimpl.py
python3 python/numbaimpl.py

echo "----- Running Python (CPython) implementation ------"
python3 python/cpythonimpl.py
python3 python/cpythonimpl.py
python3 python/cpythonimpl.py

# echo "----- Running Python (Numpy Matrix) implementation ------"
# python3 python/numbaimpl.py
FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# Install GCC for C example
RUN apt update
RUN apt install gcc -y

# Install Python3 for Python examples
RUN apt install python3.7 -y
RUN apt install python3-pip -y

# Install Python dependencies
RUN pip3 install numba
RUN pip3 install numpy
RUN pip3 install cython

# Install pypy3
RUN wget https://bitbucket.org/pypy/pypy/downloads/pypy3.6-v7.3.1-linux64.tar.bz2
RUN tar -xvf pypy3.6-v7.3.1-linux64.tar.bz2
RUN ln -s /pypy3.6-v7.3.1-linux64/bin/pypy3 /usr/bin/pypy3
RUN pypy3 -m ensurepip
RUN pypy3 -m pip install numpy
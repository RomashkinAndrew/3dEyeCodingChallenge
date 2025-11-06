#A 3D Eye Coding Assignment

This project implements two different approaches to sorting a file:

##Merge Sort

The file is divided into chunks (called runs), which are loaded into RAM either sequentially or in parallel. Each run is sorted and written back to disk. During the merge phase, lines from all runs are read simultaneously, and the smallest line is written to the output file.

##Heap Sort

This approach builds an index (stored either in RAM or on disk) that contains the byte offsets and lengths of all lines in the source file. The index is then sorted using a heap sort algorithm, and the sorted file is constructed based on the sorted index.

Heap sort uses slightly more disk space, but can operate with almost no RAM if the index is stored on disk. Merge sort requires at least _run_\__length_ × _parallel_\__runs_ worth of RAM. Heap sort turned out to be significantly slower, but I've still left it in the solution for comparison sake.

The included test file generator can be used to produce data files for benchmarking the sorters. Files are generated using ASCII encoding to fit more data into the same file size, making the challenge slightly harder.
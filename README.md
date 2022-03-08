---
An implementation of Conway's Game of Life, made by Sophia Walsh Long as a university assignment in 2020
---

## Build Instructions

Open Life.sln in Visual Studio. Ensure that the three dropdowns in the toolbar say "Release/Any CPU/Life". Press Ctrl+Shift+B to build the solution.

After this, the location of life.dll (relative to this file's location) should be `\Life\Life\bin\Release\netcoreapp3.1`


## Usage 

This program runs a simulation of Conway's Game of Life.

Some aspects of this simulation are configurable through command line options. These should be typed after `life.dll` when running from the command line, separated by spaces. The order in which they are specified is not important.

* `--dimensions <rows> <columns>` (both arguments must be integers in the range 4-48 inclusive) sets the board dimensions as specified. The default board size is 16x16.
* `--random <probability>` (`<probability>` must be a decimal value, 0-1 inclusive) sets the random factor, i.e. the probability of any given cell being alive in the randomly generated beginning state. The default value is 0.5.
* `--generations <number>` (`<number>` must be an integer greater than 3) sets the number of generations that will be simulated. The default value is 50.
* `--memory <number>` (`<number>` must be an integer, 4-512 inclusive) sets the number of generations that will be remembered for the purpose of detecting a steady-state. The default value is 16.
* `--max-update <number>` (`<number>` must be a float value, 1-30 inclusive) sets the number of generations simulated per second. The default value is 5.
* `--seed <path>` (`<path>` must be a valid absolute or relative file path to a .seed file) uses the seed file to determine the initial state of the board rather than randomly generating it. The board must be large enough to contain the seed. By default no seed file is used.
* `--output <path>` (`<path>` must be an absolute or relative file path to a .seed file, though the file need not currently exist) causes the final state of the game to be written to the specified seed file, such that it can later be loaded with `--seed`. By default no output file is used.
* `--neighbour <type> <order> <centrecount>` (`<type>` must be either "moore" or "vonNeumann", case insensitive; `<order>` must be an integer in the range 1-10 inclusive and less than half the smallest board dimension; `<centrecount>` must be either "true" or "false", case insensitive) sets the neighbourhood used to count the alive neighbours of cells. The order determines the maximum distance between two cells for them to be considered each other's neighbours. Moore and Von Neumann neighbourhoods count distance differently, however: Moore neighbourhoods are square, while Von Neumann neighbourhoods count distance as the sum of the row and column distances, causing them to have a diamond shape. Centre-count specifies whether a cell counts as its own neighbour. The default neighbourhood is a Moore neighbourhood of order 1 with centre-count off.
* `--survival <param1> <param2> <param3> ...` (takes an arbitrary number of parameters, which must all be non-negative integers or integer ranges in the format "3...6"; duplicates are ignored; none of the numbers may be greater than the total number of cells in the specified neighbourhood) changes the set of values X for which an alive cell with X alive neighbours will be alive in the next generation (survive). The default value is ( 2...3 ).
* `--birth <param1> <param2> <param3> ...` (same parameter rules as survival) changes the set of values X for which a dead cell with X alive neighbours will be alive in the next generation (be born). The default value is ( 3 ).
* `--step` (no parameters) enables step mode; in this mode, instead of the simulation advancing automatically, you must press the space bar to manually "step" through the simulation. By default this is disabled.
* `--periodic` (no parameters) enables periodic mode; in this mode, the board "wraps around" horizontally and vertically, such that a tile on the left edge will be considered a neighbour to a tile on the right edge, and the same for the top and bottom edges. By default this is disabled.
* `--ghost` (no parameters) enables ghost mode; in this mode, the three previous generations are overlaid onto the display in lighter shades than the current generation. By default this is disabled.

## Notes 

None

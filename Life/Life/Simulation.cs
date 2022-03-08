using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Display;

namespace Life
{
    /// <summary>
    /// Game Of Life simulation using the Display API
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>August-October 2020</date>
    public class Simulation
    {
        //Declare vars
        private readonly Options settings;
        private readonly int[,] universe;
        private readonly List<int[,]> history = new List<int[,]>();
        // Construct grid
        public Grid grid;

        private Stopwatch stopwatch = new Stopwatch();
        /// <summary>
        /// Construct a new Game Of Life simulation using the Options object specified
        /// </summary>
        /// <param name="options">an Options object containing the runtime settings</param>
        public Simulation(Options options)
        {
            settings = options;
            universe = InitialiseUniverse();

            //Setup grid
            grid = new Grid(settings.Rows, settings.Columns);

            // Wait for user to press the spacebar
            Logging.Message("Press the spacebar to start...");
            stopwatch.Restart();
            WaitSpacebar(stopwatch);

            // Initialize the grid window (this will resize the window and buffer)
            grid.InitializeWindow();
            stopwatch.Stop();
        }

        /// <summary>
        /// Wait until a unique spacebar press (ignoring when it is held)
        /// </summary>
        /// <param name="watch">Stopwatch object to use for delays</param>
        public static void WaitSpacebar(Stopwatch watch)
        {
            //The watch will have been restarted after the previous spacebar press

            //550ms cooldown to prevent an edge case where a single spacebar press could progress 2 generations
            //if the spacebar was held for just over half a second before being released
            //(when a key is pressed, there's a ~500ms delay between the initial press and the computer registering
            //the key as being held; without this cooldown, the first frame of a hold would register as a unique press)
            while (watch.ElapsedMilliseconds < 550)
            {
                if (Console.KeyAvailable)
                {
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    watch.Restart();
                }
            }
            //Enter an infinite loop
            while (true)
            {
                //If there is a key in the buffer at any given step of the infinite loop
                if (Console.KeyAvailable)
                {
                    //Get the key info of that key
                    ConsoleKeyInfo c = Console.ReadKey(true);
                    //Wait 50ms
                    //If this is a press and not a hold, KeyAvailable should be false after 50ms
                    watch.Restart();
                    while (watch.ElapsedMilliseconds < 50) ;
                    //If the buffer is empty and the key was a spacebar, break the infinite loop and move on
                    if (!Console.KeyAvailable && c.Key == ConsoleKey.Spacebar)
                    {
                        break;
                    }
                    //If there's something in the buffer, empty the buffer before restarting the infinite loop
                    else if (Console.KeyAvailable)
                    {
                        while (Console.KeyAvailable)
                        {
                            Console.ReadKey(true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate an initial value for universe based on the values in settings
        /// </summary>
        /// <returns>An array of zeros and ones to assign to universe</returns>
        private int[,] InitialiseUniverse()
        {
            //If there is an input file
            if (settings.InputFile != null)
            {
                //Try to read the seed; if any of the cells specified were outside the universe size, 
                //or the unreachable seed-invalid exception (explained in ReadSeed) was thrown, warn and randomise
                try
                {
                    return ReadSeed(settings.Rows, settings.Columns, settings.InputFile);
                }
                catch (IndexOutOfRangeException)
                {
                    Logging.Warning($"Input file '{settings.InputFile}' could not be rendered " +
                        $"on a {settings.Rows}x{settings.Columns} board. Reverting to random seed...");
                }
                catch (ArgumentException)
                {
                    Logging.Warning($"Input file '{settings.InputFile}' could not be parsed. " +
                        $"Please check that the file is correctly formatted. Reverting to random seed...");
                }
            }
            //Randomise-- this only executes if the input file was null or an exception was caught
            return RandomState(settings.Rows, settings.Columns, settings.RandomFactor);
        }

        /// <summary>
        /// Read the seed file specified by input_file into a 2d array of zeros and ones
        /// A default system exception will be thrown if the seed is too big for the specified universe size
        /// But this is caught by the method where this one is called
        /// </summary>
        /// <param name="rows">Row count</param>
        /// <param name="columns">Column count</param>
        /// <param name="input_file">Input file address, relative or absolute</param>
        /// <returns>A 2D array to assign to universe</returns>
        private int[,] ReadSeed(int rows, int columns, string input_file)
        {
            //Set up an array of the right size filled with zeros
            int[,] universe = new int[rows, columns];
            using (StreamReader reader = new StreamReader(input_file))
            {
                //Get the first line of the seed
                string line = reader.ReadLine();
                //if it's a v1 seed...
                if (line == "#version=1.0")
                {
                    //For every line in the rest of the seed
                    while (!reader.EndOfStream)
                    {
                        //Get the space-separated coordinate pair specified in the line
                        line = reader.ReadLine();

                        string[] elements = line.Split(" ");

                        int row = int.Parse(elements[0]);
                        int column = int.Parse(elements[1]);

                        //Set the corresponding cell to 1
                        universe[row, column] = 1;
                    }
                }
                else //otherwise, it's a v2 seed
                {
                    //The punctuation characters delineating the elements of a v2 seed line
                    char[] separators = new char[] { ')', ':', ',' };
                    //For every line in the rest of the seed
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        //In v2 seeds, whitespace should be ignored and the elements are separated by punctuation
                        //So remove any spaces and split the line based on the punctuation
                        string[] elements = line.Replace(" ","").Split(separators);
                        //Get the alive/dead status of the structure
                        int new_value = (elements[0] == "(o") ? 1 : 0;
                        //Create a Shape object, of a child class chosen based on the specified shape type
                        //Using the value and coordinates specified in the rest of the line
                        //(Note regarding the default case argument exception: it should never be seen,
                        //since invalid seed files won't be tested, but I had to add it when I refactored this bit
                        //to prevent a syntax error on "shape.Draw(universe)", since without a default case
                        //there's nothing ensuring the var will DEFINITELY point somewhere by the end of the switch)
                        Shape shape = (elements[1]) switch
                        {
                            "cell" => new Cell(new_value, Shape.ParseCoordinates(2, elements)),
                            "rectangle" => new Rectangle(new_value, Shape.ParseCoordinates(4, elements)),
                            "ellipse" => new Ellipse(new_value, Shape.ParseCoordinates(4, elements)),
                            _ => throw new ArgumentException(),
                        };
                        //Draw the shape on the universe
                        universe = shape.Draw(universe);
                    }
                }
            }
            return universe;
        }


        /// <summary>
        /// Fills the cells array with randomly generated 0s and 1s, representing dead and alive cells respectively.
        /// The probability option dictates how likely any given cell is to be a 1
        /// </summary>
        private int[,] RandomState(int rows, int columns, double probability)
        {
            int[,] universe = new int[rows, columns];
            Random rand = new Random();
            //Iterate through all coordinates
            for (int i = 0; i < settings.Rows; i++)
            {
                for (int j = 0; j < settings.Columns; j++)
                {
                    //Set the current cell to 0 or 1 (dead or alive) based on the probability threshold
                    universe[i, j] = (rand.NextDouble() <= probability) ? 1 : 0;
                }
            }
            return universe;
        }


        /// <summary>
        /// Run 1 generation. This method is public so it can be called from the main loop, 
        /// and int rather than void so that the steady-state period can be passed to Done when the loop ends
        /// </summary>
        /// <param name="gen_no">generation number</param>
        /// <returns>The steady-state period if a steady-state was just reached, or -1 if it wasn't</returns>
        public int RunGeneration(int gen_no)
        {
            UpdateBoard(gen_no);
            int period = SteadyStatePeriod();
            //If it's not a steady state, prepare for the next generation 
            if (period == -1) 
            { 
                UpdateHistory();
                Grow();
            }
            return period;
        }
        /// <summary>
        /// Finishes the simulation up
        /// </summary>
        /// <param name="steady_state">Steady-state period (-1 if the game ended without a steady state)</param>
        public void Done(int steady_state)
        {
            // Set complete marker as true
            grid.IsComplete = true;

            // Render updates to the console window (grid should now display COMPLETE)...
            grid.Render();

            // Wait for user to press the spacebar...
            stopwatch.Restart();
            WaitSpacebar(stopwatch);

            // Revert grid window size and buffer to normal
            grid.RevertWindow();
            //if the game was aborted by a steady state, display it
            if (steady_state != -1)
            {
                Logging.Message("Steady-state detected... " +
                    $"periodicity = {(steady_state == 0 ? "N/A" : (steady_state+1).ToString())}");
            }
            else
            {
                Logging.Message("Steady-state not detected...");
            }
            //Write to output file if there is an output file
            if (settings.OutputFile != null)
            {
                using StreamWriter writer = new StreamWriter(settings.OutputFile);
                writer.WriteLine("#version=2.0");
                //Iterate through the universe; write a cell structure for every alive cell
                for (int i = 0; i < settings.Rows; i++)
                {
                    for (int j = 0; j < settings.Columns; j++)
                    {
                        if (universe[i, j] == 1)
                        {
                            writer.WriteLine($"(o) cell: {i}, {j}");
                        }
                    }
                }
                Logging.Success($"Final generation written to file: {settings.OutputFile}");
            }

            Logging.Message("Press spacebar to close program...");
            WaitSpacebar(st);
        }

        /// <summary>
        /// Update the history variable to contain a copy of the current universe at the start
        /// And make sure it doesn't get longer than the memory number
        /// </summary>
        private void UpdateHistory()
        {
            history.Insert(0, (int[,])universe.Clone());
            if (history.Count > settings.Memory) history.RemoveAt(settings.Memory);
        }


        /// <summary>
        /// Looks through the history and determines whether there's a steady state
        /// </summary>
        /// <returns>The steady state period, or -1 if no steady state is present</returns>
        private int SteadyStatePeriod() => history.FindIndex(a => Extensions.ContentEquals(universe, a)); 
        
        /// <summary>
        /// Applies the rules of the Game of Life to advance the game by one generation
        /// </summary>
        private void Grow()
        {
            int[,] alive_neighbour_counts = new int[settings.Rows, settings.Columns];
            for (int i = 0; i < settings.Rows; i++)
            {
                for (int j = 0; j < settings.Columns; j++)
                {
                    alive_neighbour_counts[i, j] = settings.GetNeighbours(i, j, universe);
                }
            }
            for (int i = 0; i < settings.Rows; i++)
            {
                for (int j = 0; j < settings.Columns; j++)
                {
                    int alive_neighbours = alive_neighbour_counts[i, j];
                    //Turn alive cells that have an alive neighbour count outside the survival set dead
                    if ((universe[i, j] == 1) && !settings.Survival.Contains(alive_neighbours))
                    {
                        universe[i, j] = 0;
                    }
                    //Turn dead cells with an alive neighbour count in the birth set alive
                    else if ((universe[i, j] == 0) && settings.Birth.Contains(alive_neighbours))
                    {
                        universe[i, j] = 1;
                    }
                    //(All other cells stay the same)
                }
            }
        }
        /// <summary>
        /// Updates the board to match the current universe and generation number
        /// </summary>
        /// <param name="gen_no">Current generation number</param>
        private void UpdateBoard(int gen_no)
        {
            //Update the footnote
            grid.SetFootnote("Iteration:    " + gen_no.ToString());
            int[,] board_to_render = settings.GhostMode ? GhostFormattedUniverse() : universe;
            //Update all grid cells so that each 0 in universe is blank and each 1 is full
            for (int i = 0; i < settings.Rows; i++)
            {
                for (int j = 0; j < settings.Columns; j++)
                {
                    grid.UpdateCell(i, j, (CellState)board_to_render[i, j]);
                }
            }
            // Render updates to the console window
            grid.Render();
        }
        /// <summary>
        /// Format the universe for ghost mode
        /// </summary>
        /// <returns>An int[,] which is universe with the 3 previous generations overlaid as 2, 3, and 4</returns>
        public int[,] GhostFormattedUniverse()
        {
            int[,] output = (int[,])universe.Clone();
            for (int i=0; i<3 && i<history.Count; i++) //for each of the 3 most recent gens other than current universe
            {
                int[,] state = history[i];
                for (int j=0;j<settings.Rows;j++)
                {
                    for (int k = 0; k < settings.Columns; k++)
                    {
                        if (output[j,k]==0 && state[j,k] == 1) //make sure cell is not already 1 in a more recent gen
                        {
                            output[j,k] = i + 2; //history[0:2] display as CellState[2:4]
                        }

                    }
                } 
            }
            return output;
        }
    }

}

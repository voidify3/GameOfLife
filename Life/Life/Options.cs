using System;
using System.Collections.Generic;
using System.IO;

namespace Life
{
    /// <summary>
    /// A class describing the settings of a simulation.
    /// </summary>
    /// <author>CAB201 teaching team; extended and refactored by Sophia Walsh Long</author>
    /// <date>September-October 2020</date>
    public class Options
    {
        private const int MIN_DIMENSION = 4;
        private const int MAX_DIMENSION = 48;
        private const int MIN_GENERATION = 4;
        private const int MIN_MEMORY = 4;
        private const int MAX_MEMORY = 512;
        private const int MIN_NEIGHBOUR_ORDER = 1;
        private const int MAX_NEIGHBOUR_ORDER = 10;
        private const int MIN_SURVIVAL = 0;
        private const int MIN_BIRTH = 0;
        private const double MIN_UPDATE = 1.0;
        private const double MAX_UPDATE = 30.0;
        private const double MIN_RANDOM = 0.0;
        private const double MAX_RANDOM = 1.0;
        private static readonly List<int> DEFAULT_SURVIVAL = new List<int> { 2, 3 };
        private static readonly List<int> DEFAULT_BIRTH = new List<int> { 3 };

        private int rows = 16;
        private int columns = 16;
        private int generations = 50;
        private int memory = 16;
        private double updateRate = 5.0;
        private double randomFactor = 0.5;
        private string inputFile = null;
        private string outputFile = null;
        private Neighbourhood neighbourhood = new MooreNeighbourhood(1, false);
        private GameRule survival = new GameRule(DEFAULT_SURVIVAL);
        private GameRule birth = new GameRule(DEFAULT_BIRTH);

        /// <summary>
        /// Row count of the simulation universe
        /// (Must be in the range <see cref="MIN_DIMENSION"/>-<see cref="MAX_DIMENSION"/>)
        /// </summary>
        public int Rows
        {
            get => rows;
            set
            {
                ValidateRange("Row dimension", value, MIN_DIMENSION, MAX_DIMENSION);
                rows = value;
            }
        }

        /// <summary>
        /// Column count of the simulation universe
        /// (Must be in the range <see cref="MIN_DIMENSION"/>-<see cref="MAX_DIMENSION"/>)
        /// </summary>
        public int Columns
        {
            get => columns;
            set
            {
                ValidateRange("Column dimension", value, MIN_DIMENSION, MAX_DIMENSION);
                columns = value;
            }
        }

        /// <summary>
        /// Number of generations to simulate
        /// (Must be at least <see cref="MIN_GENERATION" />)
        /// </summary>
        public int Generations
        {
            get => generations;
            set
            {
                ValidateRange("Generation count", value, MIN_GENERATION);
                generations = value;
            }
        }

        /// <summary>
        /// Updates per second
        /// (Must be in the range <see cref="MIN_UPDATE"/>-<see cref="MAX_UPDATE"/>)
        /// </summary>
        public double UpdateRate
        {
            get => updateRate;
            set
            {
                ValidateRange("Update rate", value, MIN_UPDATE, MAX_UPDATE, value.ToString("F2"));
                updateRate = value;
            }
        }

        /// <summary>
        /// Random factor
        /// (Must be in the range <see cref="MIN_RANDOM"/>-<see cref="MAX_RANDOM"/>)
        /// </summary>
        public double RandomFactor
        {
            get => randomFactor;
            set
            {
                ValidateRange("Random factor", value, MIN_RANDOM, MAX_RANDOM, value.ToString("F2"));
                randomFactor = value;
            }
        }

        /// <summary>
        /// Memory depth
        /// (Must be in the range <see cref="MIN_MEMORY"/>-<see cref="MAX_MEMORY"/>)
        /// </summary>
        public int Memory
        {
            get => memory;
            set
            {
                ValidateRange("Generational memory", value, MIN_MEMORY, MAX_MEMORY);
                memory = value;
            }
        }

        /// <summary>
        /// Input file
        /// (must exist and have extension ".seed")
        /// </summary>
        public string InputFile
        {
            get => inputFile;
            set
            {
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File \'{value}\' does not exist.");
                }
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException("Seed file has incompatible file extension " +
                        $"\'{Path.GetExtension(value)}\'");
                }
                inputFile = value;
            }
        }

        /// <summary>
        /// Output file
        /// (must have extension ".seed")
        /// </summary>
        public string OutputFile
        {
            get => outputFile;
            set
            {
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException($"Output file has incompatible file extension " +
                        $"\'{Path.GetExtension(value)}\'");
                }
                outputFile = value;
            }
        }

        /// <summary>
        /// GameRule object describing the survival rules
        /// (all numbers passed to the constructor must be greater than <see cref="MIN_SURVIVAL"/>)
        /// </summary>
        public GameRule Survival
        {
            get => survival;
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    ValidateRange("Survival parameter", value[i], MIN_SURVIVAL);
                }
                survival = value;
            }
        }

        /// <summary>
        /// GameRule object describing the birth rules
        /// (all numbers passed to the constructor must be greater than <see cref="MIN_BIRTH"/>)
        /// </summary>
        public GameRule Birth 
        { 
            get => birth; 
            set
            {
                for (int i=0;i<value.Count;i++)
                {
                    ValidateRange("Birth parameter", value[i], MIN_BIRTH);
                }
                birth = value;
            }
        }

        /// <summary>
        /// Neighbourhood object describing the neighbourhood in use
        /// (order passed to the constructor must be in the range 
        /// <see cref="MIN_NEIGHBOUR_ORDER"/>-<see cref="MAX_NEIGHBOUR_ORDER"/>)
        /// </summary>
        public Neighbourhood Neighbourhood
        {
            get => neighbourhood;
            set
            {
                ValidateRange("Neighbourhood order", value.Order, MIN_NEIGHBOUR_ORDER, MAX_NEIGHBOUR_ORDER);
                neighbourhood = value;
            }
        }

        /// <summary>
        /// Whether the simulation is in periodic mode
        /// </summary>
        public bool Periodic { get; set; } = false;

        /// <summary>
        /// Whether the simulation is in step mode
        /// </summary>
        public bool StepMode { get; set; } = false;

        /// <summary>
        /// Whether the simulation is in ghost mode
        /// </summary>
        public bool GhostMode { get; set; } = false;

        /// <summary>
        /// Validate that a value is inside a specified range; throw an exception with a helpful message if it isn't
        /// </summary>
        /// <typeparam name="T">A comparable value type (NB: the code is only sane for numeric types)</typeparam>
        /// <param name="name">Option name for error message</param>
        /// <param name="value">The value being checked</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value (optional)</param>
        /// <param name="stringval">String version of value (optional; value.ToString() used if unspecified)</param>
        private void ValidateRange<T>(string name, T value, T minimum, T? maximum= null, string stringval = null) 
            where T : struct, IComparable //double and int will be the only types used
        {
            if (value.CompareTo(minimum) < 0 || (maximum != null && value.CompareTo(maximum)>0))
            {
                throw new ArgumentException($"{name} \'{stringval??value.ToString()}\' is outside of the acceptable " +
                        $"range of values ({minimum} {(maximum != null ? "- " + maximum.ToString() : "and up")})");
            }
        }

        /// <summary>
        /// Render the Options object as a string listing all its properties.
        /// This string will be printed after options processing is complete
        /// </summary>
        /// <returns>A string listing all the runtime settings</returns>
        public override string ToString()
        {
            string output = "\n";
            output += LineOfDisplay("Input File: ", InputFile ?? "N/A");
            output += LineOfDisplay("Output File: ", OutputFile ?? "N/A");
            output += LineOfDisplay("Generations: ", $"{Generations}");
            output += LineOfDisplay("Memory: ", $"{Memory}");
            output += LineOfDisplay("Update Rate: ", $"{UpdateRate} updates/s");
            output += LineOfDisplay("Game Rules: ", $"S {Survival} B {Birth}");
            output += LineOfDisplay("Neighbourhood: ", $"{Neighbourhood}");
            output += LineOfDisplay("Periodic: ", BoolToYesNo(Periodic));
            output += LineOfDisplay("Rows: ", $"{Rows}");
            output += LineOfDisplay("Columns: ", $"{Columns}");
            output += LineOfDisplay("Random Factor: ", $"{RandomFactor:P}");
            output += LineOfDisplay("Step Mode: ", BoolToYesNo(StepMode));
            output += LineOfDisplay("Ghost Mode: ", BoolToYesNo(GhostMode));
            return output;
        }

        /// <summary>
        /// Generate a line of the ToString string
        /// </summary>
        /// <param name="param_name">Option name</param>
        /// <param name="value_expression">Value to display for said option</param>
        /// <returns></returns>
        private string LineOfDisplay(string param_name, string value_expression)
        {
            const int padding = 30;
            return $"{param_name,padding}{value_expression}\n";
        }

        /// <summary>
        /// Convert a bool to the string "Yes" or "No"
        /// </summary>
        /// <param name="input">A boolean value</param>
        /// <returns>"Yes" for true, "No" for false</returns>
        private string BoolToYesNo(bool input) => input ? "Yes" : "No";

        /// <summary>
        /// Validate the combined values of the Options object.
        /// Display warning messages and reset the options in question if
        /// the neighbourhood order is too big for the specified universe size,
        /// or if any numbers in the Survival or Birth sets exceed the neighbour count per cell
        /// </summary>
        public void ValidateCombinedValues()
        {
            //If the neighbourhood is too big for the board, give a warning and revert
            if (Rows <= (Neighbourhood.Order * 2) || Columns <= (Neighbourhood.Order * 2))
            {
                Logging.Warning($"Neighbourhood of order {Neighbourhood.Order} too large for {Rows}x{Columns} board." +
                    " Reverting to default neighbourhood...");
                Neighbourhood = new MooreNeighbourhood(1, false);
            }
            //If any numbers in Survival or Birth are too big for the neighbourhood, give a warning and revert
            int neighbourhood_size = Neighbourhood.NeighbourhoodSize;
            try
            {
                Survival.ValidateAgainstMaximum(neighbourhood_size);
            }
            catch (ArgumentException e)
            {
                Logging.Warning($"Number '{e.Message}' in survival set {Survival} exceeds the total " +
                    $"number of neighbours per cell ({neighbourhood_size}). Reverting to default survival setting...");
                Survival = new GameRule(DEFAULT_SURVIVAL);
            }

            try
            {
                Birth.ValidateAgainstMaximum(neighbourhood_size);
            }
            catch (ArgumentException e)
            {
                Logging.Warning($"Number '{e.Message}' in birth set {Birth} exceeds the total " +
                    $"number of neighbours per cell ({neighbourhood_size}). Reverting to default birth setting...");
                Birth = new GameRule(DEFAULT_BIRTH);
            }

        }
        /// <summary>
        /// Modulus function robust to negative values. Developed from Part 1 solution
        /// </summary>
        /// <param name="x">Numerator of the modulus</param>
        /// <param name="m">Denominator of the modulus</param>
        /// <returns>x%m, wrapped around to work with negative numbers</returns>
        private static int Modulus(int x, int m)
        {
            return (x % m + m) % m;
        }
        /// <summary>
        /// Get the alive neighbour count of universe[x,y].
        /// Declared in this class in order to more easily use Neighbourhood and Periodic
        /// </summary>
        /// <param name="x">Row index of the cell whose neighbours are being checked</param>
        /// <param name="y">Column index of the cell</param>
        /// <param name="universe">Game universe</param>
        /// <returns>The number of alive neighbours the cell has</returns>
        public int GetNeighbours(int x, int y, int[,] universe)
        {
            int alive_neighbour_count = 0;
            if (!Periodic)
            {
                for (int i = x - Neighbourhood.Order; i <= x + Neighbourhood.Order; i++)
                {
                    for (int j = y - Neighbourhood.Order; j <= y + Neighbourhood.Order; j++)
                    {
                        //Check that the cell is inbounds as well as that it's a neighbour of [x,y]
                        if (Neighbourhood.IsNeighbour(x, y, i, j) 
                            && i >= 0 && i < Rows
                            && j >= 0 && j < Columns)
                        {
                            //This line works based on the fact that an alive cell is 1 and a dead one is 0
                            //So it's equivalent to "if (universe[i,j] == 1) alive_neighbour_count++;"
                            alive_neighbour_count += universe[i, j];
                        }
                    }
                }
            }
            else
            {
                for (int i = x - Neighbourhood.Order; i <= x + Neighbourhood.Order; i++)
                {
                    for (int j = y - Neighbourhood.Order; j <= y + Neighbourhood.Order; j++)
                    {
                        if (Neighbourhood.IsNeighbour(x, y, i, j))
                        {
                            alive_neighbour_count += universe[Modulus(i, Rows), Modulus(j, Columns)];
                        }
                    }
                }
            }
            return alive_neighbour_count;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Life
{
    /// <summary>
    /// A process to parse the command-line arguments into an Options object with error handling
    /// </summary>
    /// <author>Part 1 arg processing and warning message format by CAB201 teaching team;
    /// extended for Part 2 and refactored to improve code quality by Sophia Walsh Long</author>
    /// <date>Part 1 processing written September 2020; 
    /// extension and refactoring done September-October 2020</date>
    static class ArgumentProcessor
    {
        /// <summary>
        /// Parse the string array of command line arguments into an Options object,
        /// notify the user of the success of the parsing,
        /// then print the properties of the Options object to the command line
        /// </summary>
        /// <param name="args">Command line arguments from Main</param>
        /// <returns>An Options object reflecting all args up to the first error</returns>
        public static Options Process(string[] args)
        {
            Options options = new Options();
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--dimensions":
                            int[] vals = ProcessIntOption(args, i, "dimensions", 
                                new string[] { "Row dimension", "Column dimension" });
                            options.Rows = vals[0];
                            options.Columns = vals[1];
                            break;
                        case "--generations":
                            options.Generations = ProcessIntOption(args, i, "generations", "Generation count");
                            break;
                        case "--max-update":
                            options.UpdateRate = ProcessDoubleOption(args, i, "max-update", "Update rate");
                            break;
                        case "--random":
                            options.RandomFactor = ProcessDoubleOption(args, i, "random", "Random factor");
                            break;
                        case "--seed":
                            options.InputFile = ProcessStringOption(args, i, "seed");
                            break;
                        case "--memory":
                            options.Memory = ProcessIntOption(args, i, "memory", "Generational memory depth");
                            break;
                        case "--output":
                            options.OutputFile = ProcessStringOption(args, i, "output");
                            break;
                        case "--survival":
                            options.Survival = ProcessGameRule(args, i, "survival");
                            break;
                        case "--birth":
                            options.Birth = ProcessGameRule(args, i, "birth");
                            break;
                        case "--neighbour":
                            options.Neighbourhood = ProcessNeighbourhood(args, i);
                            break;
                        case "--ghost":
                            options.GhostMode = true;
                            break;
                        case "--periodic":
                            options.Periodic = true;
                            break;
                        case "--step":
                            options.StepMode = true;
                            break;
                    }
                }
                Logging.Success("Command line arguments processed.");
            }
            catch (ArgumentException exception)
            //All validation exceptions are ArgumentExceptions so this is any argument from a throw statement
            {
                Logging.Warning(exception.Message);
                Logging.Message("Reverting to defaults for unprocessed arguments...");
            }
            catch (Exception exception)
            //if any other type of exception, i.e. an exception I didn't throw, comes up 
            //(this should never happen bc I've tested comprehensively, but if it does I want it to be distinct from 
            //my validation exceptions, since recovery isn't guaranteed for errors caused by the presence of bugs)
            {
                Logging.Error(exception.Message);
                Logging.Message("Attempting to revert to defaults for unprocessed arguments...");
            }
            finally
            {
                options.ValidateCombinedValues();
                Logging.Message("The following options will be used:");
                Console.WriteLine(options);
            }
            return options;
        }
        /// <summary>
        /// Parse an integer parameter to an output variable; throw an argument exception if it's invalid
        /// </summary>
        /// <param name="value">Value to be validated</param>
        /// <param name="name">Option name for error message</param>
        /// <param name="output">When this function returns, contains the value of the parameter in int format</param>
        private static void ParseParam(string value, string name, out int output)
        {
            if (!int.TryParse(value, out output))
            {
                throw new ArgumentException($"{name} \'{value}\' is not a valid integer.");
            }
        }
        /// <summary>
        /// Parse a double parameter to an output variable; throw an argument exception if it's invalid
        /// </summary>
        /// <param name="value">Value to be validated</param>
        /// <param name="name">Parameter name for error message</param>
        /// <param name="output">When this function returns, contains the value of the param in double format</param>
        private static void ParseParam(string value, string name, out double output)
        {
            if (!double.TryParse(value, out output))
            {
                throw new ArgumentException($"{name} \'{value}\' is not a valid double.");
            }
        }
        /// <summary>
        /// Process an option with a single argument which should remain a string
        /// Throw an exception if the argument was not found
        /// Return it if it was found
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="name">Option name for error message</param>
        /// <returns>The parameter of the option</returns>
        private static string ProcessStringOption(string[] args, int i, string name)
        {
            ValidateParameterCount(args, i, name, 1);
            return args[i + 1];
        }
        /// <summary>
        /// Process an option with a single int argument
        /// Throw exceptions if the argument was not found or is invalid
        /// Return the value converted to int if parsing completed successfully
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="shortname">Option name for "missing" error message</param>
        /// <param name="longname">Option name for "invalid" error message</param>
        /// <returns>The parameter value in int format</returns>
        private static int ProcessIntOption(string[] args, int i, string shortname, string longname)
        {
            ValidateParameterCount(args, i, shortname, 1);
            ParseParam(args[i + 1], longname, out int value);
            return value;
        }
        /// <summary>
        /// Process an option with more than one integer argument
        /// Throw exceptions if not enough arguments were found or any are invalid
        /// Return an array of the values converted to ints if parsing completed successfully
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="shortname">Option name for "missing" error message</param>
        /// <param name="longnames">Array of names for "invalid" error messages</param>
        /// <returns>An array of the parameter values in int format</returns>
        private static int[] ProcessIntOption(string[] args, int i, string shortname, string[] longnames)
        {
            int param_count = longnames.Length;
            int[] values = new int[param_count];
            ValidateParameterCount(args, i, shortname, param_count);
            for (int j=0; j<param_count;j++)
            {
                ParseParam(args[i + j + 1], longnames[j], out values[j]);
            }
            
            return values;
        }
        /// <summary>
        /// Process an option with a single double argument
        /// Throw exceptions if the argument was not found or is invalid
        /// Return the value converted to int if parsing completed successfully
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="shortname">Option name for "missing" error message</param>
        /// <param name="longname">Option name for "invalid" error message</param>
        /// <returns>The parameter value in double format</returns>
        private static double ProcessDoubleOption(string[] args, int i, string shortname, string longname)
        {
            ValidateParameterCount(args, i, shortname, 1);
            ParseParam(args[i + 1], longname, out double value);
            return value;
        }

        /// <summary>
        /// Process a GameRule option (Survival or Birth)
        /// Throw exceptions if not enough arguments were found or any are invalid
        /// Return a GameRule object if parsing completed successfully
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="name">Option name for error messages</param>
        /// <returns>A GameRule object matching the specifications</returns>
        private static GameRule ProcessGameRule(string[] args, int i, string name)
        {
            string[] parameters = GetIndefiniteParameters(args, i, name, 1);
            try
            {
                return new GameRule(parameters);
            }
            //The GameRule constructor throws an ArgumentException if non-ints are specified
            //That exception includes the value that triggered the exception as Message
            //So here it can be interpolated into the correct format and rethrown
            catch (ArgumentException e)
            {
                throw new ArgumentException($"An error occurred parsing parameter '{e.Message}' in {name} set. " +
                            "Please ensure that all parameters are either valid integers or valid integer ranges.");
            }
            
        }
        /// <summary>
        /// Process the neighbourhood option
        /// (The neighbourhood is a special case so I included some constant strings)
        /// Throw exceptions if not enough arguments were found or any are invalid
        /// Return a Neighbourhood object if parsing completed successfully
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <returns>A Neighbourhood object as specified by the parameters</returns>
        private static Neighbourhood ProcessNeighbourhood(string[] args, int i)
        {
            ValidateParameterCount(args, i, "neighbour", 3);
            string neighbour_type = args[i + 1].ToLower();

            ParseParam(args[i + 2], "Neighbourhood order", out int order);

            //Throw an exception if centre-count can't be parsed to bool 
            //(if it's neither "true" or "false", case-insensitive)
            if (!bool.TryParse(args[i + 3], out bool centre_count))
            {
                throw new ArgumentException($"Centre-count value '{args[i + 3]}' is not a valid boolean value.");
            }

            //Return a Neighbourhood object, basing the child class on the specified type
            //If the specified type is neither of the two options, throw an exceptions
            return neighbour_type switch
            {
                "moore" => new MooreNeighbourhood(order, centre_count),
                "vonneumann" => new VonNeumannNeighbourhood(order, centre_count),
                _ => throw new ArgumentException($"Neighbourhood type '{args[i + 1]}' is invalid. "
                + "Must be either 'moore' or 'vonNeumann' (case insensitive).") //this is the default case
            };
        }
        /// <summary>
        /// Validate that there are at least numParameters arguments between args[i] and the end of the array
        /// (throw an exception if this is not true)
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="option">Option name for error message</param>
        /// <param name="numParameters">Number of parameters needed for the option at args[i]</param>
        private static void ValidateParameterCount(string[] args, int i, string option, int numParameters)
        {
            if (i >= args.Length - numParameters)
            {
                throw new ArgumentException($"Insufficient parameters for \'--{option}\' option " +
                    $"(provided {args.Length - i - 1}, expected {numParameters})");
            }
        }
        /// <summary>
        /// For an option at args[i] which can take an arbitrary number of parameters,
        /// check that there are at least minParameters arguments after it
        /// (throwing an exception if this isn't the case).
        /// Then return an array of all the arguments which seem to be parameters for args[i]
        /// </summary>
        /// <param name="args">Argument array</param>
        /// <param name="i">Option index</param>
        /// <param name="option">Option name for error message</param>
        /// <param name="minParameters">Minimum number of parameters needed for the option</param>
        /// <returns>An array of the parameters for args[i]</returns>
        private static string[] GetIndefiniteParameters(string[] args, int i, string option, int minParameters)
        {
            List<string> output = new List<string>();
            //Iterate over all the arguments after args[i]; abort the loop if something option-formatted is reached
            for (int j = i + 1; j < args.Length && !args[j].StartsWith("--"); j++)
            {
                output.Add(args[j]);
            }
            if (output.Count < minParameters)
            {
                throw new ArgumentException($"Insufficient parameters for \'--{option}\' option " +
                    $"(provided {output.Count}, expected at least {minParameters})");
            }
            return output.ToArray();
        }

    }
}

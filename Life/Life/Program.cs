using System;
using System.Diagnostics;

namespace Life
{
    /// <summary>
    /// A static class to contain a generic extension method used in Simulation
    /// (Method taken from StackOverflow, modified insubstantially by me)
    /// </summary>
    /// <author>StackOverflow user FlyingStreudel</author>
    /// <date>March 2013</date>
    static class Extensions
    {
        /// <summary>
        /// Determine whether the contents of two 2d arrays of IComparable member types are equal
        /// Code from https://stackoverflow.com/a/15414914, modified insubstantially to comply with style guide
        /// Used for the purpose of detecting steady states
        /// </summary>
        /// <typeparam name="T">Any type with CompareTo</typeparam>
        /// <param name="arr">A 2d array with T type members</param>
        /// <param name="other">Another 2d array of the same type</param>
        /// <returns>True if the arrays are equal, false if they are not</returns>
        public static bool ContentEquals<T>(this T[,] arr, T[,] other) where T : IComparable
        {
            if (arr.GetLength(0) != other.GetLength(0) || arr.GetLength(1) != other.GetLength(1))
            {
                return false;
            }
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j].CompareTo(other[i, j]) != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
    /// <summary>
    /// The main program class
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>Aug-Oct 2020</date>
    class Program
    {
        /// <summary>
        /// Main program. Initialises the simulation based on the command line arguments, then
        /// runs the simulation using a loop until the maximum generation is reached or a steady-state occurs
        /// </summary>
        /// <param name="args">Command line arguments split on spaces</param>
        static void Main(string[] args)
        {
            //Process the settings
            Options options = ArgumentProcessor.Process(args);
            //Initialise the simulation and stopwatch
            Simulation sim = new Simulation(options);
            Stopwatch watch = new Stopwatch();
            //Declare a variable to keep track of steady states
            int steady_state = -1;
            // For each generation
            for (int i = 0; i <= options.Generations; i++)
            {
                watch.Restart();
                //Run a generation and assign the return value to steady_state
                steady_state = sim.RunGeneration(i);
                //If a steady state has been detected, abort the simulation immediately
                if (steady_state != -1) 
                {
                    break;
                }
                //if step is true, wait for the user to press the spacebar before progressing the loop
                if (options.StepMode)
                {
                    Simulation.WaitSpacebar(watch);
                }
                //otherwise, wait for the stopwatch to time out before progressing
                else
                {
                    while (watch.ElapsedMilliseconds < 1000 / options.UpdateRate) ;
                }
            }
            //Conclude the simulation; if it was ended from a steady state the relevant displays will happen
            sim.Done(steady_state);
        }
    }
}

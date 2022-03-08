using System;
using System.Collections.Generic;
using System.Linq;

namespace Life
{
    public class GameRule
    {
        private readonly List<int> numbers; 

        /// <summary>
        /// Construct a GameRule from a list of ints without going through the validation process
        /// (this constructor is used to apply the default values, since there's no need to validate them)
        /// </summary>
        /// <param name="contents">A list of ints that is known to be sorted in ascending order 
        /// and completely made up of valid non-negative integers</param>
        public GameRule(List<int> contents)
        {
            numbers = new List<int>(contents);
        }
        /// <summary>
        /// Construct a GameRule from a string array (parsed from the command line arguments)
        /// </summary>
        /// <param name="arguments">The space-split arguments from the command line</param>
        public GameRule(string[] arguments)
        {
            numbers = ValidateArguments(arguments);
        }

        /// <summary>
        /// Iterate over the arguments, parsing them into a list of ints to assign to numbers
        /// If a TryParse fails, throw an exception with the offending value for ArgumentProcessor to handle
        /// Don't test for non-negative validation here because Options will do that
        /// </summary>
        /// <param name="name">Name of the GameRule ("survival" or "birth") for error messages</param>
        /// <param name="arguments">The space-split arguments from the command line</param>
        /// <returns>A sorted list of ints to assign to numbers</returns>
        private List<int> ValidateArguments(string[] arguments)
        {
            List<int> nums = new List<int>();
            foreach (string argument in arguments)
            {
                //If the argument is an int, add it to the list
                if (int.TryParse(argument, out int intarg))
                {
                    nums.Add(intarg);
                }
                else
                {
                    //Try to treat it as a range
                    if (Range.TryParse(argument, out Range range)) 
                    { 
                        for (int i = range.begin; i <= range.end; i++)
                        {
                            nums.Add(i);
                        }
                    }
                    //If it doesn't parse as a range, there's a problem. 
                    //Pass the offending value up the calling chain so ArgumentProcessor can handle it properly
                    else
                    {
                        throw new ArgumentException(argument);
                    }
                }
            }
            //Remove any duplicates just in case and sort the list
            nums = nums.Distinct().ToList();
            nums.Sort();
            return nums;
        }


        //Three definitions to allow indirect access to Numbers for iteration and element-checking
        
        /// <summary>
        /// Determines whether a number is specified in the GameRule
        /// </summary>
        /// <param name="element">the element being checked for</param>
        /// <returns>Whether the element is contained the GameRule's number list</returns>
        public bool Contains(int element)
        {
            return numbers.Contains(element);
        }

        /// <summary>
        /// Accesses the count of the GameRule's number list (get only).
        /// This exists so that GameRule can be iterated over
        /// </summary>
        public int Count
        {
            get => numbers.Count;
        }

        /// <summary>
        /// Accesses an element of the GameRule by index (get only).
        /// This exists so that GameRule can be iterated over 
        /// </summary>
        /// <param name="index">an index, assumed to be in range</param>
        /// <returns>The requested element of the GameRule's number list</returns>
        public int this[int index]
        {
            get => numbers[index];
        }

        /// <summary>
        /// Validate that all numbers in the GameRule do not exceed a specified value.
        /// If this is false, throw an ArgumentException with the first offending value.
        /// </summary>
        /// <param name="size">The maximum value</param>
        public void ValidateAgainstMaximum(int size)
        {
            foreach (int num in numbers)
            {
                if (num > size)
                {
                    throw new ArgumentException(num.ToString());
                }
            }
        }

        /// <summary>
        /// Using the Range class to detect ranges, render a string of the gamerule number list
        /// </summary>
        /// <returns>A string, formatted such that numbers {1 3 4 5 7} makes "( 1 3...5 7 )"</returns>
        public override string ToString()
        {
            //Create a list of Ranges describing the ranges that make up the numbers list
            List<Range> ranges = Range.ConvertToListRange(numbers);
            //Generate the string by converting each range to a string
            string output = "( ";
            foreach (Range range in ranges)
            {
                output += $"{range} ";
            }
            output += ")";
            return output;

        }
    }

    /// <summary>
    /// A struct decribing a range of integers by its start and end points.
    /// Based on code from https://stackoverflow.com/a/120349 but heavily modified, and the last two methods are mine
    /// </summary>
    /// <author>Sophia Walsh Long (some code from StackOverflow user VVS)</author>
    /// <date>October 2020 (some code from September 2010)</date>
    struct Range
    {
        public int begin;
        public int end;

        /// <summary>
        /// Construct a range with known endpoints
        /// </summary>
        /// <param name="begin">Begin number</param>
        /// <param name="end">End number</param>
        public Range(int begin, int end)
        {
            this.begin = begin;
            this.end = end;
        }
        
        /// <summary>
        /// Format the range as a string
        /// </summary>
        /// <returns>A string of the format "Begin...End"</returns>
        public override string ToString()
        {
            if (begin == end)
            {
                return begin.ToString();
            }
            else
            {
                return $"{begin}...{end}";
            }
        }
        /// <summary>
        /// Try to parse a string into a range. The string should be formatted "x...y"
        /// where x and y are both integers and x <= y
        /// </summary>
        /// <param name="input">Input string, expected in the same format that ToString() returns</param>
        /// <param name="output">When this function returns, will contain a Range object of the string</param>
        /// <returns>True if successfully parsed; false if not</returns>
        public static bool TryParse(string input, out Range output)
        {
            output = new Range();
            string[] ends = input.Split("...");
            //if this array is of length 2, both elements are valid integers, 
            //and the start num does not exceed the end num:
            if (ends.Length == 2 
                && int.TryParse(ends[0], out int begin) 
                && int.TryParse(ends[1], out int end) 
                && begin <= end)
            {
                //Assign the values to the output and indicate that the conversion was successful
                output.begin = begin;
                output.end = end;
                return true;
            }
            //Otherwise, it's an invalid range, so the conversion is unsuccessful
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Convert a list of ints (assumed to be sorted in ascending order) into a list of ranges
        /// </summary>
        /// <param name="numbers">The list of ints to convert</param>
        /// <returns>A list of Ranges describing the original list in terms of the ranges that make it up</returns>
        public static List<Range> ConvertToListRange(List<int> numbers)
        {
            //The numbers are assumed to already be sorted
            List<Range> ranges = new List<Range>();
            int prev_num = numbers.First();
            Range current_range = new Range(prev_num, prev_num);
            foreach (int num in numbers.Skip(1))
            {
                //if the difference between num and prev_num is more than 1, they're not in the same range
                if (prev_num < num - 1)
                {
                    //Current_range is therefore complete, so add it to ranges and begin a new range
                    ranges.Add(current_range);
                    current_range = new Range { begin = num };
                }
                //update the last known value of current_range; update prev_num
                current_range.end = num;
                prev_num = num;
            }
            //After the loop finishes, add the range containing the final number
            ranges.Add(current_range);
            return ranges;
        }
    }
}

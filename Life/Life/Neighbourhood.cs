using System;

namespace Life
{
    /// <summary>
    /// A base class dictating the general behaviour of neighbourhoods
    /// Such that the two child classes' Distance functions cause it to behave correctly for the two types
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    public abstract class Neighbourhood
    {
        protected bool centre_count;
        
        protected string type = null; //Used solely so that the child class can be more easily reflected in ToString

        /// <summary>
        /// Construct a new Neighbourhood object
        /// </summary>
        /// <param name="order">The order of the neighbourhood</param>
        /// <param name="centre_count">Whether a cell counts as its own neighbour</param>
        public Neighbourhood(int order, bool centre_count)
        {
            Order = order;
            this.centre_count = centre_count;
        }

        /// <summary>
        /// The order of the neighbourhood (made a property because it needs to be public)
        /// </summary>
        public int Order { get; set; }


        /// <summary>
        /// Check if Cell 2 is a neighbour of Cell 1
        /// </summary>
        /// <param name="row1">Row of cell 1</param>
        /// <param name="col1">Column of cell 1</param>
        /// <param name="row2">Row of cell 2</param>
        /// <param name="col2">column of cell 2</param>
        /// <returns>Whether cell 2 is a neighbour of cell 1</returns>
        public bool IsNeighbour(int row1, int col1, int row2, int col2)
        {
            //if it's the centre, return true if the centre counts
            if (row1 == row2 && col1 == col2)
            {
                return centre_count;
            }
            //Otherwise return true only if the distance between the cells is not greater than the order
            return Distance(row1, col1, row2, col2) <= Order;
        } 

        /// <summary>
        /// The total number of neighbours any given cell can have, based on the properties of the
        /// Neighbourhood object being queried
        /// </summary>
        public int NeighbourhoodSize
        {
            get
            {
                int neighbour_count = 0;
                //Iterate over every cell in the bounding box of the neighbourhood
                for (int i = -Order; i <= Order; i++)
                {
                    for (int j = -Order; j <= Order; j++)
                    {
                        if (IsNeighbour(0, 0, i, j)) { neighbour_count++; }
                    }
                }
                return neighbour_count;
            }
        }

        
        /// <summary>
        /// Abstract distance function to be overridden by the two child classes
        /// </summary>
        /// <param name="row1">Cell 1 row</param>
        /// <param name="col1">Cell 1 column</param>
        /// <param name="row2">Cell 2 row</param>
        /// <param name="col2">Cell 2 column</param>
        /// <returns>The distance between Cell 1 and Cell 2 given the child class</returns>
        public abstract int Distance(int row1, int col1, int row2, int col2);

        /// <summary>
        /// Override of ToString, solely for the printing of command line options
        /// </summary>
        /// <returns>A string describing the type and order of the neighbourhood</returns>
        public override string ToString()
        {
            return $"{type} ({Order})";
        }
    }

    /// <summary>
    /// Child class for Moore neighbourhoods 
    /// (where distance is equal to the greater of the two cardinal direction distances)
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    public class MooreNeighbourhood : Neighbourhood
    {
        /// <summary>
        /// Construct a new Moore neighbourhood
        /// </summary>
        /// <param name="order">Neighbourhood order</param>
        /// <param name="centre_count">Whether the centre counts</param>
        public MooreNeighbourhood(int order, bool centre_count) : base(order, centre_count) {
            type = "Moore";
        }

        /// <summary>
        /// Distance function for Moore neighbourhoods
        /// </summary>
        /// <param name="row1">Cell 1 row</param>
        /// <param name="col1">Cell 1 column</param>
        /// <param name="row2">Cell 2 row</param>
        /// <param name="col2">Cell 2 column</param>
        /// <returns>The distance between the two cells
        /// (for this type, equal to whichever of the two dimensional distances is larger)</returns>
        public override int Distance(int row1, int col1, int row2, int col2)
        {
            int height = Math.Abs(row2 - row1);
            int width = Math.Abs(col2 - col1);
            //return the larger of the two dimensions
            return (height > width) ? height : width;
        }
        
    }
    /// <summary>
    /// Child class for Von Neumann neighbourhoods 
    /// (where distance is equal to the sum of the two cardinal direction distances)
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    public class VonNeumannNeighbourhood : Neighbourhood
    {
        /// <summary>
        /// Construct a new Von Neumann neighbourhood
        /// </summary>
        /// <param name="order">Neighbourhood order</param>
        /// <param name="centre_count">Whether the centre counts</param>
        public VonNeumannNeighbourhood(int order, bool centre_count) : base(order, centre_count)
        {
            type = "VonNeumann";
        }

        /// <summary>
        /// Distance function for Von Neumann neighbourhoods
        /// </summary>
        /// <param name="row1">Cell 1 row</param>
        /// <param name="col1">Cell 1 column</param>
        /// <param name="row2">Cell 2 row</param>
        /// <param name="col2">Cell 2 column</param>
        /// <returns>The distance between the two cells
        /// (for this type, equal to wthe sum of the two dimensional distances)</returns>
        public override int Distance(int row1, int col1, int row2, int col2)
        {
            int height = Math.Abs(row2 - row1);
            int width = Math.Abs(col2 - col1);
            return height + width;
        }
        
    }
}

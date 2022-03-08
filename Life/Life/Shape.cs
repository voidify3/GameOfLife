namespace Life
{
    /// <summary>
    /// An abstract class representing a shape to be drawn when parsing a V2 seed
    /// Shapes have a value (0 or 1), a row index (bottom) and a column index (left); all protected for inheritance
    /// The index fields are named what they are for the sake of clarity in Rectangles and Ellipses
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    abstract class Shape
    {
        protected int value;
        protected int bottom;
        protected int left;

        /// <summary>
        /// Construct a new Shape with the specified properties
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the shape</param>
        /// <param name="row">The row index</param>
        /// <param name="column">The column index</param>
        public Shape(int value, int row, int column)
        {
            this.value = value;
            this.bottom = row;
            this.left = column;
        }


        /// <summary>
        /// A static method to parse an array of coordinates for the construction of a shape
        /// out of an array of strings split from a line of a v2 seed.
        /// It is assumed that elements[2] contains the first coordinate, and
        /// that all elements between it and elements[length+1], inclusive, are valid non-negative integers.
        /// </summary>
        /// <param name="length">The number of coordinates to parse (2 for Cell, 4 for Rectangle and Ellipse)</param>
        /// <param name="elements">The array from which to parse the coordinates</param>
        /// <returns>An int array of the specified length, ready to be used for constructing a shape</returns>
        public static int[] ParseCoordinates(int length, string[] elements)
        {
            int[] output = new int[length];
            for (int i = 0; i < length; i++)
            {
                //Parse the number into the correct index
                output[i] = int.Parse(elements[i + 2]);
            }
            return output;
        }

        /// <summary>
        /// Draws the shape onto the universe
        /// </summary>
        /// <param name="universe">The current state of the game universe</param>
        /// <returns>The state of the universe after drawing the shape</returns>
        public abstract int[,] Draw(int[,] universe);
    }

    /// <summary>
    /// A class representing a single-cell "shape". 
    /// A cell does not have any additional properties beyond that of the base shape.
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    class Cell : Shape
    {
        /// <summary>
        /// Construct a new cell using individual parameters for the indices.
        /// (Uses the base constructor for Shape)
        /// This version of the constructor is deprecated but remains for maintainability
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the cell</param>
        /// <param name="row">The row index</param>
        /// <param name="column">The column index</param>
        public Cell(int value, int row, int column) : base(value, row, column) { }

        /// <summary>
        /// Construct a new cell using an array of length 2 for the indices.
        /// This version of the constructor has replaced the other one 
        /// because the return value of ParseCoordinates can be passed directly to it.
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the cell</param>
        /// <param name="coords">An array of length 2 containing the row and column indices in that order</param>
        public Cell(int value, int[] coords) : base(value, coords[0], coords[1]) { }

        /// <summary>
        /// Override method to draw a cell on the universe. It takes the current universe state as a parameter, 
        /// draws the cell, then returns the new universe including the cell.
        /// </summary>
        /// <param name="universe">Current universe</param>
        /// <returns>The universe with the cell in it</returns>
        public override int[,] Draw(int[,] universe)
        {
            int[,] new_universe = universe;
            new_universe[bottom, left] = value;
            return new_universe;
        }
    }
    /// <summary>
    /// Class for a rectangle. A rectangle, in addition to the base class attributes,
    /// has a second pair of indices: the top row and right column.
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    class Rectangle : Shape
    {
        protected int top;
        protected int right;

        /// <summary>
        /// Construct a new Rectangle, using separate parameters for the four coordinates.
        /// This version of the constructor is deprecated but remains for maintainability
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the rectangle</param>
        /// <param name="bottom">Bottom row index</param>
        /// <param name="left">Left column index</param>
        /// <param name="top">Top row index</param>
        /// <param name="right">Right column index</param>
        public Rectangle(int value, int bottom, int left, int top, int right) : base(value, bottom, left)
        {
            this.top = top;
            this.right = right;
        }

        /// <summary>
        /// Construct a new Rectangle using an array of length 4 for the indices.
        /// This version of the constructor has replaced the other one 
        /// because the return value of ParseCoordinates can be passed directly to it.
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the rectangle</param>
        /// <param name="coords">An int array of length 4 containing the 
        /// top, left, bottom, and right indices in that order</param>
        public Rectangle(int value, int[] coords) : base(value, coords[0], coords[1])
        {
            top = coords[2];
            right = coords[3];
        }

        /// <summary>
        /// Override method to draw a rectangle on the universe. It takes the current universe state as a parameter, 
        /// draws the rectangle, then returns the new universe including the rectangle.
        /// </summary>
        /// <param name="universe">Current universe</param>
        /// <returns>The universe with the rectangle in it</returns>
        public override int[,] Draw(int[,] universe)
        {
            int[,] new_universe = universe;
            for (int i=bottom; i<= top; i++)
            {
                for (int j=left; j<=right; j++)
                {
                    if (PointInsideShape(i, j)) { new_universe[i, j] = value; }
                }
            }
            return new_universe;
        }

        /// <summary>
        /// Check whether a given point is inside the shape
        /// </summary>
        /// <param name="r">Row index of the point</param>
        /// <param name="c">Column index of the point</param>
        /// <returns>Whether the point being checked is inside the shape 
        /// (always true for a rectangle since only the points inside the bounding box are checked)</returns>
        public virtual bool PointInsideShape(int r, int c) => true;
    }
    /// <summary>
    /// Class for an Ellipse. Ellipses inherit all five fields and Draw from Rectangle.
    /// The difference between a Rectangle and an Ellipse lies in PointInsideShape.
    /// </summary>
    /// <author>Sophia Walsh Long</author>
    /// <date>October 2020</date>
    class Ellipse : Rectangle
    {
        /// <summary>
        /// Construct a new Ellipse, using separate parameters for the four coordinates.
        /// This version of the constructor is deprecated but remains for maintainability
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the ellipse</param>
        /// <param name="bottom">Bottom row index</param>
        /// <param name="left">Left column index</param>
        /// <param name="top">Top row index</param>
        /// <param name="right">Right column index</param>
        public Ellipse(int value, int bottom, int left, int top, int right) 
            : base(value, bottom, left, top, right) { }

        /// <summary>
        /// Construct a new Ellipse using an array of length 4 for the indices.
        /// This version of the constructor has replaced the other one 
        /// because the return value of ParseCoordinates can be passed directly to it.
        /// </summary>
        /// <param name="value">The dead/alive value (0/1) of the ellipse</param>
        /// <param name="coords">An int array of length 4 containing the 
        /// top, left, bottom, and right indices in that order</param>
        public Ellipse(int value, int[] coords) : base(value, coords) { }

        /// <summary>
        /// Override of PointInsideShape using the ellipse formula given in the task sheet
        /// </summary>
        /// <param name="r">Row index of point</param>
        /// <param name="c">Column index of point</param>
        /// <returns>Whether the point is inside the ellipse</returns>
        public override bool PointInsideShape(int r, int c)
        {
            //Doubles because sometimes centre will have a ".5" on it
            double centre_row = (bottom + top) / 2.0,
                centre_column = (left + right) / 2.0;
            double height = top - bottom + 1,
                width = right - left + 1;
            //the formula given on the task sheet 
            double formula_result = (4.0 * Square(c - centre_column) / Square(width))
                                    + (4.0 * Square(r - centre_row) / Square(height));
            return formula_result <= 1;
        }

        /// <summary>
        /// Square a double using multiplication
        /// (I know the speed disadvantage of Math.Pow probably doesn't matter given the context, 
        /// but I wrote this method anyway)
        /// </summary>
        /// <param name="input">number to square</param>
        /// <returns>input^2</returns>
        static double Square(double input) => input * input;
    }
}

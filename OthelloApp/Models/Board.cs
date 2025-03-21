using System.Text;
using OthelloApp.Exceptions;

namespace OthelloApp.Models
{
    /// <summary>
    /// Representation of an Othello board.
    /// </summary>
    public class Board
    {
        private readonly Mark[,] _board;

        public Board()
        {
            // Initialize the board to 8x8 size and set initial pieces
            _board = new Mark[8, 8];
            _board[3, 3] = Mark.White;
            _board[3, 4] = Mark.Black;
            _board[4, 3] = Mark.Black;
            _board[4, 4] = Mark.White;
        }

        /// <summary>
        /// Sets a mark on the board at the specified row and column, including capturing marks.
        /// </summary>
        /// <param name="row">Row (0-7).</param>
        /// <param name="col">Column (0-7).</param>
        /// <param name="mark">Mark, representing either Black or White.</param>
        /// <exception cref="IllegalMoveException">Thrown when the cell is already occupied.</exception>
        /// <exception cref="ArgumentException">Thrown when invalid row, column, or mark is provided.</exception>
        public void SetMarkAt(int row, int col, Mark mark)
        {
            // Validate row and column bounds
            if (!IsWithinBounds(row, col))
            {
                throw new ArgumentException($"Position out of bounds ({row}, {col})");
            }

            // Validate the mark type
            if (mark != Mark.Black && mark != Mark.White)
            {
                throw new ArgumentException($"Invalid mark: {mark}. Must be either Black or White.");
            }

            // Ensure the target cell is empty
            if (GetMarkAt(row, col) != Mark.Empty)
            {
                throw new IllegalMoveException($"Cannot place mark at ({row}, {col}) - " +
                                               $"cell is already occupied ({_board[row, col]}).");
            }

            // Check whether this move captures at least 1 mark
            List<Tuple<int, int>> capturedMarks = GetCapturedMarkLocations(row, col, mark);
            if (capturedMarks.Count == 0)
            {
                throw new IllegalMoveException($"Invalid move - does not capture any marks");
            }

            // Set the mark
            _board[row, col] = mark;

            // Flip captured marks
            CaptureMarks(capturedMarks);
        }

        /// <summary>
        /// Flips a list of marks
        /// </summary>
        /// <param name="marksToCapture">A list containing positions of marks to be flipped.</param>
        private void CaptureMarks(List<Tuple<int, int>> marksToCapture)
        {
            foreach (Tuple<int, int> position in marksToCapture)
            {
                // Flip the captured mark to the opposite mark
                _board[position.Item1, position.Item2] = _board[position.Item1, position.Item2].GetOther();
            }
        }

        /// <summary>
        /// Finds the locations that a mark would collect starting from a given row and col.
        /// </summary>
        /// <param name="row">Row of the capturing mark</param>
        /// <param name="col">Col of the capturing mark</param>
        /// <param name="mark">The mark capturing</param>
        /// <returns>A list of positions of marks that this move would capture</returns>
        /// <exception cref="ArgumentException"></exception>
        private List<Tuple<int, int>> GetCapturedMarkLocations(int row, int col, Mark mark)
        {
            List<Tuple<int, int>> capturedMarks = new List<Tuple<int, int>>();

            // Check in all directions
            capturedMarks.AddRange(CheckDirection(row, col, mark, -1, 0)); // Left
            capturedMarks.AddRange(CheckDirection(row, col, mark, 1, 0)); // Right
            capturedMarks.AddRange(CheckDirection(row, col, mark, 0, -1)); // Up
            capturedMarks.AddRange(CheckDirection(row, col, mark, 0, 1)); // Down
            capturedMarks.AddRange(CheckDirection(row, col, mark, -1, -1)); // Top-left
            capturedMarks.AddRange(CheckDirection(row, col, mark, -1, 1)); // Top-right
            capturedMarks.AddRange(CheckDirection(row, col, mark, 1, -1)); // Bottom-left
            capturedMarks.AddRange(CheckDirection(row, col, mark, 1, 1)); // Bottom-right

            return capturedMarks;
        }


        /// <summary>
        /// Helper method to check a specific direction for captured marks.
        /// </summary>
        /// <param name="row">Starting row</param>
        /// <param name="col">Starting col</param>
        /// <param name="mark">The mark capturing</param>
        /// <param name="rowDirection">Row increment (-1 for up, 1 for down, 0 for none)</param>
        /// <param name="colDirection">Col increment (-1 for left, 1 for right, 0 for none)</param>
        /// <returns>A list of captured mark positions</returns>
        private List<Tuple<int, int>> CheckDirection(int row, int col, Mark mark, int rowDirection, int colDirection)
        {
            var capturedMarks = new List<Tuple<int, int>>();
            int currentRow = row + rowDirection;
            int currentCol = col + colDirection;

            while (IsWithinBounds(currentRow, currentCol))
            {
                Mark currentMark = GetMarkAt(currentRow, currentCol);

                if (currentMark == Mark.Empty)
                {
                    // Empty space: no capture possible
                    return new List<Tuple<int, int>>();
                }

                if (currentMark == mark)
                {
                    // Found own mark: valid capture if capturedMarks is not empty
                    return capturedMarks;
                }

                // Opponent mark: add to captured list
                capturedMarks.Add(Tuple.Create(currentRow, currentCol));

                currentRow += rowDirection;
                currentCol += colDirection;
            }

            // Reached boundary without capturing
            return new List<Tuple<int, int>>();
        }

        /// <summary>
        /// Gets the mark at the specified row and column.
        /// </summary>
        /// <param name="row">Row (0-7).</param>
        /// <param name="col">Column (0-7).</param>
        /// <returns>The mark at the specified position.</returns>
        private Mark GetMarkAt(int row, int col)
        {
            return _board[row, col];
        }

        /// <summary>
        /// Checks if the given row and column are within the bounds of the board.
        /// </summary>
        /// <param name="row">Row (0-7).</param>
        /// <param name="col">Column (0-7).</param>
        /// <returns>True if the position is within bounds; otherwise, false.</returns>
        private static bool IsWithinBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        /// <summary>
        /// Creates and returns a shallow copy of the board.
        /// </summary>
        /// <returns>A shallow copy of the board (Mark[,]).</returns>
        public Mark[,] GetBoardState()
        {
            return (Mark[,])_board.Clone();
        }

        public override string ToString()
        {
            StringBuilder boardString = new StringBuilder();

            // Loop through each row
            for (int row = 0; row <= 7; row++)
            {
                // Loop through each col
                for (int col = 0; col <= 7; col++)
                {
                    boardString.Append(_board[row, col] switch
                    {
                        Mark.Black => "[B] ",
                        Mark.White => "[W] ",
                        _ => "[ ] "
                    });
                }

                boardString.AppendLine();
            }

            return boardString.ToString();
        }

        /// <summary>
        /// Gets a list of all legal moves for the specified mark.
        /// </summary>
        /// <param name="mark">The mark (either Black or White).</param>
        /// <returns>A list of tuples containing row and column of all legal moves for the mark.</returns>
        public List<Tuple<int, int>> GetLegalMoves(Mark mark)
        {
            // Validate if the mark is valid
            if (mark != Mark.Black && mark != Mark.White)
            {
                throw new ArgumentException($"Invalid mark: {mark}. Must be either Black or White.");
            }

            List<Tuple<int, int>> legalMoves = new List<Tuple<int, int>>();

            // Check every position on the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Skip if the position is not empty
                    if (GetMarkAt(row, col) != Mark.Empty)
                    {
                        continue;
                    }

                    // Check if the current position results in a valid move
                    List<Tuple<int, int>> capturedMarks = GetCapturedMarkLocations(row, col, mark);
                    if (capturedMarks.Count > 0)
                    {
                        // If capturing marks, add this position to the list of legal moves
                        legalMoves.Add(Tuple.Create(row, col));
                    }
                }
            }

            return legalMoves;
        }
    }
}
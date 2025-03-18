namespace OthelloApp.Models
{
    /// <summary>
    /// Represents the possible marks on the Othello board.
    /// </summary>
    public enum Mark
    {
        Empty,
        Black,
        White
    }

    public static class MarkExtensions
    {
        /// <summary>
        /// Returns the opposite mark (Black ⇄ White). Returns Empty if mark is Empty.
        /// </summary>
        public static Mark GetOther(this Mark mark)
        {
            return mark switch
            {
                Mark.Black => Mark.White,
                Mark.White => Mark.Black,
                _ => Mark.Empty
            };
        }
    }
}
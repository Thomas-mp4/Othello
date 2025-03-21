using OthelloApp.Models;
using OthelloApp.Exceptions;

namespace OthelloApp.Tests
{
    public class BoardTests
    {
        [Fact]
        public void Board_InitializeWithCorrectStartingPositions()
        {
            // Arrange
            Board board = new Board();

            // Act
            string boardState = board.ToString();

            // Assert
            Assert.Contains("[W]", boardState);
            Assert.Contains("[B]", boardState);
            Assert.Contains("[ ]", boardState);
        }

        [Fact]
        public void SetMarkAt_SetCorrectMark()
        {
            // Arrange
            Board board = new Board();
            Assert.Equal(Mark.Empty, board.GetBoardState()[2, 2]);

            // Act
            board.SetMarkAt(2, 3, Mark.Black);

            // Assert
            Assert.Equal(Mark.Black, board.GetBoardState()[2, 3]);
        }

        [Fact]
        public void SetMarkAt_OutOfBounds_ThrowException()
        {
            // Arrange
            Board board = new Board();
            Assert.Throws<ArgumentException>(() => board.SetMarkAt(-1, 3, Mark.White));
        }

        [Fact]
        public void SetMarkAt_IllegalMove_ThrowException()
        {
            // Arrange
            Board board = new Board();
            Assert.Throws<IllegalMoveException>(() => board.SetMarkAt(2, 3, Mark.White));
        }

        [Fact]
        public void SetMarkAt_EmptyMark_ThrowException()
        {
            // Arrange
            Board board = new Board();
            Assert.Throws<ArgumentException>(() => board.SetMarkAt(2, 4, Mark.Empty));
        }

        [Fact]
        public void SetMarkAt_SetOnOccupiedCell_ThrowException()
        {
            // Arrange
            Board board = new Board();

            // Act & Assert
            var exception = Assert.Throws<IllegalMoveException>(() => board.SetMarkAt(3, 3, Mark.Black));
            Assert.Equal("Cannot place mark at (3, 3) - cell is already occupied (White).", exception.Message);
        }

        [Fact]
        public void ToString_DisplayBoardCorrectly()
        {
            // Arrange
            var board = new Board();

            // Act
            var boardState = board.ToString();

            // Assert
            // Ensure that the string representation of the board contains the initial configuration
            Assert.Contains("[W] [B]", boardState); // Check for the starting pieces
        }

        [Fact]
        public void GetLegalMoves_AtStart_ContainMoves()
        {
            // Arrange
            Board board = new Board();

            // Act & Assert
            var blackMoves = board.GetLegalMoves(Mark.Black);
            var whiteMoves = board.GetLegalMoves(Mark.White);

            // 4 moves possible for white
            var expectedMovesWhite = new List<Tuple<int, int>>
            {
                Tuple.Create(2, 4),
                Tuple.Create(3, 5),
                Tuple.Create(4, 2),
                Tuple.Create(5, 3)
            };
            Assert.Equal(expectedMovesWhite, whiteMoves);

            // Moves for white should not be possible for black
            foreach (var expectedWhiteMove in expectedMovesWhite)
            {
                Assert.DoesNotContain(expectedWhiteMove, blackMoves);
            }

            // 4 possible moves for black
            var expectedMovesBlack = new List<Tuple<int, int>>
            {
                Tuple.Create(2, 3),
                Tuple.Create(3, 2),
                Tuple.Create(4, 5),
                Tuple.Create(5, 4)
            };
            Assert.Equal(expectedMovesBlack, blackMoves);
        }

        [Fact]
        public void GetLegalMoves_InvalidMark_ThrowException()
        {
            Board board = new Board();
            Assert.Throws<ArgumentException>(() => board.GetLegalMoves(Mark.Empty));
        }
    }
}
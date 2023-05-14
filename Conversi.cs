using System;

public enum DiscColor
{
    Empty,
    Black,
    White
}

public class OthelloGame
{
    private const int BoardSize = 10; // Change the board size to 10
    private DiscColor[,] board;
    public DiscColor CurrentPlayer { get; private set; }

    public OthelloGame()
    {
        // Initialize the board with an empty state
        board = new DiscColor[BoardSize, BoardSize];
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                board[row, col] = DiscColor.Empty;
            }
        }

        // Place the initial discs
        int center = BoardSize / 2;
        board[center - 1, center - 1] = DiscColor.White;
        board[center - 1, center] = DiscColor.Black;
        board[center, center - 1] = DiscColor.Black;
        board[center, center] = DiscColor.White;

        // Set the starting player
        CurrentPlayer = DiscColor.Black;
    }

    public void PrintBoard()
    {
        Console.WriteLine("    A B C D E F G H I J"); // Adjust the column headers
        Console.WriteLine("   ---------------------");
        for (int row = 0; row < BoardSize; row++)
        {
            Console.Write(row + 1 + " |");
            for (int col = 0; col < BoardSize; col++)
            {
                char symbol;
                switch (board[row, col])
                {
                    case DiscColor.Empty:
                        symbol = '-';
                        break;
                    case DiscColor.Black:
                        symbol = 'B';
                        break;
                    case DiscColor.White:
                        symbol = 'W';
                        break;
                    default:
                        symbol = '-';
                        break;
                }
                Console.Write(" " + symbol);
            }
            Console.WriteLine(" |");
        }
        Console.WriteLine("   ---------------------");
    }

    public bool IsMoveValid(int row, int col, DiscColor color)
    {
        // Check if the cell is empty
        if (board[row, col] != DiscColor.Empty)
            return false;

        // Check if the move is valid in any direction
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                // Skip the current cell and out-of-bounds cells
                if (rowOffset == 0 && colOffset == 0 || !IsValidCell(row + rowOffset, col + colOffset))
                    continue;

                // Check if there is a sequence of opponent's discs in this direction
                int currentRow = row + rowOffset;
                int currentCol = col + colOffset;
                bool foundOpponentDisc = false;
                while (IsValidCell(currentRow, currentCol) && board[currentRow, currentCol] == GetOpponentColor(color))
                {
                    currentRow += rowOffset;
                    currentCol += colOffset;
                    foundOpponentDisc = true;
                }

                // If a sequence of opponent's discs was found and ends with the player's disc, the move is valid
                if (foundOpponentDisc && IsValidCell(currentRow, currentCol) && board[currentRow, currentCol] == color)
                    return true;
            }
        }

        return false;
    }

    public void MakeMove(int row, int col, DiscColor color)
    {
        // Validate the move and update the board accordingly
        if (IsMoveValid(row, col, color))
        {
            // Place the disc
            board[row, col] = color;

            // Flip the opponent's discs in all valid directions
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int colOffset = -1; colOffset <= 1; colOffset++)
                {
                    // Skip the current cell and out-of-bounds cells
                    if (rowOffset == 0 && colOffset == 0 || !IsValidCell(row + rowOffset, col + colOffset))
                        continue;

                    // Check if there is a sequence of opponent's discs in this direction
                    int currentRow = row + rowOffset;
                    int currentCol = col + colOffset;
                    bool foundOpponentDisc = false;
                    while (IsValidCell(currentRow, currentCol) && board[currentRow, currentCol] == GetOpponentColor(color))
                    {
                        currentRow += rowOffset;
                        currentCol += colOffset;
                        foundOpponentDisc = true;
                    }

                    // If a sequence of opponent's discs was found and ends with the player's disc, flip the discs
                    if (foundOpponentDisc && IsValidCell(currentRow, currentCol) && board[currentRow, currentCol] == color)
                    {
                        // Flip the discs back to the player's color
                        while (currentRow != row || currentCol != col)
                        {
                            currentRow -= rowOffset;
                            currentCol -= colOffset;
                            board[currentRow, currentCol] = color;
                        }
                    }
                }
            }

            // Switch to the next player
            CurrentPlayer = GetOpponentColor(CurrentPlayer);
        }
    }

    private bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
    }

    private DiscColor GetOpponentColor(DiscColor color)
    {
        return color == DiscColor.Black ? DiscColor.White : DiscColor.Black;
    }
}

class Program
{
    static void Main(string[] args)
    {
        OthelloGame game = new OthelloGame();
        game.PrintBoard();

        // Prompt the player for moves and update the game accordingly
        while (true)
        {
            Console.WriteLine("Current Player: " + game.CurrentPlayer.ToString());

            Console.Write("Enter row (1-10): ");
            int row = 0;
            try
            {
                row = int.Parse(Console.ReadLine()) - 1;
            }
            catch
            {
                Console.WriteLine("Write a number!");
            }
            Console.Write("Enter column (A-J): ");
            string column = Console.ReadLine().ToUpper();
            int col = column[0] - 'A';

            if (game.IsMoveValid(row, col, game.CurrentPlayer))
            {
                game.MakeMove(row, col, game.CurrentPlayer);
                game.PrintBoard();
            }
            else
            {
                Console.WriteLine("Invalid move! Try again.");
            }
        }
    }
}

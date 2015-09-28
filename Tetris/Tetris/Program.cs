using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Program
    {
        private static string fileName = @"C:\Users\Dmitry\Documents\Visual Studio 2012\Projects\Tetris\tetris-tests-2015\smallest.json";

        static void Main(string[] args)
        {
            var gameBoard = new JsonParser(fileName).Parse();
            var commands = gameBoard.Commands.ToCharArray();

            var game = new Game(gameBoard);

            foreach (var command in commands)
            {
                game = game.NextStep(game, command);
//                if (game == null)
//                    return;
            }

        }
    }

    public class Game
    {
        private readonly int width;
        private readonly int height;
        private readonly List<Piece> Pieces;

        private readonly ImmutableHashSet<Cell> UsedCells;
        private readonly ImmutableHashSet<Cell> CurrCells;

        public Game()
        {
            
        }
  
        public Game(GameBoard gameBoard)
        {
            width = gameBoard.Width;
            height = gameBoard.Height;
            Pieces = gameBoard.Pieces;

            var initCell = new Cell(0, width/2);   

            UsedCells = ImmutableHashSet<Cell>.Empty;
            CurrCells = Pieces[0].Cells.Select(cell => cell + initCell).ToImmutableHashSet();

        }

        private Game(Game game, ImmutableHashSet<Cell> position)
        {
            this.width = game.width;
            this.height = game.height;
            this.Pieces = game.Pieces;

            this.UsedCells = game.UsedCells;
            this.CurrCells = position;
        }


        public Game NextStep(Game game, char command)
        {
            switch (command)
            {
                case 'A':
                    return Move(new Cell(0, -1));
                    break;
                case 'D':
                    return Move(new Cell(0, 1));
                    break;
                case 'S':
                    return Move(new Cell(1, 0));
                    break;
                case 'P':
                    PrintCommand();
                    return game;
                    break;
            }
            return null;
        }

        private Game Move(Cell direction)
        {
            return new Game(this, CurrCells.Select(cell => cell + direction).ToImmutableHashSet());
        }

        private void PrintCommand()
        {
            var field = new char[height, width];
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var c = new Cell(i, j);
                    if (UsedCells.Contains(c))
                    {
                        Console.Write('#');
                        continue;
                    }
                    if (CurrCells.Contains(c))
                    {
                        Console.Write('*');
                        continue;
                    }
                    Console.Write('.');
                }
                Console.WriteLine();
            }
            Console.WriteLine("---------");
        }
    }

 
}

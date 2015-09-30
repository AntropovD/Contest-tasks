using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tetris
{
    class Program
    {
        static string fileName = @"C:\Users\Dmitry\Documents\Visual Studio 2012\Projects\Tetris\tetris-tests-2015\smallest.json";

        static void Main(string[] args)
        {
            var gameBoard = new JsonParser(fileName).Parse();
            var game = new Game(gameBoard);
            gameBoard.Commands.ToCharArray().Aggregate(game, (current, command) => current.NextStep(current, command));
        }
    }

    public class Game
    {
        private readonly int width;
        private readonly int height;
        private readonly List<Piece> pieces;
        private readonly int pieceIndex;
        private readonly int commandIndex;
        private readonly int points;

        private Cell InitCell
        {
            get
            {
//                return new Cell(0, pieces[pieceIndex].Cells.Max(cell => cell.Y));
                return new Cell(0, width/2);
            }
        }

        private readonly ImmutableHashSet<Cell> usedCells;
        private readonly ImmutableHashSet<Cell> currCells;

        public Game(GameBoard gameBoard)
        {
            width = gameBoard.Width;
            height = gameBoard.Height;
            pieces = gameBoard.Pieces;
            pieceIndex = 0;
            commandIndex = 0;
            points = 0;
        
            usedCells = ImmutableHashSet<Cell>.Empty;
            currCells = pieces[pieceIndex].Cells.Select(cell => cell + InitCell).ToImmutableHashSet();
        }

        private Game(Game game)
        {
            width = game.width;
            height = game.height;
            pieceIndex = (game.pieceIndex + 1) % game.pieces.Count;
            commandIndex = game.commandIndex + 1;
            points = game.points;
            pieces = game.pieces;

            var allCells = game.usedCells.Union(game.currCells);

            foreach (var line in allCells.Select(cell => cell.X).Distinct())
            {
                if (allCells.Count(cell => cell.X == line) == width)
                {
                    int lineNumber = line;
                    allCells = allCells.Where(cell => cell.X != lineNumber).ToImmutableHashSet();
                    points++;
                }
            }

            usedCells = allCells;


            var piecePositions = pieces[pieceIndex].Cells.Select(cell => cell + InitCell).ToImmutableHashSet();

            if (piecePositions.Intersect(usedCells).Count == 0)
            {
                PrintPoints(this);
                currCells = piecePositions;
                return;
            }
            this.points -= 10;
            usedCells = ImmutableHashSet<Cell>.Empty;
            currCells = piecePositions;
            PrintPoints(this);
        }

        private Game(Game game, ImmutableHashSet<Cell> position)
        {
            width = game.width;
            height = game.height;
            pieceIndex = game.pieceIndex;
            commandIndex = game.commandIndex + 1;
            points = game.points;
            pieces = game.pieces;
            
            usedCells = game.usedCells;
            currCells = position;
        }
        
        public Game NextStep(Game game, char command)
        {
            PrintCommand();
            switch (command)
            {
                case 'A':
                    return Move(Direction.Left);
                case 'D':
                    return Move(Direction.Right);
                case 'S':
                    return Move(Direction.Down);
                case 'Q':
                    return Rotate(RotateDirection.Anticlockwise);
                case 'E':
                    return Rotate(RotateDirection.Clockwise);
                case 'P':
                    PrintCommand();
                    return game;
                default:
                    return game;
            }
        }

        private Game Rotate(RotateDirection direction)
        {
            if (direction == RotateDirection.Anticlockwise)
            {
                var t = this.currCells.
                                Select(cell => 
                                    new Cell(
                                        )
                                    )
                        );

            }
        }

        private Game Move(Cell direction)
        {
            var newPosition = currCells.Select(cell => cell + direction).ToImmutableHashSet();
            if (CheckBorders(newPosition))
            {
                return new Game(this, newPosition);
            }

            var game = new Game(this);
            return game;
        }

        private bool CheckBorders(ImmutableHashSet<Cell> newPosition)
        {
            foreach (var cell in newPosition)
            {
                if (cell.X < 0 || cell.X >= height || cell.Y < 0 || cell.Y >= width)
                    return false;
                if (usedCells.Contains(cell))
                    return false;
            }
            return true;
        }

        private void PrintPoints(Game game)
        {
            Console.WriteLine("{0} {1}", game.commandIndex, game.points);
        }

        private void PrintCommand()
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var cell = new Cell(i, j);
                    if (usedCells.Contains(cell))
                        Console.Write('#');
                    else if (currCells.Contains(cell))
                        Console.Write('*');
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }
            Console.WriteLine("---------");
        }
    } 


}

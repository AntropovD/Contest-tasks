using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tetris
{
    public static class Program
    {
        static string fileName = @"C:\Users\Dmitry\Documents\Visual Studio 2012\Projects\Tetris\tetris-tests-2015\smallest.json";
//        static string fileName = @"C:\Users\Dmitry\Documents\Visual Studio 2012\Projects\Tetris\tetris-tests-2015\cubes-w8-h8-c100.json";

        static void Main(string[] args)
        {
            var field = new JsonParser(fileName).Parse();
            var game = new Game(field);
            game.Run();
        }
    }

    class Game
    {
        private readonly string commands;
        private readonly Field Field;

        public Game(Field field)
        {
            commands = field.Commands;
            Field = field;
        }

        public void Run()
        {
            var step = new Step(Field);
            commands.ToCharArray().Aggregate(step, (current, command) => current.NextStep(command));
        }
    }

    class Step
    {
        private readonly int height;
        private readonly int width;
        private readonly int pieceIndex;
        private readonly int commandIndex;
        private readonly int points;

        private readonly ImmutableList<Piece> Pieces;
        private readonly ImmutableList<Cell> CurrentPieceCells;
        private readonly ImmutableHashSet<Cell> UsedCells;
        private readonly ImmutableHashSet<Cell> RunningCells;
        
        public Step(Field field)
        {
            height = field.Height;
            width = field.Width;
            UsedCells = ImmutableHashSet<Cell>.Empty;
            Pieces = field.Pieces;

            CurrentPieceCells = Pieces[1].Cells;
            Cell shift = GetShift(CurrentPieceCells);

            RunningCells = CurrentPieceCells.Select(c => new Cell(c + shift)).ToImmutableHashSet();

            pieceIndex = 2 % Pieces.Count;
            commandIndex = 0;
            points = 0;
        }
        
        private Step(Step step)
        {
            height = step.height;
            width = step.width;
            pieceIndex = step.pieceIndex;
            commandIndex = step.commandIndex + 1;
            points = step.points;

            Pieces = step.Pieces;
            CurrentPieceCells = step.CurrentPieceCells;
            UsedCells = step.UsedCells;
            RunningCells = step.RunningCells;
        }

        private Step(Step step, IEnumerable<Cell> cells)
        {
            height = step.height;
            width = step.width;
            pieceIndex = step.pieceIndex;
            commandIndex = step.commandIndex + 1;
            points = step.points;

            Pieces = step.Pieces;
            CurrentPieceCells = step.CurrentPieceCells;
            UsedCells = step.UsedCells;
            RunningCells = cells.ToImmutableHashSet();
        }

        private Step(Step field, Tuple<IEnumerable<Cell>, int> pointsNCells)
        {
            throw new NotImplementedException();
        }
        
        public Step NextStep(char command)
        {
            PrintField();
            switch (command)
            {
                case 'P':
                    //PrintField();
                    return new Step(this);
                case 'A':
                    return Move(Offset.Left);
                case 'S':
                    return Move(Offset.Down);
                case 'D':
                    return Move(Offset.Right);
                /*  case 'Q':
                      return Rotate(Rotation.Anticlockwise);
                  case 'E':
                      return Rotate(Rotation.Clockwise);*/
                default:
                    return this;
            }
        }
       
        private Step Move(Cell to)
        {
            var cells = RunningCells.Select(c => c + to).ToList();

            if (CheckBorders(cells))
            {
                return new Step(this, cells);
            }

            PrintScore();
            var allCells = UsedCells.Union(RunningCells);
            var pointsNCells = GetPointsFromCleaning(allCells);

            if (CanAddNextPiece(pointsNCells.Item1))
            {
                return new Step(this, pointsNCells);
            }
                
            return null;
           
        }

        private bool CanAddNextPiece(IEnumerable<Cell> cells)
        {
            return !Pieces[pieceIndex].Cells.
                                Select(c => c + GetShift(Pieces[pieceIndex].Cells)).
                                Intersect(cells).
                                Any();
        }

        private Step Rotate(Rotation clockwise)
        {
            throw new System.NotImplementedException();
        }
        
        private Cell GetShift(ImmutableList<Cell> currentPieceCells)
        {
            int shiftX = Math.Abs(currentPieceCells.Min(c => c.X));

            int pieceWidth = currentPieceCells.Max(c => c.Y) - currentPieceCells.Min(c => c.Y) + 1;
            int shiftY = (width - pieceWidth) / 2 + Math.Abs(currentPieceCells.Min(c => c.Y));

            return new Cell(shiftX, shiftY);
        }

        private Tuple<IEnumerable<Cell>, int> GetPointsFromCleaning(ImmutableHashSet<Cell> allCells)
        {
            IEnumerable<Cell> newCells = allCells.ToList();
            int addPoints = 0;
            foreach (int row in allCells.Select(c => c.X).Distinct().
                                         Where(row => CheckRowFull(allCells, row)))
            {
                int rowNumber = row;
                newCells = newCells.Where(c => c.X != rowNumber);
                addPoints++;
            }
            return new Tuple<IEnumerable<Cell>, int>(newCells, addPoints);
        }
     
        private bool CheckBorders(IEnumerable<Cell> cells)
        {
            return cells.All(CheckCell);
        }

        private bool CheckCell(Cell cell)
        {
            return !(cell.X < 0 || cell.X > width || cell.Y < 0 || cell.Y > height);
        }

        private bool CheckRowFull(ImmutableHashSet<Cell> allCells, int row)
        {
            return allCells.Where(c => c.X == row).
                            ToImmutableHashSet().
                            SetEquals(
                                Enumerable.Range(0, width).Select(i => new Cell(row, i))
                            );
        }

        private void PrintField()
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var cell = new Cell(i, j);
                    if (UsedCells.Contains(cell))
                        Console.Write('#');
                    else if (RunningCells.Contains(cell))
                        Console.Write('*');
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }
            Console.WriteLine("--------");
        }

        private void PrintScore()
        {
            Console.WriteLine("{0} {1}", commandIndex, points);
        }
    }
}

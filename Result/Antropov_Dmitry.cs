using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Tetris
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(args[0]);
            game.Run();
        }
    }
    
    class Game
    {
        private readonly string commands;
        private readonly Field field;

        public Game(string fileName)
        {
            field = new JsonParser(fileName).Parse();
            commands = field.Commands;
        }

        public void Run()
        {
            var step = new Step(field);
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

        private readonly Cell Center;
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

            CurrentPieceCells = Pieces[0].Cells;
            Center = GetShift(CurrentPieceCells);
            RunningCells = CurrentPieceCells.Select(c => new Cell(c + Center)).ToImmutableHashSet();

            pieceIndex = 1 % Pieces.Count;
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

            Center = step.Center;
            Pieces = step.Pieces;
            CurrentPieceCells = step.CurrentPieceCells;
            UsedCells = step.UsedCells;
            RunningCells = step.RunningCells;
        }
        
        private Step(Step step, IEnumerable<Cell> cells, IEnumerable<Cell> rotateCells, Cell newCenter)
        {
            height = step.height;
            width = step.width;
            pieceIndex = step.pieceIndex;
            commandIndex = step.commandIndex + 1;
            points = step.points;

            Center = newCenter;
            Pieces = step.Pieces;
            CurrentPieceCells = rotateCells.ToImmutableList();
            UsedCells = step.UsedCells;
            RunningCells = cells.ToImmutableHashSet();
        }

        private Step(Step step, Tuple<IEnumerable<Cell>, int> pointsNCells)
        {
            height = step.height;
            width = step.width;
            commandIndex = step.commandIndex + 1;
            points = step.points + pointsNCells.Item2;
                        
            Pieces = step.Pieces;
            CurrentPieceCells = Pieces[step.pieceIndex].Cells;
            UsedCells = pointsNCells.Item1.ToImmutableHashSet();
            Center = GetShift(CurrentPieceCells);
            RunningCells = CurrentPieceCells.Select(c => new Cell(c + Center)).ToImmutableHashSet();
            pieceIndex = (step.pieceIndex + 1) % step.Pieces.Count;
        }
        
        public Step NextStep(char command)
        {
            switch (command)
            {
                case 'P':
                    PrintField();
                    return new Step(this);
                case 'A':
                    return Move(Offset.Left);
                case 'S':
                    return Move(Offset.Down);
                case 'D':
                    return Move(Offset.Right);
                case 'Q':
                    return Rotate(Rotation.Anticlockwise);
                case 'E':
                    return Rotate(Rotation.Clockwise);
                default:
                    return this;
            }
        }
       
        private Step Move(Cell to)
        {
            var cells = RunningCells.Select(c => c + to).ToList();
            if (CheckBorders(cells) )
                return new Step(this, cells, CurrentPieceCells, Center + to);
            return CollisionWork();
        }
        
        private Step Rotate(Rotation clockwise)
        {
            int modify = clockwise == Rotation.Clockwise ? -1 : 1;
            var rotateCells = CurrentPieceCells.Select(c => new Cell(c.Y * modify, -modify * c.X)).ToList();
            var runCells = rotateCells.Select(c => c + Center).ToList();
            if (CheckBorders(runCells))
                return new Step(this, runCells, rotateCells, Center);

            return CollisionWork();
        }

        private Step CollisionWork()
        {
            var pointsNCells = GetPointsFromClearing(UsedCells.Union(RunningCells));
            if (!CanAddNextPiece(pointsNCells.Item1))
            {
                pointsNCells = new Tuple<IEnumerable<Cell>, int>(ImmutableHashSet<Cell>.Empty, pointsNCells.Item2 - 10);
            }
            PrintScore(pointsNCells.Item2);
            return new Step(this, pointsNCells);
        }

        private bool CanAddNextPiece(IEnumerable<Cell> cells)
        {
            var newCenter = GetShift(Pieces[pieceIndex].Cells);
            return !Pieces[pieceIndex].Cells.
                                Select(c => c + newCenter).
                                Intersect(cells).Any();
        }

        private Cell GetShift(ImmutableList<Cell> currentPieceCells)
        {
            int shiftY = Math.Abs(currentPieceCells.Min(c => c.Y));
            int pieceWidth = currentPieceCells.Max(c => c.X) - currentPieceCells.Min(c => c.X) + 1;
            int shiftX = (width - pieceWidth) / 2 + Math.Abs(currentPieceCells.Min(c => c.X));
            return new Cell(shiftX, shiftY);
        }

        private Tuple<IEnumerable<Cell>, int> GetPointsFromClearing(ImmutableHashSet<Cell> allCells)
        {
            IEnumerable<Cell> newCells = allCells.ToList();
            int addPoints = 0;
            foreach (int row in allCells.Select(c => c.Y).Distinct().
                                         Where(row => CheckRowFull(allCells, row)))
            {
                int rowNumber = row;
                newCells = newCells.Where(c => c.Y != rowNumber).ToList();
                var shiftCells = newCells.Where(r => r.Y < rowNumber).Select(c => new Cell(c.X, c.Y+1)).ToList();
                newCells = newCells.Where(r => r.Y > rowNumber).Union(shiftCells);
                addPoints++;
            }
            return new Tuple<IEnumerable<Cell>, int>(newCells, addPoints);
        }
     
        private bool CheckBorders(IList<Cell> cells)
        {
            return cells.All(CheckCell) && !UsedCells.Intersect(cells).Any();
        }

        private bool CheckCell(Cell cell)
        {
            return !(cell.X < 0 || cell.X >= width || cell.Y < 0 || cell.Y >= height);
        }

        private bool CheckRowFull(ImmutableHashSet<Cell> allCells, int row)
        {
            return allCells.Where(c => c.Y == row).
                            ToImmutableHashSet().
                            SetEquals(Enumerable.Range(0, width).Select(i => new Cell(i, row)));
        }

        private void PrintField()
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var cell = new Cell(j, i);
                    if (UsedCells.Contains(cell))
                        Console.Write('#');
                    else if (RunningCells.Contains(cell))
                        Console.Write('*');
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }
        }

        private void PrintScore(int addPoints)
        {
            Console.WriteLine("{0} {1}", commandIndex, points+addPoints);
        }
    }
   
    public class JsonParser
    {
        private readonly string fileName;

        public JsonParser(string fileName)
        {
            this.fileName = fileName;
        }

        public Field Parse()
        {
            return JsonConvert.DeserializeObject<Field>(File.ReadAllText(fileName));
        }
    }

    public class Field
    {
        [JsonConstructor]
        public Field(int width, int height, ImmutableList<Piece> pieces, string commands)
        {
            Width = width;
            Height = height;
            Pieces = pieces;
            Commands = commands;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public ImmutableList<Piece> Pieces { get; private set; }
        public string Commands { get; private set; }
    }

    public class Piece
    {
        [JsonConstructor]
        public Piece(ImmutableList<Cell> cells)
        {
            Cells = cells;
        }
        public ImmutableList<Cell> Cells { get; private set; }
    }

    public class Cell
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        [JsonConstructor]
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Cell(Cell other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Cell operator +(Cell one, Cell two)
        {
            return new Cell(one.X + two.X, one.Y + two.Y);
        }

        public override bool Equals(object obj)
        {
            var cell = obj as Cell;
            if (cell == null)
                return false;
            return (cell.X == X && cell.Y == Y);
        }

//        http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            // 317, 331 - Prime numbers
            int prime1 = 317;
            int prime2 = 331;

            int hash = prime1 * prime2 + X.GetHashCode();
            hash = hash * prime2 + Y.GetHashCode();
            return hash;
        }
    }

    static class Offset
    {
        public static readonly Cell Left = new Cell(-1, 0);
        public static readonly Cell Down = new Cell(0, 1);
        public static readonly Cell Right = new Cell(1, 0);
    }

    enum Rotation
    {
        Clockwise,
        Anticlockwise
    }
}

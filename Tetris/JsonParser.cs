using System.Collections.Immutable;
using System.IO;
using Newtonsoft.Json;

namespace Tetris
{
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

        public static Cell operator + (Cell one, Cell two)
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
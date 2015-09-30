using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
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
        public int Width { get; set; }
        public int Height { get; set; }
        public ImmutableList<Piece> Pieces { get; set; }
        public string Commands { get; set; }
    }

    public class Piece
    {
        public ImmutableList<Cell> Cells { get; set; }
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
            return (X.GetHashCode() * Y.GetHashCode());
        }
    }

    public static class Offset
    {
        public static readonly Cell Left = new Cell(0, -1);
        public static readonly Cell Down = new Cell(1, 0);
        public static readonly Cell Right = new Cell(0, 1);
    }

    public enum Rotation
    {
        Clockwise,
        Anticlockwise
    }
    
}
using System.Collections.Generic;
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

        public GameBoard Parse()
        {
            return JsonConvert.DeserializeObject<GameBoard>(File.ReadAllText(fileName));
        }
    }
   
    public class GameBoard
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Piece> Pieces { get; set; }
        public string Commands { get; set; }
    }

    public class Piece
    {
        public ImmutableList<Cell> Cells { get; set; }
    }

    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Cell operator +(Cell one, Cell two)
        {
            return new Cell(one.X + two.X, one.Y + two.Y);
        }

        public override bool Equals(object obj)
        {
            var cell = (obj as Cell);
            return (cell.X == X && cell.Y == Y);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() + Y.GetHashCode());
        }
    }

}
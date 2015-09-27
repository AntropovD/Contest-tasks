using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

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
    }
}

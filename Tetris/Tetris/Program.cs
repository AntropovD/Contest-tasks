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
        private static readonly string fileName = @"C:\Users\Dmitry\Documents\Visual Studio 2012\Projects\Tetris\tetris-tests-2015\smallest.json";

        static void Main(string[] args)
        {
            var gameBoard = new JsonParser(fileName).Parse();
        }
    }

    public class Game
    {
        private readonly int width;
        private readonly int height;


        private int Width { get { return width; } }
        private int Height { get { return height; } }

        public Game(GameBoard gameBoard)
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var game = new Game(gameBoard);

            game.Run();
        }
    }

    public class Game
    {
        private readonly int width;
        private readonly int height;
        private readonly string commands;
        private readonly List<Piece> pieces;

        private readonly int points;
        private readonly int commandIndex;
        private readonly int pieceIndex;


        private int Width { get { return width; } }
        private int Height { get { return height; } }
        private string Commands { get { return commands; } }
        private List<Piece> Pieces { get { return pieces; } }

        private Map field;
        

        public Game(GameBoard gameBoard)
        {
            width = gameBoard.Width;
            height = gameBoard.Height;
            commands = gameBoard.Commands;
            pieces = new List<Piece>(gameBoard.Pieces);

            points = 0;
            commandIndex = 0;
            pieceIndex = 0;

            field = new Map(height, width, '.');

            var initCell = getInitCell();

            AddPiece(initCell, pieceIndex);
            PrintCommand();
        }

        private void AddPiece(Cell initCell, int i)
        {
            field = field.Change(Pieces[i].Cells.Select(s => s + initCell), '*');
        }

        private Cell getInitCell()
        {
            int x = 0;
            int y = Width/2;
            return new Cell(x, y);
        }


        public void Run()
        {
            int pieceIndex = 0;

            foreach (var command in Commands.ToCharArray())
            {
                switch (command)
                {
                    case 'P':
                        PrintCommand();

                        break;
                    case 'S':
                        DownCommand();

                        break;

                }
            }
        }

        private void PrintCommand()
        {
            Console.WriteLine("{0} {1}", commandIndex, points);
            for (int i = 0; i < field.field.Length; ++i)
            {
                for (int j = 0; j < field.field[0].Length; ++j)
                    Console.Write(field.field[i][j]);
                Console.WriteLine();
            }
        }

        private void DownCommand()
        {
            

        }
}

    public class Map
    {
        public char[][] field;

        public Map(int h, int w, char c)
        {
            field = new char[h][];
            
            for (int i = 0; i < h; ++i)
            {
                field[i] = new char[w];
                for (int j = 0; j < w; ++j)
                    field[i][j] = c;
            }
        }

        public Map Change(IEnumerable<Cell> cells, char c)
        {
            var map = new Map(field.Length, field[0].Length, '.');
            foreach (var cell in cells)
            {
                map.field[cell.X][cell.Y] = c;
            }
            return map;
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

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Cell operator +(Cell one, Cell two)
        {
            return new Cell(one.X + two.X, one.Y + two.Y);
        }

        
    }
}

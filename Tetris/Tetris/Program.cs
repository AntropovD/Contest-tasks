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
                if (game == null)
                    return;
            }

        }
    }

    public class Game
    {
        private readonly int width;
        private readonly int height;
        private readonly List<Piece> Pieces;

        public Game(GameBoard gameBoard)
        {
            width = gameBoard.Width;
            height = gameBoard.Height;
            Pieces = gameBoard.Pieces;
        }

        public void NextStep(Game game, char command)
        {
            switch (command)
            {
                case 'D':
                    DownCommand();
                    break;
                case 'P':
                    PrintCommand();
                    break;
            }
        }

        private void DownCommand()
        {
            throw new NotImplementedException();
        }

        private void PrintCommand()
        {
            throw new NotImplementedException();
        }
    }

 
}

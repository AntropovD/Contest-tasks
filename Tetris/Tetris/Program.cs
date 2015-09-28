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

            foreach (var command in commands)
            {
                
            }

        }
    }

    public class Game
    {
        
    }

 
}

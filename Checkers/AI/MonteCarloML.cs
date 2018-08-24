using System;
using System.IO;
using System.Reflection;

namespace Checkers.AI
{
    internal class MonteCarloMl
    {
        private bool _isWhiteSide;
        private Desk _desk;
        private static string _tree = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\montecarlo.txt";

        public MonteCarloMl(bool isWhiteSide, Desk desk)
        {
            CheckFile();
            File.AppendAllText(_tree, "something...");
            _isWhiteSide = isWhiteSide;
            _desk = desk;
        }

        private static void CheckFile()
        {
            if (!File.Exists(_tree))
                File.WriteAllText(_tree, "");
            if (!File.Exists(_tree))
                throw new Exception("Can`t create file!");
            //todo: check on errors
        }
    }
}
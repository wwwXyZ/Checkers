using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Checkers;

namespace AI
{
    class MonteCarloML
    {
        private bool _isWhiteSide;
        private Desk _desk;
        private const string Tree = "Memory/montecarlo.txt";

        public MonteCarloML(bool isWhiteSide, Desk desk)
        {
            File.WriteAllText(Tree, "something...");
            _isWhiteSide = isWhiteSide;
            _desk = desk;
        }
    }
}

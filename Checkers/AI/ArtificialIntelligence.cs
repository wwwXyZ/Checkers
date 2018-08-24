using System.Threading;

namespace Checkers.AI
{
    public class ArtificialIntelligence
    {
        private readonly bool _isWhiteSide;
        private Desk _desk;

        public bool Get_isWhiteSide()
        {
            return _isWhiteSide;
        }

        public ArtificialIntelligence(bool isWhiteSide)
        {
            _isWhiteSide = isWhiteSide;
        }

        public void MoveToNextPosition()
        {
            var vanga = new Vanga(_isWhiteSide, _desk);
            vanga.GenerateMovePositions();
            var tree = new MonteCarloMl(_isWhiteSide, _desk);
        }

        public void SetDesk(Desk desk)
        {
            _desk = desk;
        }
    }
}
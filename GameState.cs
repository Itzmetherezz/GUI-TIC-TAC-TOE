using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TIC_TAC_TOE
{
    public class GameState
    {
        public Player[,] Gamegrid { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int TurnsPassed { get; private set; }
        public bool GameOver { get; private set; }

        public event Action<int, int> MoveMade;

        public event Action<GameResult> GameOverEvent;
        public event Action GameReset;

        public GameState()
        {
            Gamegrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;

        }
        private bool CanMakeMove(int r, int c)
        {
            return !GameOver && Gamegrid[r, c] == Player.None;

        }
        private bool IsGridFull()
        {
            return TurnsPassed == 9;
        }
        private void SwitchPlayer()
        {
            if (CurrentPlayer == Player.X)
            {
                CurrentPlayer = Player.O;
            }
            else
            {
                CurrentPlayer = Player.X;
            }
        }

        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {

            foreach ((int r, int c) in squares)
            {
                if (Gamegrid[r, c] != player)
                    return false;
            }
            return true;
        }

        private bool DidMoveWin(int r, int c, out WinInfo winInfo)
        {
            (int, int)[] row = { (r, 0), (r, 1), (r, 2) };
            (int, int)[] column = { (0, c), (1, c), (2, c) };
            (int, int)[] mainDiagonal = { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antiDiagonal = { (0, 2), (1, 1), (2, 0) };

            if (AreSquaresMarked(row, CurrentPlayer))
            {
                winInfo = new WinInfo { WinType = WinType.Row, Index = r };
                return true;
            }

            if (AreSquaresMarked(column, CurrentPlayer))
            {
                winInfo = new WinInfo { WinType = WinType.Column, Index = c };
                return true;
            }

            if (AreSquaresMarked(mainDiagonal, CurrentPlayer))
            {
                winInfo = new WinInfo { WinType = WinType.MainDiagonal };
                return true;
            }

            if (AreSquaresMarked(antiDiagonal, CurrentPlayer))
            {
                winInfo = new WinInfo { WinType = WinType.AntiDiagonal };
                return true;
            }

            winInfo = null;
            return false;

        }
        private bool DidMoveEndGame(int r, int c, out GameResult gameResult)
        {
            if (DidMoveWin(r, c, out WinInfo winInfo))
            {
                gameResult = new GameResult { Winner = CurrentPlayer, WinInfo = winInfo };
                return true;
            }
            if (IsGridFull())
            {
                gameResult = new GameResult { Winner = Player.None, WinInfo = null };
                return true;
            }
            gameResult = null;
            return false;

        }
        public void MakeMove(int r, int c)
        {
            if (!CanMakeMove(r, c))
                return;

            Gamegrid[r, c] = CurrentPlayer;
            TurnsPassed++;

            MoveMade?.Invoke(r, c);

            if (DidMoveEndGame(r, c, out GameResult gameResult))
            {
                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameOverEvent?.Invoke(gameResult);

            }
            else
            {
                SwitchPlayer();
                MoveMade?.Invoke(r, c);
            }


        }
        public void ResetGame()
        {
            Gamegrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;

            GameReset?.Invoke();
        }
    }
}















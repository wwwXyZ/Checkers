using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Checkers.AI;
using FontFamily = Svg.Text.FontFamily;

namespace Checkers
{
    public class Desk
    {
        public const int BotStepTimeout = 700;

        public int Width { get; set; }
        public int Height { get; set; }

        private static readonly int MinWidth;
        private static readonly int MinHeight;
        private static readonly int DefaultWidth;
        private static readonly int DefaultHeight;

        public int Rotation { get; set; } // 0 - 0 degree; 1 - 90 degree; 2 - 180 degree; 3 - 270 degree
        private int _blackCount;
        private int _whiteCount;
        private int _blackQueansCount;
        private int _whiteQueansCount;
        private bool _isBotSimulation;
        private bool _allowCheats = true;
        public bool NeedBeat { get; set; }
        public bool CurrentWhiteTurn;

        public Player FirstPlayer { get; }
        public Player SecondPlayer { get; }
        private Cell _selectedCell;
        private Checker _lastShotDownChecker;
        public List<Cell> Cells = new List<Cell>();
        public List<Cell.CellPosition> TopDefaultPositions = new List<Cell.CellPosition>();
        public List<Cell.CellPosition> BottomDefaultPositions = new List<Cell.CellPosition>();
        public List<Cell.CellPosition> AllowedPositions = new List<Cell.CellPosition>();
        public List<Cell.CellPosition> BattleCheckersPositions = new List<Cell.CellPosition>();
        public List<string> GameCellsDeskHistory = new List<string>();
        private static ArtificialIntelligence _firstPlayerBot;
        private static ArtificialIntelligence _secondPlayerBot;

        public void Toggle_allowCheats()
        {
            _allowCheats = !_allowCheats;
        }

        public bool Get_allowCheats()
        {
            return _allowCheats;
        }

        public int Get_blackCount()
        {
            return _blackCount;
        }

        public void Set_blackCount(int value)
        {
            _blackCount = value;
        }

        public int Get_whiteCount()
        {
            return _whiteCount;
        }

        public void Set_whiteCount(int value)
        {
            _whiteCount = value;
        }

        public bool Get_isBotSimulation()
        {
            return _isBotSimulation;
        }
        public void StartBotSimulation()
        {
            _isBotSimulation = true;
        }

        public Cell Get_selectedCell()
        {
            return _selectedCell;
        }

        public Checker Get_lastShotDownChecker()
        {
            return _lastShotDownChecker;
        }

        public void Set_ShotDownCecker(Checker checker)
        {
            _lastShotDownChecker = checker;
        }

        public static int GetMinHeight()
        {
            return MinHeight;
        }

        public static int GetMinWidth()
        {
            return MinWidth;
        }

        static Desk()
        {
            MinWidth = 8;
            MinHeight = 8;
            DefaultWidth = 8;
            DefaultHeight = 8;
        }

        public Desk(int width, int height, int rotation, bool currentWhiteTurn, bool firstPlayerIsHuman, bool secondPlayerIsHuman)
        {
            Width = width > MinWidth ? width : MinWidth;
            Height = height > MinHeight ? height : MinHeight;
            Rotation = rotation;
            CurrentWhiteTurn = currentWhiteTurn;
            if (!firstPlayerIsHuman)
                _firstPlayerBot = new ArtificialIntelligence(CurrentWhiteTurn);
            if (!secondPlayerIsHuman)
                _secondPlayerBot = new ArtificialIntelligence(!CurrentWhiteTurn);
            FirstPlayer = new Player(firstPlayerIsHuman, CurrentWhiteTurn);
            SecondPlayer = new Player(secondPlayerIsHuman, !CurrentWhiteTurn);
            OrganizeDefaultPositions();
        }

        public Desk()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            Rotation = 0;
            CurrentWhiteTurn = true;
            FirstPlayer = new Player(true, CurrentWhiteTurn);
            SecondPlayer = new Player(true, !CurrentWhiteTurn);
            OrganizeDefaultPositions();
        }

        /*
         * Width;Heigth;WiteOnTop;firstPlayer;secondPlayer;cell,cell,cell...
         * WiteOnTop(No 0; Yes 1;)
         * CurrentTurn(Black 0; White 1;)
         */
        public Desk(string rawDesk)
        {
            var deskParams = rawDesk.Split(';');
            if (deskParams.Length < 4)
                throw new Exception("Invalid desk params!");
            Width = int.Parse(deskParams[0]);
            Height = int.Parse(deskParams[1]);
            CurrentWhiteTurn = deskParams[2] == "1";
            FirstPlayer = new Player(deskParams[3]);
            SecondPlayer = new Player(deskParams[4]);
            var rawCells = deskParams[5].Split(',');
            if (rawCells.Length < Width * Height)
                throw new Exception("Incorrect cells count!");

            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    var rawCell = rawCells[row * Width + column];
                    Checker checker;
                    var rawChrcker = int.Parse(rawCell.Substring(1, 1));
                    switch (rawChrcker)
                    {
                        case 1:
                            checker = new Checker(false, false);
                            break;
                        case 2:
                            checker = new Checker(false, true);
                            break;
                        case 3:
                            checker = new Checker(true, false);
                            break;
                        case 4:
                            checker = new Checker(true, true);
                            break;
                        default:
                            checker = null;
                            break;
                    }

                    Cells.Add(new Cell(new Cell.CellPosition(column, row), new Cell.CellColor(rawCell[0] == '1'), checker, this));
                }
            }

            FirstPlayer = new Player(true, CurrentWhiteTurn);
            SecondPlayer = new Player(true, !CurrentWhiteTurn);
        }

        public void OrganizeDefaultPositions()
        {
            TopDefaultPositions.Clear();
            BottomDefaultPositions.Clear();
            var minusRows = Height % 2 == 0 ? 0 : 1;
            for (var column = 0; column < Width; column++)
            {
                for (var row = 0; row < Height; row++)
                {
                    if ((column + row) % 2 == 0) continue;
                    if (row < (Height - minusRows) / 2 - 1)
                        TopDefaultPositions.Add(new Cell.CellPosition(column, row));
                    else if (row >= (Height + minusRows) / 2 + 1)
                        BottomDefaultPositions.Add(new Cell.CellPosition(column, row));
                }
            }
        }

        private void Add_cell(Cell cell)
        {
            if (cell.Checker != null)
                if (cell.Checker.Get_isWhite())
                    Set_whiteCount(_whiteCount + 1);
                else
                    Set_blackCount(_blackCount + 1);
            Cells.Add(cell);
        }

        public void Clear_cells()
        {
            Set_blackCount(0);
            Set_whiteCount(0);
            Cells.Clear();
            GameCellsDeskHistory.Clear();
        }

        private Cell ConstructCell(int row, int column, bool withCheckers)
        {
            var cellColor = new Cell.CellColor(((column + row) % 2) == 0);
            var cellPosition = new Cell.CellPosition(column, row);
            var deskCell = new Cell(cellPosition, cellColor, null, this);
            if (withCheckers)
                deskCell.SetDefaultChecker(this);
            return deskCell;
        }

        public void Generate(bool withCeckers)
        {
//           switch (Rotation)
//            {
//                case 1: // right
//                    for (var column = 0; column < Width; column++)
//                    {
//                        for (var row = Height - 1; row >= 0; row--)
//                        {
//                            Add_cell(ConstructCell(row, column, withCeckers));
//                        }
//                    }
//
//                    break;
//                case 2: // invert
//                    for (var row = Height - 1; row >= 0; row--)
//                    {
//                        for (var column = Width - 1; column >= 0; column--)
//                        {
//                            Add_cell(ConstructCell(row, column, withCeckers));
//                        }
//                    }
//
//                    break;
//                case 3: // left
//                    for (var column = Width - 1; column >= 0; column--)
//                    {
//                        for (var row = 0; row < Height; row++)
//                        {
//                            Add_cell(ConstructCell(row, column, withCeckers));
//                        }
//                    }
//
//                    break;
//                default: // 0 or default
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    Add_cell(ConstructCell(row, column, withCeckers));
                }
            }

//
//                    break;
//            }
        }

        public void ShowAllowedPosition(Cell cell)
        {
            AllowedPositions.AddRange(cell.GetAllowedPositions());
        }

        public void UnselectLastCell()
        {
            AllowedPositions.Clear();
            _selectedCell = null;
        }

        public void EndTurn()
        {
            var currentPlayerCanMove = false;
            foreach (var cell in Cells)
            {
                if (cell.Checker != null && cell.Checker.Get_isShotDown())
                {
                    cell.Checker = null;
                    var position = cell.GetCellPosition();
                    Cells[position.Get_row() * Width + position.Get_column()].Checker = null;
                }

                if (
                    cell.Checker == null ||
                    cell.Checker.Get_isWhite() != CurrentWhiteTurn ||
                    (
                        cell.GetBattleCells().Count <= 0 &&
                        cell.GetAllowedPositions().Count <= 0
                    )
                ) continue;
                currentPlayerCanMove = true;
//                break;
            }

            SaveInDeskHistory();
            var isDraw = false;
            var drawType = 0;
            /*
             * Draw condition:
             * one desk position repeet 3 times - CheckTripplePosition
             * if at the end of the game three queans (or more) against one quean enemy, his 15th move (counting from the moment of equality of forces) will not take the opponent`s checker - Check15ThQueansBattle
             * if in a position in which both rivals have a quean, the balance of forces has not changed (i.e., there has not been a take, and no single checker has become a quean) over:
             * in 2 and 3-figure endings - 5 moves,
             * in 4 and 5-figure endings - 30 moves,
             * in 6 and 7-figure endings - 60 moves. - CheckPowerResistance
             */
            if (CheckTripplePosition() || Check15ThQueansBattle() || CheckPowerResistance())
                isDraw = true;


            if (isDraw) //check draw type
                drawType = ShowDrawType();
            if (isDraw || !currentPlayerCanMove)
                ((MainWindow) Application.Current.MainWindow)?.EngGame(isDraw ? drawType : (!CurrentWhiteTurn ? 1 : 0));
        }

        public void BotTurn()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                if (CurrentPlayerIsHuman() || _isBotSimulation) return;
                var currentPlayerIsWhite = GetCurrentPlayer().Get_isWhite();
                if (_firstPlayerBot != null && _firstPlayerBot.Get_isWhiteSide() == currentPlayerIsWhite)
                {
                    _firstPlayerBot.SetDesk(this);
                    _firstPlayerBot.MoveToNextPosition();
                }
                else
                {
                    _secondPlayerBot.SetDesk(this);
                    _secondPlayerBot.MoveToNextPosition();
                }
            }));
        }



        /*
         * Count available checkers two players and chose winner
         * simple checke = +1
         * quean = +3
         */
        private int ShowDrawType()
        {
            var blackPoints = 0;
            var whitePoints = 0;
            foreach (var cell in Cells)
            {
                if (cell.Checker == null) continue;
                if (cell.Checker.Get_isWhite())
                    whitePoints += cell.Checker.Is_Quean() ? 3 : 1;
                else
                    blackPoints += cell.Checker.Is_Quean() ? 3 : 1;
            }

            return blackPoints == whitePoints ? 0 : (blackPoints < whitePoints ? 1 : -1);
        }

        private bool CheckTripplePosition()
        {
            if (GameCellsDeskHistory.Count <= 0) return false;
            foreach (var deskStatement in GameCellsDeskHistory)
            {
                var gameCellsDeskHistory2 = new List<string>(GameCellsDeskHistory);
                gameCellsDeskHistory2.Remove(gameCellsDeskHistory2.FirstOrDefault(value => value == deskStatement));
                foreach (var deskStatement2 in gameCellsDeskHistory2)
                {
                    if (deskStatement != deskStatement2) continue;
                    var gameCellsDeskHistory3 = new List<string>(gameCellsDeskHistory2);
                    gameCellsDeskHistory3.Remove(gameCellsDeskHistory3.FirstOrDefault(value => value == deskStatement));
                    foreach (var deskStatement3 in gameCellsDeskHistory3)
                        if (deskStatement2 == deskStatement3)
                            return true;
                }
            }

            return false;
        }

        private bool Check15ThQueansBattle()
        {
            var deskStatementsCount = GameCellsDeskHistory.Count;
            if (deskStatementsCount < 15) return false;
            var fifteenthBackDesk = new Desk(GameCellsDeskHistory[deskStatementsCount - 15]);
            UpdateSimpleCheckersAndQueansCount();
            fifteenthBackDesk.UpdateSimpleCheckersAndQueansCount();
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (
                fifteenthBackDesk._whiteCount != _whiteQueansCount ||
                fifteenthBackDesk._blackCount != _blackQueansCount ||
                fifteenthBackDesk._whiteCount != _whiteCount ||
                fifteenthBackDesk._blackCount != _blackCount ||
                fifteenthBackDesk._whiteQueansCount <= 0 ||
                fifteenthBackDesk._blackQueansCount <= 0 ||
                _whiteQueansCount <= 0 ||
                _blackQueansCount <= 0 ||
                _whiteQueansCount != fifteenthBackDesk._whiteQueansCount ||
                _blackQueansCount != fifteenthBackDesk._blackQueansCount
            ) return false;


            return true;
        }

        private bool CheckPowerResistance()
        {
            UpdateSimpleCheckersAndQueansCount();
            var deskStatementsCount = GameCellsDeskHistory.Count;
            if (deskStatementsCount < 5) return false;
            var fivethBackDesk = new Desk(GameCellsDeskHistory[deskStatementsCount - 5]);
            fivethBackDesk.UpdateSimpleCheckersAndQueansCount();
            if ((fivethBackDesk._whiteCount <= 3 || fivethBackDesk._blackCount <= 3) && CheckPowerContition(fivethBackDesk)) return true;
            if (deskStatementsCount < 30) return false;
            var thirtiethBackDesk = new Desk(GameCellsDeskHistory[deskStatementsCount - 30]);
            thirtiethBackDesk.UpdateSimpleCheckersAndQueansCount();
            if ((thirtiethBackDesk._whiteCount <= 5 || thirtiethBackDesk._blackCount <= 5) && CheckPowerContition(thirtiethBackDesk)) return true;
            if (deskStatementsCount < 60) return false;
            var sixtiethBackDesk = new Desk(GameCellsDeskHistory[deskStatementsCount - 60]);
            sixtiethBackDesk.UpdateSimpleCheckersAndQueansCount();
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if ((sixtiethBackDesk._whiteCount <= 7 || sixtiethBackDesk._blackCount <= 7) && CheckPowerContition(sixtiethBackDesk)) return true;
            return false;
        }

        private bool CheckPowerContition(Desk desk) => desk._whiteCount == _whiteCount && desk._blackCount == _blackCount && desk._whiteQueansCount > 0 && desk._blackQueansCount > 0 && _whiteQueansCount > 0 && _blackQueansCount > 0 && _whiteQueansCount == desk._whiteQueansCount && _blackQueansCount == desk._blackQueansCount;

        private void UpdateSimpleCheckersAndQueansCount()
        {
            _whiteCount = 0;
            _blackCount = 0;
            _whiteQueansCount = 0;
            _blackQueansCount = 0;
            foreach (var cell in Cells)
            {
                if (cell.Checker == null) continue;
                if (cell.Checker.Get_isWhite())
                {
                    if (cell.Checker.Is_Quean())
                        ++_whiteQueansCount;
                    ++_whiteCount;
                }
                else
                {
                    if (cell.Checker.Is_Quean())
                        ++_blackQueansCount;
                    ++_blackCount;
                }
            }
        }

        public void ReRenderTable() => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { ((MainWindow) Application.Current.MainWindow)?.RenderBattlefield(false); })).Wait();

        public void ReRenderTable(int timeout)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                ((MainWindow) Application.Current.MainWindow)?.RenderBattlefield(false);
                Thread.Sleep(timeout);
            })).Wait();
        }

        public void SetSelectedCell(Cell cell) => _selectedCell = cell;

        public void CheckIfNeedBeate()
        {
            NeedBeat = false;
            foreach (var cell in Cells)
            {
                var battleCells = cell.GetBattleCells();
                if (battleCells.Count <= 0) continue;
                NeedBeat = true;
            }
        }

        public void CheckIfNeedBeate(Cell cell)
        {
            NeedBeat = false;

            var battleCells = cell.GetBattleCells();
            NeedBeat = battleCells.Count > 0;
        }

        /*
         * Width;Heigth;CurrentTurn;firstPlayer;secondPlayer;cell,cell,cell...
         * WiteOnTop(No 0; Yes 1;)
         * CurrentTurn(Black 0; White 1;)
         */
        public void SaveInDeskHistory()
        {
            GameCellsDeskHistory.Add(ReturnDeskAsRawText());
        }

        public string ReturnDeskAsRawText()
        {
            var deskStatement = "";
            deskStatement += Width + ";";
            deskStatement += Height + ";";
            deskStatement += (CurrentWhiteTurn ? "1" : "0") + ";";
            deskStatement += FirstPlayer.ReturnPlayerAsRawText() + ";";
            deskStatement += SecondPlayer.ReturnPlayerAsRawText() + ";";
            foreach (var cell in Cells)
                deskStatement += cell.ReturnCellAsRawText() + ",";

            deskStatement = deskStatement.Substring(0, deskStatement.Length - 1);
            return deskStatement;
        }

        public Player GetCurrentPlayer()
        {
            return CurrentWhiteTurn ? (FirstPlayer.Get_isWhite() ? FirstPlayer : SecondPlayer) : (FirstPlayer.Get_isWhite() ? SecondPlayer : FirstPlayer);
        }

        public bool CurrentPlayerIsHuman()
        {
            return GetCurrentPlayer().Get_isHuman();
        }
    }
}
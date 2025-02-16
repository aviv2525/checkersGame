using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CheckersLogic;

namespace UIWindows
{
    public class GameManagment
    {
        public enum eBoardSizeOptions
        {
            SmallSize = Board.eBoradSize.Small,
            MediumSize = Board.eBoradSize.Medium,
            LargeSize = Board.eBoradSize.Large,
        }

        private FormGame m_FormGame = new FormGame();
        private Game m_Checkers = new Game();

        public void Run()
        {
            registerToLogicEvents();
            registerToFormEvents();
            m_FormGame.ShowDialog();
        }

        private void registerToLogicEvents()
        {
            m_Checkers.NewGameStarted += m_Checkers_NewGameStarted;
            m_Checkers.BoardUpdated += m_Checkers_BoardUpdated;
            m_Checkers.GameEnded += m_Checkers_GameEnded;
        }

        private void registerToFormEvents()
        {
            m_FormGame.GameDetailsFilled += m_FormGame_GameDetailsFilled;
            m_FormGame.MoveEnterd += m_FormGame_MoveEnterd;
            m_FormGame.MessageBoxAnswered += m_FormGame_MessageBoxAnswered;
        }

        private void m_Checkers_GameEnded(object sender, EventArgs e)
        {
            GameEndedEventArgs ge = e as GameEndedEventArgs;

            m_FormGame.CreateYesNoMessageBox(ge.Message, "Damka");
        }

        private void m_FormGame_MessageBoxAnswered(object sender, EventArgs e)
        {
            MessageBoxAnsweredEventArgs mba = e as MessageBoxAnsweredEventArgs;

            if (mba.IsAnsweredYes)
            {
                m_Checkers_NewGameStarted(sender, e);
            }
            else
            {
                m_FormGame.Close();
            }
        }

        private void m_FormGame_MoveEnterd(object sender, EventArgs e)
        {
            MoveEnterdEventArgs me = e as MoveEnterdEventArgs;
            Move newMove = new Move(me.From, me.To);
            string errorMessage;
            bool isLegalMove;

            isLegalMove = m_Checkers.ImplementMove(newMove, out errorMessage);
            if (!isLegalMove)
            {
                m_FormGame.ErrorMessageBox(errorMessage);
            }
        }

        private void m_Checkers_BoardUpdated(object sender, EventArgs e)
        {
            BoardUpdatedEventArgs bu = e as BoardUpdatedEventArgs;
            List<Point> pointsToAddOnBoard = new List<Point>();
            List<Point> pointsToEraseOnBoard = new List<Point>();
            List<Point> pointsToEnableOnBoard;
            List<Point> pointsToDisableOnBoard;
            string playerToolImage;

            pointsToAddOnBoard.Add(bu.LastMove.To);
            pointsToEraseOnBoard.Add(bu.LastMove.From);
            pointsToEnableOnBoard = createPointsListFromPlayerGameTools(m_Checkers.CurrentPlayer.PlayerTools);
            pointsToDisableOnBoard = createPointsListFromPlayerGameTools(m_Checkers.NextPlayer.PlayerTools);
            if (bu.LastMove.IsSkipMove)
            {
                pointsToEraseOnBoard.Add(bu.LastMove.Eaten);
                pointsToEnableOnBoard.Add(bu.LastMove.Eaten);
            }

            getImagePlayerTool(out playerToolImage);
            markCurrentPlayer();
            m_FormGame.AddGameToolsToGameBoard(pointsToAddOnBoard, playerToolImage);
            m_FormGame.EraseGameToolsFromGameBoard(pointsToEraseOnBoard);
            m_FormGame.EnableGameButtonBoxes(pointsToEnableOnBoard);
            m_FormGame.DisableGameButtonBoxes(pointsToDisableOnBoard);
        }

        private void markCurrentPlayer()
        {
            m_FormGame.MarkCurrentPlayerLabel(m_Checkers.CurrentPlayer.Name);
        }

        private void getImagePlayerTool(out string o_ImagePlayerGameTool)
        {
            char playerToolSign = m_Checkers.LastGameToolPlaced.Sign;

            if (playerToolSign == (char)GameTool.eSigns.PlayerO)
            {
                o_ImagePlayerGameTool = ((char)GameTool.eSigns.PlayerO).ToString();
            }
            else if (playerToolSign == (char)GameTool.eSigns.PlayerOKing)
            {
                o_ImagePlayerGameTool = ((char)GameTool.eSigns.PlayerOKing).ToString();
            }
            else if (playerToolSign == (char)GameTool.eSigns.PlayerX)
            {
                o_ImagePlayerGameTool = ((char)GameTool.eSigns.PlayerX).ToString();
            }
            else
            {
                o_ImagePlayerGameTool =((char)GameTool.eSigns.PlayerXKing).ToString();
            }
        }

        private List<Point> createPointsListFromPlayerGameTools(List<GameTool> i_PlayerTools)
        {
            List<Point> result = new List<Point>();

            foreach (GameTool current in i_PlayerTools)
            {
                result.Add(current.Location);
            }

            return result;
        }

        private void m_FormGame_GameDetailsFilled(object sender, EventArgs e)
        {
            DetailsFilledEventArgs df = e as DetailsFilledEventArgs;

            m_Checkers.InitializeStartingDetails(df.GameBoardSize, df.Player1Name, df.Player2Name, df.IsPlayer2PC);
        }

        private void m_Checkers_NewGameStarted(object sender, EventArgs e)
        {
            List<Point> player1GameToolsPoints = new List<Point>();
            List<Point> player2GameToolsPoints = new List<Point>();
            int player1Score, player2Score;
            Button player1GameToolButton = new Button();
            Button player2GameToolButton = new Button();

            player1GameToolButton.Text = ((char) GameTool.eSigns.PlayerO).ToString(); // "O"
            player2GameToolButton.Text = ((char)GameTool.eSigns.PlayerX).ToString(); // "X"

            m_FormGame.ResetGameBoard();
            m_Checkers.ResetGame();
            player1GameToolsPoints = createPointsListFromPlayerGameTools(m_Checkers.CurrentPlayer.PlayerTools);
            player2GameToolsPoints = createPointsListFromPlayerGameTools(m_Checkers.NextPlayer.PlayerTools);
            m_FormGame.AddGameToolsToGameBoard(player1GameToolsPoints, player1GameToolButton.Text);
            m_FormGame.AddGameToolsToGameBoard(player2GameToolsPoints, player2GameToolButton.Text);
            m_FormGame.DisableGameButtonBoxes(player2GameToolsPoints);
            enabledBufferZone();
            player1Score = m_Checkers.CurrentPlayer.Score;
            player2Score = m_Checkers.NextPlayer.Score;
            m_FormGame.UpdateScoreBoard(player1Score, player2Score);
            markCurrentPlayer();
        }

        private void enabledBufferZone()
        {
            List<Point> enabledBufferZonePoints = new List<Point>();
            int startLoopIndex = (m_Checkers.Board.Height / 2) - 1;
            int endLoopIndex = (m_Checkers.Board.Height / 2) + 1;

            for (int i = startLoopIndex; i < endLoopIndex; i++)
            {
                for (int j = 0; j < m_Checkers.Board.Width; j++)
                {
                    if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        enabledBufferZonePoints.Add(new Point(i, j));
                    }
                }
            }

            m_FormGame.EnableGameButtonBoxes(enabledBufferZonePoints);
        }
    }
}
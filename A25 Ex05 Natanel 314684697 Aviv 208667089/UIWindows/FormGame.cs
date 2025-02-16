using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CheckersLogic;

namespace UIWindows
{
    public partial class FormGame : Form
    {


        private const int k_ButtonWidth = 50;
        private const int k_ButtonHeight = 50;
        private const int k_StartingButtonX = 0;
        private const int k_StartingButtonY = 40;
        private const int k_WidthExtention = 20;
        private const int k_HeightExtention = 80;

        public event EventHandler GameDetailsFilled;

        public event EventHandler MoveEnterd;

        public event EventHandler MessageBoxAnswered;

        private readonly FormGameSettings r_FormGameSettings = new FormGameSettings();
        private ButtonsGame[,] buttonMatrix;
        private ButtonsGame buttonPressed;
        private Label labelPlayer1 = new Label();
        private Label labelPlayer2 = new Label();
        private bool anotherButtonPressed = false;

        public FormGame()
        {
            InitializeComponent();
        }

        private void buttonBox_Click(object sender, EventArgs e)
        {
            ButtonsGame currentGameToolPressed = sender as ButtonsGame;
            MoveEnterdEventArgs me;
            if (currentGameToolPressed.IsEnabled)
            {
                if (anotherButtonPressed)
                {
                    if (currentGameToolPressed.Text == "")
                    {
                        me = createMoveEnterdEventArgs(buttonPressed.PlaceOnBoard, currentGameToolPressed.PlaceOnBoard);
                        OnMoveEnterd(me);
                        anotherButtonPressed = false;
                        buttonPressed.IsEnabled = true;
                        buttonPressed.BackColor=Color.White;
                    }
                }
                else
                {
                    if (currentGameToolPressed.Text != "")
                    {
                        buttonPressed = currentGameToolPressed;
                        currentGameToolPressed.IsEnabled = false;
                        anotherButtonPressed = true;
                        buttonPressed.BackColor = Color.LightBlue;
                    }
                }
            }
            else
            {
                anotherButtonPressed = false;
                currentGameToolPressed.IsEnabled = true;
                buttonPressed.BackColor = Color.White;

            }
        }
        public void ErrorMessageBox(string i_ErrorMessage)
        {
            MessageBox.Show(i_ErrorMessage);
        }
        public void ResetGameBoard()
        {
            foreach (ButtonsGame currentGameTool in buttonMatrix)
            {
                currentGameTool.BackColor = Color.Gray;
                currentGameTool.Text = null;
            }
        }
        private MoveEnterdEventArgs createMoveEnterdEventArgs(Point i_From, Point i_To)
        {
            MoveEnterdEventArgs result = new MoveEnterdEventArgs(i_From, i_To);

            return result;
        }
        public void m_FormGameSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (string.IsNullOrEmpty(r_FormGameSettings.Player1Name))
            {
                r_FormGameSettings.Player1Name = "Player 1";
            }

            if (string.IsNullOrEmpty(r_FormGameSettings.Player2Name))
            {
                r_FormGameSettings.Player2Name = "Player 2";
            }

            if (r_FormGameSettings.IsPlayer2PC)
            {
                r_FormGameSettings.Player2Name = "Computer";
            }

            DetailsFilledEventArgs df = new DetailsFilledEventArgs(
            r_FormGameSettings.Player1Name,
            r_FormGameSettings.Player2Name,
            r_FormGameSettings.BoardSize,
            r_FormGameSettings.IsPlayer2PC);
            buttonMatrix = new ButtonsGame[r_FormGameSettings.BoardSize, r_FormGameSettings.BoardSize];
            setFormBoarders();
            createButtonMatrix();
            setPlayersLabels();
            OnGameDetailsFiled(df);
        }
        public void MarkCurrentPlayerLabel(string i_CurrentPlayerName)
        {
            if (i_CurrentPlayerName == r_FormGameSettings.Player1Name)
            {
                labelPlayer1.ForeColor = Color.Blue;
                labelPlayer2.ForeColor = Color.Black;
            }
            else if (i_CurrentPlayerName == r_FormGameSettings.Player2Name)
            {
                labelPlayer2.ForeColor = Color.Red;
                labelPlayer1.ForeColor = Color.Black;
            }
        }
        private void setPlayersLabels()
        {
            int playerStartingScore = 0;

            setPlayersLabelLocation();
            setPlayerLabelsFont();
            UpdateScoreBoard(playerStartingScore, playerStartingScore);
            this.Controls.Add(labelPlayer1);
            this.Controls.Add(labelPlayer2);
        }
        private void setPlayerLabelsFont()
        {
            labelPlayer1.Font = new Font(labelPlayer1.Font, FontStyle.Bold);
            labelPlayer2.Font = new Font(labelPlayer2.Font, FontStyle.Bold);
        }
        private void setPlayersLabelLocation()
        {
            ButtonsGame middleButton = buttonMatrix[0, (r_FormGameSettings.BoardSize / 2) - 1];
            int gameButtonWidth = middleButton.Width;
            int gameButtonHeight = middleButton.Height / 2;
            Point middle = middleButton.Location;
            Point player1LabelLocation = middle, player2LabelLocation = middle;

            player1LabelLocation.Offset(-middleButton.Width, -gameButtonHeight);
            player2LabelLocation.Offset(middleButton.Width, -gameButtonHeight);
            labelPlayer1.Location = player1LabelLocation;
            labelPlayer1.AutoSize = true;
            labelPlayer2.Location = player2LabelLocation;
            labelPlayer2.AutoSize = true;
        }
        public void UpdateScoreBoard(int i_Player1Score, int i_Player2Score)
        {
            labelPlayer1.Text = string.Format("{0}: {1}", r_FormGameSettings.Player1Name, i_Player1Score);
            labelPlayer2.Text = string.Format("{0}: {1}", r_FormGameSettings.Player2Name, i_Player2Score);
        }
        private void setFormBoarders()
        {
            this.Size = new Size((r_FormGameSettings.BoardSize * k_ButtonWidth) + k_WidthExtention, (r_FormGameSettings.BoardSize * k_ButtonHeight) + k_HeightExtention);
        }
        private void createButtonMatrix()
        {
            bool newLine = false, isFirstButton = true;
            Button lastButtonInMatrix = new Button();

            for (int i = 0; i < r_FormGameSettings.BoardSize; i++)
            {
                for (int j = 0; j < r_FormGameSettings.BoardSize; j++)
                {
                    ButtonsGame currentButton = new ButtonsGame(i, j);
                    setButtonLocation(currentButton, newLine, isFirstButton, lastButtonInMatrix);
                    intializeButtons(currentButton);
                    buttonMatrix[i, j] = currentButton;
                    this.Controls.Add(currentButton);
                    newLine = false;
                    isFirstButton = false;
                    lastButtonInMatrix = currentButton;
                }

                newLine = true;
            }
        }

        private void intializeButtons(ButtonsGame i_CurrentButton)
        {
            setButtonFigure(i_CurrentButton);
            i_CurrentButton.Enabled = false;
            i_CurrentButton.Click += buttonBox_Click;
        }

        private void setButtonFigure(ButtonsGame i_CurrentButton)
        {
            i_CurrentButton.Height = k_ButtonHeight;
            i_CurrentButton.Width = k_ButtonWidth;
        }

        private void setButtonLocation(
            ButtonsGame i_CurrentButton,
            bool i_NewLine,
            bool i_IsFirstButton,
            Button i_LastButtonInMatrix)
        {
            Point newLocation;
            int buttonBoxMatrixMaxLine = r_FormGameSettings.BoardSize - 1;
            int buttonBoxMatrixMaxCol = r_FormGameSettings.BoardSize - 1;

            if (i_IsFirstButton)
            {
                newLocation = new Point(k_StartingButtonX, k_StartingButtonY);
            }
            else
            {
                newLocation = i_LastButtonInMatrix.Location;
                if (!i_NewLine)
                {
                    newLocation.Offset(i_LastButtonInMatrix.Width, 0);
                }
                else
                {
                    newLocation.X = k_StartingButtonX;
                    newLocation.Offset(0, i_LastButtonInMatrix.Height);
                }
            }

            i_CurrentButton.Location = newLocation;
        }

        protected virtual void OnGameDetailsFiled(DetailsFilledEventArgs df)
        {
            if (GameDetailsFilled != null)
            {
                GameDetailsFilled(this, df);
            }
        }

        protected virtual void OnMoveEnterd(MoveEnterdEventArgs me)
        {
            if (MoveEnterd != null)
            {
                MoveEnterd(this, me);
            }
        }

        public void AddGameToolsToGameBoard(List<Point> i_PointsToAdd, string i_GameToolImage)
        {
            foreach (Point currentPoint in i_PointsToAdd)
            {
                setButtonBox(currentPoint, i_GameToolImage);
            }

            EnableGameButtonBoxes(i_PointsToAdd);
        }

        private void setButtonBox(Point i_CurrentPoint, string i_GameToolImage)
        {
            buttonMatrix[i_CurrentPoint.X, i_CurrentPoint.Y].Text = i_GameToolImage;
        }

        public void EraseGameToolsFromGameBoard(List<Point> i_PointsToErase)
        {
            foreach (Point currentPoint in i_PointsToErase)
            {
                buttonMatrix[currentPoint.X, currentPoint.Y].Text = null;
            }
        }

        public void EnableGameButtonBoxes(List<Point> i_GameButtonToEnable)
        {
            foreach (Point currentPoint in i_GameButtonToEnable)
            {
                buttonMatrix[currentPoint.X, currentPoint.Y].Enabled = true;
                buttonMatrix[currentPoint.X, currentPoint.Y].BackColor = Color.White;
                buttonMatrix[currentPoint.X, currentPoint.Y].IsEnabled = true;
            }
        }

        public void DisableGameButtonBoxes(List<Point> i_GameButtonToDisable)
        {
            foreach (Point currentPoint in i_GameButtonToDisable)
            {
                buttonMatrix[currentPoint.X, currentPoint.Y].Enabled = false;
                buttonMatrix[currentPoint.X, currentPoint.Y].IsEnabled = false;
            }
        }

        public void CreateYesNoMessageBox(string i_MessageBoxString, string i_Caption)
        {
            DialogResult dialogResult = MessageBox.Show(i_MessageBoxString, i_Caption, MessageBoxButtons.YesNo);
            MessageBoxAnsweredEventArgs mba;
            bool isMessageBoxAnswerIsYes = false;

            if (dialogResult == DialogResult.Yes)
            {
                isMessageBoxAnswerIsYes = true;
            }
            else if (dialogResult == DialogResult.No)
            {
                isMessageBoxAnswerIsYes = false;
            }

            mba = new MessageBoxAnsweredEventArgs(isMessageBoxAnswerIsYes);
            OnMessageBoxAnswered(mba);
        }

        private void OnMessageBoxAnswered(MessageBoxAnsweredEventArgs mba)
        {
            if (MessageBoxAnswered != null)
            {
                MessageBoxAnswered(this, mba);
            }
        }
        private void InitializeComponent()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Damka";
            this.BackColor = Color.Silver;
            this.Load += FormGame_Load;
        }

        private void FormGame_Load(object sender, EventArgs e)
        {
            r_FormGameSettings.FormClosed += m_FormGameSettings_FormClosed;
            r_FormGameSettings.ShowDialog();
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        #endregion
    }
}

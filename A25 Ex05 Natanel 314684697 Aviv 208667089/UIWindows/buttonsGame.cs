using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CheckersLogic;


namespace UIWindows
{
    public class ButtonsGame: Button
    {
        private Point m_PlaceOnBoard;
        private bool m_IsEnabled;
        public ButtonsGame(int i_Row, int i_Col)
        {
            m_PlaceOnBoard = new Point(i_Row, i_Col);
        }

        public Point PlaceOnBoard
        {
            get
            {
                return m_PlaceOnBoard;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return m_IsEnabled;
            }

            set
            {
                m_IsEnabled = value;
            }
        }
    }
}

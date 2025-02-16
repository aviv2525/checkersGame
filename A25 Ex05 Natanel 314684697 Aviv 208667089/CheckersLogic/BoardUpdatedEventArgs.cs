﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CheckersLogic
{
    public class BoardUpdatedEventArgs : EventArgs
    {
        private Move m_LastMove;
        public BoardUpdatedEventArgs(Move i_LastMove)
        {
            m_LastMove = i_LastMove;
        }
        public Move LastMove
        {
            get
            {
                return m_LastMove;
            }
        }
    }
}

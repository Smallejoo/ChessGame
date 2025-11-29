using Chess.Classes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChessGame
{
    public class GameForm : Form
    {
        private const int BoardSize = 8;
        //2d buttons array 
        private Button[,] _buttons = new Button[BoardSize, BoardSize];
        //memory board
        private ChessBoard _board;
        // if there will be an AI 
        private bool _vsAi;
        //if its over the internet 
        private bool _isOnline = false;
        private NetworkManager? _network;
        
        private PieceColor _myColor = PieceColor.White;


        private ChessAi _ai = new ChessAi();

        private PieceColor _currentPlayer = PieceColor.White;

        //place holder 
        private Point _selectedSquare = null;

        //place holder 
        private HashSet<(int row, int col)> _legalMoves = new();

        public GameForm(bool vsAi)
        {
            _vsAi = vsAi;
            Text = "Chess - White to move";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(640, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            //init the board and init all figures 
            _board = new ChessBoard();
            _board.SetupInitialPosition();
            // creating all the buttons and visual stuff 
            CreateBoardButtons();
            RefreshBoardUI();
        }
        //still not working 
        public GameForm(NetworkManager network, PieceColor myColor)
        {
            _isOnline = true;
            _network = network;
            _myColor = myColor;
            _vsAi = false; // no AI in online mode

            Text = "Chess - Online";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(640, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            _board = new ChessBoard();
            _board.SetupInitialPosition();

            CreateBoardButtons();
            RefreshBoardUI();

            // subscribe to incoming moves from the other player
            _network.OnMoveReceived += Network_OnMoveReceived;
        }
        private void Network_OnMoveReceived(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Network_OnMoveReceived(fromRow, fromCol, toRow, toCol)));
                return;
            }

            _board.MovePiece(fromRow, fromCol, toRow, toCol);
            ClearSelection();
            SwitchPlayer();
            RefreshBoardUI();

            // you can also add checkmate detection here if you want
        }



        // creating the visual buttons 
        private void CreateBoardButtons()
        {
            int tileSize = ClientSize.Width / BoardSize;
            //2d 
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {

                    //perfect math 
                    var btn = new Button();
                    btn.Size = new Size(tileSize, tileSize);
                    btn.Location = new Point(col * tileSize, row * tileSize);
                    btn.Tag = new Point(row, col);
                    //adding listner 
                    btn.Click += Square_Click;
                    //adding the button to the over all buttons arry 
                    _buttons[row, col] = btn;
                    Controls.Add(btn);
                }
            }
        }

        private void Square_Click(object sender, EventArgs e)
        {

            // if the click is not his figure just return 
            if (_isOnline && _currentPlayer != _myColor)
                return;

            var btn = (Button)sender!;
            var pos = (Point)btn.Tag;
            int row = pos.X;
            int col = pos.Y;

            var clickedPiece = _board.GetPiece(row, col);

            // no selection yet then we are waiting for aselection 
            if (_selectedSquare == null)
            {
                // if not our figure return .
                if (clickedPiece == null || clickedPiece.Color != _currentPlayer)
                    return;
                // you picked a correct one
                SelectSquare(row, col, clickedPiece);
                return;
            }

            // there is a selected square 
            var sel = _selectedSquare.Value;

            // click same square → unselect
            if (sel.X == row && sel.Y == col)
            {
                ClearSelection();
                return;
            }

            // click a legal move → move piece
            // click a legal move → move piece
            // click a legal move → move piece
            // click a legal move → move piece

            // is the clicked place contains in leagal moves ?  then move there 
            if (_legalMoves.Contains((row, col)))
            {
                int fromRow = sel.X;
                int fromCol = sel.Y;
                int toRow = row;
                int toCol = col;
                // update and move the game 
                _board.MovePiece(fromRow, fromCol, toRow, toCol);
                ClearSelection();
                SwitchPlayer();
                RefreshBoardUI();

                // checkmate 
                if (_board.IsCheckmate(_currentPlayer))
                {
                    string loser = _currentPlayer == PieceColor.White ? "White" : "Black";
                    string winner = _currentPlayer == PieceColor.White ? "Black" : "White";

                    var result = MessageBox.Show(
                        $"{loser} is checkmated!\n{winner} wins.\n\nPlay again?",
                        "Checkmate",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        _board.SetupInitialPosition();
                        _currentPlayer = PieceColor.White;
                        ClearSelection();
                        RefreshBoardUI();
                    }
                    else
                    {
                        var menu = new MainMenuForm();
                        menu.Show();
                        Close();
                    }

                    return;
                }

                // ONLINE: send move to other player // still at work 
                if (_isOnline)
                {
                    _network?.SendMove(fromRow, fromCol, toRow, toCol);
                }
                else
                {
                    //  OFFLINE: AI turn // working  if ai is enabled then do this . 
                    if (_vsAi && _currentPlayer == PieceColor.Black)
                    {
                        DoAiTurn();
                    }
                }

                return;
            }





            // click another own piece → change selection
            if (clickedPiece != null && clickedPiece.Color == _currentPlayer)
            {
                SelectSquare(row, col, clickedPiece);
                return;
            }

            
        }
        // save the square and calculate the moves of the current figure 
        // and refresh UI 
        private void SelectSquare(int row, int col, BasicPiece piece)
        {
            _selectedSquare = new Point(row, col);
            _legalMoves = GetSafeMoves(piece);   // only moves that keep own king safe
            RefreshBoardUI();
        }

        
        private void ClearSelection()
        {
            _selectedSquare = null;
            _legalMoves.Clear();
            RefreshBoardUI();
        }


        private void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            Text = $"Chess - {(_currentPlayer == PieceColor.White ? "White" : "Black")} to move";
        }

        // 
        private HashSet<(int row, int col)> GetSafeMoves(BasicPiece piece)
        {
            var safe = new HashSet<(int, int)>();

            foreach (var move in piece.GetLegalMoves(_board))
            {
                int fromRow = piece.Row;
                int fromCol = piece.Col;
                var captured = _board.GetPiece(move.row, move.col);

                // simulate
                _board.MovePiece(fromRow, fromCol, move.row, move.col);
                bool inCheck = _board.IsKingInCheck(piece.Color);

                // undo
                _board.MovePiece(move.row, move.col, fromRow, fromCol);
                _board.SetPiece(move.row, move.col, captured);

                if (!inCheck)
                    safe.Add(move);
            }

            return safe;
        }

        private void DoAiTurn()
        {
            // just in case
            if (_board.IsCheckmate(_currentPlayer))
                return;

            bool moved = _ai.MakeBestMove(_board, _currentPlayer);

            RefreshBoardUI();

            if (!moved)
            {
                // no legal moves for AI
                if (_board.IsKingInCheck(_currentPlayer))
                {
                    MessageBox.Show("AI has no legal moves and is in check. You win!");
                }
                else
                {
                    MessageBox.Show("Stalemate. It's a draw.");
                }
                return;
            }

            // after AI move, give turn back to human
            SwitchPlayer();
            RefreshBoardUI();

            if (_board.IsCheckmate(_currentPlayer))
            {
                string loser = _currentPlayer == PieceColor.White ? "White" : "Black";
                string winner = _currentPlayer == PieceColor.White ? "Black" : "White";

                var result = MessageBox.Show(
                    $"{loser} is checkmated!\n{winner} wins.\n\nPlay again?",
                    "Checkmate",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    _board.SetupInitialPosition();
                    _currentPlayer = PieceColor.White;
                    ClearSelection();
                    RefreshBoardUI();
                }
                else
                {
                    var menu = new MainMenuForm();
                    menu.Show();
                    Close();
                }
            }
        }


        // importent function refreshing the visual side 
        // every change there is this function has to be called to show the changes .
        private void RefreshBoardUI()
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    var btn = _buttons[row, col];
                    var piece = _board.GetPiece(row, col);

                    // base board colors  ... what is the base color of this squre overall 
                    bool isDark = (row + col) % 2 == 1;
                    btn.BackColor = isDark ? Color.SaddleBrown : Color.Beige;

                    // highlight selected square (yellow)
                    if (_selectedSquare.HasValue && _selectedSquare.Value.X == row && _selectedSquare.Value.Y == col)
                    {
                        btn.BackColor = Color.Yellow;
                    }
                    // highlight legal moves (green)
                    else if (_legalMoves.Contains((row, col)))
                    {
                        btn.BackColor = Color.LightGreen;
                    }

                    if (piece == null)
                    {
                        btn.BackgroundImage = null;
                        btn.Text = "";
                    }
                    else
                    {
                        btn.BackgroundImage = piece.GetImage();
                        btn.BackgroundImageLayout = ImageLayout.Zoom;  // auto-resize
                        btn.Text = "";
                    }

                   
                }
            }
        }
    }
}

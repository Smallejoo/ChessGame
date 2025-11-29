using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGame
{
    public class MainMenuForm : Form
    {
        private Button btnHumanVsHuman;
        private Button btnHumanVsAi;
        private Button btnOnlineHost;
        private Button btnOnlineJoin;

        public MainMenuForm()
        {
            Text = "Chess - Main Menu";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(300, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            btnHumanVsHuman = new Button
            {
                Text = "1 vs 1 (Same PC)",
                Size = new Size(200, 40),
                Location = new Point(50, 40)
            };
            btnHumanVsHuman.Click += (s, e) =>
            {
                var gameForm = new GameForm(vsAi: false);
                gameForm.Show();
                Hide();
            };

            btnHumanVsAi = new Button
            {
                Text = "Vs AI ",
                Size = new Size(200, 40),
                Location = new Point(50, 100)
            };
            btnHumanVsAi.Click += (s, e) =>
            {
                var gameForm = new GameForm(vsAi: true);
                gameForm.Show();
                Hide();
            };

            Controls.Add(btnHumanVsHuman);
            Controls.Add(btnHumanVsAi);
        }
    }
}

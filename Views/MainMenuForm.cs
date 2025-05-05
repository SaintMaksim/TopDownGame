using System;
using System.Drawing;
using System.Windows.Forms;

namespace TopDownGame.Views
{
    public class MainMenuForm : Form
    {
        public event Action NewGameClicked;
        public event Action LoadGameClicked;
        public event Action SaveGameClicked; // Новое событие
        public event Action ExitClicked;

        public MainMenuForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Nowheria";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackgroundImage = Properties.Resources.Village;
            this.BackgroundImageLayout = ImageLayout.Stretch;



            var titleLabel = new Label
            {
                Text = "NOWHERIA",
                Font = new Font("Arial", 50, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            titleLabel.Location = new Point(
                (this.ClientSize.Width - titleLabel.Width) / 2 - 125,
                75);



            var newGameButton = CreateMenuButton("Новая игра", 200);
            var loadGameButton = CreateMenuButton("Загрузить игру", 300);
            var exitButton = CreateMenuButton("Выход", 400);

            newGameButton.Click += (s, e) => NewGameClicked?.Invoke();
            loadGameButton.Click += (s, e) => LoadGameClicked?.Invoke();
            exitButton.Click += (s, e) => ExitClicked?.Invoke();

            this.Controls.Add(titleLabel);
            this.Controls.Add(newGameButton);
            this.Controls.Add(loadGameButton);
            this.Controls.Add(exitButton);

        }

        private Button CreateMenuButton(string text, int yPosition)
        {
            return new Button
            {
                Text = text,
                Size = new Size(300, 60),
                Location = new Point((ClientSize.Width - 300) / 2, yPosition),
                Font = new Font("Arial", 16),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    if (e.CloseReason == CloseReason.UserClosing)
        //    {
        //        var result = MessageBox.Show("Вы действительно хотите выйти?", "Выход",
        //            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //        if (result == DialogResult.No)
        //        {
        //            e.Cancel = true;
        //        }
        //    }
        //    base.OnFormClosing(e);
        //}

        public string LastSaveSlot { get; set; } = "save1";
    }
}
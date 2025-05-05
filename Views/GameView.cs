using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TopDownGame.Models;
using TopDownGame.Controllers;
using System;

namespace TopDownGame.Views
{
    public class GameView : Form
    {
        private readonly GameModel _model;
        private readonly QuestManager _questManager;
        public event Action PauseRequested;

        public GameView(GameModel model, QuestManager questManager)
        {
            _model = model;
            _questManager = questManager;
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Nowheria";
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(_model.CurrentZone, ClientRectangle);
            foreach (var npc in _model.Npcs.OrderBy(n => n.Y))
            {
                npc.Draw(e.Graphics);
            }
            foreach (var collider in _model.CurrentColliders)
            {
                e.Graphics.DrawRectangle(Pens.Transparent, collider.Bounds);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0, 0)), collider.Bounds);
            }
            _model.Player.Draw(e.Graphics);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && !e.Handled)
            {
                e.Handled = true;
                PauseRequested?.Invoke();
            }
            else if (e.KeyCode == Keys.I && !e.Handled)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                var inventoryForm = new InventoryForm(_model.Player.Inventory);
                inventoryForm.ShowDialog();
            }
            else if (e.KeyCode == Keys.Q && !e.Handled)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                var questLogForm = new QuestLogForm(_questManager);
                questLogForm.ShowDialog();
            }
            else if (e.KeyCode == Keys.E && !e.Handled)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            base.OnKeyDown(e);
        }
    }
}
using System.Drawing;
using System.Windows.Forms;
using TopDownGame.Models;
using TopDownGame.Services;

namespace TopDownGame.Views
{
    public class SaveLoadDialog : Form
    {
        private readonly GameModel _model;
        private readonly bool _isSaving;

        public SaveLoadDialog(GameModel model, bool isSaving)
        {
            _model = model;
            _isSaving = isSaving;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = _isSaving ? "Сохранить игру" : "Загрузить игру";
            Size = new Size(300, 150);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var label = new Label
            {
                Text = _isSaving ? "Сохранить текущий прогресс?" : "Загрузить последнее сохранение?",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 10, 0, 10)
            };

            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel };

            okButton.Click += (s, e) => ProcessSaveLoad();

            buttonPanel.Controls.Add(okButton);
            buttonPanel.Controls.Add(cancelButton);
            okButton.SetBounds(10, 5, 100, 25);
            cancelButton.SetBounds(120, 5, 100, 25);

            Controls.Add(label);
            Controls.Add(buttonPanel);
        }

        private void ProcessSaveLoad()
        {
            string saveName = "autosave"; // Фиксированное имя сохранения

            if (_isSaving)
            {
                _model.SaveGame(saveName);
                MessageBox.Show("Игра сохранена", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _model.LoadGame(saveName);
                MessageBox.Show("Игра загружена", "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            DialogResult = DialogResult.OK;
        }
    }
}
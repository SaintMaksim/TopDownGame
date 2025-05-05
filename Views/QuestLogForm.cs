using System.Drawing;
using System.Windows.Forms;
using TopDownGame.Controllers;

namespace TopDownGame.Views
{
    public class QuestLogForm : Form
    {
        public QuestLogForm(QuestManager questManager)
        {
            Text = "Журнал квестов";
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 12)
            };

            foreach (var quest in questManager.GetActiveQuests())
            {
                listBox.Items.Add($"{quest.Title}\n{quest.Description}");
            }

            var closeButton = new Button
            {
                Text = "Закрыть",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            closeButton.Click += (s, e) => Close();

            Controls.Add(listBox);
            Controls.Add(closeButton);
        }
    }
}
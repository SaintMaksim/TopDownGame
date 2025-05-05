using System.Drawing;
using System.Windows.Forms;

public class PauseMenu : Form
{
    public PauseMenu()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Пауза";
        this.Size = new Size(300, 200);
        this.StartPosition = FormStartPosition.CenterParent;

        var resumeButton = new Button
        {
            Text = "Продолжить",
            Size = new Size(200, 40),
            Location = new Point(50, 30)
        };
        resumeButton.Click += (s, e) => this.Close();

        var exitButton = new Button
        {
            Text = "Выйти в меню",
            Size = new Size(200, 40),
            Location = new Point(50, 90)
        };
        exitButton.Click += (s, e) =>
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        };

        this.Controls.Add(resumeButton);
        this.Controls.Add(exitButton);
    }
}
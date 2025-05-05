using System;
using System.Windows.Forms;
using TopDownGame.Controllers;
using TopDownGame.Services;
using TopDownGame.Views;

namespace TopDownGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var menu = new MainMenuForm();

            menu.NewGameClicked += () =>
            {
                StartNewGame(menu);
            };

            menu.LoadGameClicked += () =>
            {
                LoadGame(menu);
            };

            menu.ExitClicked += () => Application.Exit();

            Application.Run(menu);
        }

        private static void StartNewGame(Form menuForm)
        {
            var controller = GameController.CreateGame();
            menuForm.Hide();
            controller.RunGame();
            menuForm.Close();
        }

        private static void LoadGame(Form menuForm)
        {
            var controller = GameController.CreateGame();
            if (SaveLoadService.SaveExists("quicksave"))
            {
                var saveData = SaveLoadService.LoadGame("quicksave");
            }
            menuForm.Hide(); 
            controller.RunGame();
            menuForm.Close(); 
        }
    }
}
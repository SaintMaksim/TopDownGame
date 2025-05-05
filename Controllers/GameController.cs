using System;
using System.Drawing;
using System.Windows.Forms;
using TopDownGame.Models;
using TopDownGame.Services;
using TopDownGame.Views;
using static TopDownGame.Models.ZoneManager;
using System.Linq;
using static TopDownGame.Models.ItemNpc;

namespace TopDownGame.Controllers
{
    public class GameController
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private readonly InputController _input;
        private readonly Timer _gameLoop;
        private readonly QuestManager _questManager;
        private readonly MainMenuForm _mainMenu;
        private bool _interactionRequested;
        private bool _isPaused = false;
        private bool _pauseMenuOpen = false;
        private System.Media.SoundPlayer _musicPlayer;

        public GameController()
        {
            var player = new Player(Properties.Resources.Hero);
            _questManager = new QuestManager(player);
            _model = new GameModel(player, _questManager);
            _view = new GameView(_model, _questManager);
            _input = new InputController(
                _model.Player,
                CheckCollision,
                () => _isPaused,
                _view.ClientSize.Width,
                _view.ClientSize.Height
            );
            _mainMenu = new MainMenuForm();

            _mainMenu.SaveGameClicked += ShowSaveMenu;
            _mainMenu.LoadGameClicked += ShowLoadMenu;
            _input.OnInteractionRequested += () => _interactionRequested = true;
            _view.KeyDown += (s, e) => _input.HandleKeyDown(s, e);
            _view.KeyUp += (s, e) => _input.HandleKeyUp(s, e);
            _view.PauseRequested += TogglePause;
            _input.OnSaveRequested += ShowSaveMenu;
            _input.OnLoadRequested += ShowLoadMenu;
            _input.OnInteractionRequested += () => _interactionRequested = true;
            _gameLoop = new Timer { Interval = 16 };
            _gameLoop.Tick += (s, e) => Update();
            _questManager.OnGameComplete += CompleteGame;
            PlayBackgroundMusic();
        }

        private void TogglePause()
        {
            if (_pauseMenuOpen) return;

            _isPaused = !_isPaused;
            _pauseMenuOpen = true;

            if (_isPaused)
            {
                _gameLoop.Stop();
                ShowPauseMenu();
            }
            else
            {
                _gameLoop.Start();
                _pauseMenuOpen = false;
            }
        }

        private void ShowSaveMenu()
        {
            var menu = new SaveLoadDialog(_model, true);
            menu.ShowDialog();
        }

        private void ShowPauseMenu()
        {
            var pauseMenu = new PauseMenu();
            pauseMenu.FormClosed += (s, e) => {
                _pauseMenuOpen = false;
                if (_isPaused)
                {
                    _isPaused = false;
                    _gameLoop.Start();
                }
            };

            var result = pauseMenu.ShowDialog();

            if (result == DialogResult.Abort)
            {
                _view.Close();
                ShowMainMenu();
            }
        }

        private void ShowMainMenu()
        {
            _mainMenu.ShowDialog();
        }

        private bool CheckCollision(Rectangle bounds)
        {
            return _model.CurrentColliders.Any(c => c.CheckCollision(bounds));
        }

        private void Update()
        {
            if (_isPaused) return;

            _input.Update();
            CheckNpcInteraction();
            CheckZoneTransition();
            CheckMapBoundaries();
            _view.Invalidate();
        }

        private bool AllStonesActivated()
        {
            return _model.Npcs.OfType<ItemNpc>()
                             .Where(item => item.CurrentItemType == ItemType.Statue ||
                                           item.CurrentItemType == ItemType.Statue_Active)
                             .All(item => item.IsCollected);
        }

        private void CheckNpcInteraction()
        {
            if (!_interactionRequested) return;
            _interactionRequested = false;

            for (int i = _model.Npcs.Count - 1; i >= 0; i--)
            {
                var npc = _model.Npcs[i];

                if (IsPlayerNearNpc(npc))
                {
                    if (npc is ItemNpc item)
                    {
                        var result = item.Interact();
                        if (result == DialogResult.OK)
                        {
                            if (item.CurrentItemType == ItemType.Statue ||
                                item.CurrentItemType == ItemType.Statue_Active)
                            {
                                if (AllStonesActivated())
                                {
                                    _model.Player.CompletedQuests.Add("stones_activated");
                                    _questManager.UpdateQuests();
                                }
                            }
                            else if (item.CurrentItemType == ItemType.GoldChest)
                            {
                                _model.Player.Inventory.AddGold(300);
                                _model.IsVillageChestTaken = true;
                                MessageBox.Show("Вы нашли 300 золота!",
                                              "Сундук",
                                              MessageBoxButtons.OK,
                                              MessageBoxIcon.Information);
                                _model.Npcs.RemoveAt(i);
                            }
                            else
                            {
                                _model.Player.Inventory.AddItem(item.ItemId);
                                if (item.ShouldBeRemoved())
                                {
                                    _model.Npcs.RemoveAt(i);
                                }
                            }
                            _view.Invalidate();
                        }
                        return;
                    }
                    else
                    {
                        var result = npc.Interact(_model.Player);
                        if (result == DialogResult.OK)
                        {
                            if (npc.Name == "Лесник")
                            {
                                if (!_questManager.GetActiveQuests().Any(q => q.Id == "quest_find_cat") &&
                                    !_model.Player.CompletedQuests.Contains("quest_find_cat"))
                                {
                                    _questManager.StartQuest("quest_find_cat");
                                }
                                else if (_model.Player.Inventory.GetItemCount("Кот") == 1)
                                {
                                    _questManager.UpdateQuests();
                                }
                            }
                            else if (npc.Name == "Травница")
                            {
                                if (!_questManager.GetActiveQuests().Any(q => q.Id == "quest_herb_collection") &&
                                    !_model.Player.CompletedQuests.Contains("quest_herb_collection"))
                                {
                                    _questManager.StartQuest("quest_herb_collection");
                                }
                                else if (_model.Player.Inventory.GetItemCount("Трава") >= 5)
                                {
                                    _questManager.UpdateQuests();
                                }
                            }
                            else if (npc.Name == "Исследователь")
                            {
                                if (!_questManager.GetActiveQuests().Any(q => q.Id == "quest_statue_activation") &&
                                    !_model.Player.CompletedQuests.Contains("quest_statue_activation"))
                                {
                                    _questManager.StartQuest("quest_statue_activation");
                                }
                                else if (AllStatuesActivated())
                                {
                                    _questManager.UpdateQuests();
                                    _model.Player.CompletedQuests.Add("statues_activated");
                                }
                            }
                            else if (npc.Name == "Извозчик")
                            {
                                var quest = _questManager.GetQuest("quest_gold_collection");
                                if (quest == null) return;
                                if (quest.State == QuestState.NotStarted)
                                {
                                    _questManager.StartQuest("quest_gold_collection");
                                }
                                else if (quest.State == QuestState.InProgress &&
                                        _model.Player.Inventory.Gold >= 1000)
                                {
                                    _questManager.UpdateQuests();
                                }
                            }
                        }

                        return;
                    }
                }
            }
        }
        private bool AllStatuesActivated()
        {
            var statues = _model.Npcs.OfType<ItemNpc>()
                .Where(n => n.CurrentItemType == ItemNpc.ItemType.Statue ||
                           n.CurrentItemType == ItemNpc.ItemType.Statue_Active)
                .ToList();

            return statues.Count >= 2 && statues.All(s => s.IsCollected);
        }

        private void CheckMapBoundaries()
        {
            int playerWidth = _model.Player.Sprite.Width;
            int playerHeight = _model.Player.Sprite.Height;
            int screenWidth = _view.ClientSize.Width;
            int screenHeight = _view.ClientSize.Height;

            _model.Player.X = Clamp(_model.Player.X, 0, screenWidth - playerWidth);
            _model.Player.Y = Clamp(_model.Player.Y, 0, screenHeight - playerHeight);
        }

        private void PlayBackgroundMusic()
        {
            _musicPlayer = new System.Media.SoundPlayer(Properties.Resources.Village1);
            _musicPlayer.PlayLooping();
        }

        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void CheckZoneTransition()
        {
            int playerWidth = _model.Player.Sprite.Width;
            int playerHeight = _model.Player.Sprite.Height;
            int screenWidth = _view.ClientSize.Width;
            int screenHeight = _view.ClientSize.Height;

            Direction? direction = null;

            if (_model.Player.X >= screenWidth - playerWidth)
                direction = Direction.Right;
            else if (_model.Player.X <= 0)
                direction = Direction.Left;
            else if (_model.Player.Y <= 0)
                direction = Direction.Up;
            else if (_model.Player.Y >= screenHeight - playerHeight)
                direction = Direction.Down;

            if (direction.HasValue)
            {
                var currentPos = new Point(_model.Player.X, _model.Player.Y);
                var transition = _model.ZoneManager.GetTransition(_model.CurrentZoneType, direction.Value, currentPos);

                if (transition != null)
                {
                    _model.TransitionToZone(transition.TargetZone);
                    _model.Player.X = transition.SpawnPosition.X;
                    _model.Player.Y = transition.SpawnPosition.Y;
                    _view.Invalidate();
                }
            }
        }

        private bool IsPlayerNearNpc(Npc npc)
        {
            var playerRect = new Rectangle(
                _model.Player.X, _model.Player.Y,
                _model.Player.Sprite.Width, _model.Player.Sprite.Height
            );

            var npcRect = new Rectangle(
                npc.X - 30, npc.Y - 30,
                npc.Sprite.Width + 60, npc.Sprite.Height + 60
            );

            return playerRect.IntersectsWith(npcRect);
        }

        public void ShowLoadMenu()
        {
            var menu = new SaveLoadDialog(_model, false);
            menu.ShowDialog();
        }

        public void RunGame()
        {
            _gameLoop.Start();
            _view.ShowDialog();
        }

        public static GameController CreateGame()
        {
            return new GameController();
        }
        private void CompleteGame()
        {
            _gameLoop.Stop();

            MessageBox.Show("Ты наконец-то полностью отрезвел.\n" +
                          "Ты уже представлешь как приедешь и ляжешь спать\n" +
                          "А пока тебя ждёт долгая дорога",
                          "Конец игры",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);

            _view.Close();
            ShowMainMenu();
        }

    }
}
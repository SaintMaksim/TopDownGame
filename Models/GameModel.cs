using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TopDownGame.Models;
using static TopDownGame.Models.ZoneManager;
using TopDownGame.Services;
using TopDownGame.Controllers;
using static TopDownGame.Models.ItemNpc;
using System.Runtime.InteropServices;

namespace TopDownGame.Models
{
    public enum ZoneType
    {
        Road,
        Village,
        Village2,
        River,
        Village3,
    }

    public class GameModel
    {
        public List<Collider> CurrentColliders { get; } = new List<Collider>();
        public ZoneManager ZoneManager { get; }
        public Player Player { get; }
        public ZoneType CurrentZoneType { get; private set; }
        public List<Npc> Npcs { get; } = new List<Npc>();
        public bool IsVillageChestTaken { get; set; } = false;
        private readonly Dictionary<ZoneType, Bitmap> _zoneResources;
        private readonly QuestManager _questManager;
        private Bitmap _currentZone;

        public Bitmap CurrentZone
        {
            get => _currentZone;
            private set
            {
                _currentZone = value;
                CurrentZoneType = GetZoneTypeFromBitmap(value);
                InitializeNpcs();
            }
        }

        public GameModel(Player player, QuestManager questManager)
        {
            Player = player;
            _questManager = questManager;

            _zoneResources = new Dictionary<ZoneType, Bitmap>
            {
                { ZoneType.Road, Properties.Resources.Road },
                { ZoneType.Village, Properties.Resources.Village },
                { ZoneType.Village2, Properties.Resources.Village2 },
                { ZoneType.Village3, Properties.Resources.Village3 },
                { ZoneType.River, Properties.Resources.River }
            };

            CurrentZone = Properties.Resources.Village;
            CurrentZoneType = ZoneType.Village;
            ZoneManager = new ZoneManager();
            InitializeZoneTransitions();
            InitializeCollidersForZone(ZoneType.Village);
            InitializeNpcs();
            InitializeItems();
        }

        public void TransitionToZone(ZoneType newZone)
        {
            if (_zoneResources.TryGetValue(newZone, out var zoneBitmap))
            {
                CurrentZone = zoneBitmap;
                CurrentZoneType = newZone;
                InitializeCollidersForZone(newZone);
                InitializeNpcs();
                InitializeItems();
            }
        }

        public bool CheckCollision(Rectangle bounds)
        {
            return CurrentColliders.Any(c => c.CheckCollision(bounds));
        }

        public GameSaveData CreateSaveData()
        {
            return new GameSaveData
            {
                PlayerX = Player.X,
                PlayerY = Player.Y,
                Gold = Player.Inventory?.Gold ?? 0,
                InventoryItems = Player.Inventory?.GetAllItems() ?? new Dictionary<string, int>(),
                CompletedQuests = new List<string>(Player.CompletedQuests ?? Enumerable.Empty<string>()),
                ActiveQuests = _questManager.GetActiveQuests()?
                    .Where(q => q != null)
                    .Select(q => q.Id)
                    .ToList() ?? new List<string>(),
                CurrentZone = CurrentZoneType
            };
        }

        public void LoadFromSaveData(GameSaveData saveData)
        {
            Player.X = saveData.PlayerX;
            Player.Y = saveData.PlayerY;

            Player.Inventory.AddGold(saveData.Gold);
            foreach (var item in saveData.InventoryItems)
            {
                Player.Inventory.AddItem(item.Key, item.Value);
            }

            Player.CompletedQuests.Clear();
            foreach (var questId in saveData.CompletedQuests)
            {
                Player.CompletedQuests.Add(questId);
            }

            foreach (var questId in saveData.ActiveQuests)
            {
                _questManager.StartQuest(questId);
            }

            TransitionToZone(saveData.CurrentZone);
        }

        private void InitializeZoneTransitions()
        {
            ZoneManager.AddTransition(ZoneType.Road, ZoneType.Village, Direction.Down);
            ZoneManager.AddTransition(ZoneType.Village, ZoneType.Road, Direction.Up);

            ZoneManager.AddTransition(ZoneType.Village, ZoneType.Village2, Direction.Right);
            ZoneManager.AddTransition(ZoneType.Village2, ZoneType.Village, Direction.Left);

            ZoneManager.AddTransition(ZoneType.Village2, ZoneType.River, Direction.Down);
            ZoneManager.AddTransition(ZoneType.River, ZoneType.Village2, Direction.Up);

            ZoneManager.AddTransition(ZoneType.Village, ZoneType.Village3, Direction.Down);
            ZoneManager.AddTransition(ZoneType.Village3, ZoneType.Village, Direction.Up);
        }

        private void InitializeCollidersForZone(ZoneType zoneType)
        {
            CurrentColliders.Clear();

            switch (zoneType)
            {
                case ZoneType.Village:
                    CurrentColliders.AddRange(new[]
                    {
                        new Collider(14, 200, 146, 140),
                        new Collider(216, 18, 130, 140),
                        new Collider(492, 16, 250, 140),
                        new Collider(445, 280, 60, 60),
                        new Collider(550, 300, 250, 90),
                        new Collider(490, 390, 80, 210),
                        new Collider(490, 390, 200, 60),
                        new Collider(475, 545, 325, 55),
                        new Collider(90, 400, 100, 60),
                        new Collider(304, 400, 100, 60),
                        new Collider(290, 320, 125, 30)
                    });
                    break;

                case ZoneType.Village2:
                    CurrentColliders.AddRange(new[]
                    {
                        new Collider(23, 0, 700, 152),
                        new Collider(635, 162, 120, 170),
                        new Collider(720, 336, 80, 50),
                        new Collider(0, 537, 512, 63)
                    });
                    break;

                case ZoneType.River:
                    CurrentColliders.AddRange(new[]
                    {
                        new Collider(0, 0, 470, 75),
                        new Collider(0, 0, 60, 172),
                        new Collider(0, 260, 800, 63),
                        new Collider(737, 90, 63, 174),
                        new Collider(150, 120, 50, 100),
                        new Collider(56, 170, 290, 100),
                        new Collider(672, 180, 63, 70)
                    });
                    break;

                case ZoneType.Road:
                    CurrentColliders.AddRange(new[]
                    {
                        new Collider(0, 0, 800, 172),
                        new Collider(291, 291, 36, 85),
                        new Collider(443, 220, 100, 50)
                    });
                    break;

                case ZoneType.Village3:
                    CurrentColliders.AddRange(new[]
                    {
                        new Collider(0, 0, 175, 150),
                        new Collider(285, 170, 40, 60),
                        new Collider(150, 170, 40, 60),
                        new Collider(510, 345, 290, 255),
                        new Collider(0, 350, 85, 85),
                        new Collider(85, 375, 75, 50),
                        new Collider(265, 390, 60, 50),
                        new Collider(0, 530, 65, 60),
                        new Collider(215, 525, 70, 55),
                        new Collider(415, 0, 385, 130),
                        new Collider(700, 130, 385, 90),
                        new Collider(675, 135, 100, 90)
                    });
                    break;
            }
        }

        public void InitializeItems()
        {
            if (CurrentZoneType == ZoneType.Village)
            {
                if (!IsVillageChestTaken && !Npcs.Any(n => n is ItemNpc item && item.CurrentItemType == ItemType.GoldChest))
                {
                    Npcs.Add(new ItemNpc(ItemType.GoldChest, new Point(600, 500)));
                }
            }
            else if (CurrentZoneType == ZoneType.Village2)
            {
                Npcs.Add(new ItemNpc(ItemType.Herb, new Point(380, 285)));
                Npcs.Add(new ItemNpc(ItemType.Herb, new Point(110, 400)));
                Npcs.Add(new ItemNpc(ItemType.Herb, new Point(480, 460)));
                Npcs.Add(new ItemNpc(ItemType.Herb, new Point(330, 516)));
                Npcs.Add(new ItemNpc(ItemType.Herb, new Point(190, 470)));
            }
            else if (CurrentZoneType == ZoneType.River)
            {
                Npcs.Add(new ItemNpc(ItemType.Cat, new Point(80, 150)));
            }
            else if (CurrentZoneType == ZoneType.Village3)
            {
                Npcs.Add(new ItemNpc(ItemType.Statue, new Point(150, 170)));
                Npcs.Add(new ItemNpc(ItemType.Statue, new Point(280, 170)));
            }
        }

        private void InitializeNpcs()
        {
            Npcs.Clear();

            var npcInitializers = new Dictionary<ZoneType, Action<List<Npc>>>
            {
                { ZoneType.Road, npcs => {
                    npcs.Add(new Npc(Properties.Resources.Npc1, "Извозчик", 430, 265)
                        .WithQuest("quest_gold_collection"));
                }},
                {ZoneType.Village, npcs => {
                    npcs.Add(new Npc(Properties.Resources.Old_man, "Лесник", 280, 140)
                    .WithQuest("quest_find_cat"));
                    npcs.Add(new Npc(Properties.Resources.Man, "Барин", 600, 125));
                    npcs.Add(new Npc(Properties.Resources.Npc2, "Травница", 92, 330)
                    .WithQuest("quest_herb_collection"));
                }},
                { ZoneType.Village3, npcs => npcs.Add(new Npc(Properties.Resources.Woman, "Исследователь", 110, 425)
                    .WithQuest("quest_statue_activation"))}
                    };

            if (npcInitializers.TryGetValue(CurrentZoneType, out var initializer))
            {
                initializer(Npcs);
            }
        }

        private ZoneType GetZoneTypeFromBitmap(Bitmap bitmap)
        {
            return _zoneResources.FirstOrDefault(pair => pair.Value == bitmap).Key;
        }

        public bool SaveGame(string saveName)
        {
            var saveData = CreateSaveData();
            SaveLoadService.SaveGame(saveData, saveName);
            return true;
        }

        public bool LoadGame(string saveName)
        {
            if (SaveLoadService.SaveExists(saveName))
            {
                var saveData = SaveLoadService.LoadGame(saveName);
                LoadFromSaveData(saveData);
                return true;
            }
            return false;
        }
    }
}
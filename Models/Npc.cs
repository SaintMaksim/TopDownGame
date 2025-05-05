using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TopDownGame.Controllers;

namespace TopDownGame.Models
{
    public static class NpcExtensions
    {
        public static Npc WithQuest(this Npc npc, string questId)
        {
            npc.LinkedQuestId = questId;
            return npc;
        }
    }
    public class ItemNpc : Npc
    {
        public enum ItemType
        {
            Herb,
            Cat,
            Statue,
            Statue_Active,
            GoldChest
        }
        public bool IsCollected { get; private set; }
        private bool _shouldBeRemoved = false;
        public ItemType CurrentItemType { get; private set; }
        public string ItemId { get; }

        public ItemNpc(ItemType itemType, Point position)
        : base(GetItemSprite(itemType), GetItemName(itemType), position.X, position.Y)
        {
            CurrentItemType = itemType;
            ItemId = GetItemName(itemType);
        }

        private static Image GetItemSprite(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.Herb => Properties.Resources.Herb,
                ItemType.Statue => Properties.Resources.Statue,
                ItemType.Statue_Active => Properties.Resources.Statue_Active,
                ItemType.Cat => Properties.Resources.Cat,
                ItemType.GoldChest => Properties.Resources.Chest,
                _ => Properties.Resources.Herb
            };
        }

        private static string GetItemName(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.Herb => "Трава",
                ItemType.Cat => "Кот",
                ItemType.Statue => "Статуя",
                ItemType.Statue_Active => "Статуя",
                _ => "Сундук"
            };
        }
        public new DialogResult Interact()
        {
            if (IsCollected)
                return DialogResult.Cancel;

            if (CurrentItemType == ItemType.Statue)
            {
                CurrentItemType = ItemType.Statue_Active;
                Sprite = GetItemSprite(ItemType.Statue_Active);
                IsCollected = true;
                return DialogResult.OK;
            }

            IsCollected = true;
            _shouldBeRemoved = true;
            return DialogResult.OK;
        }

        public new void Draw(Graphics g)
        {
            if (!IsCollected)
            {
                g.DrawImage(Sprite, X, Y, Sprite.Width, Sprite.Height);
            }
        }

        public bool ShouldBeRemoved() => _shouldBeRemoved;


    }
    public class Npc
    {
        private readonly QuestManager _questManager;
        public string LinkedQuestId { get; set; }
        public bool HasOfferedQuest { get; set; } = false;
        public bool HasInteracted { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Image Sprite { get; set; }
        public string Name { get; private set; }
        private Random random = new Random();
        private int moveCooldown = 0;

        public Npc(Image sprite, string name, int spawnX, int spawnY)
        {
            Sprite = sprite;
            Name = name;
            X = spawnX;
            Y = spawnY;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(Sprite, X, Y, Sprite.Width, Sprite.Height);
            var font = new Font("Arial", 10);
            var nameSize = g.MeasureString(Name, font);
            g.DrawString(Name, font, Brushes.White,
                X + (Sprite.Width - nameSize.Width) / 2,
                Y - nameSize.Height - 5);
        }

        public DialogResult Interact(Player player)
        {
            if (!string.IsNullOrEmpty(LinkedQuestId) && player.CompletedQuests.Contains(LinkedQuestId))
            {
                return MessageBox.Show(
                    GetCompletionResponse(),
                    $"Диалог с {Name}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            return GetInitialDialogue();
        }

        private string GetCompletionResponse()
        {
            return Name switch
            {
                "Лесник" => "Спасибо еще раз за то, что нашел нашего кота!",
                "Травница" => "Эти травы очень помогли, благодарю!",
                _ => "Спасибо за помощь!"
            };
        }

        private DialogResult GetInitialDialogue()
        {
            switch (Name)
            {
                case "Лесник":
                    return MessageBox.Show(
                        "У меня тут дочурка котика своего потеряла. " +
                        "Вчера его сам поймал, а он убежал опять. Поможешь?",
                        "Диалог с Лесником",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                case "Барин":
                    return MessageBox.Show(
                        "Нервно бурчит под нос: Да чтож такое! Украли мой сундук с золотом!",
                        "Диалог с Барином",
                        MessageBoxButtons.OKCancel);
                case "Травница":
                    return MessageBox.Show(
                        "У нас здесь бурно растут разные травы. " +
                        "Можешь помочь собрать 5 целебных трав? Они растут в лесной зоне.",
                        "Диалог с Травницей",
                        MessageBoxButtons.OKCancel);
                case "Исследователь":
                    return MessageBox.Show(
                        "Я изучаю древние статуи в этих руинах. Можешь активировать две статуи для моего исследования?",
                        "Диалог с Исследователем",
                        MessageBoxButtons.OKCancel);
                case "Извозчик":
                    return MessageBox.Show(
                        "О, да я вижу, что ты пьяный! Собери 1000 золота и я отвезу тебя домой",
                        "Диалог с Извозчиком",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                default:
                    return MessageBox.Show(
                        $"{Name}: Приветствую, путник!",
                        "Диалог с NPC",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
        }
    }
}
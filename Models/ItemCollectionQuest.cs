using System.Windows.Forms;

namespace TopDownGame.Models
{
    public class ItemCollectionQuest : Quest
    {
        public ItemCollectionQuest(string id, string title, string description,
                                 string itemId, int requiredCount, int reward)
            : base(id, title, description)
        {
            RequiredItems.Add(itemId);
            RequiredItemCount = requiredCount;
            RewardGold = reward;
        }

        public class FindCatQuest : Quest
        {
            public FindCatQuest(string id, string title, string description, string requiredItemId, int reward)
                : base(id, title, description)
            {
                RequiredItems.Add(requiredItemId);
                RequiredItemCount = 1;
                RewardGold = reward;
            }

            public override void GiveReward(Player player)
            {
                base.GiveReward(player);
                MessageBox.Show("Спасибо за помощь! Вот твоя награда.",
                              "Кот найден!",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }

        public class HerbCollectionQuest : ItemCollectionQuest
        {
            public HerbCollectionQuest(string id, string title, string description,
                                     string itemId, int requiredCount, int reward)
                : base(id, title, description, itemId, requiredCount, reward)
            {
            }

            public override bool CheckCompletion(Player player)
            {
                return player.Inventory.GetItemCount("Трава") >= RequiredItemCount;
            }

            public override void GiveReward(Player player)
            {
                base.GiveReward(player);
                MessageBox.Show($"Спасибо! Вот твоя награда - {RewardGold} золота.",
                              "Квест завершён",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }

        public class StatueActivationQuest : Quest
        {
            public StatueActivationQuest(string id, string title, string description, int reward)
                : base(id, title, description)
            {
                RewardGold = reward;
                RequiredItems.Add("activated_statues");
                RequiredItemCount = 2;
            }

            public override bool CheckCompletion(Player player)
            {
                return player.CompletedQuests.Contains("statues_activated");
            }

            public override void GiveReward(Player player)
            {
                player.Inventory.AddGold(RewardGold);
                player.CompletedQuests.Add(Id);
                State = QuestState.Completed;

                MessageBox.Show($"Блестяще! Вот твоя награда - {RewardGold} золота.",
                              "Квест завершён",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }
        public class GoldCollectionQuest : Quest
        {
            public GoldCollectionQuest(string id, string title, string description, int requiredGold, int reward)
                : base(id, title, description)
            {
                RequiredItemCount = requiredGold;
                RewardGold = reward;
            }

            public override bool CheckCompletion(Player player)
            {
                return player.Inventory.Gold >= RequiredItemCount;
            }

            public override void GiveReward(Player player)
            {
                player.Inventory.AddGold(RewardGold);
                player.CompletedQuests.Add(Id);
                State = QuestState.Completed;

                MessageBox.Show($"О, ты уже собрал достаточно денег! Ну тогда поехали.",
                              "Квест завершён",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }
    }
}
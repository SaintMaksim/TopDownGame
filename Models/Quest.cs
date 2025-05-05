using System;
using System.Collections.Generic;

namespace TopDownGame.Models
{
    public enum QuestState { NotStarted, InProgress, Completed }

    public abstract class Quest
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public QuestState State { get; set; }
        public List<string> RequiredItems { get; } = new List<string>();
        public int RequiredItemCount { get; protected set; }
        public int RewardGold { get; protected set; }

        public event Action<Quest> QuestCompleted;


        protected Quest(string id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
            State = QuestState.NotStarted;
        }

        public virtual bool CheckCompletion(Player player)
        {
            if (State == QuestState.Completed) return true;

            foreach (var itemId in RequiredItems)
            {
                if (player.Inventory.GetItemCount(itemId) < RequiredItemCount)
                    return false;
            }
            return true;
        }

        public virtual void GiveReward(Player player)
        {
            RemoveQuestItems(player);
            player.Inventory.AddGold(RewardGold);
            State = QuestState.Completed;
            QuestCompleted?.Invoke(this);
        }

        public virtual void RemoveQuestItems(Player player)
        {
            foreach (var itemId in RequiredItems)
            {
                player.Inventory.RemoveItem(itemId, RequiredItemCount);
            }
        }
    }
}
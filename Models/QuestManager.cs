using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TopDownGame.Models;
using static TopDownGame.Models.ItemCollectionQuest;

namespace TopDownGame.Controllers
{
    public class QuestManager
    {
        private readonly Dictionary<string, Quest> _allQuests = new();
        private readonly List<Quest> _activeQuests = new();
        private readonly List<Quest> _completedQuests = new();
        private readonly Player _player;
        public event Action<Quest> OnQuestCompleted;
        public event Action OnGameComplete;
        public QuestManager(Player player)
        {
            _player = player;
            InitializeQuests();
        }

        private void InitializeQuests()
        {
            _allQuests.Add("quest_find_cat", new FindCatQuest(
                "quest_find_cat",
                "Потеря кошки",
                "Найти чёрную кошку. Она где-то в районе реки.", "Кот",
                200));
            _allQuests.Add("quest_herb_collection", new HerbCollectionQuest(
                "quest_herb_collection",
                "Трава-мурава",
                "Соберите 5 целебных трав",
                "Трава", 5, 300));
            _allQuests.Add("quest_statue_activation", new StatueActivationQuest(
                "quest_statue_activation",
                "Тайна древних камней",
                "Активируйте древние статуи в руинах",
                200));
            _allQuests.Add("quest_gold_collection", new GoldCollectionQuest(
                "quest_gold_collection",
                "Наконец-то домой",
                "Накопите 1000 золота для извозчика",
                1000,
                200
    ));
        }

        public void StartQuest(string questId)
        {
            if (_allQuests.TryGetValue(questId, out var quest) &&
                !_activeQuests.Contains(quest))
            {
                quest.State = QuestState.InProgress;
                _activeQuests.Add(quest);
            }
        }

        public Quest GetQuest(string questId)
        {
            return _allQuests.TryGetValue(questId, out var quest) ? quest : null;
        }

        public void UpdateQuests()
        {
            foreach (var quest in _activeQuests.ToList())
            {
                if (quest.CheckCompletion(_player))
                {
                    quest.GiveReward(_player);
                    _activeQuests.Remove(quest);
                    _completedQuests.Add(quest);
                    OnQuestCompleted?.Invoke(quest);
                    if (quest.Id == "quest_gold_collection")
                    {
                        OnGameComplete?.Invoke();
                    }
                    break;
                }
            }
        }

        public IEnumerable<Quest> GetActiveQuests() => _activeQuests;
    }
}
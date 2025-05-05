using NUnit.Framework;
using System.Windows.Forms;
using TopDownGame.Models;

namespace TopDownGame.Tests
{
    [TestFixture]
    public class NpcTests
    {
        private class TestNpc : Npc
        {
            public TestNpc(string name, int x, int y)
                : base(null, name, x, y) { }
        }

        private TestNpc _npc;

        [SetUp]
        public void Setup()
        {
            _npc = new TestNpc("Тестовый NPC", 100, 200);
        }

        [Test]
        public void Constructor_SetsBasicProperties()
        {
            Assert.AreEqual("Тестовый NPC", _npc.Name);
            Assert.AreEqual(100, _npc.X);
            Assert.AreEqual(200, _npc.Y);
        }

        [Test]
        public void WithQuest_SetsQuestId()
        {
            var npc = new TestNpc("NPC", 0, 0).WithQuest("test_quest");
            Assert.AreEqual("test_quest", npc.LinkedQuestId);
        }

        [Test]
        public void Name_IsDisplayedCorrectly()
        {
            Assert.AreEqual("Тестовый NPC", _npc.Name);
        }

        [Test]
        public void Position_CanBeModified()
        {
            _npc.X = 150;
            _npc.Y = 250;

            Assert.AreEqual(150, _npc.X);
            Assert.AreEqual(250, _npc.Y);
        }
    }
}
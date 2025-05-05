using NUnit.Framework;
using TopDownGame.Models;

namespace TopDownGame.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        private Player _player;

        [SetUp]
        public void Setup()
        {
            _player = new Player(null);
        }

        [Test]
        public void Player_ShouldHaveInitialPosition()
        {
            Assert.AreEqual(400, _player.X);
            Assert.AreEqual(260, _player.Y);
        }

        [Test]
        public void Player_CanChangePosition()
        {
            _player.X = 100;
            _player.Y = 200;

            Assert.AreEqual(100, _player.X);
            Assert.AreEqual(200, _player.Y);
        }

        [Test]
        public void Player_ShouldHaveDefaultSpeed()
        {
            Assert.AreEqual(5, _player.Speed);
        }

        [Test]
        public void Player_CanChangeSpeed()
        {
            _player.Speed = 10;
            Assert.AreEqual(10, _player.Speed);
        }

        [Test]
        public void Player_ShouldHaveEmptyInventoryInitially()
        {
            Assert.AreEqual(0, _player.Inventory.Gold);
            Assert.IsEmpty(_player.Inventory.GetAllItems());
        }

        [Test]
        public void Player_CanAddGoldToInventory()
        {
            _player.Inventory.AddGold(50);
            Assert.AreEqual(50, _player.Inventory.Gold);
        }

        [Test]
        public void Player_ShouldHaveEmptyQuestListInitially()
        {
            Assert.IsEmpty(_player.CompletedQuests);
        }
    }
}
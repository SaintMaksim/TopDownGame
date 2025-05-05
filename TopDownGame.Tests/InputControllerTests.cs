using NUnit.Framework;
using System;
using TopDownGame.Models;
using TopDownGame.Controllers;

namespace TopDownGame.Tests
{
    [TestFixture]
    public class InputControllerTests
    {
        private InputController _inputController;
        private Player _player;
        private bool _eventTriggered;
        private class TestKeyEventArgs : EventArgs
        {
            public Keys KeyCode { get; }
            public TestKeyEventArgs(Keys key) => KeyCode = key;
        }

        [SetUp]
        public void Setup()
        {
            _player = new Player(null);
            _inputController = new InputController(
                _player,
                bounds => false,
                () => false,
                800, 600
            );
            _eventTriggered = false;
        }

        [Test]
        public void HandleKeyDown_SetsMovementFlags()
        {
            _inputController.HandleKeyDown(null, new TestKeyEventArgs(Keys.W));
            _inputController.HandleKeyDown(null, new TestKeyEventArgs(Keys.A));

            _inputController.Update();

            Assert.AreEqual(400 - _player.Speed, _player.X);
            Assert.AreEqual(260 - _player.Speed, _player.Y);
        }

        [Test]
        public void HandleKeyUp_ResetsMovementFlags()
        {
            _inputController.HandleKeyDown(null, new TestKeyEventArgs(Keys.D));
            _inputController.HandleKeyUp(null, new TestKeyEventArgs(Keys.D));

            _inputController.Update();

            Assert.AreEqual(400, _player.X);
        }

        [Test]
        public void SaveEvent_TriggersOnF5()
        {
            _inputController.OnSaveRequested += () => _eventTriggered = true;
            _inputController.HandleKeyDown(null, new TestKeyEventArgs(Keys.F5));
            Assert.IsTrue(_eventTriggered);
        }

        [Test]
        public void Update_RespectsPauseState()
        {
            var pausedController = new InputController(
                _player,
                bounds => false,
                () => true,
                800, 600
            );

            pausedController.HandleKeyDown(null, new TestKeyEventArgs(Keys.D));
            pausedController.Update();
            Assert.AreEqual(400, _player.X);
        }
    }
    public enum Keys
    {
        W,
        A,
        S,
        D,
        F5,
        F9,
        E
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using TopDownGame.Models;

namespace TopDownGame.Controllers
{
    public class InputController
    {
        private readonly Player _player;
        private readonly Func<Rectangle, bool> _collisionCheck;
        private readonly Func<bool> _isPausedChecker;

        private bool _f5Pressed, _f9Pressed, _ePressed;

        public event Action OnSaveRequested;
        public event Action OnLoadRequested;
        public event Action OnInteractionRequested;

        public InputController(Player player, Func<Rectangle, bool> collisionCheck,
                             Func<bool> isPausedChecker, int screenWidth, int screenHeight)
        {
            _player = player;
            _collisionCheck = collisionCheck;
            _isPausedChecker = isPausedChecker;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (_isPausedChecker()) return;

            switch (e.KeyCode)
            {
                case Keys.F5 when !_f5Pressed:
                    _f5Pressed = true;
                    OnSaveRequested?.Invoke();
                    break;

                case Keys.F9 when !_f9Pressed:
                    _f9Pressed = true;
                    OnLoadRequested?.Invoke();
                    break;

                case Keys.E when !_ePressed:
                    _ePressed = true;
                    OnInteractionRequested?.Invoke();
                    break;
            }
        }

        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5: _f5Pressed = false; break;
                case Keys.F9: _f9Pressed = false; break;
                case Keys.E: _ePressed = false; break;
            }
        }

        public void Update()
        {
            if (_isPausedChecker()) return;
            HandleMovementInput();
        }

        private void HandleMovementInput()
        {
            int newX = _player.X;
            int newY = _player.Y;
            int speed = _player.Speed;

            if (IsKeyPressed(Keys.W)) newY -= speed;
            if (IsKeyPressed(Keys.S)) newY += speed;
            if (IsKeyPressed(Keys.A)) newX -= speed;
            if (IsKeyPressed(Keys.D)) newX += speed;

            var bounds = new Rectangle(newX, newY, _player.Sprite.Width, _player.Sprite.Height);
            if (!_collisionCheck(bounds))
            {
                _player.X = newX;
                _player.Y = newY;
            }
        }

        public static bool IsKeyPressed(Keys key)
        {
            return Control.ModifierKeys == key ||
                   (key != Keys.None && (GetAsyncKeyState((int)key) & 0x8000) != 0);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
    }
}
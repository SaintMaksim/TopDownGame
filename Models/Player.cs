using System.Collections.Generic;
using System.Drawing;

namespace TopDownGame.Models
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Speed { get; set; } = 5;
        public Image Sprite { get; }

        public Player(Image sprite)
        {
            Sprite = sprite;
            X = 400;
            Y = 260;
        }

        public Inventory Inventory { get; } = new();
        public HashSet<string> CompletedQuests { get; } = new();
        public void Draw(Graphics g)
        {
            g.DrawImage(Sprite, X, Y, Sprite.Width, Sprite.Height);
        }
    }
}
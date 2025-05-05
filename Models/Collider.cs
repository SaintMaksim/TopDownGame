using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TopDownGame.Controllers;

namespace TopDownGame
{
    public class Collider
    {
        public Rectangle Bounds { get; set; }
        public bool IsActive { get; set; } = true;
        public Color DebugColor { get; set; } = Color.FromArgb(50, 255, 0, 0);
        public Collider(int x, int y, int width, int height)
        {
            Bounds = new Rectangle(x, y, width, height);
        }

        public bool CheckCollision(Rectangle objectBounds)
        {
            return IsActive && Bounds.IntersectsWith(objectBounds);
        }

        public void Draw(Graphics g)
        {
            using (var brush = new SolidBrush(DebugColor))
            using (var pen = new Pen(Color.FromArgb(150, DebugColor), 1))
            {
                g.FillRectangle(brush, Bounds);
                g.DrawRectangle(pen, Bounds);
            }
        }
    }
}
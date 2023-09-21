using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwordRush
{
    internal class SceneObject : GameObject
    {
        private bool wall;
        private int tile;
        private Rectangle sourceRectangle;
        private Random rng;

        // --- Constructor --- //

        public SceneObject(bool wall, int tile, Texture2D texture, Rectangle rectangle) : base(texture, rectangle)
        {
            this.wall = wall;
            this.tile = tile;
            rng = new Random();
            if (tile == 20)
            {
                sourceRectangle = new Rectangle(16, 64, 16, 16);
            }
            else if (tile == 1)
            {
                sourceRectangle = new Rectangle(32, 0, 16, 16);
            }
            else if (tile == 2)
            {
                sourceRectangle = new Rectangle(16, 112, 16, 16);
            }
            else if (tile == 3)
            {
                sourceRectangle = new Rectangle(16, 144, 16, 16);
            }
            else if (tile == 4)
            {
                sourceRectangle = new Rectangle(32, 16, 16, 16);
            }
            else if (tile == 5)
            {
                sourceRectangle = new Rectangle(0, 112, 16, 16);
            }
            else if (tile == 6)
            {
                sourceRectangle = new Rectangle(0, 144, 16, 16);
            }
            else if (tile == 7)
            {
                sourceRectangle = new Rectangle(0, 128, 16, 16);
            }
            else if (tile == 8)
            {
                sourceRectangle = new Rectangle(16, 128, 16, 16);
            }
            else if (tile == 9)
            {
                sourceRectangle = new Rectangle(32, 144, 16, 16);
            }
            else if (tile == 10)
            {
                sourceRectangle = new Rectangle(48, 144, 16, 16);
            }

        }

        public void Draw(SpriteBatch sb)
        {
            
            if (tile == 0)
            {
                sb.Draw(texture, Rectangle, Color.Black);
            }
            else if (tile > 0)
            {
                sb.Draw(texture, Rectangle, sourceRectangle, Color.White);
            }
        }

        public void Update(GameTime gt, GameObject collider)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SwordRush
{
    internal class Chest : GameObject
    {
        private bool open;

        //constructor
        public Chest(Texture2D texture, Rectangle rectangle) : base(texture, rectangle)
        {
            open = false;
        }

        /// <summary>
        /// getter and setter for open
        /// </summary>
        public bool Open { get { return open; }  set { open = value; } }

        /// <summary>
        /// getter for rectangle
        /// </summary>
        public Rectangle Rectangle { get { return rectangle; } }

        public void Draw(SpriteBatch sb)
        {
            if (open)
            {
                sb.Draw(texture, Rectangle, new Rectangle(336, 288, 16, 16), Color.White);
            }
            else
            {
                sb.Draw(texture, Rectangle, new Rectangle(304, 288, 16, 16), Color.White);
            }
        }

        public void Update(GameTime gt, GameObject collider)
        {

        }
    }
}

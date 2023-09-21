using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SwordRush
{
    internal class GameObject
    {
        protected Texture2D texture;
        protected Rectangle rectangle;
        protected SoundEffect sound;
        protected Vector2 position;
        protected Point size;

        public Rectangle Rectangle
        {

            get { return rectangle; }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }

            set 
            {
                position = value;
                rectangle = new Rectangle((int)(position.X - rectangle.Width / 2), (int)(position.Y - rectangle.Height / 2), size.X,size.Y);
            }
        }

        /// <summary>
        /// gets and sets x of position vector
        /// </summary>
        public float X
        {
            get 
            { 
                return position.X; 
            }

            set
            {
                position.X = value;
                rectangle = new Rectangle((int)(position.X - rectangle.Width / 2), (int)(position.Y - rectangle.Height / 2), size.X, size.Y);
            }
        }

        //gets and sets y of position vector
        public float Y
        {
            get
            {
                return position.Y;
            }

            set
            {
                position.Y = value;
                rectangle = new Rectangle((int)(position.X - rectangle.Width / 2), (int)(position.Y - rectangle.Height / 2), size.X, size.Y);
            }
        }

        public GameObject (Texture2D texture, Rectangle rectangle)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.position = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
            this.size = rectangle.Size;
        }

        public virtual void Update(GameTime gt)
        {
            
        }

        

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(
                texture,
                rectangle,
                Microsoft.Xna.Framework.Color.White);
        }

    }
}

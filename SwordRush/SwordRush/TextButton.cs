using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SwordRush
{
    internal class TextButton : GameObject
    {
        // --- Fields --- //

        protected string text;
        protected Color color;
        protected SpriteFont font;
        protected MouseState currentMState;
        protected MouseState previousMState;
        protected bool leftClicked;

        // --- Properties --- //

        /// <summary>
        /// Returns if the button was left clicked or not
        /// </summary>
        public bool LeftClicked { get { return leftClicked; } }

        /// <summary>
        /// The text the button will display
        /// </summary>
        public string Text { get { return text; } set { text = value; } }



        // --- Constructor --- //

        public TextButton(Rectangle rectangle, string text, SpriteFont font)
            : base(null, rectangle)
        {
            this.text = text;
            color = Color.LightGray;
            this.font = font;
            currentMState = new MouseState();
            previousMState = new MouseState();
            leftClicked = false;
        }

        public override void Update(GameTime gt)
        {
            currentMState = Mouse.GetState();

            // Highlight text when mouse hovers
            if (new Rectangle(currentMState.Position, new Point(0, 0)).Intersects(rectangle))
            {
                color = Color.DarkGoldenrod;
            }
            else
            {
                color = Color.LightGray;
            }

            // When the button is clicked sets leftClicked to true
            if (color == Color.DarkGoldenrod
                && currentMState.LeftButton == ButtonState.Released
                && previousMState.LeftButton == ButtonState.Pressed)
            {
                leftClicked = true;
                SoundManager.Get.ClickEffect.Play();
            }
            else
            {
                leftClicked = false;
            }

            previousMState = Mouse.GetState();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.DrawString(
                font,
                text,
                new Vector2(rectangle.X, rectangle.Y),
                color);
        }
    }
}

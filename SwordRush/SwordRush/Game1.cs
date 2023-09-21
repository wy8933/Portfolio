using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Andrew, Bin, Weijie, Josh
// Started March 6, 2023
// Group Game "Sword Rush" for IGME 106 Class

namespace SwordRush
{
    // Delegates
    public delegate void ToggleGameState();
    public delegate void GameOver(int roomsCleared);

    public class Game1 : Game
    {
        #region fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Fonts
        SpriteFont bellMT16;
        SpriteFont bellMT24;

        // window
        Point windowSize;

        //blank rectangle
        Texture2D whiteRectangle;

        #endregion

        public Game1()
        {
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d); //second number is the fps
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Change window size
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // Window Size
            windowSize = new Point(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);

            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //create blank rectangle sprite
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            // Load Fonts
            bellMT24 = Content.Load<SpriteFont>("Bell_MT-24");
            bellMT16 = Content.Load<SpriteFont>("Bell_MT-16");

            
            // UI Manager
            UI.Get.Initialize(Content, windowSize, GraphicsDevice);
            // Game Manager
            GameManager.Get.Initialize(Content, windowSize, whiteRectangle, GraphicsDevice);
            //Sound Manager
            SoundManager.Get.Initialize(Content);

            // Events and delegates
            UI.Get.startGame += GameManager.Get.StartGame;
            GameManager.Get.gameOver += UI.Get.EndGame;
            UI.Get.quitGame += GameManager.Get.QuitGame;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)
                && UI.Get.GameFSM == GameFSM.Menu)
                Exit();

            GameManager.Get.Update(gameTime);
            UI.Get.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(12,12,12));
            _spriteBatch.Begin();

            // Draw UI elements (Text, Menus, Icons)
            UI.Get.Draw(_spriteBatch);

            
            GameManager.Get.Draw(_spriteBatch);

            if (UI.Get.ShowGrid
                && UI.Get.GameFSM == GameFSM.Game) 
            {
                DrawCords(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawCords(SpriteBatch sb)
        {
            
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    Color gridColor = new Color();

                    if ((x+y)%2 == 0)
                    {
                        gridColor = Color.Black * 0.1f;
                    } else
                    {
                        gridColor = Color.Gray * 0.1f;
                    }

                    // Draw Tile size
                    sb.Draw(whiteRectangle,
                        new Rectangle(x * windowSize.X / 20, y * windowSize.Y / 12,
                        windowSize.X / 20, windowSize.Y / 12),
                        gridColor);
                    
                    // Draw Tile location
                    sb.DrawString(
                        bellMT16,
                        $"{GameManager.Get.Grid[x,y]}",
                        new Vector2(x*windowSize.X/20+10,y*windowSize.Y/12+9),
                        Color.White * 0.5f);
                    
                }
            }
        }
    }
}

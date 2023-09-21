 using System;
using System.Collections.Generic;
 using System.Diagnostics;
 using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace SwordRush
{
    enum GameFSM
    {
        Menu,
        Game,
        Settings,
        Credits,
        Instructions,
        Shop
    }

    internal class UI
    {
        // --- Fields --- //

        private int level;
        private int playerLevel;
        private int health;

        // Finite State Machine
        private GameFSM gameFSM;

        // Fonts
        private SpriteFont bellMT36;
        private SpriteFont bellMT48;
        private SpriteFont bellMT72;

        // Mouse State
        private MouseState currentMState;
        private MouseState previousMState;

        // Buttons
        private List<TextButton> menuButtons;
        private List<TextButton> settingButtons;
        private List<TextButton> shopButtons;
        private TextButton exitButton;

        //shop states
        public bool shieldPurchased;
        public bool dodgePurchased;
        public bool vampirePurchased;

        // Window dimensions
        private Rectangle window;

        // Textures
        private Texture2D menuImageTexture;
        private Texture2D singleColor;
        private Texture2D mouseLeft;
        private Texture2D mouseRight;

        // Event to communicate with GameManager
        public event ToggleGameState startGame;
        public event ToggleGameState quitGame;

        // Settings - Debug
        private int sfxLevel;
        private int musicLevel;
        private bool takeDamage;
        private bool showHitboxes;
        private bool showGrid;


        private static UI instance = null;
        
        public static UI Get
        {
            get
            {
                if (instance == null)
                {
                    instance = new UI();
                }
                
                return instance;
            }
        }

        // --- Properties --- //

        public GameFSM GameFSM { get { return gameFSM; } }

        public int SfxLevel { get { return sfxLevel; } }

        public int MusicLevel { get { return musicLevel; } }

        public bool TakeDamage { get { return takeDamage; } }

        public bool ShowHitboxes { get {  return showHitboxes; } }

        public bool ShowGrid { get { return showGrid; } }


        // --- Constructor --- //

        public void Initialize(ContentManager content, Point windowSize, GraphicsDevice gd)
        {
            // State Machine
            gameFSM = GameFSM.Menu;

            // Fonts
            bellMT36 = content.Load<SpriteFont>("Bell_MT-36");
            bellMT48 = content.Load<SpriteFont>("Bell_MT-48");
            bellMT72 = content.Load<SpriteFont>("Bell_MT-72");

            // Textures
            menuImageTexture = content.Load<Texture2D>("MenuBackground3");
            singleColor = new Texture2D(gd, 1, 1);
            singleColor.SetData(new Color[] { Color.White });
            mouseLeft = content.Load<Texture2D>("mouse-left-click");
            mouseRight = content.Load<Texture2D>("mouse-right-click");

            //shop bools
            dodgePurchased = false;
            shieldPurchased = false;
            vampirePurchased = false;


            // Controls Mouse
            currentMState = new MouseState();
            previousMState = new MouseState();

            // Used for window height and width
            this.window = new Rectangle(0,0,
                windowSize.X, windowSize.Y);
                        // Creates all the buttons
            initalizeButtons();
            takeDamage = true;
            showHitboxes = false;
            showGrid = false;

            sfxLevel = 10;
            musicLevel = 10;
        }



        // --- Methods --- //

        public void Update(GameTime gt)
        {
            currentMState = Mouse.GetState();

            // Loads the 
            switch (gameFSM)
            {
                case GameFSM.Menu: // --- Menu --------------------------------------------------//
                    
                    // Updates hover color change
                    foreach (TextButton b in menuButtons)
                    {
                        b.Update(gt);
                    }

                    // changes states when clicked
                    if (menuButtons[0].LeftClicked)
                    {
                        gameFSM = GameFSM.Game;

                        // Sends event that will be recieved by game manager
                        GameManager.Get.LocalPlayer.NewRound();
                        startGame();
                        
                    }
                    if (menuButtons[1].LeftClicked)
                    {
                        gameFSM = GameFSM.Instructions;
                    }
                    if (menuButtons[2].LeftClicked)
                    {
                        gameFSM = GameFSM.Credits;
                    }
                    if (menuButtons[3].LeftClicked)
                    {
                        gameFSM = GameFSM.Shop;
                    }
                    if (menuButtons[4].LeftClicked)
                    {
                        gameFSM = GameFSM.Settings;
                    }

                    break;

                case GameFSM.Game: // --- Game --------------------------------------------------//
                    // Right click to quit the game
                    // Code for quitting game in GameManager
                    break;

                case GameFSM.Shop: // ----- Shop ------------------------------------------------//

                    foreach(TextButton b in shopButtons)
                    {
                        b.Update(gt);
                    }

                    if (exitButton.LeftClicked)
                    {
                        gameFSM = GameFSM.Menu;
                    }

                    //TODO: Update shop buttons
                    if (currentMState.LeftButton == ButtonState.Released
                        && previousMState.LeftButton == ButtonState.Pressed)
                    {
                        //buy powers
                        if (!dodgePurchased && shopButtons[0].LeftClicked && GameManager.Get.TotalCoin >= 50)
                        {
                            dodgePurchased = true;
                            GameManager.Get.TotalCoin -= 50;
                            GameManager.Get.UpdateEcon();
                        }
                        if (!shieldPurchased && shopButtons[1].LeftClicked && GameManager.Get.TotalCoin >= 50)
                        {
                            shieldPurchased = true;
                            GameManager.Get.TotalCoin -= 50;
                            GameManager.Get.UpdateEcon();
                        }
                        if (!vampirePurchased && shopButtons[2].LeftClicked && GameManager.Get.TotalCoin >= 50)
                        {
                            vampirePurchased = true;
                            GameManager.Get.TotalCoin -= 50;
                            GameManager.Get.UpdateEcon();
                        }
                        //equip powers
                        if (dodgePurchased && shopButtons[0].LeftClicked)
                        {
                            GameManager.Get.player.BackUpPower = true;
                            GameManager.Get.player.backUpLevel = 1;
                            GameManager.Get.player.shieldPower = false;
                            GameManager.Get.player.vampirePower = false;
                            GameManager.Get.player.Perk = PlayerPerk.Dodge;
                        }
                        if (shieldPurchased && shopButtons[1].LeftClicked)
                        {
                            GameManager.Get.player.BackUpPower = false;
                            GameManager.Get.player.shieldPower = true;
                            GameManager.Get.player.shiledLevel = 1;
                            GameManager.Get.player.vampirePower = false;
                            GameManager.Get.player.Perk = PlayerPerk.Sheild;
                        }
                        if (vampirePurchased && shopButtons[2].LeftClicked)
                        {
                            GameManager.Get.player.BackUpPower = false;
                            GameManager.Get.player.shieldPower = false;
                            GameManager.Get.player.vampirePower = true;
                            GameManager.Get.player.vampireLevel = 1;
                            GameManager.Get.player.Perk = PlayerPerk.Vampire;
                        }


                    }

                        break;

                case GameFSM.Settings:

                    foreach(TextButton b in settingButtons)
                    {
                        b.Update(gt);
                    }

                    // TODO: implement debug code here to adjust volumes and enable invincibility
                    if (currentMState.LeftButton == ButtonState.Released 
                        && previousMState.LeftButton == ButtonState.Pressed)
                    {
                        // Lower SFX
                        if (settingButtons[0].LeftClicked)
                        {
                            if (sfxLevel > 0)
                            {
                                sfxLevel--;
                                SoundManager.Get.UpdateVolume();
                            }
                        }
                        // Raise SFX
                        else if (settingButtons[1].LeftClicked)
                        {
                            if (sfxLevel < 10)
                            {
                                sfxLevel++;
                                SoundManager.Get.UpdateVolume();
                            }
                        }
                        // Lower Music
                        else if (settingButtons[2].LeftClicked)
                        {
                            if (musicLevel > 0)
                            {
                                musicLevel--;
                                SoundManager.Get.UpdateVolume();
                            }
                        }
                        // Raise Music
                        else if (settingButtons[3].LeftClicked)
                        {
                            if (musicLevel < 10)
                            {
                                musicLevel++;
                                SoundManager.Get.UpdateVolume();

                            }
                        }
                        // Toggle TakeDamage
                        else if (settingButtons[4].LeftClicked)
                        {
                            if (takeDamage == false)
                            {
                                takeDamage = true;
                                settingButtons[4].Text = "True";
                            }
                            else
                            {
                                takeDamage = false;
                                settingButtons[4].Text = "False";
                            }
                            
                        }
                        // Toggle Show Hitboxes
                        else if (settingButtons[5].LeftClicked)
                        {
                            if (showHitboxes == false)
                            {
                                showHitboxes = true;
                                settingButtons[5].Text = "True";
                            }
                            else
                            {
                                showHitboxes = false;
                                settingButtons[5].Text = "False";
                            }

                        }
                        // Show locations
                        else if (settingButtons[6].LeftClicked)
                        {
                            if (showGrid == false)
                            {
                                showGrid = true;
                                settingButtons[6].Text = "True";
                            }
                            else
                            {
                                showGrid = false;
                                settingButtons[6].Text = "False";
                            }
                        }
                    }

                    if (exitButton.LeftClicked)
                    {
                        gameFSM = GameFSM.Menu;
                    }

                    break;

                default:  // --- Other ----------------------------------------------------------//
                    // In any state that is not game left click will bring you back to menu
                    if (exitButton.LeftClicked)
                    {
                        gameFSM = GameFSM.Menu;
                    }
                    break;
            }

            exitButton.Update(gt);

            previousMState = Mouse.GetState();
        }

        public void Draw(SpriteBatch sb)
        {
            // Testing Menu Text
            switch (gameFSM)
            {
                case GameFSM.Menu:  // --- Menu -------------------------------------------------//

                    // Background Image
                    sb.Draw(
                     menuImageTexture,
                     window,
                     Color.White);

                    // Background for title
                    sb.Draw(singleColor,                                                        // Texture
                        new Rectangle((int)(window.Width * 0.10f), (int)(window.Height*0.1f),   // X / Y
                        (int)(window.Width*0.5f),(int)(window.Height*0.13f)),                   // Width / Height
                        new Color(32, 32, 32) * 0.4f);                                                        // Color

                    // Draw Title
                    sb.DrawString(
                        bellMT72,                           // Font
                        "SWORD RUSH",                       // Text
                        new Vector2((window.Width * 0.10f), // X Pos
                        (window.Height * 0.10f)),           // Y Pos
                        Color.Goldenrod);                   // Color

                    // Rectangles behind the menu buttons
                    sb.Draw(singleColor,                                                        // Texture
                        new Rectangle((int)(window.Width * 0.10f), (int)(window.Height * 0.3f),   // X / Y
                        (int)(window.Width * 0.24f), (int)(window.Height * 0.08f)),                   // Width / Height
                        new Color(32, 32, 32) * 0.4f);

                    sb.Draw(singleColor,                                                        // Texture
                        new Rectangle((int)(window.Width * 0.10f), (int)(window.Height * 0.42f),   // X / Y
                        (int)(window.Width * 0.25f), (int)(window.Height * 0.08f)),                   // Width / Height
                        new Color(32, 32, 32) * 0.4f);

                    sb.Draw(singleColor,                                                        // Texture
                        new Rectangle((int)(window.Width * 0.10f), (int)(window.Height * 0.54f),   // X / Y
                        (int)(window.Width * 0.15f), (int)(window.Height * 0.08f)),                   // Width / Height
                        new Color(32, 32, 32) * 0.4f);

                    sb.Draw(singleColor,                                                        // Texture
                        new Rectangle((int)(window.Width * 0.10f), (int)(window.Height * 0.66f),   // X / Y
                        (int)(window.Width * 0.11f), (int)(window.Height * 0.08f)),                   // Width / Height
                        new Color(32, 32, 32) * 0.4f);

                    sb.Draw(singleColor,                                                        // Texture
                        new Rectangle((int)(window.Width * 0.10f), (int)(window.Height * 0.78f),   // X / Y
                        (int)(window.Width * 0.17f), (int)(window.Height * 0.08f)),                   // Width / Height
                        new Color(32, 32, 32) * 0.4f);

                    // Draw all Buttons
                    foreach (TextButton b in menuButtons)
                    {
                        b.Draw(sb);
                    }
                    break;

                case GameFSM.Instructions: // --- Instructions ----------------------------------//
                    sb.DrawString(
                        bellMT72,                       // Font
                        $"How to Play",                 // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT72,                       // Font
                        $"Controls",                    // Text
                        new Vector2(window.Width * 0.6f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"Kill enemies to gain XP" +
                        $"\nSee how many rooms you" +
                        $"\ncan clear." +
                        $"\nUnlock upgrades with XP", // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"Attack/Move" +
                        $"\n - Left Click" +
                        $"\n\nDodge" +
                        $"\n - Right Click" +
                        $"\n\nPause/Quit" +
                        $"\n - Escape",                 // Text
                        new Vector2(window.Width * 0.6f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.White);                   // Color

                    sb.Draw(mouseLeft, new Rectangle((int)(window.Width * 0.85), (int)(window.Height * 0.3), 100, 100), Color.White); //draw left click icon

                    sb.Draw(mouseRight, new Rectangle((int)(window.Width * 0.85), (int)(window.Height * 0.5), 100, 100), Color.White); //draw right click icon


                    break;

                case GameFSM.Credits: // --- Credits --------------------------------------------//

                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Developers",                  // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"Bin Xu" +
                        $"\nJosh Leong" +
                        $"\nWeijie Ye" +
                        $"\nAndrew Ebersole",           // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.White);                   // Color


                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Art",                         // Text
                        new Vector2(window.Width * 0.4f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.LightGray);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"frosty_rabbid" +
                        $"\nguilemus" +
                        $"\n0x72 \"Robert\"" +
                        $"\nThe Outlander",                  // Text
                        new Vector2(window.Width * 0.4f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.LightGray);                   // Color

                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Audio",                       // Text
                        new Vector2(window.Width * 0.7f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"Music: Leohpaz" +
                        $"\nSFX: jsfxr",                       // Text
                        new Vector2(window.Width * 0.7f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.White);                   // Color
                    break;

                case GameFSM.Shop: // --- Shop --------------------------------------//

                    sb.DrawString(
                        bellMT72,                       // Font
                        $"Shop",                 // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"50 Coins Each",                 // Text
                        new Vector2(window.Width * 0.3f,// X Position
                        window.Height * 0.15f),          // Y Position
                        Color.Yellow);                   // Color


                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Dodge: " +
                        $"\n\nShield:" +
                        $"\n\nVampire:",                        // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.White);                   // Color

                    if (dodgePurchased)
                    {
                        shopButtons[0].Text = "Equip";
                        if (GameManager.Get.LocalPlayer.BackUpPower)
                        {
                            shopButtons[0].Text = "Equipped";
                        }
                    }
                    if (shieldPurchased)
                    {
                        shopButtons[1].Text = "Equip";
                        if (GameManager.Get.LocalPlayer.shieldPower)
                        {
                            shopButtons[1].Text = "Equipped";
                        }
                    }
                    if (vampirePurchased)
                    {
                        shopButtons[2].Text = "Equip";
                        if (GameManager.Get.LocalPlayer.vampirePower)
                        {
                            shopButtons[2].Text = "Equipped";
                        }
                    }

                    foreach (TextButton b in shopButtons)
                    {
                        b.Draw(sb);
                    }

                    break;

                case GameFSM.Settings: // --- Settings ------------------------------------------//
                    
                    sb.DrawString(
                        bellMT72,                       // Font
                        $"Settings",                    // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.1f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT48,                       // Font
                        $"SFX Volume",                  // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.3f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"{sfxLevel}",                  // Text
                        new Vector2(window.Width * 0.63f,// X Position
                        window.Height * 0.30f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Music Volume",                // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.4f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT36,                       // Font
                        $"{musicLevel}",                // Text
                        new Vector2(window.Width * 0.63f,// X Position
                        window.Height * 0.40f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Take Damage",                 // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.5f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                        bellMT48,                       // Font
                        $"Show Hitboxes",               // Text
                        new Vector2(window.Width * 0.1f,// X Position
                        window.Height * 0.6f),          // Y Position
                        Color.White);                   // Color

                    sb.DrawString(
                       bellMT48,                       // Font
                       $"Show Tile Grid",               // Text
                       new Vector2(window.Width * 0.1f,// X Position
                       window.Height * 0.7f),          // Y Position
                       Color.White);                   // Color

                    foreach (TextButton b in settingButtons)
                    {
                        b.Draw(sb);
                    }
                    break;

            }
            if (gameFSM != GameFSM.Menu && gameFSM != GameFSM.Game)
            {
                exitButton.Draw(sb);
            }
            

        }

        private void initalizeButtons()
        {

            // --- Menu Buttons -----------------------------------------------------------------//
            menuButtons = new List<TextButton>();

            menuButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.10f), (int)(window.Height * 0.30f),  // Location
                (int)(window.Width * 0.23f), (int)(window.Height * 0.09f)), // Hitbox
                "Start Game",                                               // Text
                bellMT48));                                                 // Font

            menuButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.10f), (int)(window.Height * 0.42f),  // Location
                (int)(window.Width * 0.25f), (int)(window.Height * 0.09f)), // Hitbox
                "Instructions",                                             // Text
                bellMT48));                                                 // Font

            menuButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.10f), (int)(window.Height * 0.54f),  // Location
                (int)(window.Width * 0.15f), (int)(window.Height * 0.09f)), // Hitbox
                "Credits",                                                  // Text
                bellMT48));                                                 // Font

            menuButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.10f), (int)(window.Height * 0.66f),  // Location
                (int)(window.Width * 0.25f), (int)(window.Height * 0.09f)), // Hitbox
                "Shop",                                              // Text
                bellMT48));                                                 // Font

            menuButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.10f), (int)(window.Height * 0.78),  // Location
                (int)(window.Width * 0.17f), (int)(window.Height * 0.09f)), // Hitbox
                "Settings",                                                 // Text
                bellMT48));                                                 // Font

            // --- Shop Buttons -----------------------------------------------------------------//
            shopButtons = new List<TextButton>();

            shopButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.40f), (int)(window.Height * 0.30f),  // Location
                (int)(window.Width * 0.20f), (int)(window.Height * 0.09f)), // Hitbox
                "Purchase",                                               // Text
                bellMT48));

            shopButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.40f), (int)(window.Height * 0.485f),  // Location
                (int)(window.Width * 0.20f), (int)(window.Height * 0.09f)), // Hitbox
                "Purchase",                                               // Text
                bellMT48));

            shopButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.40f), (int)(window.Height * 0.67f),  // Location
                (int)(window.Width * 0.20f), (int)(window.Height * 0.09f)), // Hitbox
                "Purchase",                                               // Text
                bellMT48));


            // --- Settings Buttons -------------------------------------------------------------//
            settingButtons = new List<TextButton>();

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.60f), (int)(window.Height * 0.30f),  // Location
                (int)(window.Width * 0.04f), (int)(window.Height * 0.12f)), // Hitbox
                "<",                                                        // Text
                bellMT36));                                                 // Font

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.68f), (int)(window.Height * 0.30f),  // Location
                (int)(window.Width * 0.04f), (int)(window.Height * 0.12f)), // Hitbox
                ">",                                                        // Text
                bellMT36));                                                 // Font

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.60f), (int)(window.Height * 0.40f),  // Location
                (int)(window.Width * 0.04f), (int)(window.Height * 0.12f)), // Hitbox
                "<",                                                        // Text
                bellMT36));                                                 // Font

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.68f), (int)(window.Height * 0.40f),  // Location
                (int)(window.Width * 0.04f), (int)(window.Height * 0.12f)), // Hitbox
                ">",                                                        // Text
                bellMT36));                                                 // Font

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.60f), (int)(window.Height * 0.50f),  // Location
                (int)(window.Width * 0.10f), (int)(window.Height * 0.09f)), // Hitbox
                "True",                                                     // Text
                bellMT36));                                                 // Font

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.60f), (int)(window.Height * 0.60f),  // Location
                (int)(window.Width * 0.10f), (int)(window.Height * 0.09f)), // Hitbox
                "False",                                                     // Text
                bellMT36));                                                 // Font

            settingButtons.Add(new TextButton(new Rectangle(
                (int)(window.Width * 0.60f), (int)(window.Height * 0.70f),  // Location
                (int)(window.Width * 0.10f), (int)(window.Height * 0.09f)), // Hitbox
                "False",                                                     // Text
                bellMT36));                                                 // Font

            // --- Exit Button ------------------------------------------------------------------//

            exitButton = new TextButton(new Rectangle(
                (int)(window.Width * 0.10f), (int)(window.Height * 0.91f),  // Location
                (int)(window.Width * 0.10f), (int)(window.Height * 0.09f)), // Hitbox
                "Exit",                                                    // Text
                bellMT36);                                                 // Font
        }

        public void EndGame(int roomsCleared)
        {
            gameFSM = GameFSM.Menu;
        }
    }
}

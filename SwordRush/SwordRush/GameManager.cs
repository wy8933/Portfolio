using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Linq;
using System.Transactions;
using System.Drawing;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace SwordRush
{
    internal class GameManager
    {
        // --- Fields --- //

        private enum GameFSM
        {
            Playing,
            Paused,
            LevelUp,
            GameOver,
            Menu
        }

        private GameFSM gameFSM;
        private ContentManager contentManager_;
        private FileManager fileManager_;
        //game event
        private Rectangle window;
        public event GameOver gameOver;
        private KeyboardState currentKeyState;
        private KeyboardState previousKeyState;

        //objects
        public Player player;
        private List<Enemy> enemies;
        private Chest chest;

        //textures
        private Texture2D spriteSheet;  
        private Texture2D dungeontilesTexture2D;
        private Texture2D healthBarTexture;
        private Texture2D xpBarTexture;
        private Texture2D emptyBarTexture;
        private Texture2D whiteRectangle;
        private Texture2D abilityIcons;
        private Texture2D singleColor;

        // fonts
        private SpriteFont BellMT16;
        private SpriteFont BellMT18;
        private SpriteFont BellMT24;
        private SpriteFont BellMT48;
        private SpriteFont BellMT72;

        //tiling
        private int mapNum;
        private int totalRoom;
        private List<SceneObject> walls;
        private int[,] grid;
        private SceneObject[,] floorTiles;
        private List<SceneObject> wallTiles;
        private List<List<AStarNode>> graph;
        
        private static GameManager instance = null;

        // Level up buttons
        private List<ImageButton> levelUpButtons;
        private List<int> maxedPowers;

        // Graphics Device
        private GraphicsDevice graphicsDevice;

        // Mouse States
        MouseState currentMS;
        MouseState previousMS;

        //Economy system
        private int totalCoin;
        private int startAttack;
        private int startHealth;
        private int difficulty; 
        // Timer
        private double clickCooldown;

        // --- Properties --- //

        public static GameManager Get
        {
            get
            {
                // Does it exist yet? No? Make it!
                if (instance == null)
                {
                    // Call the default constructor.
                    instance = new GameManager();
                }

                // Either way, return the (newly made or already made) instance
                return instance;
            }

            // NEVER a set for the instance
        }

        public List<SceneObject> Walls => walls;
        public ContentManager ContentManager { get { return contentManager_; } }

        public int[,] Grid { get { return grid; } }
        public List<List<AStarNode>> Graph { get { return graph; } }
        public int TotalCoin { get { return totalCoin; } set { totalCoin = value; } }
        

        // --- Constructor --- //

        public void Initialize(ContentManager content, Point windowSize, Texture2D whiteRectangle, GraphicsDevice graphicsDevice)
        {
            contentManager_ = content;
            fileManager_ = new FileManager();

            //sprites & game states
            this.spriteSheet = spriteSheet;
            gameFSM = GameFSM.Menu;
            dungeontilesTexture2D = content.Load<Texture2D>("DungeonTiles");
            this.whiteRectangle = whiteRectangle;
            
            //objects
            enemies = new List<Enemy>();
            player = new Player(dungeontilesTexture2D, new Rectangle(0,0, 32, 64), graphicsDevice);
            chest = null;


            //tiling
            mapNum = 0;
            totalRoom = 10;
            grid = new int[20, 12];
            graph = new List<List<AStarNode>>();
            walls = new List<SceneObject>();
            floorTiles = new SceneObject[40, 24];
            wallTiles = new List<SceneObject>();
            for (int i = 0; i < floorTiles.GetLength(1); i++)
            {
                for (int j = 0; j < floorTiles.GetLength(0); j++)
                {
                    floorTiles[j,i] = new SceneObject(false, 20, dungeontilesTexture2D, new Rectangle(j * 32, i * 32, 32, 32));
                }
            }

            //window
            this.window = new Rectangle(0, 0,
                windowSize.X, windowSize.Y);

            // health and xp bars
            emptyBarTexture = content.Load<Texture2D>("empty_bar");
            healthBarTexture = content.Load<Texture2D>("health_bar");
            xpBarTexture = content.Load<Texture2D>("energy_bar");

            // Single color texture
            singleColor = new Texture2D(graphicsDevice, 1, 1);
            singleColor.SetData(new Color[] { Color.White });

            // Font
            BellMT16 = content.Load<SpriteFont>("Bell_MT-16");
            BellMT18 = content.Load<SpriteFont>("Bell_MT-18");
            BellMT24 = content.Load<SpriteFont>("Bell_MT-24");
            BellMT48 = content.Load<SpriteFont>("Bell_MT-48");
            BellMT72 = content.Load<SpriteFont>("Bell_MT-72");

            // graphicsDevice
            this.graphicsDevice = graphicsDevice;

            // Mouse States
            currentMS = Mouse.GetState(); 
            previousMS = Mouse.GetState();

            // level up buttons
            levelUpButtons = new List<ImageButton>();
            abilityIcons = content.Load<Texture2D>("AbilityIcons");
            InitializeLevelUpButtons();
            maxedPowers = new List<int>();

            //init player econ
            InitPlayerStats();
            difficulty = 1;

            // Player perk
            player.Perk = PlayerPerk.None;
        }

        public Player LocalPlayer
        {
            get { return player; }
        }

        // --- Methods --- //

        public void Update(GameTime gt)
        {
            currentMS = Mouse.GetState();
            currentKeyState = Keyboard.GetState();
            //special action to reset the stats
            if (currentKeyState.IsKeyDown(Keys.A)&& currentKeyState.IsKeyDown(Keys.B) && currentKeyState.IsKeyDown(Keys.C) &&
                currentKeyState.IsKeyDown(Keys.D) && currentKeyState.IsKeyDown(Keys.E) && currentKeyState.IsKeyDown(Keys.F) )
            {
                string stats = "";
                stats += 0 + ",";//coin
                stats += 1 + ",";
                stats += 10 + ",";
                stats += false + ",";
                stats += false + ",";
                stats += false;
                FileManager.SaveStats($"Content/PlayerProgress.txt", stats);
            }

            switch (gameFSM)
            {
                case GameFSM.Playing: // Playing Game
                    player.Update(gt);
                    //update chests
                    if (chest != null && player.Rectangle.Intersects(chest.Rectangle) && chest.Open == false)
                    {
                        chest.Open = true;

                        Random rand = new Random();
                        int num = rand.Next(100);
                        //heal
                        if (num < 20)
                        {
                            player.health += (int)(player.maxHealth / 4);
                            if (player.health > player.maxHealth)
                            {
                                player.health = player.maxHealth;
                            }
                            // gain exp
                        }else if (num < 60)
                        {
                            player.exp += (int)(player.LevelUpExp * 0.25);
                            //gain coin
                        }else
                        {
                            totalCoin += (int)(((player.RoomsCleared+1)*100)/ (num - 30));
                        }
                    }

                    //update all the enemies
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update(gt);

                        //update enemy collision
                        WallCollision(enemies[i], walls);

                        if (player.PlayerState == PlayerStateMachine.Attack &&
                            SwordCollision(enemies[i].Rectangle, 0, player.Sword.Rectangle, player.SwordRotateAngle()))
                        {
                            enemies[i].Damaged();
                            Rectangle r0 = player.Sword.Rectangle;
                        }

                        if (enemies[i].Rectangle.Intersects(player.Rectangle) && enemies[i].Health > 0
                            && enemies[i].EnemyState == EnemyStateMachine.Move)
                        {
                            SoundManager.Get.EnemyAttackEffect.Play();
                            enemies[i].AttackCooldown();
                            player.Damaged(enemies[i].Atk);
                            

                        }

                        if (enemies[i].Health <= 0)
                        {
                            player.GainExp(enemies[i].Level);
                            SoundManager.Get.EnemyDeathEffect.Play();
                            enemies.RemoveAt(i);

                            // Vampire Heal
                            Random rng = new Random();
                            if (player.VampirePower >= rng.Next(1, 4))
                            {
                                player.Heal(1);
                            }
                        }
                    }

                    // End game if player health runs out
                    if (player.Health <= 0)
                    {
                        gameFSM = GameFSM.GameOver;
                        gameOver(player.RoomsCleared);
                        clickCooldown = 0;

                        //add coin when game over
                        totalCoin += player.RoomsCleared * difficulty + player.Level;
                        UpdateEcon();
                    }
                    //update player collision
                    WallCollision(player, walls);
                    
                    //get keyboard state
                    currentKeyState = Keyboard.GetState();

                    //go to next level shortcut
                    if (currentKeyState.IsKeyDown(Keys.Enter) && previousKeyState.IsKeyUp(Keys.Enter))
                    {
                        enemies.Clear();
                    }

                    //check if enemies are all dead
                    if (enemies.Count == 0 && UI.Get.GameFSM == SwordRush.GameFSM.Game)
                    {
                        SoundManager.Get.LevelCleardEffect.Play();
                        player.RoomsCleared += 1;
                        StartGame();
                        player.NewRoom();

                    }
                    
                    // Pause game on esc click
                    if (currentKeyState.IsKeyDown(Keys.Escape)
                        && previousKeyState.IsKeyUp(Keys.Escape))
                    {
                        gameFSM = GameFSM.Paused;
                    }

                    // Go To Level Up Screen
                    if (player.Exp >= player.LevelUpExp)
                    {
                        if (currentKeyState.IsKeyDown(Keys.Space)
                            && previousKeyState.IsKeyUp(Keys.Space))
                        {
                            gameFSM |= GameFSM.LevelUp;
                            RandomizeLevelUpAbilities();
                            clickCooldown = 0;
                        }
                    }
                    break;

                case GameFSM.Paused:

                    // End game on right click
                    if (currentMS.RightButton == ButtonState.Released
                        && previousMS.RightButton == ButtonState.Pressed)
                    {
                        player.Die();
                        gameOver(player.RoomsCleared);
                        gameFSM = GameFSM.Menu;
                    }
                    // Unpause when left click
                    if (currentMS.LeftButton == ButtonState.Released
                        && previousMS.LeftButton == ButtonState.Pressed)
                    {
                        gameFSM = GameFSM.Playing;
                    }

                    break;

                case GameFSM.LevelUp:

                    // Update buttons to check if clicked
                    UpdateLevelButtons(gt);

                    // Only allow click after one second has passed to give player
                    // time to read menu and not accidentally click
                    clickCooldown += gt.ElapsedGameTime.TotalMilliseconds;
                    if (clickCooldown >= 1000)
                    {
                        // Increase Stat when pressed
                        if (levelUpButtons[0].LeftClicked)
                        {
                            // Heal to max health
                            player.LevelUp(LevelUpAbility.Heal);
                            gameFSM = GameFSM.Playing;
                        }
                        else if (levelUpButtons[1].LeftClicked)
                        {
                            // Increase max health
                            player.LevelUp(LevelUpAbility.MaxHealth);
                            gameFSM = GameFSM.Playing;
                        }
                        else if (levelUpButtons[2].LeftClicked)
                        {
                            // Increase attack speed
                            player.LevelUp(LevelUpAbility.AttackSpeed);
                            gameFSM = GameFSM.Playing;

                            // Max attack speed
                            if (player.AtkSpd >= 8)
                            {
                                maxedPowers.Add(2);
                            }
                        }
                        else if (levelUpButtons[3].LeftClicked)
                        {
                            // Increase attack damage
                            player.LevelUp(LevelUpAbility.AttackDamage);
                            gameFSM = GameFSM.Playing;
                        }
                        else if (levelUpButtons[4].LeftClicked)
                        {
                            // Increase movement range
                            player.LevelUp(LevelUpAbility.Range);
                            gameFSM = GameFSM.Playing;

                            // Max movement range
                            if (player.AtkSpd >= 3)
                            {
                                maxedPowers.Add(4);
                            }
                        }
                        else if (levelUpButtons[5].LeftClicked)
                        {
                            // Increase backup range
                            player.LevelUp(LevelUpAbility.BackUp);
                            gameFSM = GameFSM.Playing;
                        }
                        else if (levelUpButtons[6].LeftClicked)
                        {
                            // Increase Vampire Power
                            player.LevelUp(LevelUpAbility.Vampire);
                            gameFSM = GameFSM.Playing;

                            // Max movement range
                            if (player.VampirePower >= 3)
                            {
                                maxedPowers.Add(6);
                            }
                        }
                        else if (levelUpButtons[7].LeftClicked)
                        {
                            // Increase Sheild Power
                            player.LevelUp(LevelUpAbility.Shield);
                            gameFSM = GameFSM.Playing;
                        }
                    }

                    break;

                case GameFSM.GameOver:

                    // Only allow click after one second has passed to give player
                    // time to read menu and not accidentally click
                    clickCooldown += gt.ElapsedGameTime.TotalMilliseconds;
                    if (clickCooldown >= 1000)
                    { 
                        // Return to menu when mouse down
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            gameFSM = GameFSM.Menu;
                            gameOver(player.RoomsCleared);
                        }
                    }
                    break;
            }
            previousMS = Mouse.GetState();
            previousKeyState = currentKeyState;
            UpdateGrid();
            UpdateGraph();
        }

        public void Draw(SpriteBatch sb)
        {
            //draw the total coin
            sb.DrawString(BellMT24,
                $"Total Coins: {totalCoin}",
                new Vector2(1000, 10),
                Color.LightGray);
            // Testing image buttons
            switch (gameFSM)
            {
                case GameFSM.Playing:

                    DrawGame(sb);

                    break;

                case GameFSM.Paused: // --- Paused --------------------------------------------//

                    // elements of the game a drawn but not updated
                    DrawGame(sb);

                    // Add a transparent black rectangle over the game
                    // to darken screen and make text stand out
                    sb.Draw(singleColor,
                        window,
                        Color.Black * 0.4f);

                    // Text for the pause menu
                    sb.DrawString(BellMT72,
                        $"Paused",
                        new Vector2(window.Width *0.38f, window.Height * 0.36f),
                        Color.White);

                    sb.DrawString(BellMT24,
                        $"Right Click to Quit\n" +
                        $"Left Click to Resume",
                        new Vector2(window.Width * 0.38f, window.Height * 0.56f),
                        Color.White);

                    // Draw all the power ups & levels
                    
                    // Attack Speed
                    if (maxedPowers.Contains(2))
                    {
                        DrawPowerUpLevel(sb, new Point(11, 3),
                            $"Attack Cooldown: {800 - 75 * player.AtkSpd}ms: Maxed", 0.3f);
                    }
                    else
                    {
                        DrawPowerUpLevel(sb, new Point(11, 3), 
                            $"Attack Cooldown: {800 - 75 * player.AtkSpd}ms", 0.3f);
                    }

                    // Damage
                    DrawPowerUpLevel(sb, new Point(0, 3),
                            $"Damage: {Math.Round(player.Atk,2)}", 0.4f);

                    // Range
                    if (maxedPowers.Contains(4))
                    {
                        DrawPowerUpLevel(sb, new Point(10, 3),
                            $"Range: {player.Range}: Maxed", 0.5f);
                    }
                    else
                    {
                        DrawPowerUpLevel(sb, new Point(10, 3),
                            $"Range: {player.Range}", 0.5f);
                    }


                    // --- Perks ---------------------------------------------------//
                    // Dodge Distance
                    // Check if perk is equipted
                    if (player.Perk == PlayerPerk.Dodge)
                    {
                        if (maxedPowers.Contains(5))
                        {
                            DrawPowerUpLevel(sb, new Point(13, 2),
                                $"Dodge Distance: {player.BackUpLevel}: Maxed", 0.6f);
                        }
                        else
                        {
                            DrawPowerUpLevel(sb, new Point(13, 2),
                                $"Dodge Distance: {player.BackUpLevel}", 0.6f);
                        }
                    } else
                    {
                        DrawPowerUpLevel(sb, new Point(13, 2),
                                $"NOT EQUIPTED", 0.6f);
                    }

                    // Vampire Power
                    // Check if perk is equipted
                    if (player.Perk == PlayerPerk.Vampire)
                    {
                        if (maxedPowers.Contains(6))
                        {
                            DrawPowerUpLevel(sb, new Point(2, 3),
                                $"Life Steal: 100%: Maxed", 0.7f);
                        }
                        else
                        {
                            DrawPowerUpLevel(sb, new Point(2, 3),
                                $"Life Steal: {player.VampirePower * 33}%", 0.7f);
                        }
                    }
                    else
                    {
                        DrawPowerUpLevel(sb, new Point(2, 3),
                                $"NOT EQUIPTED", 0.7f);
                    }

                    // Sheild
                    if (player.Perk == PlayerPerk.Sheild)
                    {
                        DrawPowerUpLevel(sb, new Point(7, 0),
                            $"Sheild: {player.shiledLevel}", 0.8f);
                    }
                    else
                    {
                        DrawPowerUpLevel(sb, new Point(7, 0),
                            $"NOT EQUIPTED", 0.8f);
                    }
                    break;

                case GameFSM.LevelUp:

                    // elements of the game a drawn but not updated
                    DrawGame(sb);

                    // Add a transparent black rectangle over the game
                    // to darken screen and make text stand out
                    sb.Draw(singleColor,
                        window,
                        Color.Black * 0.4f);

                    // Draw Text
                    sb.DrawString(BellMT72,
                        $"LEVEL UP",
                        new Vector2(window.Width * 0.3f, window.Height * 0.16f),
                        Color.LightGreen);

                    // Draw all buttons
                    foreach (ImageButton i in levelUpButtons)
                    {
                        i.Draw(sb);
                    }

                    break;

                case GameFSM.GameOver:  // --- Game Over -----------------------------------------------//

                    // elements of the game a drawn but not updated
                    DrawGame(sb);

                    // Add a transparent black rectangle over the game
                    // to darken screen and make text stand out
                    sb.Draw(singleColor,
                        window,
                        Color.Black * (float)(clickCooldown / 2000));

                    // Draw Game over
                    sb.DrawString(
                        BellMT72,
                        "GAME OVER",
                        new Vector2((window.Width * 0.26f),
                        (window.Height * 0.38f)),
                        Color.White);

                    // Draw Score
                    sb.DrawString(
                        BellMT48,                           // Font
                        $"YOU CLEARED {player.RoomsCleared} ROOMS\n" +
                        $"YOU GOT {player.RoomsCleared*difficulty+player.Level} COINS",            // Text
                        new Vector2((window.Width * 0.2f),  // X Position
                        (window.Height * 0.52f)),            // Y Position
                        Color.White);                       // Color

                    break;

            }
        }

        public void InitPlayerStats()
        {
            string[] stats = fileManager_.LoadStats($"Content/PlayerProgress.txt");
            totalCoin = int.Parse(stats[0]);
            startAttack = int.Parse(stats[1]);
            startHealth = int.Parse(stats[2]);
            UI.Get.dodgePurchased = (stats[3] == "True");
            UI.Get.shieldPurchased = (stats[4] == "True");
            UI.Get.vampirePurchased = (stats[5] == "True");
        }

        public void UpdateEcon()
        {
            string stats = "";
            stats += totalCoin+",";
            stats += startAttack + ",";
            stats += startHealth + ",";
            stats += UI.Get.dodgePurchased  + ",";
            stats += UI.Get.shieldPurchased + ",";
            stats += UI.Get.vampirePurchased;
            Debug.WriteLine(UI.Get.dodgePurchased);
            
            FileManager.SaveStats($"Content/PlayerProgress.txt",stats);
        }

        public void UpdateGraph()
        {
            graph.Clear();
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                List<AStarNode> temp = new List<AStarNode>();
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    bool walkable = true;
                    if (grid[j, i] == 1)
                    {
                        walkable = false;
                    }
                    temp.Add(new AStarNode(new Vector2(j*64,i*64),walkable,1));
                }
                graph.Add(temp);
            }
        }

        /// <summary>
        /// Draws all elements of the game
        /// Player,Enemies,Tiles,Health and XP bar
        /// </summary>
        /// <param name="sb"></param>
        private void DrawGame(SpriteBatch sb)
        {
            foreach (SceneObject tile in floorTiles)
            {
                tile.Draw(sb);
            }

            //draw walls
            foreach (SceneObject obj in walls)
            {
                obj.Draw(sb);
            }

            //draw tiles
            foreach (SceneObject tile in wallTiles)
            {
                tile.Draw(sb);
            }

            //draw objects
            if (chest != null)
            {
                chest.Draw(sb);
            }
            player.Draw(sb);
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(sb);
            }

            // Display health and xp bars
            drawBar(healthBarTexture,
                player.Health,
                player.MaxHealth,
                new Rectangle((int)(window.Width * 0.12f), (int)(window.Height * 0.92f),
                (int)(window.Width * 0.3f), (int)(window.Height * 0.079)),
                sb);

            drawBar(xpBarTexture,
                player.Exp,
                player.LevelUpExp,
                new Rectangle((int)(window.Width * 0.62f), (int)(window.Height * 0.92f),
                (int)(window.Width * 0.3f), (int)(window.Height * 0.079f)),
                sb);

            // If level up availabe say so
            if (player.Exp >= player.LevelUpExp
                && gameFSM != GameFSM.LevelUp)
            {
                // Change the color randomly
                Random rng = new Random();
                Color color = Color.Green;
                if (rng.Next(0,2) == 1)
                {
                    color = Color.LightGreen;
                }
                // Draw Background
                sb.Draw(singleColor,
                    new Rectangle((int)(window.Width * 0.615f), (int)(window.Height * 0.865f),
                (int)(window.Width * 0.27f), (int)(window.Height * 0.05f)),
                    Color.Black * 0.5f);

                // Draw Text
                sb.DrawString(
                    BellMT24,
                    "Press Space To Level Up",
                    new Vector2((int)(window.Width * 0.62f), (int)(window.Height * 0.87f)),
                    color);
            }

                // Draw Box around Room Number top right
                sb.Draw(singleColor,
                new Rectangle(0, 0,
                (int)(window.Width * (0.19f + 0.011f*(player.RoomsCleared.ToString().Count()))),
                (int)(window.Height * 0.07f)),
                Color.White);
            sb.Draw(singleColor,
                new Rectangle(1, 1,
                (int)(window.Width * (0.19f + 0.011f * (player.RoomsCleared.ToString().Count())) - 2),
                (int)(window.Height * 0.07f - 2)),
                Color.Black);

            // Draw Room Number
            sb.DrawString(BellMT24,
                $"Rooms Cleared: {player.RoomsCleared}",
                new Vector2(10, 10),
                Color.LightGray);


            // Tutorial text on first room
            if (player.RoomsCleared == 0)
            {
                // Draw Box around Room Number top right
                sb.Draw(singleColor,
                    new Rectangle(0, (int)(window.Height*0.07f),
                    (int)(window.Width * 0.25f), (int)(window.Height * 0.24f)),
                    Color.White);
                sb.Draw(singleColor,
                    new Rectangle(1, (int)(window.Height * 0.07f + 1),
                    (int)(window.Width * 0.25f - 2), (int)(window.Height * 0.24f-2)),
                    Color.Black);

                sb.DrawString(BellMT24,                           // Font
                        $"Left Click To Dash\n(When sword is solid)" +
                        $"\nPress space level up" +
                        $"\nDefeat the enemies to\nclear the room",            // Text
                        new Vector2(10,  // X Position
                        (window.Height * 0.08f)),            // Y Position
                        Color.White);                       // Color
            }
        }

        /// <summary>
        /// creates wall objects based on where 1 values in the grid are
        /// </summary>
        public void GenerateRoom()
        {
            walls.Clear();
            chest = null;
            int enemyLevelGrow = 3;

            switch (difficulty)
            {
                case 1:
                    enemyLevelGrow = 3;
                    break;
                case 2:
                    enemyLevelGrow = 2;
                    break;
                case 3:
                    enemyLevelGrow = 1;
                    break;
            }

            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[j,i] == 1)
                    {
                        walls.Add(new SceneObject(true,0, whiteRectangle, new Rectangle(j*64, i*64, 64, 64)));
                    }
                    else if (grid[j,i] == 2)
                    {
                        enemies.Add(new ShortRangeEnemy(dungeontilesTexture2D, new Rectangle(j*64, i*64, 32, 32), player, (player.RoomsCleared / enemyLevelGrow) + 1, graphicsDevice));
                    }
                    else if (grid[j,i] == 3)
                    {
                        player.X = (j * 64 + 32);
                        player.Y = (i * 64 + 32);
                    }
                    else if (grid[j,i] == 4 && player.RoomsCleared%5 == 0)
                    {
                        chest = new Chest(dungeontilesTexture2D, new Rectangle(j*64+16, i*64+16, 32, 32));
                    }
                    else if (grid[j, i] == 5)
                    {
                        enemies.Add(new LongRangeEnemy(dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32), player, (player.RoomsCleared / enemyLevelGrow) + 1, graphicsDevice));
                    }
                }
            }

            wallTiles.Clear();
            int[,] tileGrid;
            //if game just start generate the intro room, else randomly pick room
            if (player.RoomsCleared == 0)
            {
                tileGrid = fileManager_.LoadWallTiles($"Content/Level{0}.txt");
            }
            else
            {
                //the second parameter is the number of total room
                tileGrid = fileManager_.LoadWallTiles($"Content/Level{mapNum}.txt");
            }


            for (int i = 0; i < tileGrid.GetLength(1); i++)
            {
                for (int j = 0; j < tileGrid.GetLength(0); j++)
                {
                    switch (tileGrid[j,i])
                    {
                        case 1:
                            wallTiles.Add(new SceneObject(false, 1, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 1, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 4, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 4, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            break;
                        case 2:
                            wallTiles.Add(new SceneObject(false, 2, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 8, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            break;
                        case 3:
                            wallTiles.Add(new SceneObject(false, 3, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            break;
                        case 4:
                            wallTiles.Add(new SceneObject(false, 4, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 4, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            break;
                        case 5:
                            wallTiles.Add(new SceneObject(false, 5, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 7, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 +32, 32, 32)));
                            break;
                        case 6:
                            wallTiles.Add(new SceneObject(false, 6, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            break;
                        case 7:
                            wallTiles.Add(new SceneObject(false, 7, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 7, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            break;
                        case 8:
                            wallTiles.Add(new SceneObject(false, 8, dungeontilesTexture2D, new Rectangle(j * 64 , i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 8, dungeontilesTexture2D, new Rectangle(j * 64 , i * 64 + 32, 32, 32)));
                            break;
                        case 9:
                            wallTiles.Add(new SceneObject(false, 8, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 9, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 1, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            break;
                        case 10:
                            wallTiles.Add(new SceneObject(false, 7, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 10, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 1, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            break;
                        case 11:
                            wallTiles.Add(new SceneObject(false, 1, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 1, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            break;
                        case 12:
                            wallTiles.Add(new SceneObject(false, 4, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 4, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 9, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 10, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            break;
                        case 13:
                            wallTiles.Add(new SceneObject(false, 6, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 7, dungeontilesTexture2D, new Rectangle(j * 64 + 32, i * 64, 32, 32)));
                            break;
                        case 14:
                            wallTiles.Add(new SceneObject(false, 3, dungeontilesTexture2D, new Rectangle(j * 64, i * 64 + 32, 32, 32)));
                            wallTiles.Add(new SceneObject(false, 8, dungeontilesTexture2D, new Rectangle(j * 64, i * 64, 32, 32)));
                            break;
                    }
                    
                }
            }

        }

        /// <summary>
        /// updates the position of the player and enemies on the grid
        /// </summary>
        public void UpdateGrid()
        {
            //clear old positions of enemies and players and save wall positions
            int[,] tempWallGrid = new int[20, 12];
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[j,i] == 1)
                    {
                        tempWallGrid[j,i] = 1;
                    }
                    if (grid[j,i] == 3 || grid[j,i] == 2)
                    {
                        grid[j,i] = 0;
                    }
                }
            }

            //add enemy positions
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Position.X > 0 && enemy.Position.Y > 0
                                         && enemy.Position.X < window.Width && enemy.Position.Y < window.Height)
                {
                    if (enemy is ShortRangeEnemy)
                    {
                        ShortRangeEnemy SRenemy = (ShortRangeEnemy)enemy;
                        SRenemy.graphPoint = new Point(Convert.ToInt32(enemy.Position.X) / 64, Convert.ToInt32(enemy.Position.Y) / 64);
                        grid[Convert.ToInt32(enemy.Position.X) / 64, Convert.ToInt32(enemy.Position.Y) / 64] = 2;
                    }
                    
                }
            }

            //add player position
            if (player.Position.X > 0 && player.Position.Y > 0
                && player.Position.X < window.Width && player.Position.Y < window.Height)
            {
                player.graphPoint = new Point(Convert.ToInt32(player.Position.X) / 64, Convert.ToInt32(player.Position.Y) / 64);
                grid[Convert.ToInt32(player.Position.X) / 64, Convert.ToInt32(player.Position.Y) / 64] = 3;
            }

            //return walls to old values
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (tempWallGrid[j, i] == 1)
                    {
                        grid[j, i] = 1;
                    }
                }
            }
        }

        public bool WallCollision(GameObject entity, List<SceneObject> walls)
        {
            bool collide = false;
            //temporary variables for calculations
            Rectangle entityRect = entity.Rectangle;

            // For taller entities we only want to check collision with their feet
            if (entity.Rectangle.Width < entity.Rectangle.Height)
            {
                entityRect = new Rectangle(entityRect.X, entityRect.Y + entityRect.Height / 2, entityRect.Width, entityRect.Height / 2);
            }
            List<Rectangle> intersectionsList = new List<Rectangle>();
            List<Rectangle> wallRects = new List<Rectangle>();
            intersectionsList.Clear();
            wallRects.Clear();
            
            foreach (SceneObject obj in walls)
            {
                wallRects.Add(obj.Rectangle);
            }

            //get list of intersections
            for (int i = 0; i < walls.Count; i++)
            {
                if (entityRect.Intersects(wallRects[i]))
                {
                    intersectionsList.Add(wallRects[i]);
                    collide = true;
                }
            }

            //collision

            //loop through intersects
            for (int i = 0; i < intersectionsList.Count; i++)
            {
                Rectangle intersect = Rectangle.Intersect(entityRect, intersectionsList[i]);

                //horizontal collision
                if (intersect.Width < intersect.Height)
                {
                    if (entityRect.X < intersect.X)
                    {
                        entityRect.X -= intersect.Width;
                    }
                    else
                    {
                        entityRect.X += intersect.Width;
                    }
                }
                //vertical collision
                else
                {
                    if (entityRect.Y < intersect.Y)
                    {
                        entityRect.Y -= intersect.Height;
                    }
                    else
                    {
                        entityRect.Y += intersect.Height;
                    }
                    
                }
            }

            //update player position
            entity.X = entityRect.X+entityRect.Width / 2;
            entity.Y = entityRect.Y+entityRect.Height / 2;

            // For player sprite to collide with wall properly and not have space above head
            if (entity.Rectangle.Width < entity.Rectangle.Height)
            {
                entity.Y = entityRect.Y;
            }

            return collide;

        }

        /// <summary>
        /// check collision of the sword and other objects
        /// </summary>
        /// <param name="other">rectangle of other</param>
        /// <param name="rotationR0">rotation angle of other</param>
        /// <param name="sword">rectangle of the sword</param>
        /// <param name="rotationR1">rotation angle of the sword</param>
        /// <returns>if collision had happen</returns>
        public static bool SwordCollision(Rectangle other, float rotationR0, Rectangle sword, float rotationR1)
        {
            Vector2 rotOriginR0 = new Vector2(other.X + other.Width * .5f, other.Y + other.Height * .5f);
            Vector2 rotOriginR1 = new Vector2(sword.X + 16, sword.Y + 16);

            // get rotated points of rectangle 1
            Vector2 A0 = new Vector2(other.Left, other.Top);
            Vector2 B0 = new Vector2(other.Right, other.Top);
            Vector2 C0 = new Vector2(other.Right, other.Bottom);
            Vector2 D0 = new Vector2(other.Left, other.Bottom);
            // optimally you store the shapes points in clockwise (cw) or ccw order.
            // we could also do this with just two rotations saving a lot of this extra work
            A0 = RotatePoint(A0, rotOriginR0, rotationR0);
            B0 = RotatePoint(B0, rotOriginR0, rotationR0);
            C0 = RotatePoint(C0, rotOriginR0, rotationR0);
            D0 = RotatePoint(D0, rotOriginR0, rotationR0);

            // get rotated points of rectangle 2
            Vector2 A1 = new Vector2(sword.Left, sword.Top);
            Vector2 B1 = new Vector2(sword.Right, sword.Top);
            Vector2 C1 = new Vector2(sword.Right, sword.Bottom);
            Vector2 D1 = new Vector2(sword.Left, sword.Bottom);
            A1 = RotatePoint(A1, rotOriginR1, rotationR1);
            B1 = RotatePoint(B1, rotOriginR1, rotationR1);
            C1 = RotatePoint(C1, rotOriginR1, rotationR1);
            D1 = RotatePoint(D1, rotOriginR1, rotationR1);

            // you can return true with just one match but this is left to demonstrate.
            bool match = false;

            // first rectangle
            if (IsPointWithinRectangle(A0, B0, C0, D0, A1)) { match = true; }
            if (IsPointWithinRectangle(A0, B0, C0, D0, B1)) { match = true; }
            if (IsPointWithinRectangle(A0, B0, C0, D0, C1)) { match = true; }
            if (IsPointWithinRectangle(A0, B0, C0, D0, D1)) { match = true; }
            // second rectangle
            if (IsPointWithinRectangle(A1, B1, C1, D1, A0)) { match = true; }
            if (IsPointWithinRectangle(A1, B1, C1, D1, B0)) { match = true; }
            if (IsPointWithinRectangle(A1, B1, C1, D1, C0)) { match = true; }
            if (IsPointWithinRectangle(A1, B1, C1, D1, D0)) { match = true; }

            return match;
        }

        /// <summary>
        /// check if the collision point in in the rectangle
        /// </summary>
        /// <param name="A">point A</param>
        /// <param name="B">point B</param>
        /// <param name="C">point C</param>
        /// <param name="D">point D</param>
        /// <param name="collision_point">the collision point</param>
        /// <returns>if the point is in the rectangle</returns>
        public static bool IsPointWithinRectangle(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 collision_point)
        {
            int numberofplanescrossed = 0;
            if (HasPointCrossedPlane(A, B, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; }
            if (HasPointCrossedPlane(B, C, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; }
            if (HasPointCrossedPlane(C, D, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; }
            if (HasPointCrossedPlane(D, A, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; }

            return numberofplanescrossed >= 4;
        }

        /// <summary>
        /// calculate if the point has cross the plane
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="collision_point">the point to be check</param>
        /// <returns>if the point had crossed plane</returns>
        public static bool HasPointCrossedPlane(Vector2 start, Vector2 end, Vector2 collision_point)
        {
            Vector2 B = (end - start);
            Vector2 A = (collision_point - start);
            // We only need the signed result
            // cross right and dot 
            float sign = A.X * -B.Y + A.Y * B.X;
            return sign > 0.0f;
        }

        /// <summary>
        /// rotate the point
        /// </summary>
        /// <param name="p">the point</param>
        /// <param name="o">the origin</param>
        /// <param name="q">the rotating angle</param>
        /// <returns>the new point</returns>
        public static Vector2 RotatePoint(Vector2 p, Vector2 o, double q)
        {
            //x' = x*cos s - y*sin s , y' = x*sin s + y*cos s 
            double x = p.X - o.X; // transform locally to the orgin
            double y = p.Y - o.Y;
            double rx = x * Math.Cos(q) - y * Math.Sin(q);
            double ry = x * Math.Sin(q) + y * Math.Cos(q);
            p.X = (float)rx + o.X; // translate back to non local
            p.Y = (float)ry + o.Y;
            return p;
        }

        public void StartGame()
        {
            gameFSM = GameFSM.Playing;

            //load grid
            // randomly pick a room to generate
            if (player.RoomsCleared == 0)
            {
                mapNum = 0;
            }
            else
            {
                int temp = new Random().Next(1, totalRoom);
                while (mapNum != temp)
                {
                    mapNum = temp;
                }
            }

            grid = fileManager_.LoadGrid($"Content/Level{mapNum}.txt");
            
            enemies.Clear();
            //generates current room based on grid
            GenerateRoom();
        }

        public void QuitGame()
        {
            player.Die();
            gameFSM = GameFSM.Menu;
        }

        public void drawBar(Texture2D texture, int value, int maxValue, Rectangle size, SpriteBatch sb)
        {
            sb.Draw(emptyBarTexture,
                size,
                new Rectangle(0,0,emptyBarTexture.Width,(int)(emptyBarTexture.Height*0.25f)),
                Color.White);

            sb.Draw(texture,
                new Rectangle((int)(size.X - window.Width * 0.06f), size.Y, size.Width / 6, size.Height),
                new Rectangle(0, (int)(texture.Height * 0.55f), texture.Width / 3, texture.Height / 2),
                Color.White);

            int barPercent = value * 120 / maxValue;

            sb.Draw(texture,
                new Rectangle(size.X,size.Y,(size.Width * barPercent) / 120,size.Height),
                new Rectangle(0,0,(texture.Width* barPercent) / 120,(int)(texture.Height*0.25f)),
                Color.White);

            sb.DrawString(
                BellMT24,
                $"{value}/{maxValue}",
                new Vector2(size.X+ window.Width * 0.015f, size.Y + window.Height * 0.015f),
                Color.White);
            
        }

        /// <summary>
        /// Show power up levels during pause menu
        /// </summary>
        /// <param name="sb">spritebatch</param>
        /// <param name="source"> source rectangle in ability tile sheet</param>
        /// <param name="text"> Power up and level </param>
        /// <param name="height"> what height to display on screen</param>
        public void DrawPowerUpLevel(SpriteBatch sb, Point source, string text, float height)
        {
            int tileSize = abilityIcons.Width / 16;
            // Draw Icon
            sb.Draw(abilityIcons,
                new Rectangle((int)(window.Width*0.01f),(int)(window.Height*height),
                (int)(window.Width * 0.05f), (int)(window.Width * 0.05f)),
                new Rectangle(source.X * tileSize, source.Y * tileSize, tileSize, tileSize),
                Color.White);

            // Draw Text
            sb.DrawString(BellMT24,
                text,
                new Vector2((int)(window.Width*0.07f), (int)(window.Height*(height+0.015f))),
                Color.White);
        }

        /// <summary>
        /// Set the defualt values of all the upgrade buttons
        /// </summary>
        public void InitializeLevelUpButtons()
        {
            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height * 2,0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Heal",
                "Fully Heal Player" +
                $"\n{player.Health} => {player.MaxHealth}",
                BellMT24,
                abilityIcons,
                new Vector2(6,2)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height * 2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Max Health",
                "Increase Maximum" +
                "\nHealth of Player" +
                $"\n{player.MaxHealth} => {player.MaxHealth * 1.5}",
                BellMT24,
                abilityIcons,
                new Vector2(12, 5)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height*2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Attack Speed",
                "Decrease cooldown" +
                "\nbetween attacks" +
                $"\n{800 - 75 * player.AtkSpd}ms => {800 - 75 * (player.AtkSpd + 2)}ms",
                BellMT24,
                abilityIcons,
                new Vector2(11, 3)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height*2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Damage",
                "Increase Attack Damage" +
                $"\n{player.Atk} => {player.Atk * 1.5}",
                BellMT24,
                abilityIcons,
                new Vector2(0, 3)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height*2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Range",
                "Increase distance moved" +
                "\nforward during attack" +
                $"\n{player.Range} => {player.Range + 1}",
                BellMT24,
                abilityIcons,
                new Vector2(10, 3)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height * 2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Dodge Distance",
                "Increase backwards" +
                "\ndistance during dodge" +
                $"\n{player.BackUpLevel} => {player.BackUpLevel + 1}",
                BellMT24,
                abilityIcons,
                new Vector2(13, 2)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height * 2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Life Steal",
                "Chance to gain Health" +
                "\nAfter Killing Enemy" +
                $"\n{100*Math.Round(player.VampirePower*0.33f,2)}% " +
                $"=> {100*Math.Round((1+player.VampirePower) * 0.3333f, 2)}%",
                BellMT24,
                abilityIcons,
                new Vector2(2, 3)));

            levelUpButtons.Add(new ImageButton(
                new Rectangle(window.Height * 2, 0,
                (int)(window.Height * 0.2f), (int)(window.Height * 0.2f)),
                "Sheild",
                "Grant Immunity" +
                "\nAfter Taking Damage" +
                $"\n{player.shiledLevel}" +
                $"=> {player.shiledLevel+1}",
                BellMT24,
                abilityIcons,
                new Vector2(7, 0)));
        }

        /// <summary>
        /// Randomly pick 3 abilities to offer
        /// </summary>
        public void RandomizeLevelUpAbilities()
        {
            // remove all buttons from the screen
            foreach (ImageButton button in levelUpButtons)
            {
                button.Rectangle = new Rectangle(button.Rectangle.X, window.Height*2,
                    button.Rectangle.Width, button.Rectangle.Height);
            }
            Random rng = new Random();

            // Generate 3 unique numbers
            int[] randomAbilities = new int[3]
            {
                -1,-1,-1
            };

            for (int i = 0; i < 3; i+= 0)
            {
                int next = rng.Next(0, 8);

                // Check if the perk is equipted
                // if not return to beginning of loop
                // And choose new power up
                if (next > 4)
                {
                    if (next != 5 && player.Perk == PlayerPerk.Dodge)
                    {
                        continue;
                    }
                    if (next != 6 && player.Perk == PlayerPerk.Vampire)
                    {
                        continue;
                    }
                    if (next != 7 && player.Perk == PlayerPerk.Sheild)
                    {
                        continue;
                    }
                    if (player.Perk == PlayerPerk.None)
                    {
                        continue;
                    }
                }
                // Make sure the ability is not already picked
                // or it hasn't been maxed out
                if (!randomAbilities.Contains(next)
                    && !maxedPowers.Contains(next))
                {
                    randomAbilities[i] = next;
                    i++;
                }
            }

            // 1st ability
            levelUpButtons[randomAbilities[0]].Rectangle =
                new Rectangle((int)(window.Width*0.1f),(int)(window.Height*0.4f),
                (int)(window.Width*0.2f),(int)(window.Width*0.2f));

            // 2nd ability
            levelUpButtons[randomAbilities[1]].Rectangle =
                new Rectangle((int)(window.Width * 0.4f), (int)(window.Height * 0.4f),
                (int)(window.Width * 0.2f), (int)(window.Width * 0.2f));

            // 3rd ability
            levelUpButtons[randomAbilities[2]].Rectangle =
                new Rectangle((int)(window.Width * 0.7f), (int)(window.Height * 0.4f),
                (int)(window.Width * 0.2f), (int)(window.Width * 0.2f));

        }

        public void UpdateLevelButtons(GameTime gt)
        {
            levelUpButtons[0].Update(
                gt,
                "Fully Heal Player" +
                $"\n{player.Health} => {player.MaxHealth}");

            levelUpButtons[1].Update(
                gt,
                "Increase Maximum" +
                "\nHealth of Player" +
                $"\n{player.MaxHealth} => {(int)(player.MaxHealth * 1.5)}");

            levelUpButtons[2].Update(
                gt,
                "Decrease cooldown" +
                "\nbetween attacks" +
                $"\n{800 - 75 * player.AtkSpd}ms => {800 - 75 * (player.AtkSpd + 2)}ms");

            levelUpButtons[3].Update(
                gt,
                "Increase Attack Damage" +
                $"\n{Math.Round(player.Atk,1)} => {Math.Round(player.Atk * 1.5,1)}");

            levelUpButtons[4].Update(
                gt,
                "Increase distance moved" +
                "\nforward during attack" +
                $"\n{player.Range} => {player.Range + 1}");

            levelUpButtons[5].Update(
                gt,
                "Increase backwards" +
                "\ndistance during dodge" +
                $"\n{player.BackUpLevel} => {player.BackUpLevel + 1}");

            levelUpButtons[6].Update(
                gt,
                "Chance to gain Health" +
                "\nAfter Killing Enemy" +
                $"\n{100 * Math.Round(player.VampirePower * 0.3333f, 2)}% " +
                $"=> {100 * Math.Round((1 + player.VampirePower) * 0.3333f, 2)}%");

            levelUpButtons[7].Update(
                gt,
                "Decrease Sheild" +
                "\nRecharge Cooldown" +
                $"\n{player.shiledLevel}" +
                $"=> {player.shiledLevel + 1}");
        }
    }
}

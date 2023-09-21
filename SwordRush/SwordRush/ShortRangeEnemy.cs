using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Reflection.Metadata;

namespace SwordRush
{
    internal class ShortRangeEnemy : Enemy
    {
        Player player;
        private double damageFrame;
        private Animation<Texture2D> _animation = new(0.2f);
        private int maxHP;
        private Vector2 direction;
        private Astar astar;
        private Stack<AStarNode> path;
        public Point graphPoint;
        private Vector2 distance;

        private AStarNode pos;

        // --- Constructor --- //
        public ShortRangeEnemy(Texture2D texture, Rectangle rectangle, Player player, int level,
            GraphicsDevice graphicsDevice) : base(texture, rectangle, player, graphicsDevice)
        {
            this.player = player;
            astar = new Astar(null);
            pos = new AStarNode(position, true, 1);

            _animation.AddFrame(GameManager.Get.ContentManager.Load<Texture2D>("skelet_idle_anim_f0"));
            _animation.AddFrame(GameManager.Get.ContentManager.Load<Texture2D>("skelet_idle_anim_f1"));
            _animation.AddFrame(GameManager.Get.ContentManager.Load<Texture2D>("skelet_idle_anim_f2"));
            _animation.AddFrame(GameManager.Get.ContentManager.Load<Texture2D>("skelet_idle_anim_f3"));

            this.level = level;
            initStat(level);
            direction = new Vector2(0, 0);
        }

        public void initStat(int level)
        {
            health = (int)(1.5f * level);
            atk = (level / 3) + 1;
            maxHP = health;
        }

        /// <summary>
        /// Push the enemy away from the player when it was damaged
        /// </summary>
        public override void Damaged()
        {
            if (enemyState != EnemyStateMachine.Damaged)
            {
                health -= (int)player.Atk;
                Vector2 distance = position - player.Position;
                direction = Vector2.Normalize(distance);
                AttackCooldown();
                SoundManager.Get.EnemyDamagedEffect.Play();
            }
        }

        /// <summary>
        /// Move the enemy towards to player
        /// </summary>
        public void AI(GameTime gameTime)
        {
            //update the graph, and get the path
            astar.UpdateGraph(GameManager.Get.Graph);
            path = astar.FindPath(graphPoint.ToVector2() * 64, player.graphPoint.ToVector2() * 64);

            damageFrame += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (damageFrame >= 1000)
            {
                //get the distance between the enemy and the player
                Vector2 playerDistance = position - player.Position;

                //get the next position to get to
                pos = path.Pop();

                //make the enemy move toward the player when they are at the same grid
                if (player.graphPoint == graphPoint)
                {
                    distance = playerDistance;
                }
                else
                {
                    distance = position - pos.Center;
                }

                //move the enemy toward to player when they are in certain range
                if (path != null && path.Count < 7 + 1 && playerDistance.Length() < 350)
                {
                    Vector2 direction = Vector2.Normalize(distance);


                    // Make sure that the direction is a real number
                    // so the enemy doesn't disappear
                    if (!(Math.Abs(direction.X) >= 0))
                    {
                        direction = new Vector2(0, 1);
                    }
                    //Debug.WriteLine($"{direction}");

                    Position -= direction * 2.99f; //will cause error if increase speed, should be lower than 3
                    enemyState = EnemyStateMachine.Move;
                }
                else
                {
                    enemyState = EnemyStateMachine.Idle;
                }
            }
            else if (damageFrame < 200)
            {
                Position += direction * 20;
            }
            else
            {
                enemyState = EnemyStateMachine.Idle;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(_animation.Frame, Rectangle, Color.White);
            //sb.Draw(dungeontilesTexture2D, Rectangle, new Rectangle(368, 80, 16, 16), Color.White);

            // Draw Healthbar
            if (enemyState != EnemyStateMachine.Idle)
            {
                sb.Draw(singleColor, // Texture
                    new Rectangle((int)this.Position.X - 16, (int)this.Position.Y - 10, // X-Y position
                        (int)((this.Rectangle.Width * health / maxHP)), 3), // Width-Height
                    Color.DarkRed); // Color
            }

            if (UI.Get.ShowHitboxes == true)
            {
                sb.Draw(singleColor,
                    new Rectangle(rectangle.X, rectangle.Y,
                        rectangle.Width, rectangle.Height),
                    Color.Red * 0.3f);
            }
        }

        public override void Update(GameTime gt)
        {
            AI(gt);
            _animation.Update(gt);
            if (_animation.Done)
            {
                _animation.Reset();
            }
        }

        /// <summary>
        /// Reset the enemies attack when its hit or does damage
        /// </summary>
        public override void AttackCooldown()
        {
            damageFrame = 0;
            enemyState = EnemyStateMachine.Damaged;
        }
    }
}
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SwordRush
{
    internal class Projectile : GameObject
    {
        private readonly Vector2 _direction;
        private const float Speed = 400;
        private readonly int _damage;
        private readonly LongRangeEnemy _owner;

        // --- Constructor --- //
        public Projectile(Point position, Vector2 direction, LongRangeEnemy owner, int damage) : base(
            GameManager.Get.ContentManager.Load<Texture2D>("coin_anim_f0"), new Rectangle(position, new Point(10, 10)))
        {
            _direction = direction;
            _damage = damage;
            _owner = owner;
        }

        public override void Update(GameTime gt)
        {
            Rectangle newRect = rectangle;
            newRect.X = rectangle.X + (int)(_direction.X * Speed * gt.ElapsedGameTime.TotalSeconds);
            newRect.Y = rectangle.Y + (int)(_direction.Y * Speed * gt.ElapsedGameTime.TotalSeconds);
            rectangle = newRect;
        }
    }
}
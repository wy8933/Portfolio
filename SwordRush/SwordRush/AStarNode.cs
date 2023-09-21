using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SwordRush
{

    internal class AStarNode
    {
        //node size is the size of the block
        public static int NODE_SIZE = 64;
        public AStarNode Parent;
        public Vector2 Position;
        public bool Walkable; // false when this is a wall
        public float DistanceToTarget;
        public float Cost;
        public float Weight;
        public Vector2 Center
        {
            get { return new Vector2(Position.X + NODE_SIZE / 2, Position.Y + NODE_SIZE / 2); }
        }
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }

        public AStarNode(Vector2 pos, bool walkable, float weight = 1)
        {
            Parent = null;
            Position = pos;
            DistanceToTarget = -1;
            Cost = 1;
            Weight = weight;
            Walkable = walkable;
        }
    }
}

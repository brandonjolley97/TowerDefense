using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefense.Objects
{
    public class FlyingCreep : Enemy
    {
        
        public FlyingCreep(Vector2 size, Vector2 center, Texture2D spriteSheet, List<Cell> path) : base(size, center, spriteSheet, new int[] { 150, 150, 150, 150 }, 125 / 1000.0, MathHelper.Pi / 1000.0, path)
        {
            this.delay = 0.75;
            this.health = 12;
            this.pointValue = 120;
        }
    }
}

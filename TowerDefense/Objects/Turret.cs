using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefense.Objects
{
    public class Turret : AnimatedSprite
    {
        public int currentTier = 1;
        public int expectedTier = 1;
        public int cost;
        public int sellValue;
        public float fireRate = 0;
        public float fireTime = 0;
        public float bulletVelocity = 0;
        public int towerType;
        public bool isShopTurret = false;
        public bool selected = false;
        public bool startShooting = false;
        public Texture2D radiusTexture;
        public int radius;
        public Enemy targetCreep;
        public Cell location = null;

        public Turret(Vector2 size, Vector2 center, Texture2D spriteSheet, int towerType, Texture2D radiusTexture) : base(size, center, spriteSheet, new int[] { 1, 1, 1 })
        {
            this.towerType = towerType;
            this.radiusTexture = radiusTexture;

            if(this.towerType == 0)
            {
                this.radius = 250;
                this.fireRate = 800;
                this.cost = 200;
                this.damage = 2;
            }
            else if(this.towerType == 1)
            {
                this.radius = 350;
                this.fireRate = 1400;
                this.cost = 1000;
                this.canTargetFlying = true;
                this.damage = 4;
            }
            else if (this.towerType == 2)
            {
                this.radius = 300;
                this.fireRate = 1200;
                this.cost = 500;
                this.damage = 4;
            }
            else if (this.towerType == 3)
            {
                this.radius = 400;
                this.fireRate = 2000;
                this.cost = 1500;
                this.canTargetFlying = true;
                this.damage = 7;
            }

            this.sellValue = cost / 2;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (this.selected)
            {
                showRadius(spriteBatch, this.radius);
            }

            base.draw(spriteBatch);
        }

        public override void update(GameTime gameTime)
        {
            if (currentTier < 3)
            {
                if (this.towerType == 0)
                {
                    this.damage += 1;
                }
                else if (this.towerType == 1)
                {
                    this.damage += 2;
                }
                else if (this.towerType == 2)
                {
                    this.damage += 2;
                }
                else if (this.towerType == 3)
                {
                    this.damage += 3;
                }
            }
            base.update(gameTime);
        }

        public void faceCreep(Enemy target)
        {
            this.m_direction = target.Center - this.Center;

            if (m_direction.Y < 0)
            {
                this.m_rotation = Angle(new Vector2(-1, 0), m_direction);
                this.m_rotation -= MathHelper.ToRadians(90);
            }
            else
            {
                this.m_rotation = Angle(new Vector2(1, 0), m_direction);
                this.m_rotation += MathHelper.ToRadians(90);
            }





            Enemy temp = target;
        }

        private void showRadius(SpriteBatch spriteBatch, float radius)
        {
            List<Vector2> pixels = new List<Vector2>();
            for (int i = 0; i < 540; i++)
            {
                pixels.Add(new Vector2((float) (radius * Math.Sin(MathHelper.ToRadians(i))), (float)(radius * Math.Cos(MathHelper.ToRadians(i)))));
            }

            foreach (Vector2 position in pixels)
            {
                spriteBatch.Draw(radiusTexture, new Rectangle((int)(position.X + this.Center.X), (int)(position.Y + this.Center.Y), 2, 2), Color.Firebrick);
            }
        }
    }
}

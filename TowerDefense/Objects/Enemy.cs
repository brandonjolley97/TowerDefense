using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using TowerDefense.Menu;

namespace TowerDefense.Objects
{
    public class Enemy : AnimatedSprite
    {
        private readonly double m_moveRate;
        private readonly double m_rotateRate;
        public bool movingHorizontal;
        public double delay;
        public List<Cell> path;
        public Cell currCell;
        public Cell nextCell;
        public Vector2 gridCoords;
        public int health;
        public bool targetable;

        public Enemy(Vector2 size, Vector2 center, Texture2D spriteSheet, int[] spriteTime, double moveRate, double rotateRate, List<Cell> path) : base(size, center, spriteSheet, spriteTime)
        {
            m_moveRate = moveRate;
            m_rotateRate = rotateRate;
            this.path = path;
            currCell = path[0];
            nextCell = path[1];
            this.currCell.creepCount += 1;
            this.nextCell.creepCount += 1;

            this.targetable = false;

            m_direction = Vector2.Normalize(this.nextCell.ActualCenter - this.Center);

            movingHorizontal = true;
        }

        public void moveForward(GameTime gameTime)
        {
            //
            // Create a normalized direction vector
            double vectorX = System.Math.Cos(m_rotation);
            double vectorY = System.Math.Sin(m_rotation);
            
            
            // With the normalized direction vector, move the center of the sprite
            //m_center.X += (float)(vectorX * m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            //m_center.Y += (float)(vectorY * m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            //this.Center = new Vector2(m_center.X, m_center.Y);

            m_center.X += (float)(m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            m_center.Y += (float)(m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            this.Center = new Vector2(m_center.X, m_center.Y);
        }


        public override void update(GameTime gameTime)
        {
            this.gridCoords = currCell.gridCoords;



            if (Vector2.DistanceSquared(this.Center, nextCell.ActualCenter) < 1.5f)
            {
                if (this.currCell.creepCount >= 1)
                {
                    this.currCell.creepCount -= 1;
                }

                this.currCell = nextCell;
                int temp = path.IndexOf(currCell);

                if(temp + 1 < path.Count)
                {
                    this.nextCell = path[temp + 1];
                    this.nextCell.creepCount += 1;
                }
                else
                {
                    m_direction = new Vector2(0, 0);
                    this.destroy = true;
                }

            }

            m_direction = Vector2.Normalize(this.nextCell.ActualCenter - this.Center);
            m_rotation = Angle(new Vector2(1, 0), m_direction);

            if (m_direction.Y < 0 && MathF.Abs(m_direction.X) < 1)
            {
                m_rotation = -m_rotation;
            }

            float movement = (float)(m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);

            this.Center += movement * m_direction;

            base.update(gameTime);
        }

        public override void onCollison(AnimatedSprite victimSprite)
        {
            if(victimSprite is Projectile && this.targetable)
            {
                if(this is FlyingCreep)
                {
                    if (((Projectile)victimSprite).canTargetFlying)
                    {
                        this.health -= victimSprite.damage;
                        if (this.health <= 0)
                        {
                            this.killed = true;

                            if(this.currCell.creepCount >= 1)
                            {
                                this.currCell.creepCount -= 1;
                            }

                            if (this.nextCell.creepCount >= 1)
                            {
                                this.nextCell.creepCount -= 1;
                            }

                            this.destroy = true;
                        }
                    }
                }
                else
                {
                    this.health -= victimSprite.damage;
                    if (this.health <= 0)
                    {
                        this.killed = true;

                        if (this.currCell.creepCount >= 1)
                        {
                            this.currCell.creepCount -= 1;
                        }

                        if (this.nextCell.creepCount >= 1)
                        {
                            this.nextCell.creepCount -= 1;
                        }

                        this.destroy = true;
                    }
                }
                
            }

            base.onCollison(victimSprite);
        }

    }
}

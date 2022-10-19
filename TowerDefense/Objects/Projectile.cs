using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefense.Objects
{
    class Projectile : AnimatedSprite
    {
        private readonly double m_moveRate;
        public int projectileType;
        public Turret fromTurret;

        public Projectile(Vector2 size, Vector2 center, Texture2D spriteSheet, int[] spriteTime, double moveRate, Turret fromTurret) : base(size, center, spriteSheet, spriteTime)
        {
            m_moveRate = moveRate;
            this.fromTurret = fromTurret;
            this.projectileType = this.fromTurret.towerType;
            this.damage = this.fromTurret.damage;
            this.canTargetFlying = this.fromTurret.canTargetFlying;

        }

        
        public override void update(GameTime gameTime)
        {
            if (this.projectileType == 0)
            {
                
            }
            else if (this.projectileType == 1)
            {
                
            }
            else if (this.projectileType == 2)
            {
                
            }
            else if (this.projectileType == 3)
            {
                
            }

            float movement = (float)(m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            this.Center += movement * Vector2.Normalize(m_direction);

            if(this.Center.X > 3000 || this.Center.Y > 3000 || this.Center.X < -100 || this.Center.Y < -100)
            {
                this.destroy = true;
            }

            base.update(gameTime);
        }

        public override void onCollison(AnimatedSprite victimSprite)
        {
            if (victimSprite is Enemy && ((Enemy)victimSprite).targetable)
            {
                if(victimSprite is FlyingCreep)
                {
                    if (this.canTargetFlying)
                    {
                        this.destroy = true;
                    }
                }
                else
                {
                    this.destroy = true;
                }
            }
            else
            {
                
            }
            base.onCollison(victimSprite);
        }
    }
}

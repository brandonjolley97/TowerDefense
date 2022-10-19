using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace TowerDefense.Objects
{
    public class AnimatedSprite
    {
        private readonly Vector2 m_size;
        protected Vector2 m_center;
        public Vector2 m_direction;
        public float m_rotation = 0;
        public bool enabled = true;
        public bool destroy = false;
        public bool invincible = false;
        public bool killed = false;
        public bool canTargetFlying = false;
        public int pointValue = 0;
        public int tag;
        public int identifier;
        public int damage;

        public bool mark = false;

        public BoundingSphere collisionSphere;
        public Rectangle collisionRec;

        public Texture2D m_spriteSheet;
        public int[] m_spriteTime;

        private TimeSpan m_animationTime;
        private int m_subImageIndex;
        private int m_subImageWidth;

        public AnimatedSprite(Vector2 size, Vector2 center, Texture2D spriteSheet, int[] spriteTime)
        {
            m_size = size;
            m_center = center;

            this.m_spriteSheet = spriteSheet;
            this.m_spriteTime = spriteTime;

            m_subImageWidth = spriteSheet.Width / spriteTime.Length;

            //collisionSphere = new BoundingSphere(new Vector3(center, 0), size.Length() / 2);
            collisionRec = new Rectangle((center - (size / 2)).ToPoint(), size.ToPoint());

        }

        public Vector2 Size
        {
            get { return m_size; }
        }

        public Vector2 Center
        {
            get { return m_center; }
            set { m_center = value;
                //this.collisionSphere.Center.X = m_center.X;
                //this.collisionSphere.Center.Y = m_center.Y;
                this.collisionRec.X = (int)(m_center.X - (this.m_size.X / 2));
                this.collisionRec.Y = (int) (m_center.Y - (this.m_size.Y / 2));
            }
            
        }

        public float Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        // Finds the angle between two vectors
        public static float Angle(Vector3 a, Vector3 b)
        {
           


            float magnitudes = a.Length() * b.Length();

            if (magnitudes == 0)
                // The zero vector can't have an angle with another vector.
                return float.NaN;

            float dot = Vector3.Dot(a, b);

            float cosine = dot / magnitudes;

            return MathF.Acos(cosine);
        }

        // Allows calls with Vector2 instead of Vector3
        public static float Angle(Vector2 a, Vector2 b)
        {
            return Angle(new Vector3(a.X, a.Y, 0f), new Vector3(b.X, b.Y, 0f));
        }

        public virtual void update(GameTime gameTime)
        {
            m_animationTime += gameTime.ElapsedGameTime;
            if (m_animationTime.TotalMilliseconds >= m_spriteTime[m_subImageIndex])
            {
                m_animationTime -= TimeSpan.FromMilliseconds(m_spriteTime[m_subImageIndex]);
                m_subImageIndex++;
                m_subImageIndex = m_subImageIndex % m_spriteTime.Length;
            }

        }

        public virtual void draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(
            //    m_spriteSheet,
            //    collisionRec,
            //    //new Rectangle((int)this.Center.X - (int)this.Size.X / 2, (int)this.Center.Y - (int)this.Size.Y / 2, (int)this.Size.X, (int)this.Size.Y), // Destination rectangle
            //    new Rectangle(m_subImageIndex * m_subImageWidth, 0, m_subImageWidth, m_spriteSheet.Height), // Source sub-texture
            //    Color.White,
            //    this.Rotation, // Angular rotation
            //    this.Size / 2, // Center point of rotation
            //    SpriteEffects.None, 0);

            // Be mindful of the weirdness of center when working with rotation
            //this.Rotation += 1f/60f;
            Rectangle offsetRec = collisionRec;
            offsetRec.Offset(this.Size / 2f);

            // This Draw method gives an incorrect center point of rotation 
            spriteBatch.Draw(m_spriteSheet, offsetRec, new Rectangle(m_subImageIndex * m_subImageWidth, 0, m_subImageWidth, m_spriteSheet.Height), Color.White, this.Rotation, Size / 2f, SpriteEffects.None, 0);

            // This Draw method doesn't allow me to resize the sprite
            //spriteBatch.Draw(m_spriteSheet, this.Center - new Vector2(32, 32), new Rectangle(m_subImageIndex * m_subImageWidth, 0, m_subImageWidth, (int)Size.Y), Color.White, 0f, (this.Size * (46 / 64)) / 2f, 1.37f, SpriteEffects.None, 0f);

            // This Draw method doesn't allow me to set a rotation value or a center of rotation
            //spriteBatch.Draw(m_spriteSheet, collisionRec, new Rectangle(m_subImageIndex * m_subImageWidth, 0, m_subImageWidth, m_spriteSheet.Height), Color.White);
        }

        public virtual void onCollison(AnimatedSprite victimSprite)
        {

        }

        public virtual void addPoints()
        {
            
        }


    }
}

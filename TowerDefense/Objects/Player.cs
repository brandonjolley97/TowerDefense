
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TowerDefense.Objects
{
    public class Player
    {
        public int health = 20;
        public int gameScore = 2000;
        public int overallScore = 0;
        public int round = 0;
        public bool nextRound = false;
        public bool roundOver = true;
        public bool gameOver = false;
        public bool gameWon = false;
        public bool enabled = true;
        public bool killed = false;
        public int identifier;

        public Player ()
        {
            
        }

        public void update(GameTime gameTime)
        {
            
        }

    }
}

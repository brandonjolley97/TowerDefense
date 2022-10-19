using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace TowerDefense.Objects
{
    public class Cell
    {
        public bool hasTurret;
        public int creepCount;
        public Turret currTurret;
        public Vector2 gridCoords;
        public Vector2 cartCoords;

        public Cell(Vector2 gridCoords, Vector2 cartCoords)
        {
            this.gridCoords = gridCoords;
            this.cartCoords = cartCoords;
            this.creepCount = 0;
            //this.cartCoords += new Vector2(32, 32);
        }

        public bool HasTurret
        {
            get { if (currTurret != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set { this.hasTurret = value;  }
        }

        public Vector2 ActualCenter => this.cartCoords + new Vector2(32, 32);



        // A Utility Function to check whether the given cell is
        // blocked or not
        public bool isUnBlocked()
        {
            // Returns true if the cell is not blocked else false
            if (!this.hasTurret)
                return true;
            else
                return false;
        }

        // A Utility Function to check whether destination cell has been reached or not
        public bool isDestination(Vector2 point)
        {
            if ((int)this.gridCoords.Y == (int)point.Y && (int)this.gridCoords.X == (int)point.X)
                return true;
            else
                return false;
        }

        // A Utility Function to calculate the 'h' heuristics.
        public float calculateHValue(Vector2 point)
        {
            // Return using the distance formula
            return Vector2.Distance(point, this.gridCoords);
        }



    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TowerDefense.Objects
{
    class Visit
    {
        public Visit parent;
        public Cell subject;

        public Visit(Visit parent, Cell subject)
        {
            this.parent = parent;
            this.subject = subject;
        }

        public List<Cell> getPath()
        {
            Stack<Cell> bottomUp = new Stack<Cell>();
            bottomUp.Push(this.subject);
            if (this.parent != null)
            {
                this.parent.getPath(bottomUp);
            }


            List<Cell> currPath = new List<Cell>(bottomUp);
            //currPath.Reverse();
            return currPath;
        }

        
        private void getPath(Stack<Cell> bottomUp)
        {
            bottomUp.Push(this.subject);
            if (this.parent != null)
            {
                // Fixed this recursive call.  Was this.getPath(bottomUp)
                this.parent.getPath(bottomUp);
            }
        }
    }
}

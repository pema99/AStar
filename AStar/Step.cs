using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class Step
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int H { get; set; }
        public int G { get; set; }
        public Step Parent { get; set; }
        public int Score { get { return H + G; } }

        public Step(int X, int Y, int H, int G, Step Parent = null)
        {
            this.X = X;
            this.Y = Y;
            this.H = H;
            this.G = G;
            this.Parent = Parent;
        }

        //Used for checking if a list contains a tile with a certain position already
        public override bool Equals(object obj)
        {
            if (!(obj is Step))
            {
                return false;
            }
            Step Temp = (Step)obj;
            return Temp.X == X && Temp.Y == Y;
        }
    }
}

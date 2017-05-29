using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AStar
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D Blank;

        //Map
        bool[,] World = new bool[,]
        {
            {false, false, false, false, false, false, false},
            {false, false, false, true, false, false, false},
            {false, false, false, true, false, false, false},
            {false, false, false, true, false, false, false},
            {false, true, false, true, false, false, false},
            {false, false, false, false, false, false, false},
        };
        //Open list
        public List<Step> Open = new List<Step>();
        //Closed list
        public List<Step> Close = new List<Step>();

        //Finished path
        public List<Step> Path { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //Blank texture for rendering
            this.Blank = new Texture2D(GraphicsDevice, 1, 1);
            this.Blank.SetData<Color>(new Color[] { Color.White });

            //Find path initially
            Path = Pathfind(1, 3, 5, 4);
        }

        #region Pathfinding methods
        public List<Step> Pathfind(int X1, int Y1, int X2, int Y2)
        {
            //Declare destination and add first square to open
            Step Destination = new Step(X2, Y2, 0, 0);
            Open.Add(new Step(X1, Y1, Heuristic(X1, Y1, X2, Y2), 0));

            //Until we are out of neighbours to check or we have reached destination
            do 
            {
                //Find lowest cost square
                Step Current = Open.OrderBy(p => p.Score).First();

                //Move it from open to closed
                Close.Add(Current);
                Open.Remove(Current);

                //If we have reached destination somehow, stop searching for path
                if (Close.Contains(Destination))
                {
                    break;
                }

                //Go through valid neighbours
                foreach (Step Neighbour in Adjacent(Current))
                {
                    //If neighbour is already part of the path, skip it
                    if (Close.Contains(Neighbour))
                    {
                        continue;
                    }
                    //If neighbour is not checked yet, add it to open
                    if (!Open.Contains(Neighbour))
                    {
                        Neighbour.Parent = Current;
                        Neighbour.G = Neighbour.Parent.G + 1;
                        Neighbour.H = Heuristic(Neighbour.X, Neighbour.Y, X2, Y2);
                        Open.Add(Neighbour);
                    }
                    //If neighbour was already checked, recalculate it with the new path, if it is cheaper
                    else
                    {
                        if ((Current.G + 1) < Neighbour.G)
                        {
                            Neighbour.G = Current.G + 1;
                        }
                    }
                }
            }
            while(Open.Count != 0);

            //Go backwards through path from destination to start, and construct a list from that
            List<Step> Result = new List<Step>();
            CreatePath(Result, Close.Last());
            return Result;
        }

        //Recursive path walk
        public List<Step> CreatePath(List<Step> Path, Step Current)
        {
            Path.Add(Current);
            if (Current.Parent != null)
            {
                CreatePath(Path, Current.Parent);
            }
            return Path;
        }

        //Get valid neighbour tiles
        public List<Step> Adjacent(Step Current)
        {
            List<Step> Result = new List<Step>();
            if (IsValid(Current.X, Current.Y - 1))
            {
                if (!World[Current.Y - 1, Current.X])
                {
                    Result.Add(new Step(Current.X, Current.Y - 1, 0, 0));
                }
            }
            if (IsValid(Current.X, Current.Y + 1))
            {
                if (!World[Current.Y + 1, Current.X])
                {
                    Result.Add(new Step(Current.X, Current.Y + 1, 0, 0));
                }
            }
            if (IsValid(Current.X - 1, Current.Y))
            {
                if (!World[Current.Y, Current.X - 1])
                {
                    Result.Add(new Step(Current.X - 1, Current.Y, 0, 0));
                }
            }
            if (IsValid(Current.X + 1, Current.Y))
            {
                if (!World[Current.Y, Current.X + 1])
                {
                    Result.Add(new Step(Current.X + 1, Current.Y, 0, 0));
                }
            }
            return Result;
        }

        //Simple manhattan distance heuristic
        public int Heuristic(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X2-X1) + Math.Abs(Y2-Y1);
        }

        //Checks if tile is within bounds
        public bool IsValid(int X, int Y)
        {
            return X >= 0 && X < World.GetLength(1)
                && Y >= 0 && Y < World.GetLength(0);
        }
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            //Draw map
            for (int i = 0; i < World.GetLength(1); i++)
            {
                for (int j = 0; j < World.GetLength(0); j++)
                {
                    spriteBatch.Draw(Blank, new Rectangle(i*32+i, j*32+j, 32, 32), World[j, i] ? Color.Black : Color.White);
                }
            }
            //Draw path
            foreach (Step Cur in Path)
            {
                spriteBatch.Draw(Blank, new Rectangle(Cur.X*32+Cur.X, Cur.Y*32+Cur.Y, 16, 16), Color.Red);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

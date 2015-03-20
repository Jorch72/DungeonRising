using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    [Serializable]
    public struct Position
    {
        public int X;
        public int Y;
        
        public Position(int y, int x) : this()
        {
            X = x;
            Y = y;
        }
        public bool Validate(int Height, int Width)
        {
            return (Y >= 0 && Y < Height && X >= 0 && X < Width);
        }
        public void MakeValid(int Height, int Width)
        {
            if (Y < 0)
                Y = 0;
            if (Y >= Height)
                Y = Height - 1;
            if (X < 0)
                X = 0;
            if (X >= Width)
                X = Width - 1;
        }
        public void Move(int y, int x)
        {
            Y += y;
            X += x;
        }
        public void SetAll(int y, int x)
        {

            Y = y;
            X = x;
            
        }
        public static Position FromIndex(int index, int Width)
        {
            return new Position(index / Width, index % Width);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Position))
                return false;
            else
                return X == ((Position)obj).X && Y == ((Position)obj).Y;
        }

        public override int GetHashCode()
        {
            return (Y << 16) | (X & 0xffff);
        }


        public static bool operator ==(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(Position a, Position b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
    }

}

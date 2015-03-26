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
        public bool Validate(int height, int width)
        {
            return (Y >= 0 && Y < height && X >= 0 && X < width);
        }
        public void MakeValid(int height, int width)
        {
            if (Y < 0)
                Y = 0;
            if (Y >= height)
                Y = height - 1;
            if (X < 0)
                X = 0;
            if (X >= width)
                X = width - 1;
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
        public static Position FromIndex(int index, int width)
        {
            return new Position(index / width, index % width);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    [Serializable]
    [System.ComponentModel.TypeConverter(typeof(PositionConverter))]
    public struct Position
    {
        public int X;
        public int Y;
        
        public Position(int y, int x)
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
    }

    public class PositionConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Equals(typeof(string)))
                return true;
            return false;
        }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(string)))
                return true;
            return false;
        }
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            Position pos = new Position();
            string[] xy = ((string)value).Split(' ');
            pos.X = int.Parse(xy[0]);
            pos.Y = int.Parse(xy[1]);
            return pos;
        }
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return ((Position)value).X + " " + ((Position)value).Y;
        }
    }
}

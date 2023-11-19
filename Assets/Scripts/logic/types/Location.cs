using System;

public class Location
{
    private int x, y, z;

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

    public int Z
    {
        get => z;
        set => z = value;
    }

    public Location(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public int Distance(Location other)
    {
        return (int) Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2) + Math.Pow(other.Z - Z, 2));
    }

    public override int GetHashCode()
    {
        return (x, y, z).GetHashCode();
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        Location other = (Location) obj;
        return other.X == X && other.Y == Y && other.Z == Z;
    }
    
    public override string ToString()
    {
        return $"Location({x}, {y}, {z})";
    }
}
using logic.models;

/// <summary>
/// A block in the world.
/// </summary>
public class Block
{
    private BlockType type;
    private Location position;
    
    public Block(BlockType type, Location position)
    {
        this.type = type;
        this.position = position;
    }
    
    public BlockType GetBlockType()
    {
        return type;
    }
    
    public void SetBlockType(BlockType type)
    {
        this.type = type;
    }
    
    public Location GetPosition()
    {
        return position;
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        Block other = (Block) obj;
        return other.GetPosition().Equals(GetPosition());
    }
    
    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
    
    public override string ToString()
    {
        return $"Block({type}, {position})";
    }
}
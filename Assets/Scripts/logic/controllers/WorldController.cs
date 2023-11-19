using logic.models;

public class WorldController
{
    private WorldGeneration world;
    
    public void SetTile(int x, int y, int z, BlockType blockType)
    {
        this.world.SetBlockType(x, y, z, blockType);
    }
    
    public void SetTile(Location position, BlockType blockType)
    {
        this.world.SetBlockType(position, blockType);
    }

    public WorldGeneration GetWorld()
    {
        return this.world;
    }
}

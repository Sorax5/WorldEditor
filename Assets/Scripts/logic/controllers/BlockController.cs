using System;
using logic.models;
using UnityEngine;

public class BlockController
{
    private WorldGeneration worldGeneration;
    
    public WorldGeneration GetWorld()
    {
        return worldGeneration;
    }
    
    public Action<Location, BlockType> OnBlockChanged;
    
    public BlockController(WorldGeneration worldGeneration)
    {
        this.worldGeneration = worldGeneration;
    }
    
    public void SetTile(int x, int y, int z, BlockType blockType)
    {
        this.worldGeneration.SetBlockType(x, y, z, blockType);
        OnBlockChanged?.Invoke(new Location(x, y, z), blockType);
    }
        
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using logic;
using logic.models;

/// <summary>
/// WorldGeneration is an interface for the world generation.
/// </summary>
public abstract class WorldGeneration
{
    public event Action<Location, BlockType> OnBlockChanged;
    private WorldGeneratorSettings _settings;
    
    /// <summary>
    /// Get the settings for the world generation.
    /// </summary>
    public WorldGeneratorSettings Settings
    {
        get => _settings;
        set => _settings = value;
    }

    /// <summary>
    /// Raised when a block is changed.
    /// </summary>
    /// <param name="x">x position of the block.</param>
    /// <param name="y">y position of the block.</param>
    /// <param name="z">z position of the block.</param>
    /// <param name="blockType">Type of the block.</param>
    protected void RaiseOnBlockChanged(int x, int y, int z, BlockType blockType)
    {
        RaiseOnBlockChanged(new Location(x,y,z), blockType);
    }
    
    /// <summary>
    /// Raised when a block is changed.
    /// </summary>
    /// <param name="position">Position of the block.</param>
    /// <param name="blockType">Type of the block.</param>
    protected void RaiseOnBlockChanged(Location position, BlockType blockType)
    {
        RaiseOnBlockChanged(position, blockType);
    }

    /// <summary>
    /// Constructor for the world generation.
    /// </summary>
    /// <param name="settings">Settings for the world generation.</param>
    public WorldGeneration(WorldGeneratorSettings settings)
    {
        this._settings = settings;
    }
    
    public abstract BlockType GetBlockType(int x, int y, int z);
    public abstract BlockType GetBlockType(Location position);
    public abstract Block GetBlock(Location position);
    public abstract Block GetBlock(int x, int y, int z);
    public abstract void SetBlockType(int x, int y, int z, BlockType blockType);
    public abstract void SetBlockType(Location position, BlockType blockType);
    public abstract List<Block> GetBlocksInRadius(Location position, int radius);
    public abstract List<Block> GetBlocksBetween(Location position1, Location position2);
    
    public abstract Task<List<Block>> GetBlocksAsyncInRadius(Location position, int radius);
    public abstract Task<List<Block>> GetBlocksAsyncBetween(Location position1, Location position2);
    public abstract void ResetBlocksInRadius(Location position, int radius);
}
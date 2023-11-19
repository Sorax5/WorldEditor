using System.Collections.Generic;
using logic;
using logic.models;

/// <summary>
/// Class for representing a chunk of the world.
/// </summary>
public abstract class BaseChunk
{
    /// <summary>
    /// Position of the chunk in the world.
    /// </summary>
    public Location Position => new Location(0, 0, 0);
    
    /// <summary>
    /// Settings for the world generation.
    /// </summary>
    public WorldGeneratorSettings Settings { get; }
    
    public IChunkGenerator ChunkGenerator { get; }

    /// <summary>
    /// Constructor for the chunk.
    /// </summary>
    /// <param name="worldPosition">Chunk position in the world.</param>
    /// <param name="settings">Settings for the world generation.</param>
    protected BaseChunk(int x, int y, WorldGeneratorSettings settings, IChunkGenerator chunkGenerator)
    {
        this.Settings = settings;
        this.ChunkGenerator = chunkGenerator;
    }

    /// <summary>
    /// Sets the block type at the given position.
    /// </summary>
    /// <param name="x">X position of the block.</param>
    /// <param name="y">Y position of the block.</param>
    /// <param name="z">Z position of the block.</param>
    /// <param name="type">Type of the block.</param>
    public abstract void SetBlockType(int x, int y, int z, BlockType type);

    /// <summary>
    /// Sets the block type at the given position.
    /// </summary>
    /// <param name="localPosition">Local position of the block.</param>
    /// <param name="type">Type of the block.</param>
    public abstract void SetBlockType(Location localPosition, BlockType type);
    
    /// <summary>
    /// Gets the block type at the given position.
    /// </summary>
    /// <param name="x">X position of the block.</param>
    /// <param name="y">Y position of the block.</param>
    /// <param name="z">Z position of the block.</param>
    /// <returns>Type of the block.</returns>
    public abstract BlockType GetBlockType(int x, int y, int z);
    
    /// <summary>
    /// Gets the block type at the given position.
    /// </summary>
    /// <param name="position">Position of the block.</param>
    /// <returns>Type of the block.</returns>
    public abstract BlockType GetBlockType(Location position);
    
    /// <summary>
    /// Gets the block at the given position.
    /// </summary>
    /// <param name="x">X position of the block.</param>
    /// <param name="y">Y position of the block.</param>
    /// <param name="z">Z position of the block.</param>
    /// <returns>Instance of the block.</returns>
    public abstract Block GetBlock(int x, int y, int z);
    
    /// <summary>
    /// Gets the block type at the given position.
    /// </summary>
    /// <param name="position">Position of the block.</param>
    /// <returns>Type of the block.</returns>
    public abstract Block GetBlock(Location position);

    /// <summary>
    /// Gets all the blocks in the chunk.
    /// </summary>
    /// <returns>List of blocks in the chunk.</returns>
    public abstract List<Block> GetAllBlocks();
    
    /// <summary>
    /// Adds a block to the chunk.
    /// </summary>
    /// <param name="block">Block to add.</param>
    public abstract void AddBlock(Block block);

    /// <summary>
    /// Gets the local position of the given world position.
    /// </summary>
    /// <param name="worldPosition">World position.</param>
    /// <returns>Local position.</returns>
    public abstract Location LocalPostion(Location worldPosition);
}
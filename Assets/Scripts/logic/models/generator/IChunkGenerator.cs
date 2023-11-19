using System.Collections.Generic;
using logic.models;

/// <summary>
/// Generates a block type for a given position in a chunk.
/// </summary>
public interface IChunkGenerator
{
    public List<Block> GenerateBlockType(int x, int y, WorldGeneratorSettings settings);
}
using System.Collections.Generic;
using logic.models;

public class FlatChunkGenerator : IChunkGenerator
{
    public List<Block> GenerateBlockType(int x, int y, WorldGeneratorSettings settings)
    {
        List<Block> blocks = new List<Block>();
        for (int i = 0; i < settings.GetChunkSize(); i++)
        {
            for (int j = 0; j < settings.GetChunkSize(); j++)
            {
                blocks.Add(new Block(BlockType.GRASS, new Location(x + i, y + j, 0)));
            }
        }

        return blocks;
    }
}
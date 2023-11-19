using System.Collections.Generic;
using logic.models;
using UnityEngine;

public class PlainChunkGenerator : IChunkGenerator
{
    public List<Block> GenerateBlockType(int x, int y, WorldGeneratorSettings settings)
    {
        int chunkSize = settings.GetChunkSize();
        List<Block> blocks = new List<Block>(chunkSize * chunkSize * settings.GetMaxHillHeight());

        float frequency = settings.GetNoiseScale();
        int maxHeight = settings.GetMaxHillHeight();
        int minHeight = settings.GetMinHillHeight();
        int baseHeight = settings.GetBaseHeight();
        int seed = settings.GetSeed();

        int waterLevel = 2;

        UnityEngine.Random.InitState(seed + x + y);

        for (int localX = 0; localX < chunkSize; localX++)
        {
            for (int localY = 0; localY < chunkSize; localY++)
            {
                float perlinValue = Mathf.PerlinNoise((x + localX + seed) * frequency, (y + localY + seed) * frequency);

                int height = Mathf.FloorToInt(Mathf.Lerp(minHeight, maxHeight, perlinValue)) + baseHeight;

                for (int z = 0; z < height; z++)
                {
                    BlockType blockType = z == height - 1 ? BlockType.GRASS : BlockType.DIRT;
                    blocks.Add(new Block(blockType, new Location(x + localX, y + localY, z)));

                    if (z == height - 1 && UnityEngine.Random.Range(0, 100) < 10)
                    {
                        blocks.Add(new Block(BlockType.WEED, new Location(x + localX, y + localY, z + 1)));
                    }
                }

                if (height < waterLevel)
                {
                    blocks.Add(new Block(BlockType.WATER, new Location(x + localX, y + localY, waterLevel)));
                    blocks.Add(new Block(BlockType.SAND, new Location(x + localX, y + localY, waterLevel - 1)));
                }
            }
        }

        return blocks;
    }

}
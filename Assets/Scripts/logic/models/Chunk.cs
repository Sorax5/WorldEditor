using System;
using System.Collections.Generic;
using System.Threading;
using logic;
using logic.models;
using PimDeWitte.UnityMainThreadDispatcher;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Chunk
{

    private Vector2Int worldPosition; // Position du chunk dans le monde
    private BlockType[,,] blocks; // Tableau de blocs du chunk
    private WorldGenerationSettings _settings;
    private System.Random _random;
    public int ChunkSize
    {
        get => blocks.GetLength(0);
    }

    public Chunk(Vector2Int worldPosition, WorldGenerationSettings settings)
    {
        this.worldPosition = worldPosition;
        blocks = new BlockType[settings.GetChunkSize(), settings.GetChunkSize(), settings.GetChunkSize()];
        this._settings = settings;
        this._random = new System.Random(settings.GetSeed());

        GenerateChunk();
    }

    public BlockType GetBlock(Vector3Int localPosition)
    {
        return blocks[localPosition.x, localPosition.y, localPosition.z];
    }

    private void GenerateChunk()
    {
        var width = blocks.GetLength(0);
        var depth = blocks.GetLength(1);
        var height = blocks.GetLength(2);

        var maxHillHeight = _settings.GetMaxHillHeight();
        var minHillHeight = _settings.GetMinHillHeight();
        var baseHeight = _settings.GetBaseHeight();
        var scale = _settings.GetNoiseScale();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < depth; y++)
            {
                var xCoord = (float)(this.worldPosition.x + x) / width * scale;
                var yCoord = (float)(this.worldPosition.y + y) / depth * scale;
                var perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
                
                var heightValue = Mathf.Lerp(minHillHeight, maxHillHeight, perlinValue) + baseHeight;

                // Now, you can set the heightValue to your generatedBlocks array
                for (var z = 0; z < height; z++)
                {
                    BlockType blockBelow = z > 0 ? blocks[x, y, z - 1] : BlockType.AIR;
                    if (z < heightValue)
                    {
                        blocks[x, y, z] = DetermineBlockType(z, this._random.Next(), blockBelow);
                    }
                    else
                    {
                        blocks[x, y, z] = BlockType.AIR; // Assuming BlockType.Air represents empty space.
                    }
                    
                    if (z == 1 && blocks[x, y, z] == BlockType.AIR)
                    {
                        blocks[x, y, z] = BlockType.WATER;

                        for (var i = x-1; i < x+1; i++)
                        {
                            for (var k = y-1; k < y-1; k++)
                            {
                                if (blocks[i, k, z] != BlockType.WATER)
                                {
                                    blocks[i, k, z] = BlockType.SAND;

                                }
                            }
                        }
                    }
                }
            }
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < depth; y++)
            {
                for (var z = 0; z < height; z++)
                {
                    BlockType type = blocks[x, y, z];

                    if (z + 1 > height || z - 1 < 0 || type != BlockType.GRASS)
                    {
                        //Debug.Log($"type: {type}| above: {z + 1 > height}| under: {z - 1 < 0}");
                        continue;
                    }

                    BlockType above = blocks[x, y, z + 1];

                    if (above != BlockType.AIR)
                    {
                        continue;
                    }

                    float rdn = this._random.Next(0, 10);

                    if (rdn < 2)
                    {
                        blocks[x, y, z + 1] = BlockType.WEED;
                    }
                }
            }
        }

    }
    
    private void GenerateChunkInBackground(object state)
    {
        
        BlockType[,,] generatedBlocks = (BlockType[,,])state;
        
        int width = blocks.GetLength(0);
        int depth = blocks.GetLength(1);
        int height = blocks.GetLength(2);

        int maxHillHeight = _settings.GetMaxHillHeight();
        int minHillHeight = _settings.GetMinHillHeight();
        int baseHeight = _settings.GetBaseHeight();
        float scale = _settings.GetNoiseScale();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                float xCoord = (float)(this.worldPosition.x + x) / width * scale;
                float yCoord = (float)(this.worldPosition.y + y) / depth * scale;
                float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
                
                float heightValue = Mathf.Lerp(minHillHeight, maxHillHeight, perlinValue) + baseHeight;

                // Now, you can set the heightValue to your generatedBlocks array
                for (int z = 0; z < height; z++)
                {
                    BlockType blockBelow = z > 0 ? generatedBlocks[x, y, z - 1] : BlockType.AIR;
                    if (z < heightValue)
                    {
                        lock ((this._random))
                        {
                            generatedBlocks[x, y, z] = DetermineBlockType(z, this._random.Next(), blockBelow);
                        }
                    }
                    else
                    {
                        generatedBlocks[x, y, z] = BlockType.AIR; // Assuming BlockType.Air represents empty space.
                    }
                    
                    if (z == 1 && generatedBlocks[x, y, z] == BlockType.AIR)
                    {
                        generatedBlocks[x, y, z] = BlockType.WATER;

                        for (int i = x-1; i < x+1; i++)
                        {
                            for (int k = y-1; k < y-1; k++)
                            {
                                if (generatedBlocks[i, k, z] != BlockType.WATER)
                                {
                                    generatedBlocks[i, k, z] = BlockType.SAND;

                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                for (int z = 0; z < height; z++)
                {
                    BlockType type = generatedBlocks[x, y, z];

                    if (z + 1 > height || z - 1 < 0 || type != BlockType.GRASS)
                    {
                        //Debug.Log($"type: {type}| above: {z + 1 > height}| under: {z - 1 < 0}");
                        continue;
                    }

                    BlockType above = generatedBlocks[x, y, z + 1];

                    if (above != BlockType.AIR)
                    {
                        continue;
                    }

                    lock (this._random)
                    {
                        float rdn = this._random.Next(0, 10);

                        if (rdn < 2)
                        {
                            generatedBlocks[x, y, z + 1] = BlockType.WEED;
                        }
                    }
                }
            }
        }

        
        /*int iterations = generatedBlocks.GetLength(0) * generatedBlocks.GetLength(1);
        
        // chunk shape generation
        for (int i = 0; i < iterations; i++)
        {
            int x = i % generatedBlocks.GetLength(0);
            int y = i / generatedBlocks.GetLength(0);
            
            float perlinValue = Mathf.PerlinNoise((x + worldPosition.x) / 10f, (y + worldPosition.y) / 10f);
            int height = (int) (perlinValue * CHUNK_SIZE);
            
            
            for (int z = 0; z < this.blocks.GetLength(2); z++)
            {
                // just the shape set dirt
                if (z < height)
                {
                    generatedBlocks[x, y, z] = BlockType.DIRT;
                }
                else
                {
                    generatedBlocks[x, y, z] = BlockType.AIR;
                }
            }
        }
        
        // water generation
        for (int i = 0; i < iterations; i++)
        {
            int x = i % generatedBlocks.GetLength(0);
            int y = i / generatedBlocks.GetLength(0);
            
            float perlinValue = Mathf.PerlinNoise((x + worldPosition.x) / 10f, (y + worldPosition.y) / 10f);
            int height = (int) (perlinValue * CHUNK_SIZE);
            
            for (int z = 0; z < this.blocks.GetLength(2); z++)
            {
                // water
                if (z <= 4 && generatedBlocks[x, y, z] == BlockType.AIR)
                {
                    generatedBlocks[x, y, z] = BlockType.WATER;
                }
            }
        }
        
        // sand generation
        for (int i = 0; i < iterations; i++)
        {
            int x = i % generatedBlocks.GetLength(0);
            int y = i / generatedBlocks.GetLength(0);
            
            float perlinValue = Mathf.PerlinNoise((x + worldPosition.x) / 10f, (y + worldPosition.y) / 10f);
            int height = (int) (perlinValue * CHUNK_SIZE);
            
            for (int z = 0; z < this.blocks.GetLength(2); z++)
            {
                BlockType blockType = generatedBlocks[x, y, z];
                
                if(blockType == BlockType.WATER)
                    continue;
                
                // sand if blocks around is water
                List<Vector3Int> blocksAroundXY = new List<Vector3Int>
                {
                    new Vector3Int(x - 1, y, z),
                    new Vector3Int(x + 1, y, z),
                    new Vector3Int(x, y - 1, z),
                    new Vector3Int(x, y + 1, z)
                };

                // if water around, change to sand
                foreach (Vector3Int blockAroundXY in blocksAroundXY)
                {

                    if (blockAroundXY.x >= 0 && blockAroundXY.x < CHUNK_SIZE && blockAroundXY.y >= 0 && blockAroundXY.y < CHUNK_SIZE)
                    {
                        BlockType ablockType = generatedBlocks[blockAroundXY.x, blockAroundXY.y, z];
                        if (ablockType == BlockType.WATER)
                        {
                            generatedBlocks[x, y, z] = BlockType.SAND;
                            
                            if (Random.Range(0, 10) == 0)
                            {
                                generatedBlocks[x, y, z + 1] = BlockType.DIRT;
                            }
                        }
                    }
                }
            }
            
        }
        
        // grass generation
        for (int i = 0; i < iterations; i++)
        {
            int x = i % generatedBlocks.GetLength(0);
            int y = i / generatedBlocks.GetLength(0);
            
            float perlinValue = Mathf.PerlinNoise((x + worldPosition.x) / 10f, (y + worldPosition.y) / 10f);
            int height = (int) (perlinValue * CHUNK_SIZE);
            
            for (int z = 0; z < this.blocks.GetLength(2); z++)
            {
                // if block above is void set grass && if block is not void && block is dirt
                BlockType blockType = generatedBlocks[x, y, z];
                if (z < CHUNK_SIZE - 1 && generatedBlocks[x, y, z + 1] == BlockType.AIR && blockType != BlockType.AIR && blockType == BlockType.DIRT)
                {
                    generatedBlocks[x, y, z] = BlockType.GRASS;
                    
                    // if block above is void set block above to weed
                    if (z < CHUNK_SIZE - 2)
                    {
                        if (Random.Range(0,10) == 0)
                        {
                            generatedBlocks[x, y, z + 1] = BlockType.WEED;
                        }
                    }
                }
            }
        }*/
        
        
        // Transférez les données générées sur le thread principal.
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Array.Copy(generatedBlocks, blocks, generatedBlocks.Length);
        });
    }

    private BlockType DetermineBlockType(float height, float randomValue, BlockType blockBelow)
    {
        if (height > 10.0f)
        {
            return BlockType.STONE; // Par exemple, de la pierre pour les zones élevées.
        }
        else if (height > 0.0f)
        { 
            if (blockBelow == BlockType.AIR)
            {
                if (height > 5.0f) // Profondeur minimale pour de la STONE (ajustez selon vos besoins)
                {
                    return BlockType.STONE;
                }
                else
                {
                    return BlockType.DIRT; // Par exemple, de la terre pour les zones moyennes.
                }
            }
            else
            {
                return BlockType.GRASS; // Par exemple, de l'herbe pour les zones moyennes.
            }
        }
        else
        {
            if (randomValue < 0.5f)
            {
                return BlockType.WATER; // Par exemple, de l'eau pour les zones basses (niveau de l'eau) avec une petite probabilité.
            }
            else
            {
                return BlockType.SAND; // Par exemple, du sable pour les zones basses (niveau de l'eau).
            }
        }
    }



    /// <summary>
    /// Set the block type at the given worldPosition.
    /// </summary>
    /// <param name="x"> Must be a local worldPosition</param>
    /// <param name="y"> Must be a local worldPosition</param>
    /// <param name="z"> Must be a local worldPosition</param>
    /// <param name="type"> Type of the block</param>
    public void SetBlockType(int x, int y, int z, BlockType type)
    {
        blocks[x, y, z] = type;
    }
    
    /// <summary>
    /// Set the block type at the given worldPosition.
    /// </summary>
    /// <param name="localPosition">Must be a local worldPosition</param>
    /// <param name="type">Type of the block</param>
    public void SetBlockType(Vector3Int localPosition, BlockType type)
    {
        SetBlockType(localPosition.x, localPosition.y, localPosition.z, type);
    }

    /// <summary>
    /// Get a list of all the world worldPosition of the blocks in the chunk.
    /// </summary>
    /// <returns></returns>
    public List<Vector3Int> GetWorldBlockPosition()
    {
        int iterations = this.blocks.GetLength(0) * this.blocks.GetLength(1) * this.blocks.GetLength(2);
        
        List<Vector3Int> positions = new List<Vector3Int>();
        
        for (int i = 0; i < iterations; i++)
        {
            int x = i % this.blocks.GetLength(0);
            int y = (i / this.blocks.GetLength(0)) % this.blocks.GetLength(1);
            int z = i / (this.blocks.GetLength(0) * this.blocks.GetLength(1));

            Vector3Int localPosition = new Vector3Int(x, y, z);
            Vector3Int worldPosition = WorldTilePosition(localPosition);
            
            positions.Add(worldPosition);
        }
        return positions;
    }
    
    public List<Vector3Int> GetRelativeBlockPosition()
    {
        int iterations = this.blocks.GetLength(0) * this.blocks.GetLength(1) * this.blocks.GetLength(2);
        
        List<Vector3Int> positions = new List<Vector3Int>();
        
        for (int i = 0; i < iterations; i++)
        {
            int x = i % this.blocks.GetLength(0);
            int y = (i / this.blocks.GetLength(0)) % this.blocks.GetLength(1);
            int z = i / (this.blocks.GetLength(0) * this.blocks.GetLength(1));
            
            positions.Add(new Vector3Int(x, y, z));
        }
        return positions;
    }

    public BlockType GetBlockType(int x, int y, int z)
    {
        return GetBlock(new Vector3Int(x,y,z));
    }

    public Vector2Int GetPosition()
    {
        return this.worldPosition;
    }

    public Vector3Int WorldTilePosition(Vector3Int relativePosition)
    {
        return new Vector3Int(this.worldPosition.x + relativePosition.x, this.worldPosition.y + relativePosition.y, relativePosition.z);
    }

    public Vector3Int RelativePostion(Vector3Int worldPosition)
    {
        int x = Mod((worldPosition.x + 32), this.blocks.GetLength(0));
        int y = Mod((worldPosition.y + 32), this.blocks.GetLength(1));
        
        return new Vector3Int(x,y,worldPosition.z);
    }
    
    private int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }
    
    public Vector3Int RelativePosition(int x, int y, int z)
    {
        return RelativePostion(new Vector3Int(x,y,z));
    }

    public BlockType[,,] GetBlocks()
    {
        return this.blocks;
    }

}
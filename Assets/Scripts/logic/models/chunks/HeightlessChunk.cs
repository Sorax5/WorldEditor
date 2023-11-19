using System;
using System.Collections.Generic;
using logic;
using logic.models;

/// <summary>
/// Chunk without height.
/// </summary>
public class HeightlessChunk : BaseChunk
{
    private readonly int[,] _heightMap;
    
    private List<Block> _blocks;
    
    private int x, y;
    
    public Location Position => new Location(x, y, 0);
    
    public HeightlessChunk(int x, int y, WorldGeneratorSettings settings, IChunkGenerator chunkGenerator) : base(x, y, settings, chunkGenerator)
    {
        this.x = x;
        this.y = y;
        this._heightMap = new int[settings.GetChunkSize(), settings.GetChunkSize()];
        this._blocks = this.ChunkGenerator.GenerateBlockType(this.x, this.y, this.Settings);
    }

    public override void SetBlockType(int x, int y, int z, BlockType type)
    {
        Block block = new Block(type, new Location(x, y, z));
        var position = (x, y, z);
        if (_blocks.Contains(block))
        {
            _blocks.Remove(block);
        }
        else
        {
            throw new ArgumentException("Aucun bloc à cette position dans le chunk.");
        }
    }

    public override void SetBlockType(Location localPosition, BlockType type)
    {
        SetBlockType(localPosition.X, localPosition.Y, localPosition.Z, type);
    }

    public override BlockType GetBlockType(int x, int y, int z)
    {
        Location position = new Location(x, y, z);
        for (var i = _blocks.Count - 1; i >= 0; i--)
        {
            Block block = _blocks[i];
            
            if (block.GetPosition().Equals(position))
            {
                return block.GetBlockType();
            }
        }
        
        throw new ArgumentException("Aucun bloc à cette position dans le chunk.");
    }
    public override BlockType GetBlockType(Location position)
    {
        return GetBlockType(position.X, position.Y, position.Z);
    }
    public override Block GetBlock(int x, int y, int z)
    {
        Location position = new Location(x, y, z);
        for (var i = _blocks.Count - 1; i >= 0; i--)
        {
            Block block = _blocks[i];
            
            if (block.GetPosition().Equals(position))
            {
                return block;
            }
        }
        
        throw new ArgumentException("Aucun bloc à cette position dans le chunk.");
    }
    public override Block GetBlock(Location position)
    {
        return GetBlock(position.X, position.Y, position.Z);
    }

    public override List<Block> GetAllBlocks()
    {
        return _blocks;
    }

    public override void AddBlock(Block block)
    {
        if (_blocks.Contains(block))
        {
            throw new ArgumentException("Le bloc est déjà présent dans le chunk.");
        }
        else
        {
            _blocks.Add(block);
        }
    }

    public override Location LocalPostion(Location worldPosition)
    {
        int x = worldPosition.X - this.x * Settings.GetChunkSize();
        int y = worldPosition.Y - this.y * Settings.GetChunkSize();
        int z = worldPosition.Z;
        return new Location(x, y, z);
    }

    public void SetHeight(int x, int y, int height)
    {
        if (x >= 0 && x < Settings.GetChunkSize() && y >= 0 && y < Settings.GetChunkSize())
        {
            _heightMap[x, y] = height;
        }
        else
        {
            throw new ArgumentOutOfRangeException("Les coordonnées (x, y) doivent être comprises entre 0 et 15 inclus.");
        }
    }
}
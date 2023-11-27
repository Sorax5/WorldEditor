using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using logic;
using logic.models;

public class ChunkWorldGeneration : WorldGeneration
{
    private readonly Dictionary<Location, BaseChunk> _chunks = new Dictionary<Location, BaseChunk>();
    
    public ChunkWorldGeneration(WorldGeneratorSettings settings) : base(settings)
    {
    }

    public override BlockType GetBlockType(int x, int y, int z)
    {
        Location chunkPosition = GetChunkPosition(new Location(x, y, z));
        BaseChunk chunk = GetChunk(chunkPosition);
        return chunk.GetBlockType(x, y, z);
    }
    public override BlockType GetBlockType(Location position)
    {
        return GetBlockType(position.X, position.Y, position.Z);
    }

    public override void SetBlockType(int x, int y, int z, BlockType blockType)
    {
        SetBlockType(new Location(x,y,z),blockType);
    }
    
    public override void SetBlockType(Location position, BlockType blockType)
    {
        Location chunkPosition = GetChunkPosition(position);
        BaseChunk chunk = GetChunk(chunkPosition);

        Location localPosition = chunk.LocalPostion(position);
        
        chunk.SetBlockType(localPosition, blockType);
        RaiseOnBlockChanged(position, blockType);
    }

    public override List<Block> GetBlocksInRadius(Location position, int radius)
    {
        List<Block> blocks = new List<Block>();
        int chunkSize = this.Settings.GetChunkSize();

        Location startChunkPos = GetChunkPosition(new Location(position.X - radius, position.Y - radius, position.Z));
        Location endChunkPos = GetChunkPosition(new Location(position.X + radius, position.Y + radius, position.Z));

        int estimatedBlockCount = ((endChunkPos.X - startChunkPos.X) / chunkSize + 1) * ((endChunkPos.Y - startChunkPos.Y) / chunkSize + 1);
        blocks.Capacity = estimatedBlockCount;

        for (int x = startChunkPos.X; x <= endChunkPos.X; x += chunkSize)
        {
            for (int y = startChunkPos.Y; y <= endChunkPos.Y; y += chunkSize)
            {
                var chunkBlocks = GetChunk(new Location(x, y, position.Z)).GetAllBlocks();
                blocks.AddRange(chunkBlocks);
            }
        }

        return blocks;
    }
    
    
    
    public override List<Block> GetBlocksBetween(Location position1, Location position2)
    {
        return new List<Block>();
    }

    public override async Task<List<Block>> GetBlocksAsyncInRadius(Location position, int radius)
    {
        HashSet<BaseChunk> chunksInRadius = new HashSet<BaseChunk>();
        
        int CHUNK_RADIUS = Settings.GetChunkSize() * radius;

        Location startChunkPos = GetChunkPosition(new Location(position.X - radius, position.Y - radius, position.Z));
        Location endChunkPos = GetChunkPosition(new Location(position.X + radius, position.Y + radius, position.Z));

        HashSet<Location> uniqueChunkPositions = new HashSet<Location>();

        for (int x = startChunkPos.X; x <= endChunkPos.X; x += CHUNK_RADIUS)
        {
            for (int y = startChunkPos.Y; y <= endChunkPos.Y; y += CHUNK_RADIUS)
            {
                uniqueChunkPositions.Add(new Location(x, y, position.Z));
            }
        }

        var tasks = uniqueChunkPositions.Select(chunkPosition => Task.Run(() =>
        {
            chunksInRadius.Add(GetChunk(chunkPosition));
        }));

        await Task.WhenAll(tasks);

        return chunksInRadius.SelectMany(chunk => chunk.GetAllBlocks()).ToList();
    }


    public override async Task<List<Block>> GetBlocksAsyncBetween(Location position1, Location position2)
    {
        List<Block> blocks = new List<Block>();
        List<Task> chunkTasks = new List<Task>();

        int chunkSize = this.Settings.GetChunkSize();
        
        foreach (BaseChunk chunk in GetChunkInBetween(position1, position2))
        {
            chunkTasks.Add(Task.Run(() =>
            {
                /*BlockType[,,] chunkBlocks = chunk.GetBlocks();
                Vector2Int chunkPosition = chunk.GetPosition();

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        for (int z = 0; z < chunkSize; z++)
                        {
                            Vector3Int cellPosition = new Vector3Int(chunkPosition.x, chunkPosition.y, z) + new Vector3Int(x, y, z);
                            BlockType blockType = chunkBlocks[x, y, z];

                            lock (blocks)
                            {
                                blocks.Add(cellPosition, blockType);
                            }
                        }
                    }
                }*/
            }));
        }
        
        await Task.WhenAll(chunkTasks);
        
        return blocks;
        
    }

    public override void ResetBlocksInRadius(Location position, int radius)
    {
        int chunkSize = this.Settings.GetChunkSize();

        foreach (BaseChunk chunk in GetChunksInRadius(position, radius))
        {
            this._chunks.Remove(chunk.Position);
        }
    }

    private BaseChunk GetChunk(Location chunkPosition)
    {
        if (_chunks.TryGetValue(chunkPosition, out BaseChunk chunk))
        {
            return chunk;
        }

        if (!_chunks.ContainsKey(chunkPosition))
        {
            BaseChunk newChunk = CreateChunk(chunkPosition);
            _chunks.Add(chunkPosition, newChunk);
            return newChunk;
        }
        
        return null;
    }

    private BaseChunk CreateChunk(Location chunkPosition)
    {
        return new HeightlessChunk(chunkPosition.X, chunkPosition.Y, this.Settings, new PlainChunkGenerator());
    }

    private Location GetChunkPosition(Location blockPosition)
    {
        var chunkSize = this.Settings.GetChunkSize();
        
        int x = (int)MathF.Floor((float)blockPosition.X / chunkSize) * chunkSize;
        int y = (int)MathF.Floor((float)blockPosition.Y / chunkSize) * chunkSize;

        return new Location(x, y, blockPosition.Z);
    }

    public override HashSet<BaseChunk> GetChunksInRadius(Location position, int radius)
    {
        HashSet<BaseChunk> chunksInRadius = new HashSet<BaseChunk>();
        int chunkSize = 16;

        Location startChunkPos = GetChunkPosition(new Location(position.X - radius, position.Y - radius, position.Z));
        Location endChunkPos = GetChunkPosition(new Location(position.X + radius, position.Y + radius, position.Z));

        HashSet<Location> uniqueChunkPositions = new HashSet<Location>();

        for (int x = startChunkPos.X; x <= endChunkPos.X; x += chunkSize)
        {
            for (int y = startChunkPos.Y; y <= endChunkPos.Y; y += chunkSize)
            {
                uniqueChunkPositions.Add(new Location(x, y, position.Z));
            }
        }

        foreach (Location chunkPosition in uniqueChunkPositions)
        {
            chunksInRadius.Add(GetChunk(chunkPosition));
        }

        return chunksInRadius;
    }
    
    private HashSet<BaseChunk> GetChunkInBetween(Location position1, Location position2)
    {
        HashSet<BaseChunk> chunksInRadius = new HashSet<BaseChunk>();
        
        int minX = (int)MathF.Min(position1.X, position2.X);
        int maxX = (int)MathF.Max(position1.X, position2.X);
        int minY = (int)MathF.Min(position1.Y, position2.Y);
        int maxY = (int)MathF.Max(position1.Y, position2.Y);

        for (int x = minX; x <= maxX; x += 1)
        {
            for (int y = minY; y <= maxY; y += 1)
            {
                Location chunkPosition = GetChunkPosition(new Location(x, y, position1.Z));
                BaseChunk chunk = GetChunk(chunkPosition);
                chunksInRadius.Add(chunk);
            }
        }

        return chunksInRadius;
    }

    public override Block GetBlock(Location position)
    {
        BaseChunk chunk = GetChunk(GetChunkPosition(position));
        return chunk.GetBlock(position);
    }

    public override Block GetBlock(int x, int y, int z)
    {
        return GetBlock(new Location(x, y, z));
    }
}
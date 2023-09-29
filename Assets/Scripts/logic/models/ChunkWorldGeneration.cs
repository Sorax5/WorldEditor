using System.Collections.Generic;
using System.Threading.Tasks;
using logic.models;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkWorldGeneration", menuName = "WorldGeneration/ChunkWorldGeneration", order = 0)]
public class ChunkWorldGeneration : WorldGeneration
{
    private readonly Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    public override BlockType GetBlockType(int x, int y, int z)
    {
        Vector2Int chunkPosition = GetChunkPosition(new Vector2Int(x, y));
        Chunk chunk = GetChunk(chunkPosition);

        if (chunk != null)
        {
            return chunk.GetBlock(new Vector3Int(x - chunkPosition.x, y - chunkPosition.y));
        }
        
        Chunk newChunk = new Chunk(chunkPosition, GameManager.Instance.Settings);
        _chunks.Add(chunkPosition, newChunk);

        return newChunk.GetBlock(new Vector3Int(x - chunkPosition.x, y - chunkPosition.y));
    }

    public override BlockType GetBlockType(Vector3Int position)
    {
        return GetBlockType(position.x, position.y, position.z);
    }

    public override void SetBlockType(int x, int y, int z, BlockType blockType)
    {
        SetBlockType(new Vector3Int(x,y,z),blockType);
    }
    
    public override void SetBlockType(Vector3Int position, BlockType blockType)
    {
        Vector2Int chunkPosition = GetChunkPosition(new Vector2Int(position.x, position.y));
        Chunk chunk = GetChunk(chunkPosition);
        
        Vector3Int localPosition = chunk.RelativePosition(position.x, position.y, position.z);
        
        chunk.SetBlockType(localPosition, blockType);
        RaiseOnBlockChanged(position, blockType);
    }

    public override Dictionary<Vector3Int,BlockType> GetBlocksInRadius(Vector3Int position, int radius)
    {
        Dictionary<Vector3Int, BlockType> blocks = new Dictionary<Vector3Int, BlockType>();
        int chunkSize = GameManager.Instance.Settings.GetChunkSize();
        
        foreach (Chunk chunk in GetChunksInRadius(new Vector2Int(position.x, position.y), radius))
        {
            BlockType[,,] chunkBlocks = chunk.GetBlocks();
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
            }
        }

        return blocks;
    }

    public override Dictionary<Vector3Int,BlockType> GetBlocksBetween(Vector3Int position1, Vector3Int position2)
    {
        return new Dictionary<Vector3Int, BlockType>();
    }

    public override async Task<Dictionary<Vector3Int, BlockType>> GetBlocksAsyncInRadius(Vector3Int position, int radius)
    {
        Dictionary<Vector3Int, BlockType> blocks = new Dictionary<Vector3Int, BlockType>();
        List<Task> chunkTasks = new List<Task>();
        
        int chunkSize = GameManager.Instance.Settings.GetChunkSize();

        foreach (Chunk chunk in GetChunksInRadius(new Vector2Int(position.x, position.y), radius))
        {
            chunkTasks.Add(Task.Run(() =>
            {
                BlockType[,,] chunkBlocks = chunk.GetBlocks();
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
                }
            }));
        }

        await Task.WhenAll(chunkTasks);

        return blocks;
    }

    public override async Task<Dictionary<Vector3Int, BlockType>> GetBlocksAsyncBetween(Vector3Int position1, Vector3Int position2)
    {
        Dictionary<Vector3Int, BlockType> blocks = new Dictionary<Vector3Int, BlockType>();
        List<Task> chunkTasks = new List<Task>();
        
        int chunkSize = GameManager.Instance.Settings.GetChunkSize();
        
        foreach (Chunk chunk in GetChunkInBetween(new Vector2Int(position1.x, position1.y), new Vector2Int(position2.x, position2.y)))
        {
            chunkTasks.Add(Task.Run(() =>
            {
                BlockType[,,] chunkBlocks = chunk.GetBlocks();
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
                }
            }));
        }
        
        await Task.WhenAll(chunkTasks);
        
        return blocks;
        
    }

    public override void ResetBlocksInRadius(Vector3Int position, int radius)
    {
        int chunkSize = GameManager.Instance.Settings.GetChunkSize();

        foreach (Chunk chunk in GetChunksInRadius(new Vector2Int(position.x, position.y), radius))
        {
            this._chunks.Remove(chunk.GetPosition());
        }
    }

    private Chunk GetChunk(Vector2Int chunkPosition)
    {
        if (_chunks.TryGetValue(chunkPosition, out Chunk chunk))
        {
            return chunk;
        }

        Chunk newChunk = CreateChunk(chunkPosition);
        _chunks.Add(chunkPosition, newChunk);
        return newChunk;
    }

    private Chunk CreateChunk(Vector2Int chunkPosition)
    {
        return new Chunk(chunkPosition, GameManager.Instance.Settings);
    }

    private Vector2Int GetChunkPosition(Vector2Int blockPosition)
    {
        var chunkSize = GameManager.Instance.Settings.GetChunkSize();

        // Calculez les coordonnées du chunk.
        int x = Mathf.FloorToInt((float)blockPosition.x / chunkSize) * chunkSize;
        int y = Mathf.FloorToInt((float)blockPosition.y / chunkSize) * chunkSize;

        return new Vector2Int(x, y);
    }

    private HashSet<Chunk> GetChunksInRadius(Vector2Int position, int radius)
    {
        HashSet<Chunk> chunksInRadius = new HashSet<Chunk>();

        int minX = position.x - radius;
        int maxX = position.x + radius;
        int minY = position.y - radius;
        int maxY = position.y + radius;

        for (int x = minX; x <= maxX; x += 1)
        {
            for (int y = minY; y <= maxY; y += 1)
            {
                Vector2Int chunkPosition = GetChunkPosition(new Vector2Int(x, y));
                Chunk chunk = GetChunk(chunkPosition);
                chunksInRadius.Add(chunk);
            }
        }

        return chunksInRadius;
    }
    
    private HashSet<Chunk> GetChunkInBetween(Vector2Int position1, Vector2Int position2)
    {
        HashSet<Chunk> chunksInRadius = new HashSet<Chunk>();

        int minX = Mathf.Min(position1.x, position2.x);
        int maxX = Mathf.Max(position1.x, position2.x);
        int minY = Mathf.Min(position1.y, position2.y);
        int maxY = Mathf.Max(position1.y, position2.y);

        for (int x = minX; x <= maxX; x += 1)
        {
            for (int y = minY; y <= maxY; y += 1)
            {
                Vector2Int chunkPosition = GetChunkPosition(new Vector2Int(x, y));
                Chunk chunk = GetChunk(chunkPosition);
                chunksInRadius.Add(chunk);
            }
        }

        return chunksInRadius;
    }
}
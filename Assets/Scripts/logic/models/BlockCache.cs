using System.Collections.Generic;
using logic.models;
using UnityEngine;

public class BlockCache
{
    private Dictionary<Vector3Int, Dictionary<Vector3Int, BlockType>> cache = new Dictionary<Vector3Int, Dictionary<Vector3Int, BlockType>>();

    public bool TryGet(Vector3Int position, out Dictionary<Vector3Int, BlockType> cachedResult)
    {
        return cache.TryGetValue(position, out cachedResult);
    }

    public void Add(Vector3Int position, Dictionary<Vector3Int, BlockType> blocks)
    {
        cache[position] = blocks;
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using logic.models;
using UnityEngine;

/// <summary>
/// WorldGeneration is an interface for the world generation.
/// </summary>

public abstract class WorldGeneration : ScriptableObject
{
    public event Action<Vector3Int, BlockType> OnBlockChanged;

    protected void RaiseOnBlockChanged(Vector3Int position, BlockType blockType)
    {
        OnBlockChanged?.Invoke(position, blockType);
    }
    
    public abstract BlockType GetBlockType(int x, int y, int z);
    public abstract BlockType GetBlockType(Vector3Int position);
    public abstract void SetBlockType(int x, int y, int z, BlockType blockType);
    public abstract void SetBlockType(Vector3Int position, BlockType blockType);
    public abstract Dictionary<Vector3Int,BlockType> GetBlocksInRadius(Vector3Int position, int radius);
    public abstract Dictionary<Vector3Int,BlockType> GetBlocksBetween(Vector3Int position1, Vector3Int position2);
    
    public abstract Task<Dictionary<Vector3Int,BlockType>> GetBlocksAsyncInRadius(Vector3Int position, int radius);
    public abstract Task<Dictionary<Vector3Int,BlockType>> GetBlocksAsyncBetween(Vector3Int position1, Vector3Int position2);
    public abstract void ResetBlocksInRadius(Vector3Int position, int radius);
}
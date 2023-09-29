using System.Collections;
using System.Collections.Generic;
using logic.models;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WorldView))]
public class WorldController : MonoBehaviour
{
    [SerializeField] private WorldGeneration world;
    
    public void SetTile(int x, int y, int z, BlockType blockType)
    {
        this.world.SetBlockType(x, y, z, blockType);
    }
    
    public void SetTile(Vector3Int position, BlockType blockType)
    {
        this.world.SetBlockType(position, blockType);
    }

    public WorldGeneration GetWorld()
    {
        return this.world;
    }
}

using System;
using System.Collections.Generic;
using logic.models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/**
 * This class is responsible for rendering the world.
 */
[RequireComponent( typeof(WorldController))]
public class WorldView : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Grid globalGrid;
    
    [Header("Tilemaps")]
    [SerializeField] private Tilemap ground;
    
    [Header("Tiles")]
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase sandTile;
    [SerializeField] private TileBase stoneTile;
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase weedTile;
    
    [Header("Controller")]
    [SerializeField] private WorldController worldController;
    [FormerlySerializedAs("cameraMovement")] [SerializeField] private PlayerMovementBehavior playerMovementBehavior;
    [SerializeField] private int renderDistance = 1;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform player;

    private Dictionary<BlockType, TileBase> _blockTypeToTile = new Dictionary<BlockType, TileBase>();
    private Queue<Action> worldGenerationQueue = new Queue<Action>();
    private bool isGeneratingWorld = false;
    

    private void Awake()
    {
        //this.worldController.GetWorld().OnBlockChanged += OnBlockChanged;
        //this.playerMovementBehavior.OnPlayerMoved += OnPlayerMoved;
    }

    // Start is called before the first frame update
    void Start()
    {
        ground.ClearAllTiles();
        _blockTypeToTile.Add(BlockType.AIR, null);
        _blockTypeToTile.Add(BlockType.WATER, waterTile);
        _blockTypeToTile.Add(BlockType.GRASS, grassTile);
        _blockTypeToTile.Add(BlockType.SAND, sandTile);
        _blockTypeToTile.Add(BlockType.STONE, stoneTile);
        _blockTypeToTile.Add(BlockType.DIRT, dirtTile);
        _blockTypeToTile.Add(BlockType.WEED, weedTile);
        
    }

    private void GenerateWorld(WorldGeneration world)
    {
        /*Vector3Int playerPosition = ground.WorldToCell(this.playerMovementBehavior.transform.position);
        List<Chunk> chunks = world.GetChunksInRadius(new Vector2Int(playerPosition.x,playerPosition.y), renderDistance);

        int maxPositionsCount = chunks.Count * Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE;
        Vector3Int[] positions = new Vector3Int[maxPositionsCount];
        TileBase[] tiles = new TileBase[maxPositionsCount];

        int tileIndex = 0;

        foreach (Chunk chunk in chunks)
        {
            BlockType[,,] blocks = chunk.GetBlocks();
            Vector2Int chunkPosition = chunk.GetPosition();

            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                    {
                        Vector3Int cellPosition = new Vector3Int(chunkPosition.x,chunkPosition.y,0) + new Vector3Int(x, y, z);

                        if (!ground.HasTile(cellPosition))
                        {
                            positions[tileIndex] = cellPosition;
                            BlockType blockType = blocks[x, y, z];
                            tiles[tileIndex] = _blockTypeToTile.TryGetValue(blockType, out TileBase specificTile) ? specificTile : null;
                            tileIndex++;
                        }
                    }
                }
            }
        }

        ground.SetTiles(positions, tiles);*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.playerMovementBehavior.transform.position, renderDistance);
    }
}

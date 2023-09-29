using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using logic.models;
using Unity.VisualScripting;
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
        this.worldController.GetWorld().OnBlockChanged += OnBlockChanged;
        this.playerMovementBehavior.OnPlayerMoved += OnPlayerMoved;
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

        this.GenerateWorldAsync(worldController.GetWorld());
    }

    private void OnBlockChanged(Vector3Int position, BlockType blockType)
    {
        TileBase tile = _blockTypeToTile.TryGetValue(blockType, out TileBase specificTile) ? specificTile : null;
        ground.SetTile(position, tile);
    }

    private void OnPlayerMoved(Vector3 lastPosition, Vector3 currentPosition)
    {
        if (!isGeneratingWorld)
        {
            // Ajoutez la génération du monde à la file d'attente.
            worldGenerationQueue.Enqueue(() => GenerateWorldAsync(this.worldController.GetWorld()));

            // Démarrez le processus de génération du monde s'il n'est pas déjà en cours.
            if (!isGeneratingWorld)
            {
                StartCoroutine(ProcessWorldGenerationQueue());
            }
        }
    }
    
    private IEnumerator ProcessWorldGenerationQueue()
    {
        if (worldGenerationQueue.Count > 0)
        {
            isGeneratingWorld = true;

            // Attendez un petit délai pour regrouper les appels.
            yield return new WaitForSeconds(0.5f);

            // Exécutez la génération du monde pour le dernier appel.
            Action generateAction = worldGenerationQueue.Dequeue();
            generateAction.Invoke();
        }

        isGeneratingWorld = false;
    }

    private void GenerateWorldAsync(WorldGeneration world)
    {
        Vector3Int playerPosition = ground.WorldToCell(playerMovementBehavior.transform.position);
        int renderSize = renderDistance * GameManager.Instance.Settings.GetChunkSize();

        Dictionary<Vector3Int, BlockType> map = world.GetBlocksInRadius(playerPosition, renderDistance * renderSize);
        
        // Utilisez LINQ pour filtrer les tuiles à mettre à jour et les convertir en tableau.
        var updatedTiles = map
            .Where(kvp => ShouldUpdateTile(kvp.Value, ground.GetTile(kvp.Key)))
            .ToArray();
        
        Vector3Int[] tilePositionsToUpdate = new Vector3Int[updatedTiles.Length];

        for (int i = 0; i < updatedTiles.Length; i++)
        {
            tilePositionsToUpdate[i] = updatedTiles[i].Key;
        }
        
        TileBase[] tilesToUpdate = updatedTiles
            .Select(kvp => GetCounterPartTile(kvp.Value))
            .ToArray();
        
        ground.SetTiles(tilePositionsToUpdate, tilesToUpdate);

        /*foreach (var kvp in map)
        {
            if (!_blockTypeToTile.TryGetValue(kvp.Value, out TileBase specificTile))
            {
                continue; // Ignore les blocs sans tuile correspondante.
            }

            TileBase tile = ground.GetTile(kvp.Key);

            if (tile == null || tile != specificTile)
            {
                tilesToUpdate.Add(kvp.Key);
            }
        }

        if (tilesToUpdate.Count > 0)
        {
            // Convertissez la liste en tableau pour l'utilisation de SetTiles.
            Vector3Int[] tilePositions = tilesToUpdate.ToArray();

            // Créez un tableau de tuiles à partir des positions.
            TileBase[] tileArray = new TileBase[tilePositions.Length];
            for (int i = 0; i < tilePositions.Length; i++)
            {
                tileArray[i] = _blockTypeToTile[map[tilePositions[i]]];
            }

            // Mettez à jour les tuiles en une seule opération.
            ground.SetTiles(tilePositions, tileArray);
        }*/

        /*foreach (var keyValuePair in map.ToList())
        {
            TileBase tile = ground.GetTile(keyValuePair.Key);
            BlockType type = _blockTypeToTile.FirstOrDefault(x => x.Value == tile).Key;

            if (keyValuePair.Value == type)
            {
                continue;
            }
            
            tileMap.Add(keyValuePair.Key, _blockTypeToTile.TryGetValue(keyValuePair.Value, out TileBase specificTile) ? specificTile : null);
        }

        ground.SetTiles(tileMap.Keys.ToArray(), tileMap.Values.ToArray());*/
    }

    private TileBase GetCounterPartTile(BlockType argValue)
    {
        return _blockTypeToTile.TryGetValue(argValue, out TileBase specificTile) ? specificTile : null;
    }

    private bool ShouldUpdateTile(BlockType argValue, TileBase getTile)
    {
        return GetCounterPartTile(argValue) != getTile;
    }

    private TileBase[] getCounterPart(BlockType[] array)
    {
        TileBase[] tiles = new TileBase[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            tiles[i] = _blockTypeToTile.TryGetValue(array[i], out TileBase specificTile)
                ? specificTile
                : null;
        }

        return tiles;
    }

    private List<Vector3> GetCardinalCameraCoordinate()
    {
        Vector3[] cardinalCameraCoordinate = new Vector3[4];
        this.mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), this.mainCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, cardinalCameraCoordinate);
        return cardinalCameraCoordinate.ToList();
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

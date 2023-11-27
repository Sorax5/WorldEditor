using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using logic.models;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockView : MonoBehaviour
{
    private BlockController blockController;
    
    public BlockController BlockController
    {
        get => blockController;
        set
        {
            if (blockController != null)
            {
                blockController.OnBlockChanged -= OnBlockChanged;
            }

            blockController = value;
            blockController.OnBlockChanged += OnBlockChanged;
        }
    }
    
    [Header("GameObjects")]
    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private Camera mainCamera;

    [Header("Data")]
    [SerializeField] private UDictionary<BlockType, TileBase> blockTypeToTile;
    [SerializeField] private int renderDistance = 1;
    
    private Queue<Action> worldGenerationQueue;
    
    private bool isGeneratingWorld = false;

    private void Start()
    {
        worldGenerationQueue = new Queue<Action>();
        this.GenerateWorldByChunk(blockController.GetWorld());
    }

    private void OnBlockChanged(Location position, BlockType blockType)
    {
        TileBase tile = blockTypeToTile.TryGetValue(blockType, out TileBase specificTile) ? specificTile : null;
        groundTileMap.SetTile(new Vector3Int(position.X, position.Y, position.Z), tile);
    }
    
    public void OnPlayerMoved(Vector3 lastPosition, Vector3 currentPosition)
    {
        if (!isGeneratingWorld)
        {
            // Ajoutez la génération du monde à la file d'attente.
            worldGenerationQueue.Enqueue(() => GenerateWorldByChunk(this.blockController.GetWorld()));

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

    private async void GenerateWorldAsync(WorldGeneration world)
    {
        Vector3Int playerPosition = groundTileMap.WorldToCell(mainCamera.transform.position);
        Location pl = new Location(playerPosition.x, playerPosition.y, playerPosition.z);
        int renderSize = renderDistance * 16;

        List<Block> blocks = world.GetBlocksInRadius(pl, renderSize);
            
        List<Vector3Int> tilePositionsToUpdate = new List<Vector3Int>();
        List<TileBase> tilesToUpdate = new List<TileBase>();

        foreach (var kvp in blocks)
        {
            var blockType = kvp.GetBlockType();
            var position = kvp.GetPosition();
            var tile = groundTileMap.GetTile(new Vector3Int(position.X, position.Y, position.Z));

            if (ShouldUpdateTile(blockType, tile))
            {
                tilePositionsToUpdate.Add(new Vector3Int(position.X, position.Y, position.Z));
                tilesToUpdate.Add(GetCounterPartTile(blockType));
            }
        }

        PlaceTiles(tilePositionsToUpdate.ToArray(), tilesToUpdate.ToArray());
    }

    private void GenerateWorldByChunk(WorldGeneration world)
    {
        Vector3Int playerPosition = groundTileMap.WorldToCell(mainCamera.transform.position);
        Location pl = new Location(playerPosition.x, playerPosition.y, playerPosition.z);
        int renderSize = renderDistance * 16;
        
        HashSet<BaseChunk> chunks = world.GetChunksInRadius(pl, renderSize);
        foreach (BaseChunk chunk in chunks)
        {
            GenerateChunkByEachBlock(chunk);
        }
    }
    
    private void GenerateChunk(BaseChunk chunk)
    {
        List<Block> blocks = chunk.GetAllBlocks();
        
        List<Vector3Int> tilePositionsToUpdate = new List<Vector3Int>();
        List<TileBase> tilesToUpdate = new List<TileBase>();
        
        foreach (var kvp in blocks)
        {
            var blockType = kvp.GetBlockType();
            var position = kvp.GetPosition();
            var tile = groundTileMap.GetTile(new Vector3Int(position.X, position.Y, position.Z));

            if (ShouldUpdateTile(blockType, tile))
            {
                tilePositionsToUpdate.Add(new Vector3Int(position.X, position.Y, position.Z));
                tilesToUpdate.Add(GetCounterPartTile(blockType));
            }
        }

        PlaceTiles(tilePositionsToUpdate.ToArray(), tilesToUpdate.ToArray());
    }

    private void GenerateChunkByEachBlock(BaseChunk chunk)
    {
        List<Block> blocks = chunk.GetAllBlocks();
        
        Vector3Int playerPosition = groundTileMap.WorldToCell(mainCamera.transform.position);
        Location pl = new Location(playerPosition.x, playerPosition.y, playerPosition.z);
        
        StartCoroutine(PlaceBlock(blocks));
    }

    private IEnumerator PlaceBlock(List<Block> blocks)
    {
        List<Vector3Int> positionsToUpdate = new List<Vector3Int>();
        List<TileBase> counterpartTiles = new List<TileBase>();

        BlockType blockType;
        Location position;
        Vector3Int positionInt;
        TileBase tile;

        foreach (var block in blocks)
        {
            blockType = block.GetBlockType();
            position = block.GetPosition();
            positionInt = new Vector3Int(position.X, position.Y, position.Z);
            tile = groundTileMap.GetTile(positionInt);

            if (ShouldUpdateTile(blockType, tile))
            {
                positionsToUpdate.Add(positionInt);
                counterpartTiles.Add(GetCounterPartTile(blockType));
            }
        }

        if (positionsToUpdate.Count > 0)
        {
            PlaceTiles(positionsToUpdate.ToArray(), counterpartTiles.ToArray());
        }

        yield return null;
    }

    
    
    private void PlaceTiles(Vector3Int[] positions, TileBase[] tiles)
    {
        groundTileMap.SetTiles(positions, tiles);
    }

    private TileBase GetCounterPartTile(BlockType argValue)
    {
        return blockTypeToTile.TryGetValue(argValue, out TileBase specificTile) ? specificTile : null;
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
            tiles[i] = blockTypeToTile.TryGetValue(array[i], out TileBase specificTile)
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
}
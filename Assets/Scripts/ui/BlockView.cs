using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using logic.models;
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
        this.GenerateWorldAsync(blockController.GetWorld());
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
            worldGenerationQueue.Enqueue(() => GenerateWorldAsync(this.blockController.GetWorld()));

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

    private async Task GenerateWorldAsync(WorldGeneration world)
    {
        Debug.Log("Generating world...");
        Vector3Int playerPosition = groundTileMap.WorldToCell(mainCamera.transform.position);
        Debug.Log($"Player position: {playerPosition}");
        Location pl = new Location(playerPosition.x, playerPosition.y, playerPosition.z);
        int renderSize = renderDistance * 16;
        Debug.Log($"Player position: {pl}");

        List<Block> blocks = world.GetBlocksInRadius(pl, renderSize);
        Debug.Log($"Blocks: {blocks.Count}");

        List<Vector3Int> tilePositionsToUpdate = new List<Vector3Int>();
        List<TileBase> tilesToUpdate = new List<TileBase>();
        
        Debug.Log($"Blocks to update: {blocks.Count}");

        foreach (var kvp in blocks)
        {
            var blockType = kvp.GetBlockType();
            var position = kvp.GetPosition();
            var tile = groundTileMap.GetTile(new Vector3Int(position.X, position.Y, position.Z));
            
            Debug.Log($"Block: {position} - {blockType} - {tile}");

            if (ShouldUpdateTile(blockType, tile))
            {
                tilePositionsToUpdate.Add(new Vector3Int(position.X, position.Y, position.Z));
                tilesToUpdate.Add(GetCounterPartTile(blockType));
            }
        }

        groundTileMap.SetTiles(tilePositionsToUpdate.ToArray(), tilesToUpdate.ToArray());
        
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
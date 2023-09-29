using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridView : MonoBehaviour
{
    
    [SerializeField] private GameObject player;
    
    [SerializeField] private Tilemap gridTileMap;
    
    [SerializeField] private TileBase gridTile;
    
    [SerializeField] private int radius = 20;

    // Update is called once per frame
    void Update()
    {
        // place grid around main camera position in a radius of 10 & remove the grid that is not in the radius
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                Vector3Int cameraCellPosition = this.gridTileMap.WorldToCell(this.player.transform.position);
                
                Vector3Int cellPosition = new Vector3Int(cameraCellPosition.x + x, cameraCellPosition.y + y, 0);
                
                if (Vector3Int.Distance(cellPosition, cameraCellPosition) < radius)
                {
                    this.gridTileMap.SetTile(cellPosition, this.gridTile);
                }
                else
                {
                    this.gridTileMap.SetTile(cellPosition, null);
                }
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseView : MonoBehaviour
{
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap groundTileMap;

    [SerializeField] private WorldController worldController;
    
    private List<Vector3Int> _highlightedTiles = new List<Vector3Int>();

    // Update is called once per frame
    void Update()
    {
        
        _highlightedTiles.ForEach(tile =>
        {
            groundTileMap.SetTileFlags(tile, TileFlags.None);
            groundTileMap.SetColor(tile, Color.white);
        });
        
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = groundTileMap.WorldToCell(mousePosition);

        //cellPosition = getTopCell(cellPosition);

        // fetch tiledata
        /*if (groundTileMap.HasTile(cellPosition))
        {
            // highlight tile
            groundTileMap.SetTileFlags(cellPosition, TileFlags.None);
            groundTileMap.SetColor(cellPosition, Color.red);
        }
        else
        {
            // unhighlight tile
            groundTileMap.SetTileFlags(cellPosition, TileFlags.None);
            groundTileMap.SetColor(cellPosition, Color.white);
        }
        
        _highlightedTiles.Add(cellPosition);*/
        
    }
    
    /*public Vector3Int GetTopCell()
    {
 
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = t.WorldToCell(mousePos);
 
        //Iterate through tile layers starting at the top
        for (int i = zMax; i >= 0; i--)
        {
            //Adjust mouse world coordinate to include z (i)
            Vector3 v = new Vector3(mousePos.x, mousePos.y + i * ydelta, i);
            v += mouseOffset;
            //check cell
            cell = t.WorldToCell(v);
            if (t.HasTile(cell))
            {
                cell = getTopCell(cell);
                Debug.Log(i + " : " + cell);
                this.transform.position = t.CellToWorld(cell);
                break;
            }
        }
 
        return cell;
    }*/
 
    //Get a cell that is not covered by any tiles
    private Vector3Int getTopCell(Vector3Int cell)
    {
        //default to given cell
        Vector3Int newCell = cell;
 
        //iterate from given cell -> upwards
        for (int i = cell.z; i <= 5; i++)
        {
            Vector3Int _cell = new Vector3Int(cell.x, cell.y, i);
            if (groundTileMap.HasTile(_cell))
            {
                newCell = _cell;
            }
            else
            {
                //break on empty and return last tile found
                break;
            }
        }
        return newCell;
    }
}

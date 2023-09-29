using System;
using System.Collections;
using System.Collections.Generic;
using logic.models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This class is responsible for handling the user input and updating the world.
 */
public class PlayerView : MonoBehaviour
{
    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private WorldController worldController;
    
    [SerializeField] private int zMax = 100;

    // Update is called once per frame
    void Update()
    {
        // get mouseInput
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = groundTileMap.WorldToCell(mousePosition);

        cellPosition = getTopCell(cellPosition);
        

        if(Input.GetMouseButton(1))
        {
            Vector3Int cellPosition2 = cellPosition + new Vector3Int(0,0,1);
            this.worldController.SetTile(cellPosition2, BlockType.GRASS);
        }
        
        if(Input.GetMouseButton(0))
        {
            this.worldController.SetTile(cellPosition, BlockType.AIR);
        }
    }

    //Get a cell that is not covered by any tiles
    private Vector3Int getTopCell(Vector3Int cell)
    {
        //default to given cell
        Vector3Int newCell = cell;
 
        //iterate from given cell -> upwards
        for (int i = cell.z; i <= zMax; i++)
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

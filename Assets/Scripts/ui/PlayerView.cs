using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    
    [SerializeField] private int zMax = 100;
    
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    
    private BlockController blockController;
    
    public BlockController BlockController
    {
        get => blockController;
        set
        {
            blockController = value;
        }
    }

    public event Action<Vector3,Vector3> OnPlayerMoved;
    
    private Vector3 _lastPosition;
    private Coroutine _smoothCameraMove;

    private void Start()
    {
        this._lastPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        // get mouseInput
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = groundTileMap.WorldToCell(mousePosition);

        cellPosition = getTopCell(cellPosition);
        

        /*if(Input.GetMouseButton(1))
        {
            Vector3Int cellPosition2 = cellPosition + new Vector3Int(0,0,1);
            try
            {
                this.blockController.SetTile(cellPosition2.x, cellPosition2.y, cellPosition2.z, BlockType.AIR);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        
        if(Input.GetMouseButton(0))
        {
            try
            {
                this.blockController.SetTile(cellPosition.x, cellPosition.y, cellPosition.z, BlockType.STONE);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }*/
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 move = new Vector3(horizontal, vertical, 0);
        
        // speed by zoom
        move *= this.mainCamera.m_Lens.OrthographicSize / 10;
        Vector3 pos = transform.position += move * speed;

        if (Vector3.Distance(_lastPosition, pos) > 0.1f)
        {
            _lastPosition = pos;
            this.OnPlayerMoved?.Invoke(this._lastPosition, pos);
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

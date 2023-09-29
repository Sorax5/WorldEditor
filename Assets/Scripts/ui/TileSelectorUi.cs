using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelectorUi : MonoBehaviour
{
    
    [SerializeField] private GameObject tileUiComponentPrefab;
    [SerializeField] private GameObject content;
    
    private List<TileUiComponent> tileUiComponents = new List<TileUiComponent>();

    private void Awake()
    {
        foreach (var tile in Resources.LoadAll<Tile>("Tiles"))
        {
            TileUiComponent tileUiComponent = Instantiate(tileUiComponentPrefab, content.transform).GetComponent<TileUiComponent>();
            tileUiComponent.Tile = tile;
            tileUiComponents.Add(tileUiComponent);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

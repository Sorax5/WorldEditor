using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Class that represents a ui component for a tile.
/// </summary>
public class TileUiComponent : MonoBehaviour
{
    
    private Tile tile;
    
    [SerializeField] private TextMeshProUGUI textMeshProUgui;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    
    /// <summary>
    /// Getter and setter for the tile.
    /// </summary>
    public Tile Tile
    {
        get => tile;
        set{
            this.tile = value;
            this.textMeshProUgui.text = tile.name;
            this.image.sprite = tile.sprite;
        }
    }

    private void Awake()
    {
        //button.onClick.AddListener((() => GameManager.Instance.SelectedTile = tile));
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockTypeConstructor
{

    private readonly TileBase _tile;
    
    public BlockTypeConstructor(TileBase tileR)
    {
        _tile = tileR;

    }

    public TileBase Generate()
    {
        return _tile;
    }
}
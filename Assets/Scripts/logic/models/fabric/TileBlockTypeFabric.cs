using System;
using System.Collections.Generic;
using System.Linq;
using logic.models;
using UnityEngine.Tilemaps;

/// <summary>
/// Fabric of type of block for tilemap
/// </summary>
public class TileBlockTypeFabric
{
    private readonly Dictionary<BlockType, BlockTypeConstructor> _constructors;

    /// <summary>
    /// Constructor
    /// </summary>
    public TileBlockTypeFabric()
    {
        this._constructors = new Dictionary<BlockType, BlockTypeConstructor>();
    }

    /// <summary>
    /// register a new constructor
    /// </summary>
    /// <param name="type">Type of block</param>
    /// <param name="constructor">Constructor of the block</param>
    /// <exception cref="Exception">Type already register</exception>
    public void Register(BlockType type, BlockTypeConstructor constructor)
    {
        if (_constructors.ContainsKey(type))
        {
            throw new Exception();
        }
        this._constructors.Add(type,constructor);
    }

    /// <summary>
    /// Generate tilebase of type
    /// </summary>
    /// <param name="type">type to generate</param>
    /// <returns>Tilebase block</returns>
    /// <exception cref="Exception">Type doesn't register</exception>
    public TileBase Generate(BlockType type)
    {
        if (!_constructors.ContainsKey(type))
        {
            throw new Exception();
        }

        return _constructors[type].Generate();
    }

    /// <summary>
    /// Get list of type of block register
    /// </summary>
    /// <returns>array of blocktype</returns>
    public BlockType[] GetTypeRegister()
    {
        return _constructors.Keys.ToArray();
    }
}
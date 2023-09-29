using System.Collections;
using System.Collections.Generic;
using System.Linq;
using logic;
using logic.models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// Main class of the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    
    #region Singleton
    
    private static GameManager instance;
    
    private GameManager()
    {
        
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    [SerializeField] private WorldGenerationSettings settings;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private List<BlockType> types;

    private TileBlockTypeFabric fabric;

    public WorldGenerationSettings Settings => this.settings;


    // Start is called before the first frame update
    void Start()
    {
        /*fabric = new TileBlockTypeFabric();

        foreach (var tileBase in tiles)
        {
            foreach (var blockType in types)
            {
                BlockTypeConstructor constructor = new BlockTypeConstructor(tileBase);
                fabric.Register(blockType,constructor);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static GameManager Instance => instance;
}

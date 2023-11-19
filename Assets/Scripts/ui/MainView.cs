using System;
using logic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainView : MonoBehaviour
{

    [Header("Views")]
    [SerializeField] private BlockView blockView;
    [SerializeField] private PlayerView playerView;
    
    [Header("Models")]
    private WorldGeneration world;
    [SerializeField] private ScriptableWorldSettings scriptableWorldSettings;
    
    private BlockController blockController;

    private void Awake()
    {
        this.world = new ChunkWorldGeneration(this.scriptableWorldSettings.GetWorldGeneratorSettings());
        this.blockController = new BlockController(this.world);
        
        this.blockView.BlockController = this.blockController;
        this.playerView.BlockController = this.blockController;
        
        this.playerView.OnPlayerMoved += this.blockView.OnPlayerMoved;
    }
}
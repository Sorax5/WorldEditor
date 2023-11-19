using UnityEngine;
using UnityEngine.Serialization;

public sealed class ScriptableWorldSettings : ScriptableObject
    {

        [SerializeField] private int CHUNK_SIZE = 16;
        [SerializeField] private float NOISE_SCALE = 1f;
        [SerializeField] private int SEED = 0;
        [SerializeField] private int BASE_HEIGHT = 2;
        [SerializeField] private int MAX_HILL_HEIGHT = 10;
        [SerializeField] private int MIN_HILL_HEIGHT = 2;

        public int GetMaxHillHeight()
        {
            return this.MAX_HILL_HEIGHT;
        }
        
        public int GetMinHillHeight()
        {
            return this.MIN_HILL_HEIGHT;
        }
        
        public int GetBaseHeight()
        {
            return this.BASE_HEIGHT;
        }

        public int GetChunkSize()
        {
            return CHUNK_SIZE;
        }
    
        public float GetNoiseScale()
        {
            return NOISE_SCALE;
        }

        public int GetSeed()
        {
            return SEED;
        }
    
        public Vector2Int GetChunkPosition(Vector2Int position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / CHUNK_SIZE) * CHUNK_SIZE,
                Mathf.FloorToInt(position.y / CHUNK_SIZE) * CHUNK_SIZE
            );
        }
    
        public Vector2Int GetChunkPosition(int x, int y)
        {
            return GetChunkPosition(new Vector2Int(x, y));
        }
    
        public Vector2Int GetChunkPosition(Vector3Int position)
        {
            return GetChunkPosition(new Vector2Int(position.x, position.y));
        }
    
        public Vector3Int GetRelativePosition(Vector3Int position)
        {
            Vector2Int chunkPosition = GetChunkPosition(position);
            return new Vector3Int(
                position.x - chunkPosition.x,
                position.y - chunkPosition.y,
                position.z
            );
        }
        
        /**
         * Returns a WorldGeneratorSettings object with the values of this ScriptableWorldSettings.
         */
        public WorldGeneratorSettings GetWorldGeneratorSettings()
        {
            return new WorldGeneratorSettings(
                CHUNK_SIZE,
                NOISE_SCALE,
                SEED,
                BASE_HEIGHT,
                MIN_HILL_HEIGHT,
                MAX_HILL_HEIGHT
            );
        }
    }

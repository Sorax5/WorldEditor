public class WorldGeneratorSettings
{
    private int chunkSize;
    private float noiseScale;
    private int seed;
    private int baseHeight;
    private int minHillHeight;
    private int maxHillHeight;
    
    public WorldGeneratorSettings(int chunkSize, float noiseScale, int seed, int baseHeight, int minHillHeight, int maxHillHeight)
    {
        this.chunkSize = chunkSize;
        this.noiseScale = noiseScale;
        this.seed = seed;
        this.baseHeight = baseHeight;
        this.minHillHeight = minHillHeight;
        this.maxHillHeight = maxHillHeight;
    }
    
    public int GetChunkSize()
    {
        return this.chunkSize;
    }
    
    public float GetNoiseScale()
    {
        return this.noiseScale;
    }
    
    public int GetSeed()
    {
        return this.seed;
    }
    
    public int GetBaseHeight()
    {
        return this.baseHeight;
    }
    
    public int GetMinHillHeight()
    {
        return this.minHillHeight;
    }
    
    public int GetMaxHillHeight()
    {
        return this.maxHillHeight;
    }
}
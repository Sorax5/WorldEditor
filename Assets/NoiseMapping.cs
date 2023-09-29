using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoiseMapping : MonoBehaviour
{
    
    [SerializeField] private Texture2D texture2D;
    
    private Dictionary<Color, List<Vector2Int>> _colorDict;

    // Start is called before the first frame update
    void Awake()
    {
        if(texture2D == null)
        {
            Debug.LogError("Texture2D is null");
            return;
        }
        
        
        Color[] pixels = texture2D.GetPixels();
        int width = texture2D.width;
        int height = texture2D.height;

        this._colorDict = new Dictionary<Color, List<Vector2Int>>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[y * width + x];

                if (!_colorDict.ContainsKey(pixelColor))
                {
                    _colorDict[pixelColor] = new List<Vector2Int>();
                }

                _colorDict[pixelColor].Add(new Vector2Int(x, y));
            }
        }
        
        this._colorDict = _colorDict.OrderBy(pair => ComputeLuminance(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
        

        foreach (var kvp in this._colorDict)
        {
            Color color = kvp.Key;
            List<Vector2Int> positions = kvp.Value;

            Debug.Log("Color: " + color + ", Number of Pixels: " + positions.Count);

            foreach (Vector2Int position in positions)
            {
                Debug.Log("Position: (" + position.x + ", " + position.y + ")");
            }
        }
    }

    public Dictionary<Color, List<Vector2Int>> GetColorDict()
    {
        return this._colorDict;
    }
    
    public int GetImageWidth()
    {
        return this.texture2D.width;
    }
    
    public int GetImageHeight()
    {
        return this.texture2D.height;
    }
    
    private int CompareColorByluminance(Color a, Color b)
    {
        float aLuminance = a.r * 0.299f + a.g * 0.587f + a.b * 0.114f;
        float bLuminance = b.r * 0.299f + b.g * 0.587f + b.b * 0.114f;
        return aLuminance.CompareTo(bLuminance);
    }
    
    private float ComputeLuminance(Color color)
    {
        return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessing : MonoBehaviour
{
    
    [SerializeField] private NoiseMapping noiseMapping;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject imagePrefab;

    private float _width;
    private float _height;
    
    private Dictionary<Vector2Int,GameObject> _images = new Dictionary<Vector2Int, GameObject>();

    private float _pixelSize;

    // Start is called before the first frame update
    void Start()
    {
        this._width = this.canvas.GetComponent<RectTransform>().rect.width;
        this._height = this.canvas.GetComponent<RectTransform>().rect.height;

        this._pixelSize = this._width / this.noiseMapping.GetImageWidth();

        Debug.Log(this._pixelSize);
        
        
        StartCoroutine(LoopPlace());
    }

    IEnumerator LoopPlace()
    {
        foreach (var kvp in this.noiseMapping.GetColorDict())
        {
            List<Vector2Int> positions = kvp.Value;

            foreach (Vector2Int position in positions)
            {
                yield return StartCoroutine(PlaceImage(position));
            }
        }
        
        yield return new WaitForSeconds(1f);
        
        // for reverse the dictionary
        
        foreach (var kvp in this.noiseMapping.GetColorDict().Reverse())
        {
            List<Vector2Int> positions = kvp.Value;

            foreach (Vector2Int position in positions)
            {
                GameObject image = this._images[position];
                this._images.Remove(position);
                yield return StartCoroutine(RemoveImage(image));
            }
        }
        
    }


    IEnumerator PlaceImage(Vector2Int pos)
    {
        GameObject imageObject = Instantiate(this.imagePrefab, this.canvas.transform);
        imageObject.name = "Image";
        imageObject.transform.position = new Vector3((pos.x * this._pixelSize) + (this._pixelSize/2), (pos.y * this._pixelSize) + (this._pixelSize/2), 0);
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this._pixelSize, this._pixelSize);
        this._images.Add(pos, imageObject);
        
        yield return null;
    }
    
    IEnumerator RemoveImage(GameObject image)
    {
        Destroy(image);
        
        yield return null;
    }

}

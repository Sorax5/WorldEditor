using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapShadowCaster2D : MonoBehaviour
{
    
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject sc;
    
    private Dictionary<Vector3Int,GameObject> _shadowCasters = new Dictionary<Vector3Int, GameObject>();
    
    public void Start() {
        GenerateShadowCasters();
    }
    
    public void GenerateShadowCasters() {
        int i = 0;
        Debug.Log(tileMap.cellBounds);
        foreach (var position in tileMap.cellBounds.allPositionsWithin) {
            Debug.Log(position);
            if (tileMap.GetTile(position) == null)
                continue;
            
            if (_shadowCasters.ContainsKey(position))
                continue;
         
            GameObject shadowCaster = GameObject.Instantiate(sc, container.transform);
            shadowCaster.transform.position = tileMap.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0);
            shadowCaster.name = "shadow_caster_" + i;
            i++;
            _shadowCasters.Add(position, shadowCaster);
        }
    }
}

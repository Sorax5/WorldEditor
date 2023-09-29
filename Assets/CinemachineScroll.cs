using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CinemachineScroll : MonoBehaviour
{
    
    [FormerlySerializedAs("cameraMovement")] [SerializeField] private PlayerMovementBehavior playerMovementBehavior;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // when scrolling, zoom in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        //max zoom out = 8 & in = 1
        
        if (scroll > 0f)
        {
            // zoom in
            if (this.mainCamera.m_Lens.OrthographicSize > 1)
            {
                this.mainCamera.m_Lens.OrthographicSize -= 1;
            }
        }
        else if (scroll < 0f)
        {
            // zoom out
            if (this.mainCamera.m_Lens.OrthographicSize < 8)
            {
                this.mainCamera.m_Lens.OrthographicSize += 1;
            }
        }

    }
}

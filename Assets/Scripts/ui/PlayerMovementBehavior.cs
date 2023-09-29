using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovementBehavior : MonoBehaviour
{
    
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private CinemachineVirtualCamera mainCamera;

    public event Action<Vector3,Vector3> OnPlayerMoved;
    
    private Vector3 _lastPosition;
    private Coroutine _smoothCameraMove;

    // Start is called before the first frame update
    void Start()
    {
        this._lastPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // drag camera

        Vector3 move = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W))
        {
            move += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += Vector3.right;
        }
        
        // speed by zoom
        move *= this.mainCamera.m_Lens.OrthographicSize / 10;
        Vector3 pos = transform.position += move * speed;

        if (Vector3.Distance(_lastPosition, pos) > 0.1f)
        {
            _lastPosition = pos;
            this.OnPlayerMoved?.Invoke(this._lastPosition, pos);
        }
        
    }
}

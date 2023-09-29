using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    public float zoomSpeed = 1.0f; // Vitesse de zoom, ajustez-la selon vos besoins
    public CinemachineVirtualCamera virtualCamera;
    private float initialOrthographicSize;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Veuillez assigner une caméra virtuelle Cinemachine.");
            enabled = false;
            return;
        }

        // Enregistrez la taille orthographique initiale de la caméra
        initialOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scrollValue * zoomSpeed;

        // Ajustez la taille orthographique pour qu'elle reste un multiple entier de la taille des pixels
        float currentOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        float newOrthographicSize = Mathf.Round(currentOrthographicSize + zoomAmount);

        // Limitez la taille orthographique pour éviter des valeurs non valides
        newOrthographicSize = Mathf.Clamp(newOrthographicSize, 1.0f, float.MaxValue);

        // Appliquez la nouvelle taille orthographique à la caméra
        virtualCamera.m_Lens.OrthographicSize = newOrthographicSize;
    }

    public void ResetZoom()
    {
        // Réinitialisez le zoom à la valeur initiale
        virtualCamera.m_Lens.OrthographicSize = initialOrthographicSize;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraZoom : MonoBehaviour
{

    private Camera cam;
    private float desiredZoom;
    private float targetZoom;
    private float zoomFactor = 3f;
    private float zoomLerpSpeed = 10;

    float scrollData;

    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        desiredZoom = 5;
    }

    void Update()
    {
        scrollData = 0;
        if (targetZoom > desiredZoom)
            scrollData = .1f;
        else if (targetZoom < desiredZoom)
            scrollData = -.1f;

        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 3f, 5f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }

    public void SetDesiredZoom(float tZoom)
    {
        desiredZoom = tZoom;
    }
}

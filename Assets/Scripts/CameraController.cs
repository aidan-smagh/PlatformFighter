using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CameraController : MonoBehaviour
{

    [Header("Targets")]
    public List<Transform> fighters;

    [Header("Zoom")]
    public float baseZoom = 5f;
    public float minZoom = 3f;
    public float maxZoom = 20f;
    public float paddingX = 10f;
    public float paddingY = 8f;

    [Header("Smoothing")]
    public float panSpeed = 4f;
    public float zoomInSpeed = 2f;
    public float zoomOutSpeed = 6f;

    [Header("Stage Bounds")]
    public Bounds stageBounds;

    Camera cam;

    public void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Start()
    {
        GameObject[] fighterObjs = GameObject.FindGameObjectsWithTag("Fighter");
        fighters = fighterObjs.Select(p => p.transform).ToList();
    }

    public void LateUpdate()
    {
        if (fighters == null || fighters.Count == 0) return;

        //bounding box of players
        Vector3 min = fighters[0].position;
        Vector3 max = fighters[0].position;
        foreach (var f in fighters)
        {
            min = Vector3.Min(min, f.position);
            max = Vector3.Max(max, f.position);
        }

        //target center and zoom
        Vector3 targetCenter = (min + max) / 2f;
        float spreadX = (max.x - min.x) + paddingX;
        float spreadY = (max.y - min.y) + paddingY;

        float aspect = cam.aspect;
        float zoomForWidth = spreadX / (2f * aspect);
        float zoomForHeight = spreadY / 2f;
        float targetZoom = Mathf.Clamp(Mathf.Max(zoomForWidth, zoomForHeight), minZoom, maxZoom);


        //smooth
        bool zoomingOut = targetZoom > cam.orthographicSize;
        float zSpeed = zoomingOut ? zoomOutSpeed : zoomInSpeed;

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(targetCenter.x, targetCenter.y, transform.position.z),
            panSpeed * Time.deltaTime
        );

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zSpeed * Time.deltaTime);

        ClampToStage();
    }

    void ClampToStage()
    {
        float verticalExtent = cam.orthographicSize;
        float horizontalExtent = verticalExtent * cam.aspect;

        float minX = stageBounds.min.x + horizontalExtent;
        float maxX = stageBounds.max.x - horizontalExtent;
        float minY = stageBounds.min.y + verticalExtent;
        float maxY = stageBounds.max.y - verticalExtent;

        Vector3 pos = transform.position;
        if (minX <= maxX) pos.x = Mathf.Clamp(pos.x, minX, maxX);
        if (minY <= maxY) pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
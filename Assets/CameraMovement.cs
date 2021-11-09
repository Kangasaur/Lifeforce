using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public BoxCollider2D worldBounds;

    float xMin, xMax, yMin, yMax;
    float camY, camX, camSize, camRatio;
    float camMoveX, camMoveY, camZoom;

    Camera mainCam;

    public float speed, zoomSpeed;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GetComponent<Camera>();
        xMin = worldBounds.bounds.min.x;
        xMax = worldBounds.bounds.max.x;
        yMin = worldBounds.bounds.min.y;
        yMax = worldBounds.bounds.max.y;

        camSize = mainCam.orthographicSize;
        camRatio = 8.0f / 5.0f;
    }

    private void Update()
    {
        camMoveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        camMoveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        camZoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        mainCam.orthographicSize -= camZoom;
        mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, 1f, 8f);
        camSize = mainCam.orthographicSize;

        Vector3 movePos = new Vector3(transform.position.x + camMoveX, transform.position.y + camMoveY, -10f);
        movePos.x = Mathf.Clamp(movePos.x, xMin + (camSize * camRatio), xMax - (camSize * camRatio));
        movePos.y = Mathf.Clamp(movePos.y, yMin + camSize, yMax - camSize);

        transform.position = movePos;
    }
}

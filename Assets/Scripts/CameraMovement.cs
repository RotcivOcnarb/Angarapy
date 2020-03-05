using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraMovement : MonoBehaviour
{

    public GameObject whoToFollow;
    public float followFactor = 5;

    public Camera[] otherCameras;
    public GameObject[] planes;

    public SpriteRenderer bgRenderer;

    public Rect boundaries;
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        float z = transform.position.z;
        transform.position = new Vector3(whoToFollow.transform.position.x, whoToFollow.transform.position.y, z);

        mainCamera.orthographicSize = 7;
        foreach (Camera c in otherCameras) {
            c.orthographicSize = 7;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float targetSize = 7;

        if (Input.GetButton("ZoomOut")) {
            targetSize = 13;
        }

        mainCamera.orthographicSize += (targetSize - mainCamera.orthographicSize) / 5f;
        foreach (Camera c in otherCameras) {
            c.orthographicSize += (targetSize - c.orthographicSize) / 5f;
        }

        Vector3 tp = transform.position;
        tp += (whoToFollow.transform.position - tp) / followFactor;
        tp.z = -10;
        transform.position = tp;

        //Locka a camera nos boundaries
        if (transform.position.x < boundaries.position.x - boundaries.width / 2f + mainCamera.orthographicSize * mainCamera.aspect) {
            transform.position = new Vector3(
                boundaries.position.x - boundaries.width / 2f + mainCamera.orthographicSize * mainCamera.aspect,
                transform.position.y,
                transform.position.z);
        }
        if (transform.position.x > boundaries.position.x + boundaries.width / 2f - mainCamera.orthographicSize * mainCamera.aspect) {
            transform.position = new Vector3(
                boundaries.position.x + boundaries.width / 2f - mainCamera.orthographicSize * mainCamera.aspect,
                transform.position.y,
                transform.position.z);
        }
        if (transform.position.y < boundaries.position.y - boundaries.height / 2f + mainCamera.orthographicSize) {
            transform.position = new Vector3(
                transform.position.x,
                boundaries.position.y - boundaries.height / 2f + mainCamera.orthographicSize,
                transform.position.z);
        }
        if (transform.position.y > boundaries.position.y + boundaries.height / 2f - mainCamera.orthographicSize) {
            transform.position = new Vector3(
                transform.position.x,
                boundaries.position.y + boundaries.height / 2f - mainCamera.orthographicSize,
                transform.position.z);
        }

        foreach(GameObject pl in planes){
            pl.transform.localScale = new Vector3(
                mainCamera.orthographicSize / 5f * (16f/9f),
                1,
                mainCamera.orthographicSize / 5f
            );
        }

        Texture texture = bgRenderer.sprite.texture;
        
        bgRenderer.gameObject.transform.localScale = new Vector3(
            mainCamera.aspect * mainCamera.orthographicSize * 2 / (texture.width / bgRenderer.sprite.pixelsPerUnit),
            mainCamera.orthographicSize * 2 / (texture.height / bgRenderer.sprite.pixelsPerUnit),
            1
        );
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(boundaries.position + boundaries.size * new Vector2(-.5f, -.5f), boundaries.position + boundaries.size * new Vector2(.5f, -.5f));
        Gizmos.DrawLine(boundaries.position + boundaries.size * new Vector2(.5f, -.5f), boundaries.position + boundaries.size * new Vector2(.5f, .5f));
        Gizmos.DrawLine(boundaries.position + boundaries.size * new Vector2(.5f, .5f), boundaries.position + boundaries.size * new Vector2(-.5f, .5f));
        Gizmos.DrawLine(boundaries.position + boundaries.size * new Vector2(-.5f, .5f), boundaries.position + boundaries.size * new Vector2(-.5f, -.5f));



    }
}

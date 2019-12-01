using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public GameObject whoToFollow;
    public float followFactor = 5;

    public Camera[] otherCameras;
    public GameObject[] planes;

    public SpriteRenderer bgRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tp = transform.position;
        tp += (whoToFollow.transform.position - tp) / followFactor;
        tp.z = -10;
        transform.position = tp;

        float targetSize = 7;

        if(Input.GetKey(KeyCode.LeftShift)){
            targetSize = 13;
        }

        Camera mainCamera = GetComponent<Camera>();

        mainCamera.orthographicSize += (targetSize - mainCamera.orthographicSize) / 5f;
        foreach(Camera c in otherCameras){
            c.orthographicSize += (targetSize - c.orthographicSize) / 5f;
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

    
}

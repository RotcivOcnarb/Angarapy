using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AdaptBGToCamera : MonoBehaviour
{
    // Start is called before the first frame update
    Camera mainCamera;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        mainCamera = transform.parent.gameObject.GetComponent<Camera>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

}

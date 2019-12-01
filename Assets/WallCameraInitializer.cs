using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCameraInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    public MeshRenderer wallRenderer;
    public RenderTexture toCopyFormat;
    void Start()
    {
        //wallRenderer.gameObject.transform.localScale = new Vector3(Screen.width/(float)Screen.height, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KaduTrail : MonoBehaviour
{

    public float frequency = 1;
    public float speed = 1;
    public Camera kaduCamera;
    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * speed;
        if(timer > frequency){
            timer = 0;
            //Spawna o trail
            GameObject trail = new GameObject();
            SpriteRenderer sr = trail.AddComponent<SpriteRenderer>();
            kaduCamera.Render();

            Debug.Log(kaduCamera.targetTexture.format);
            
            Texture2D myTexture2D = 
            
            new Texture2D(
                1280,
                720,
                UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB,
                UnityEngine.Experimental.Rendering.TextureCreationFlags.None
            );

            Graphics.CopyTexture(kaduCamera.targetTexture, myTexture2D);
            sr.sprite = Sprite.Create(myTexture2D, new Rect(0, 0, 1280, 720), new Vector2(0.5f, 0.5f), 100);

            trail.transform.position = transform.position;
            trail.transform.localScale = new Vector3(.4f, .4f, 1f);
            trail.transform.rotation = transform.rotation;
            trail.AddComponent<TrailDissipator>();
            
        }
    }
}

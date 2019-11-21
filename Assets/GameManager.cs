using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Text fpsText;
    public Text velocityText;
    public GameObject player;

    float updateTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer += Time.unscaledDeltaTime;

        if(updateTimer > 0.5f){
            updateTimer = 0;
            fpsText.text = (int)(1 / Time.unscaledDeltaTime) + "";
        }

        velocityText.text = player.GetComponent<Rigidbody2D>().velocity + "";
        
    }
}

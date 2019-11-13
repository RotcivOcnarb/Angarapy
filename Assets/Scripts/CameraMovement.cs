using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public GameObject whoToFollow;
    public float followFactor = 5;
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

    }
}

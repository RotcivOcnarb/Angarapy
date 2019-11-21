using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    Vector3 positionDif;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = transform.parent.gameObject;
        positionDif = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = transform.parent.gameObject;

        transform.position = player.transform.position + positionDif;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}

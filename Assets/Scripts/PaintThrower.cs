using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintThrower : MonoBehaviour
{
    // Start is called before the first frame update
    public float throwForce;
    public GameObject paintPrefab;
    public Camera mainCamera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButton(0)){

            //Calcula a direção do tiro
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - gameObject.transform.position).normalized;
            direction = Rotate(direction, Random.Range(-10, 10));

            //Cria o objeto tinta
            GameObject paint = Instantiate(paintPrefab, gameObject.transform.position, paintPrefab.transform.rotation);
            Rigidbody2D paintBody = paint.GetComponent<Rigidbody2D>();
            SpriteRenderer sr = paint.GetComponentInChildren<SpriteRenderer>();

            //Define propriedades
            paint.transform.localScale *= 0.3f; //Tamanho
            paintBody.velocity = direction * throwForce; // Velocidade
            sr.color = Color.HSVToRGB((Time.fixedTime*360 % 360) / 360f, 1f, 1f);//Cor

        }
        
    }

    public Vector2 Rotate(Vector2 v, float degrees) {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);
         return v;
     }
}

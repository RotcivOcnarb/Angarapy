using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

public class PaintThrower : MonoBehaviour
{
    // Start is called before the first frame update
    public float throwForce;
    public int paintQuantity = 1;
    public float shootOpening = 10;
    public GameObject paintPrefab;
    public Camera mainCamera;
    public CCDSolver2D solver;
    public IKManager2D ikManager;

    public GameObject throwPoint;
    public GameObject elbow;

    public GameObject ikTarget;

    AudioSource audioSource;

    public RandomSplashPlayer splash;

    float audioVolume = 0;
    float targetVolume = 0;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        ikTarget.transform.position = mouseWorldPos;

        if(GetComponent<SpriteRenderer>().flipX)
                ikTarget.transform.localPosition = new Vector3(
                    -ikTarget.transform.localPosition.x,
                    ikTarget.transform.localPosition.y,
                    ikTarget.transform.position.z);

        Vector2 diff = ikTarget.transform.position - elbow.transform.position;
        ikTarget.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);

        ikManager.enabled = Input.GetMouseButton(0);

        solver.weight += ((Input.GetMouseButton(0) ? 1 : 0) - solver.weight) / 5f;

        if (Input.GetMouseButton(0)){
            //Calcula a direção do tiro

            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direction = mousePos - (Vector2)throwPoint.transform.position;//new Vector2(Mathf.Cos(throwPoint.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(throwPoint.transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
            direction.Normalize();
            direction = Rotate(direction, Random.Range(-shootOpening, shootOpening));

            //if(GetComponent<SpriteRenderer>().flipX)
               // direction.x *= -1;

            for(int i = 0; i < paintQuantity; i ++){
                //Cria o objeto tinta
                GameObject paint = Instantiate(paintPrefab, throwPoint.transform.position, paintPrefab.transform.rotation);

                Rigidbody2D paintBody = paint.GetComponent<Rigidbody2D>();
                //SpriteRenderer sr = paint.GetComponent<SpriteRenderer>();

                paint.GetComponent<NewMetaballSingle>().splash = splash;

                Vector3 position = throwPoint.transform.position;
                
                //Define propriedades
                paintBody.velocity = direction * throwForce; // Velocidade

                if(GetComponent<SpriteRenderer>().flipX){
                    float dif = position.x - gameObject.transform.position.x;
                    position = new Vector3(gameObject.transform.position.x - dif, position.y, position.z);
                }
                   
                position += new Vector3(direction.x, direction.y, 0) * Random.Range(0f, 1f);
                paint.transform.position = position;
                
                //sr.color = addColor(new Color(100/255f, 160/255f, 255/255f, 0.5f), Random.onUnitSphere*0.3f);
                //sr.color = Color.HSVToRGB((Time.fixedTime*360 % 360) / 360f, 1f, 1f);//Cor
            }

            targetVolume = 1;
        }
        else
            targetVolume = 0;

        audioVolume += (targetVolume - audioVolume) / 5f;

        audioSource.volume = audioVolume;
        
    }

    Color addColor(Color c1, Vector3 val){
        return new Color(c1.r + val.x, c1.g + val.y, + c1.b + val.z, c1.a);
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

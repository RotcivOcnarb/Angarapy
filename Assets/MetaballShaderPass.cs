using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[ExecuteInEditMode]
public class MetaballShaderPass : MonoBehaviour
{
    public string tagToRender;
    MeshRenderer meshRenderer;
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        List<GameObject> gos = GameObject.FindGameObjectsWithTag(tagToRender).ToList();

        while(gos.Count >= 1023){
            Destroy(gos[gos.Count-1]);
            gos.RemoveAt(gos.Count-1);
        }

        List<Vector4> positions = gos.Select( (go) => {
            return new Vector4(
                go.transform.position.x,
                go.transform.position.y,
                go.transform.position.z,
                0);
        }).ToList();

        while(positions.Count < 1023){
            positions.Add(new Vector4());
        }

        List<Vector4> colors = gos.Select( (go) => {
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            return new Vector4(
                sr.color.r,
                sr.color.g,
                sr.color.b,
                sr.color.a);
        }).ToList();

        while(colors.Count < 1023){
            colors.Add(new Vector4());
        }

        meshRenderer.sharedMaterial.SetVectorArray("_Metaballs", positions);
        meshRenderer.sharedMaterial.SetVectorArray("_MetaColors", colors);
        meshRenderer.sharedMaterial.SetFloat("_MetaballsSize", positions.Count);

        meshRenderer.sharedMaterial.SetVector("viewport", new Vector2(mainCamera.orthographicSize * mainCamera.aspect * 2, mainCamera.orthographicSize * 2));
        meshRenderer.sharedMaterial.SetVector("cameraPosition", mainCamera.transform.position);
        meshRenderer.sharedMaterial.SetFloat("cameraZoom", mainCamera.transform.localScale.x);

    }
}

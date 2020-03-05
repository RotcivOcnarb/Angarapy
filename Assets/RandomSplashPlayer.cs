using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSplashPlayer : MonoBehaviour
{

    public AudioClip[] audios;

    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Rnd(int size){
        return Random.Range(0, size);
    }

    public void PlayRandomSplash(float volume){
        source.PlayOneShot(audios[Rnd(audios.Length)], volume);
    }
}

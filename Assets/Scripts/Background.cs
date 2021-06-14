using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{

    // Update is called once per frame
    public Texture[] frames;
    int framesPerSecond = 10;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
 
    void Update() {
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        rend.material.mainTexture = frames[index];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unmute : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnmuteMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UnmuteMusic()
    {
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.UnmuteMusic();
        }
    }
}

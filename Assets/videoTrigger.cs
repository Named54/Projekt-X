using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoTrigger : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    private bool destroyed = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Players")
        {
            videoPlayer.Play();
            if (destroyed)
            {
                Destroy(gameObject);
            }
        }
    }
}

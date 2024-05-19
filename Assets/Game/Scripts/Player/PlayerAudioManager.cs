using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSFX;
    [SerializeField] private AudioSource glideSFX;
    [SerializeField] private AudioSource landingSFX;
    [SerializeField] private AudioSource punchSFX;
    private void PlayFootstepSFX(){
        footstepSFX.volume = Random.Range(0.7f,1f);
        footstepSFX.pitch = Random.Range(0.5f,2.5f);
        footstepSFX.Play();
    }

    public void PlayGlideSFX(){
        glideSFX.Play();
    }

    public void StopGlideSFX(){
        glideSFX.Stop();
    }

    private void PlayLandingSFX(){
        landingSFX.Play();
    }
    private void PlayPunchSFX(){
        punchSFX.volume = Random.Range(0.7f,1f);
        punchSFX.pitch = Random.Range(0.5f,2.5f);
        punchSFX.Play();
    }
}

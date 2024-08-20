using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource m_Source;
    [SerializeField] AudioSource m_SFX;

    public AudioClip BGM;
    public AudioClip SFX;

    private void Start()
    {
        m_Source.clip = BGM;
        m_Source.Play();
    }

    public void playSFX()
    {
        Debug.Log("Playing SFX");
        m_SFX.clip = SFX;
        m_SFX.Play();
    }

}
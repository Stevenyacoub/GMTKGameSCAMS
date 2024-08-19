using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource m_Source;

    public AudioClip BGM;

    private void Start()
    {
        m_Source.clip = BGM;
        m_Source.Play();
    }

}
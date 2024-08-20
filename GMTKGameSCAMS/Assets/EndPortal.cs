using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{ 
    public float pulseSpeed = 1.0f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    [SerializeField] public AudioManager sfxManager;

    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            sfxManager.playSFX();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void Update()
    {
        // Calculate the scale factor using a sine wave to oscillate between minScale and maxScale
        float scale = minScale + (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2 * (maxScale - minScale);
        transform.localScale = originalScale * scale;
    }
}

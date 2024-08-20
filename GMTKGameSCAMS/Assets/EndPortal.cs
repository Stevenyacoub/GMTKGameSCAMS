using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{ 
    public float pulseSpeed = 1.0f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Loading Scene Entitled '" + SceneManager.GetActiveScene().buildIndex + 1 + "'");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void Update()
    {
        // Calculate the scale factor using a sine wave to oscillate between minScale and maxScale
        float scale = minScale + (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2 * (maxScale - minScale);
        Debug.Log("scale: " + scale);
        transform.localScale = originalScale * scale;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Engine))]
public class EngineSound : MonoBehaviour
{
    public Transform engineTransform;


    public AudioClip clip;


    public float pitchFactor;
    public float iddlePitch;


    private AudioSource audioSource;
    private Engine engine;
    void Awake()
    {
        if(!clip || !engineTransform)
        {
            return;
        }
        audioSource = engineTransform.gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.clip = clip;
        audioSource.Play();

        engine = GetComponent<Engine>();
    }

    void Update()
    {
        if (!clip || !engineTransform)
        {
            return;
        }
        if (engine.IsOn())
        {
            audioSource.volume = Mathf.Lerp(0.1f, 1.0f, engine.GetPower());
            audioSource.pitch = (engine.GetPower()) * pitchFactor + iddlePitch;
        }
        else
        {
            audioSource.volume = 0;
        }
    }
}

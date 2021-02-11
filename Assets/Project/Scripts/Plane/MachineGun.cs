using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    public Transform muzzleTransform;
    public GameObject projectilePrefab;

    public Transform aimPoint;

    [Header("Stats")]
    public float reloadTime = 0.3f;
    public int ammoCount = 500;
    [Space]


    public AudioClip soundEffect;
    AudioSource audioSource;

    private float reloadTimer = 0.0f;
    private bool loaded = false;

    private Airplane airplane;

    void Awake()
    {
        if (muzzleTransform && aimPoint)
        {
            muzzleTransform.LookAt(aimPoint);
        }

        airplane = GetComponent<Airplane>();


        if (muzzleTransform && soundEffect)
        {
            audioSource = muzzleTransform.gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
            audioSource.clip = soundEffect;
        }
    }


    void Update()
    {
        if (!loaded)
        {
            reloadTimer -= Time.deltaTime;
        }
        if (reloadTimer <= 0)
        {
            loaded = true;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Fire();

            if (audioSource && ammoCount > 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            
        }
        else
        {
            if (audioSource || ammoCount < 1)
            {
                audioSource.Stop();
            }
        }

        if(ammoCount < 1)
        {
            audioSource.Stop();
        }
    }



    public void Fire()
    {
        if(muzzleTransform && projectilePrefab && ammoCount > 0)
        {
            if (loaded)
            {
                GameObject cloneProj = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
                reloadTimer = reloadTime;
                ammoCount--;
                loaded = false;
                
            }
        }
    }

}

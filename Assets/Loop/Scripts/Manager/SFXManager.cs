using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SFXManager : MonoBehaviourSingletonPersistent<SFXManager>
{
    [SerializeField] AudioSource aS;
    [SerializeField] float ambientVolume;


    [SerializeField] AudioSource sfxAS;

    [SerializeField] AudioSource musicAS;
    [SerializeField] List<MusicSettings> musics;

    public override void Awake()
    {
        base.Awake();
        aS.volume = 0f;
        aS.DOFade(ambientVolume, 1f);

        musicAS.loop = false;

        StartCoroutine(C_MusicLooper());

        IEnumerator C_MusicLooper()
        {
            while(true)
            {
                while (musicAS.isPlaying)
                    yield return null;
                var m = musics.RandomItem();
                musicAS.clip = m.clip;
                musicAS.volume = m.volume;
                musicAS.Play();
            }
        }
    }

    public void PlaySFX(AudioClip clip,float volume = 1f)
    {
        sfxAS.PlayOneShot(clip, volume);
    }

    [System.Serializable]
    public struct MusicSettings
    {
        public AudioClip clip;
        public float volume;
    }
}

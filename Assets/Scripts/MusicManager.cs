using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : ScheduledScript
{
    public  Object[] music;

    public AudioSource audioSource;

    private void Awake()
    {
        music = Resources.LoadAll<Object>("Music");

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayRandomTrack();
    }

    private void LateUpdate()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        audioSource.clip = music[Random.Range(0, music.Length)] as AudioClip;
        audioSource.Play();
    }
}

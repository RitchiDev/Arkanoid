using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager m_Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Image m_MuteButton;
    [SerializeField] private Sprite m_MusicOn, m_MusicOff;

    [Header("Music")]
    [SerializeField] private List<AudioClip> m_Songs = new List<AudioClip>();
    private Dictionary<string, Queue<AudioClip>> m_SongDictionary;
    private AudioSource m_AudioSource;
    private AudioClip m_SongToPlay;
    private float m_DefaultVolume;
    private float m_SongNumber;

    [Header("Nothing To See Here")]
    [SerializeField] private GameObject m_Secret;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != null)
        {
            Destroy(this);
        }

        m_AudioSource = GetComponent<AudioSource>();
        m_DefaultVolume = m_AudioSource.volume;

        m_SongDictionary = new Dictionary<string, Queue<AudioClip>>();

        Queue<AudioClip> songToAdd = new Queue<AudioClip>();
        for (int i = 0; i < m_Songs.Count; i++)
        {
            songToAdd.Enqueue(m_Songs[i]);
        }

        m_SongDictionary.Add("Song", songToAdd);
    }

    private void Start()
    {
        m_Secret.SetActive(false);
        m_SongNumber = 0;
        NextSong();
    }

    public void NextSong() //Wordt ook als button gebruikt
    {
        m_AudioSource.Stop();
        m_SongToPlay = m_SongDictionary["Song"].Dequeue();
        m_SongDictionary["Song"].Enqueue(m_SongToPlay);
        m_AudioSource.clip = m_SongToPlay;

        CheckForSecret();
        
        m_AudioSource.Play();
        m_MuteButton.sprite = m_MusicOn;
    }

    private void CheckForSecret()
    {
        m_SongNumber++;
        if (m_SongNumber == m_Songs.Count)
        {
            m_Secret.SetActive(true);
            m_SongNumber = 0;
        }
        else
        {
            m_Secret.SetActive(false);
        }
    }

    public void Mute()
    {
        if(m_AudioSource.isPlaying)
        {
            m_AudioSource.Pause();
            m_MuteButton.sprite = m_MusicOff;
        }
        else
        {
            m_AudioSource.Play();
            m_MuteButton.sprite = m_MusicOn;
        }
    }

    public void SetVolume()
    {
        if(Time.timeScale > 0)
        {
            m_AudioSource.volume = m_DefaultVolume;
        }
        else
        {
            m_AudioSource.volume = m_DefaultVolume * 0.2f;
        }
    }
}

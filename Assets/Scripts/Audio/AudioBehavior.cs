using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioBehavior : MonoBehaviour
{
    public static AudioBehavior Instance;

    [Header("Loop Sources")]
    [SerializeField] private AudioSource _loopSourceA;
    [SerializeField] private AudioSource _loopSourceB;

    [Header("MX Clips")]
    [SerializeField] private AudioClip _mxPlanetClip;
    [SerializeField] private AudioClip _mxHubClip;
    
    [Header("AMB Clips")]
    [SerializeField] private AudioClip _ambMachineryClip;
    [SerializeField] private AudioClip _ambRoomClip;

    [Header("Loop Source Pool")]
    [SerializeField] private Transform _loopSourcePoolRoot;

    private readonly List<AudioSource> _availableLoopSources = new List<AudioSource>();
    private readonly List<AudioSource> _allLoopSources = new List<AudioSource>();

    private AudioStateMachine _audioStateMachine;
    
    private IAudioState _hubState;
    private IAudioState _planetState;

    public AudioMixerGroup audioMixerGroup;
    [SerializeField] private AudioMixerGroup SFXMixer;
    

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_loopSourcePoolRoot == null)
        {
            GameObject poolRoot = new GameObject("Loop Source Pool");
            poolRoot.transform.SetParent(transform);
            _loopSourcePoolRoot = poolRoot.transform;
        }

        SceneManager.sceneLoaded += OnSceneLoadedAudio;
        SceneManager.sceneUnloaded += OnSceneUnloadedAudio;
        _audioStateMachine = new AudioStateMachine();
        _hubState = new AudioHubState(this, _mxHubClip, _ambMachineryClip, _ambRoomClip);
        _planetState = new AudioPlanetState(this);
    }
    

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedAudio;
        SceneManager.sceneUnloaded -= OnSceneUnloadedAudio;
        Instance = null;
    }

    private void Update()
    {
        _audioStateMachine.Update();
    }
    
    // State change methods
    public void SetHubState() => _audioStateMachine.ChangeState(_hubState);
    public void SetPlanetState() => _audioStateMachine.ChangeState(_planetState);
    
    public AudioLoopPlayer StartLoop(AudioClip clip, double loopDuration)
    {
        AudioLoopPlayer loopPlayer = new AudioLoopPlayer(this);
        loopPlayer.StartLoop(clip, loopDuration);
        return loopPlayer;
    }

    public AudioSource GetPooledLoopSource()
    {
        CleanupNullLoopSources();

        AudioSource source = null;

        if (_availableLoopSources.Count > 0)
        {
            int lastIndex = _availableLoopSources.Count - 1;
            source = _availableLoopSources[lastIndex];
            _availableLoopSources.RemoveAt(lastIndex);
        }
        else
        {
            GameObject sourceObject = new GameObject("Loop Audio Source");
            sourceObject.transform.SetParent(_loopSourcePoolRoot);

            source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
        }

        if (!_allLoopSources.Contains(source))
        {
            _allLoopSources.Add(source);
        }

        source.gameObject.SetActive(true);
        return source;
    }

    public void ReleaseLoopSource(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.clip = null;

        if (!_availableLoopSources.Contains(source))
        {
            _availableLoopSources.Add(source);
        }

        source.gameObject.SetActive(false);
    }

    public void ReleaseAllLoopSources()
    {
        CleanupNullLoopSources();

        for (int i = 0; i < _allLoopSources.Count; i++)
        {
            ReleaseLoopSource(_allLoopSources[i]);
        }
    }

    private void CleanupNullLoopSources()
    {
        for (int i = _availableLoopSources.Count - 1; i >= 0; i--)
        {
            if (_availableLoopSources[i] == null)
            {
                _availableLoopSources.RemoveAt(i);
            }
        }

        for (int i = _allLoopSources.Count - 1; i >= 0; i--)
        {
            if (_allLoopSources[i] == null)
            {
                _allLoopSources.RemoveAt(i);
            }
        }
    }
    
    public void FadeMixerGroup(AudioMixerGroup mixerGroup, float targetVolume, float duration) => StartCoroutine(Fade(mixerGroup, 0, targetVolume, duration));
    
    private IEnumerator Fade(AudioMixerGroup mixerGroup, int groupNumberInArray, float targetVolume, float duration)
    {
        double elapsedTime = 0;
        double oldTime = AudioSettings.dspTime;

        float startVolume;
        
        mixerGroup.audioMixer.GetFloat(("volume" + groupNumberInArray), out startVolume);

        while (elapsedTime < duration)
        {
            mixerGroup.audioMixer.SetFloat(("volume" + groupNumberInArray),
                Mathf.Lerp(startVolume, targetVolume, (float)elapsedTime / duration));
            
            elapsedTime += AudioSettings.dspTime - oldTime;
            oldTime = AudioSettings.dspTime;

            yield return null;
        }
        
        mixerGroup.audioMixer.SetFloat(("volume" + groupNumberInArray), targetVolume);
    }

    private void OnSceneLoadedAudio(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "NewMainLobby":
                SetHubState();
                break;
            default:
                Debug.Log("Update AudioBehavior: Scene not found");
                break;
        }
    }

    private void OnSceneUnloadedAudio(Scene scene)
    {
        switch (scene.name)
        {
            default:
                Debug.Log("Update AudioBehavior: Scene not found");
                break;
        }
    }
}
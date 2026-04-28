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
    [SerializeField] private AudioClip _mxMoveClip;
    [SerializeField] private AudioClip _mxTerrainUpClip;
    [SerializeField] private AudioClip _mxTerrainDownClip;
    
    [Header("AMB Clips")]
    [SerializeField] private AudioClip _ambMachineryClip;
    [SerializeField] private AudioClip _ambRoomClip;
    [SerializeField] private AudioClip _ambWindClip;

    [Header("Loop Source Pool")]
    [SerializeField] private Transform _loopSourcePoolRoot;

    private readonly List<AudioSource> _availableLoopSources = new List<AudioSource>();
    private readonly List<AudioSource> _allLoopSources = new List<AudioSource>();

    private AudioStateMachine _audioStateMachine;
    
    private IAudioState _hubState;
    private IAudioState _planetState;

    public AudioMixer audioMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup mxMixer;
    public AudioMixerGroup ambMixer;
    public AudioMixerGroup hubMixer;
    public AudioMixerGroup planetMixer;
    public AudioMixerGroup moveMixer;
    public AudioMixerGroup terrainUpMixer;
    public AudioMixerGroup terrainDownMixer;
    
    private Dictionary<AudioMixerGroup, string> _mixGroupVolumeParameters;
    
    public AudioMixerSnapshot playSnapshot;
    public AudioMixerSnapshot pauseSnapshot;
    private AudioMixerSnapshot _currentSnapshot;
    
    private PlayerUIFunction ui;
    
    public readonly float MinVolume = -80.0f;
    public readonly float MaxVolume = 0.00f;
    
    private readonly List<AudioLoopPlayer> _loopPlayers = new List<AudioLoopPlayer>(3);
    
    public float PlayerVelocity = 0;
    private float smoothedVelocity;
    [SerializeField] private float smoothMultiplier = 10f;
    [SerializeField] private float maxVelocity = 30f;
    [SerializeField] private AnimationCurve moveMixerCurve;

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
        //SceneManager.sceneUnloaded += OnSceneUnloadedAudio;
        
        _audioStateMachine = new AudioStateMachine();
        _hubState = new AudioHubState(this, _mxHubClip, _ambMachineryClip, _ambRoomClip);
        _planetState = new AudioPlanetState(this, _ambWindClip );
        
        _mixGroupVolumeParameters = new Dictionary<AudioMixerGroup, string>()
        {
            { hubMixer, "hubVolume"},
            { planetMixer, "planetVolume"},
            { moveMixer, "moveVolume"},
            { terrainUpMixer, "terrainUpVolume"},
            { terrainDownMixer, "terrainDownVolume"}
        };  
    }
    
    void Start()
    {
        playSnapshot.TransitionTo(1);
        _currentSnapshot = playSnapshot;
        _loopPlayers.Add(StartLoop(_mxHubClip, 160, 5));
        _loopPlayers.Add(StartLoop(_mxPlanetClip, 160, 5));
        _loopPlayers.Add(StartLoop(_mxMoveClip, 160, 5));
        //_loopPlayers.Add(StartLoop(_mxTerrainUpClip, 160, 5));
        //_loopPlayers.Add(StartLoop(_mxTerrainDownClip, 160, 5));
    }
    
    private void Update()
    {
        _audioStateMachine.Update();

        if (ui == null)
        {
            ui = FindFirstObjectByType(typeof(PlayerUIFunction)) as PlayerUIFunction;
            if (ui == null) return;
            ui.OnPause += Pause;
            ui.OnResume += Resume;
            ui.OnDestroyed += DestroyPauseUI;
        }

        foreach (var loopPlayer in _loopPlayers)
        {
            loopPlayer?.UpdateLoop();
        }
    }
    
    // called during update in PlanetState to change the mix based on player velocity
    public void OnVelocityMoveMixers()
    {
        smoothedVelocity = Mathf.Lerp(smoothedVelocity, PlayerVelocity, Time.deltaTime * smoothMultiplier);
        
        float t = moveMixerCurve.Evaluate(Mathf.Clamp01(smoothedVelocity / maxVelocity));
        
        float moveMixerVolume = Mathf.Lerp(MinVolume, MaxVolume, t);
        moveMixer.audioMixer.SetFloat(_mixGroupVolumeParameters[moveMixer], moveMixerVolume);
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedAudio;
        //SceneManager.sceneUnloaded -= OnSceneUnloadedAudio;
        ReleaseAllLoopSources();
        Instance = null;
    }
    
        //// PAUSE/RESUME METHODS ////
    
    private void Pause()
    {
        _audioStateMachine.Pause();
    }
    private void Resume()
    {
        _audioStateMachine.Resume();
    }

    private void DestroyPauseUI()
    {
        ui.OnPause -= Pause;
        ui.OnResume -= Resume;
        ui.OnDestroyed -= DestroyPauseUI;
        ui = null;
    }
    
                //// STATE CHANGE METHODS ////
    
    
    public void SetHubState() => _audioStateMachine.ChangeState(_hubState);
    public void SetPlanetState() => _audioStateMachine.ChangeState(_planetState);
    
    public AudioLoopPlayer StartLoop(AudioClip clip, double loopDuration, double fadeDuration = 0)
    {
        AudioLoopPlayer loopPlayer = new AudioLoopPlayer(this);
        loopPlayer.StartLoop(clip, loopDuration, fadeDuration);
        return loopPlayer;
    }
    
                //// LOOP PLAYERS ////

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

    // public method to stop a source with the option to fade
    public void StopLoop(AudioSource source, double fadeDuration = 0)
    {
        if (source == null)
            return;
        
        if (fadeDuration > 0)
        {
            StartCoroutine(FadeOutCoroutine(source, fadeDuration));
            return;
        }
        ReleaseLoopSource(source);
    }
    
    // stops a source and return it to the pool
    private void ReleaseLoopSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;

        if (!_availableLoopSources.Contains(source))
        {
            _availableLoopSources.Add(source);
        }

        source.gameObject.SetActive(false);
    }

    // waits till the fade routine is done before releasing the loop source
    private IEnumerator FadeOutCoroutine(AudioSource source, double fadeDuration)
    {
        yield return FadeLoopSourceCoroutine(0, fadeDuration, source);
        ReleaseLoopSource(source);
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
    
            ///// LOOP SOURCE MIXING ////////////
    
    // public method to start a fade
    public void FadeLoopSource(AudioSource source, float targetVolume, double duration) => StartCoroutine(FadeLoopSourceCoroutine(targetVolume, duration, source));

    // the actual fade logic
    private IEnumerator FadeLoopSourceCoroutine(float targetVolume, double duration, AudioSource source)
    {
        double oldDspTime = AudioSettings.dspTime;
        float fadeProgress = 0f;
        
        float startVolume = source != null ? source.volume : 0f;

        while (fadeProgress < 1f)
        {
            double currentDspTime = AudioSettings.dspTime;
            double deltaDspTime = currentDspTime - oldDspTime;
            oldDspTime = currentDspTime;

            fadeProgress += (float)(deltaDspTime / duration);
            fadeProgress = Mathf.Clamp01(fadeProgress);

            source.volume = Mathf.Lerp(startVolume, targetVolume, fadeProgress);

            yield return null;
        }
        
        source.volume = targetVolume;
    }
    
            ///// AUDIO MIXER METHODS //////
    
    public void FadeMixerGroup(AudioMixerGroup mixerGroup, float targetVolume, double duration) => StartCoroutine(FadeMixerGroupCoroutine(targetVolume, duration, mixerGroup));
    
    private IEnumerator FadeMixerGroupCoroutine(float targetVolume, double duration, AudioMixerGroup mixerGroup)
    {
        double oldDspTime = AudioSettings.dspTime;
        float fadeProgress = 0f;

        mixerGroup.audioMixer.GetFloat(_mixGroupVolumeParameters[mixerGroup], out var startVolume);

        while (fadeProgress < 1f)
        {
            double currentDspTime = AudioSettings.dspTime;
            double deltaDspTime = currentDspTime - oldDspTime;
            oldDspTime = currentDspTime;

            fadeProgress += (float)(deltaDspTime / duration);
            fadeProgress = Mathf.Clamp01(fadeProgress);

            mixerGroup.audioMixer.SetFloat(
                _mixGroupVolumeParameters[mixerGroup], 
                Mathf.Lerp(startVolume, targetVolume, fadeProgress));

            yield return null;
        }
        
        mixerGroup.audioMixer.SetFloat(_mixGroupVolumeParameters[mixerGroup], targetVolume);
    }
    
    
            ////// SNAPSHOT METHODS //////////
            
    // store only one snapshot change at a time
    private Coroutine _fadeToSnapshot;
    
    // stop the stored snapshot change
    public void StopFadeToSnapshot()
    {
        if (_fadeToSnapshot != null)
        {
            StopCoroutine(_fadeToSnapshot);
        }
    }
    
    // public method that starts the snapshot change
    public void FadeToSnapshot(AudioMixerSnapshot snapshot, float duration) => _fadeToSnapshot = StartCoroutine(FadeToSnapshotCoroutine(snapshot, duration));
    
    // the actual snapshot change
    private IEnumerator FadeToSnapshotCoroutine(AudioMixerSnapshot snapshot, float duration)
    {
        double oldDspTime = AudioSettings.dspTime;
        float transitionProgress = 0f;
        
        AudioMixerSnapshot snapshotA = _currentSnapshot;
        AudioMixerSnapshot snapshotB = snapshot;

        while (transitionProgress < 1f)
        {
            double currentDspTime = AudioSettings.dspTime;
            double deltaDspTime = currentDspTime - oldDspTime;
            oldDspTime = currentDspTime;

            transitionProgress += (float)(deltaDspTime / duration);
            transitionProgress = Mathf.Clamp01(transitionProgress);

            float weightA = 1f - transitionProgress;
            float weightB = transitionProgress;

            audioMixer.TransitionToSnapshots(
                new AudioMixerSnapshot[] { snapshotA, snapshotB },
                new float[] { weightA, weightB },
                0f); // immediate, manual control

            yield return null;
        }
        
        audioMixer.TransitionToSnapshots(
            new AudioMixerSnapshot[] { snapshotA, snapshotB },
            new float[] { 0f, 1f },
            0f);
        _currentSnapshot = snapshotB;
    }
    
    
    
    
        //////////// SCENE CHANGE METHODS ////////////
    

    private void OnSceneLoadedAudio(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "NewMainLobby":
                SetHubState();
                break;
            default:
                SetPlanetState();
                //Debug.Log("Update AudioBehavior: Scene not found");
                break;
        }
    }

    //private void OnSceneUnloadedAudio(Scene scene)
    //{
    //    switch (scene.name)
    //    {
    //        default:
    //            //Debug.Log("Update AudioBehavior: Scene not found");
    //            break;
    //    }
    //}
}
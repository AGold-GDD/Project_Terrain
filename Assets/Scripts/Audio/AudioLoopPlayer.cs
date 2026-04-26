using UnityEngine;
using UnityEngine.Audio;

public class AudioLoopPlayer
{
    private readonly AudioBehavior _audioBehavior;

    private AudioSource _sourceA;
    private AudioSource _sourceB;

    private AudioClip _clip;
    private double _loopDuration;
    private double _nextStartTime;
    private int _activeIndex;

    public AudioLoopPlayer(AudioBehavior audioBehavior)
    {
        _audioBehavior = audioBehavior;
    }

    public void StartLoop(AudioClip clip, double loopDuration, double fadeDuration = 0, double startDelay = 0.1)
    {
        Debug.Log("AudioLoopPlayer :: StopLoop"); 
        if (clip == null || _audioBehavior == null)
            return;

        _clip = clip;
        
        if (loopDuration < 0) { loopDuration = clip.length;}
        
        _loopDuration = Mathf.Max(0.01f, (float)loopDuration);
        _activeIndex = 0;

        _sourceA = _audioBehavior.GetPooledLoopSource();
        _sourceB = _audioBehavior.GetPooledLoopSource();

        AudioMixerGroup mixerGroup = null;
        
        if (_clip.name.Contains("AMB_"))
        {
            mixerGroup = _audioBehavior.ambMixer;
        }
        else if (_clip.name.Contains("MX_menu"))
        {
            mixerGroup = _audioBehavior.hubMixer;
        }
        else if (_clip.name.Contains("MX_planet_base"))
        {
            mixerGroup = _audioBehavior.planetMixer;
        }
        else if (_clip.name.Contains("MX_planet_move"))
        {
            mixerGroup = _audioBehavior.moveMixer;
        }
        else if (_clip.name.Contains("MX_planet_terrainUp"))
        {
            mixerGroup = _audioBehavior.terrainUpMixer;
        }
        else if (_clip.name.Contains("MX_planet_terrainDown"))
        {
            mixerGroup = _audioBehavior.terrainDownMixer;
        }
        else
        {
            mixerGroup = _audioBehavior.mxMixer;
        }
        
        _sourceA.outputAudioMixerGroup = mixerGroup;
        _sourceB.outputAudioMixerGroup = mixerGroup;
        
        
        if (_sourceA == null || _sourceB == null)
        {
            StopLoop();
            return;
        }

        if (fadeDuration > 0)
        {
            _sourceA.volume = 0.0f;                                             
            _sourceB.volume = 0.0f;

            _audioBehavior.FadeLoopSource(_sourceA, 1, fadeDuration);
            _audioBehavior.FadeLoopSource(_sourceB, 1, fadeDuration);
        }
        
        double dspNow = AudioSettings.dspTime;
        _nextStartTime = dspNow + startDelay;

        ScheduleSource(_activeIndex, _nextStartTime);
        _nextStartTime += _loopDuration;
    }

    public void UpdateLoop()
    {
        if (_clip == null || _sourceA == null || _sourceB == null)
            return;

        double dspNow = AudioSettings.dspTime;

        // Queue the next copy a little before the current one ends.
        // This gives the engine time to prepare it.
        if (dspNow + 0.1 >= _nextStartTime)
        {
            _activeIndex = 1 - _activeIndex;
            ScheduleSource(_activeIndex, _nextStartTime);
            _nextStartTime += _loopDuration;
        }
    }

    public void StopLoop(double fadeDuration = 0)
    {
        Debug.Log("AudioLoopPlayer :: StopLoop");
        if (_sourceA != null)
        {
            _audioBehavior.StopLoop(_sourceA, fadeDuration);
            _sourceA = null;
        }

        if (_sourceB != null)
        {
            _audioBehavior.StopLoop(_sourceB, fadeDuration);
            _sourceB = null;
        }

        _clip = null;
        _loopDuration = 0.0;
    }

    private void ScheduleSource(int index, double startTime)
    {
        Debug.Log("AudioLoopPlayer :: ScheduleSource"); 
        AudioSource source = index == 0 ? _sourceA : _sourceB;
        if (source == null)
            return;

        source.Stop();
        source.clip = _clip;
        source.PlayScheduled(startTime);
    }
}
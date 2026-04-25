using UnityEngine;

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

    public void StartLoop(AudioClip clip, double loopDuration, double startDelay = 0.1)
    {
        if (clip == null || _audioBehavior == null)
            return;

        _clip = clip;
        _loopDuration = Mathf.Max(0.01f, (float)loopDuration);
        _activeIndex = 0;

        _sourceA = _audioBehavior.GetPooledLoopSource();
        _sourceB = _audioBehavior.GetPooledLoopSource();

        if (_sourceA == null || _sourceB == null)
        {
            StopLoop();
            return;
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

    public void StopLoop()
    {
        if (_sourceA != null)
        {
            _audioBehavior.ReleaseLoopSource(_sourceA);
            _sourceA = null;
        }

        if (_sourceB != null)
        {
            _audioBehavior.ReleaseLoopSource(_sourceB);
            _sourceB = null;
        }

        _clip = null;
        _loopDuration = 0.0;
    }

    private void ScheduleSource(int index, double startTime)
    {
        AudioSource source = index == 0 ? _sourceA : _sourceB;
        if (source == null)
            return;

        source.Stop();
        source.clip = _clip;
        source.PlayScheduled(startTime);
    }
}
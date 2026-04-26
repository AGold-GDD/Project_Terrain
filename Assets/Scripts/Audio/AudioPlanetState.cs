using System.Collections.Generic;
using UnityEngine;

public class AudioPlanetState : IAudioState
{
    private readonly AudioBehavior _audioBehavior;
    private readonly AudioClip _ambWindClip;
    
    private readonly List<AudioLoopPlayer> _loopPlayers = new List<AudioLoopPlayer>();

    public AudioPlanetState(AudioBehavior audioBehavior, AudioClip ambWindClip)
    {
        _audioBehavior = audioBehavior;
        _ambWindClip = ambWindClip;
    }

    public void OnEnter()
    {
        _loopPlayers.Add(_audioBehavior.StartLoop(_ambWindClip, 88, 2));
        
        _audioBehavior.FadeMixerGroup(_audioBehavior.hubMixer, _audioBehavior.MinVolume, 8);
        _audioBehavior.FadeMixerGroup(_audioBehavior.moveMixer, _audioBehavior.MinVolume, 0);
        _audioBehavior.FadeMixerGroup(_audioBehavior.terrainUpMixer, _audioBehavior.MinVolume, 0);
        _audioBehavior.FadeMixerGroup(_audioBehavior.terrainDownMixer, _audioBehavior.MinVolume, 0);
        _audioBehavior.FadeMixerGroup(_audioBehavior.planetMixer, _audioBehavior.MaxVolume, 3);
    }

    public void OnUpdate()
    {
        foreach (var loopPlayer in _loopPlayers)
            loopPlayer?.UpdateLoop();
        _audioBehavior.OnVelocityMoveMixers();
    }
    
    public void OnPause()
    {
        _audioBehavior.StopFadeToSnapshot();
        _audioBehavior.FadeToSnapshot(_audioBehavior.pauseSnapshot, 0.5f);
    }

    public void OnResume()
    {
        _audioBehavior.StopFadeToSnapshot();
        _audioBehavior.FadeToSnapshot(_audioBehavior.playSnapshot, 0.5f);
    }

    public void OnExit()
    {
        _loopPlayers[0]?.StopLoop(3);

        _loopPlayers.Clear();
    }
}

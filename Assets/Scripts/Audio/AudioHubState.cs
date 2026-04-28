using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHubState : IAudioState
{
    private readonly AudioBehavior _audioBehavior;
    private readonly AudioClip _mxClip;
    private readonly AudioClip _ambMachineryClip;
    private readonly AudioClip _ambRoomClip;
    
    private readonly List<AudioLoopPlayer> _loopPlayers = new List<AudioLoopPlayer>(3);

    public AudioHubState(AudioBehavior audioBehavior, AudioClip mxClip, AudioClip ambMachineryClip, AudioClip ambRoomClip)
    {
        _audioBehavior = audioBehavior;
        _mxClip = mxClip;
        _ambMachineryClip = ambMachineryClip;
        _ambRoomClip = ambRoomClip;
    }

    public void OnEnter()
    {
        Debug.Log("AudioHubState :: OnEnter"); 
        _loopPlayers.Add(_audioBehavior.StartLoop(_ambMachineryClip, 46, 1));
        _loopPlayers.Add(_audioBehavior.StartLoop(_ambRoomClip, 38, 1));
        
        _audioBehavior.FadeMixerGroup(_audioBehavior.hubMixer, _audioBehavior.MaxVolume, 5);
        _audioBehavior.FadeMixerGroup(_audioBehavior.moveMixer, _audioBehavior.MinVolume, 5);
        _audioBehavior.FadeMixerGroup(_audioBehavior.terrainUpMixer, _audioBehavior.MinVolume, 5);
        _audioBehavior.FadeMixerGroup(_audioBehavior.terrainDownMixer, _audioBehavior.MinVolume, 5);
        _audioBehavior.FadeMixerGroup(_audioBehavior.planetMixer, _audioBehavior.MinVolume, 5);
        
        
    }

    public void OnUpdate()
    {
        foreach (var loopPlayer in _loopPlayers)
            loopPlayer?.UpdateLoop();
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
        foreach (var loopPlayer in _loopPlayers)
        {
            loopPlayer?.StopLoop(3);
        }
        _loopPlayers.Clear();
    }
}
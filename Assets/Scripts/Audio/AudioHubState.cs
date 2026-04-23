using System.Collections.Generic;
using UnityEngine;

public class AudioHubState : IAudioState
{
    private readonly AudioBehavior _audioBehavior;
    private readonly AudioClip _mxClip;
    private readonly AudioClip _ambMachineryClip;
    private readonly AudioClip _ambRoomClip;

    private readonly List<AudioLoopPlayer> _loopPlayers = new List<AudioLoopPlayer>();

    public AudioHubState(AudioBehavior audioBehavior, AudioClip mxClip, AudioClip ambMachineryClip, AudioClip ambRoomClip)
    {
        _audioBehavior = audioBehavior;
        _mxClip = mxClip;
        _ambMachineryClip = ambMachineryClip;
        _ambRoomClip = ambRoomClip;
    }

    public void OnEnter()
    {
        _loopPlayers.Add(_audioBehavior.StartLoop(_mxClip, 160));
        _loopPlayers.Add(_audioBehavior.StartLoop(_ambMachineryClip, 24.875));
        _loopPlayers.Add(_audioBehavior.StartLoop(_ambRoomClip, 33.971));
    }

    public void OnUpdate()
    {
        for (int i = 0; i < _loopPlayers.Count; i++)
        {
            _loopPlayers[i]?.UpdateLoop();
        }
    }

    public void OnPause()
    {
    }

    public void OnResume()
    {
    }

    public void OnExit()
    {
        for (int i = 0; i < _loopPlayers.Count; i++)
        {
            _loopPlayers[i]?.StopLoop();
        }

        _loopPlayers.Clear();
    }
}
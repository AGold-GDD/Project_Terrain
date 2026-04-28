using UnityEngine;

public class AudioStateMachine
{
    private IAudioState _currentState;

    public void ChangeState(IAudioState newState)
    {
        if (_currentState == newState) return;
        _currentState?.OnExit();
        _currentState = newState;
        _currentState?.OnEnter();
    }
    
    public void Update()
    {
        _currentState?.OnUpdate();
    }
    
    public void Pause()
    {
        _currentState?.OnPause();
    }
    public void Resume()
    {
        _currentState?.OnResume();
    }
}

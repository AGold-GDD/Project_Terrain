using UnityEngine;

public interface IAudioState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
    void OnPause();
    void OnResume();
}

using UnityEngine;
using UnityEngine.XR;

public class StateControler : MonoBehaviour
{
    private PlayerState _currentPlayerState = PlayerState.SlideIdle;
    public void ChangeState(PlayerState newPlayerState)
    {
        if (_currentPlayerState == newPlayerState)
        {
            return;
        }

        _currentPlayerState = newPlayerState;
    }
    public PlayerState GetCurrentState()
    {
        return _currentPlayerState;
    }
}

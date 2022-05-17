using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected PlayerState State;

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }

    public PlayerState GetPlayerState()
    {
        return State;
    }
}

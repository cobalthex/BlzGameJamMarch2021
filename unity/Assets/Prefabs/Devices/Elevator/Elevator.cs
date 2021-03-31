using UnityEngine;

public class Elevator : MonoBehaviour
{
    enum ElevatorState
    {
        DoorsClosed,

    }

    public AnalogState DoorState;
    public AnalogState FloorState;
    public Switch DoorSwitch;

    ElevatorState state;
    
    void Update()
    {
        
    }

    void OnSwitchStateChanged(Switch switchObj)
    {

    }
}

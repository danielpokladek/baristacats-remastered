using UnityEngine;

public class RushController
{
    private float _timeToRush;
    private float _rushDuration;

    private float _rushTimer;

    private bool _rushActive;

    public RushController()
    {
        _timeToRush = 60f;
        _rushDuration = 15f;
        _rushTimer = 0f;

        _rushActive = false;
    }

    public void Update()
    {
        _rushTimer += Time.deltaTime;

        if (_rushActive)
        {
            if (_rushTimer > _rushDuration)
            {
                EndRush();
                return;
            }
        }
        else
        {
            if (_rushTimer > _timeToRush)
            {
                StartRush();
                return;
            }
        }
    }

    private void StartRush()
    {
        _rushTimer = 0;
        _rushActive = true;

        Debug.Log("Starting rush..");

        Events.RushStart.Invoke();
    }

    private void EndRush()
    {
        _rushTimer = 0;
        _rushActive = false;

        Debug.Log("Ending rush..");

        Events.RushEnd.Invoke();
    }
}

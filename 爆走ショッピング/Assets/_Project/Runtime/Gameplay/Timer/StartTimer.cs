
using UnityEngine;

public class StartTimer: MonoBehaviour
{
    [SerializeField] private TimerManager timerManager;

    private void Start()
    {
        if (timerManager == null)
        {
            Debug.LogError("TimerManagerが設定されていません");
            return;
        }

        timerManager.StartTimer();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // [field: SerializeField] public PlayerDataManager PlayerDataManager { get; private set; }
    /*
    public GameStateManager GameStateManager { get; private set; }

    */
    public TimeManager TimeManager { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        // if (PlayerDataManager == null) PlayerDataManager = GetComponentInChildren<PlayerDataManager>();
        if (TimeManager == null) TimeManager = GetComponentInChildren<TimeManager>();
    }
}

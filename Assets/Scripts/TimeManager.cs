using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [SerializeField] private float _startTime;
    [SerializeField] private float _currentRealTimeToGetBack;

    [SerializeField] private float _originalTimeScale;
    [SerializeField] private float _originalFixedDeltaTime;

    public float showTimeScale;

    private void Start()
    {
        _originalTimeScale = Time.timeScale;
        _originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        showTimeScale = Time.timeScale;
    }

    public void ChangeTimeRate(float changeRate, float realTimeToGetBack)
    {
        /*
        if (_startTime + _currentRealTimeToGetBack < Time.realtimeSinceStartup + realTimeToGetBack)
        {
            StopCoroutine("ChangeTimeRateCoroutine");
            Time.timeScale = _originalTimeScale;
            Time.fixedDeltaTime = _originalFixedDeltaTime;
        }
        */
        StartCoroutine(ChangeTimeRateCoroutine(changeRate, realTimeToGetBack));
    }

    IEnumerator ChangeTimeRateCoroutine(float changeRate, float realTimeToGetBack)
    {
        // store original values;
        _originalTimeScale = Time.timeScale;
        _originalFixedDeltaTime = Time.fixedDeltaTime;
        
        // change time scale;
        Time.timeScale = changeRate;
        Time.fixedDeltaTime *= changeRate;
        _startTime = Time.realtimeSinceStartup;
        _currentRealTimeToGetBack = realTimeToGetBack;

        Debug.Log("Change Time Scale! RealTimeToGetBack : " + realTimeToGetBack);

        // wait for seconds we set;
        yield return new WaitForSecondsRealtime(realTimeToGetBack);
        
        Debug.Log("Get Back Time Scale!");

        // get original values back;
        Time.timeScale = _originalTimeScale;
        Time.fixedDeltaTime = _originalFixedDeltaTime;
    }
}

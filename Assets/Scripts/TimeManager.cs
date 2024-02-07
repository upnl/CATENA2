using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    private float _startTime;
    private float _currentRealTimeToGetBack;

    private float _originalTimeScale;
    private float _originalFixedDeltaTime;

    private void Start()
    {
        _originalTimeScale = Time.timeScale;
        _originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void ChangeTimeRate(float changeRate, float realTimeToGetBack)
    {
        if (_startTime + _currentRealTimeToGetBack < Time.realtimeSinceStartup + realTimeToGetBack)
        {
            StopCoroutine("ChangeTimeRateCoroutine");
            Time.timeScale = _originalTimeScale;
            Time.fixedDeltaTime = _originalFixedDeltaTime;
        }
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

        // wait for seconds we set;
        yield return new WaitForSecondsRealtime(realTimeToGetBack);

        // get original values back;
        Time.timeScale = _originalTimeScale;
        Time.fixedDeltaTime = _originalFixedDeltaTime;
    }
}

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

    private Coroutine _timeCoroutine;

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
        if (_timeCoroutine == null)
        {
            _timeCoroutine = StartCoroutine(ChangeTimeRateCoroutine(changeRate, realTimeToGetBack));
        }
        else if (_startTime + _currentRealTimeToGetBack < Time.realtimeSinceStartup + realTimeToGetBack)
        {
            StopCoroutine(_timeCoroutine);
            
            _timeCoroutine = StartCoroutine(ChangeTimeRateCoroutine(changeRate, realTimeToGetBack));
        }
    }

    IEnumerator ChangeTimeRateCoroutine(float changeRate, float realTimeToGetBack)
    {
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

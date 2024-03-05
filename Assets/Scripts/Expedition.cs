using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Expedition : MonoBehaviour
{
    public GameObject[] players;
    
    // for blocking/allowing to input key to current player/other players;
    public InputActionAsset playerAction;
    public InputActionAsset nonPlayerAction;

    // current player num;
    public int currentPlayerNum = 0;

    // switch cool down time;
    public float switchCoolDown;
    public float switchCoolDownElapsed;
    
    // camera setting;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public float originalCameraSize;
    public float switchZoomCameraSize;
    public float switchZoomSpeedRate;

    public bool isSwitching;

    private void Update()
    {
        // switch cool down mechanism;
        if (switchCoolDownElapsed > 0)
        {
            switchCoolDownElapsed -= Time.deltaTime;
            if (switchCoolDownElapsed <= 0) switchCoolDownElapsed = 0;
        }
        
        // zoom in and out when switch;
        if (isSwitching)
        {
            cinemachineVirtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, switchZoomCameraSize, Time.deltaTime * switchZoomSpeedRate);
        }
        else
        {
            cinemachineVirtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, originalCameraSize, Time.deltaTime * switchZoomSpeedRate);
        }
    }

    public void OnNumKeys(InputAction.CallbackContext value)
    {
        if (value.started && switchCoolDownElapsed == 0)
        {
            // determine what number to switch from input system;
            int input = (int)(value.ReadValue<float>()) - 1;
            
            if (input == currentPlayerNum) return;
            
            players[input].SetActive(true);
            
            // position and velocity synchronize;
            players[input].transform.position = players[currentPlayerNum].transform.position;
            players[input].GetComponent<Rigidbody2D>().velocity =
                players[currentPlayerNum].GetComponent<Rigidbody2D>().velocity;
            
            // block previous player's input << should be changed into other ways because of bugs;
            players[currentPlayerNum].GetComponent<PlayerInput>().actions = nonPlayerAction;
            players[input].GetComponent<PlayerInput>().actions = playerAction;

            cinemachineVirtualCamera.Follow = players[input].transform;
            
            switchCoolDownElapsed = switchCoolDown;

            StartCoroutine(TemporarySlowTimeMethod(input, currentPlayerNum));
            currentPlayerNum = input;
        }
    }

    IEnumerator TemporarySlowTimeMethod(int toNum, int fromNum)
    {
        isSwitching = true;
        
        GameManager.Instance.TimeManager.ChangeTimeRate(0.5f, 2f);
        
        yield return new WaitForSecondsRealtime(2f);
        
        players[fromNum].SetActive(false);
        
        isSwitching = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Expedition : MonoBehaviour
{
    public GameObject[] players;
    
    // for blocking/allowing to input key to current player/other players
    public InputActionAsset playerAction;
    public InputActionAsset nonPlayerAction;

    public int currentPlayerNum = 0;

    public float switchCoolDown;
    public float switchCoolDownElapsed;

    private void Update()
    {
        if (switchCoolDownElapsed > 0)
        {
            switchCoolDownElapsed -= Time.deltaTime;
            if (switchCoolDownElapsed <= 0) switchCoolDownElapsed = 0;
        }
    }

    public void OnNumKeys(InputAction.CallbackContext value)
    {
        if (value.started && switchCoolDownElapsed == 0)
        {
            int input = (int)(value.ReadValue<float>()) - 1;
            
            players[input].SetActive(true);
            
            players[input].transform.position = players[currentPlayerNum].transform.position;
            players[input].GetComponent<Rigidbody2D>().velocity =
                players[currentPlayerNum].GetComponent<Rigidbody2D>().velocity;
            
            players[currentPlayerNum].GetComponent<PlayerInput>().actions = nonPlayerAction;
            players[input].GetComponent<PlayerInput>().actions = playerAction;

            switchCoolDownElapsed = switchCoolDown;

            StartCoroutine(TemporarySlowTimeMethod(input, currentPlayerNum));
            currentPlayerNum = input;
        }
    }

    IEnumerator TemporarySlowTimeMethod(int toNum, int fromNum)
    {
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime *= 0.5f;

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime *= 2f;

        players[fromNum].SetActive(false);
    }
}

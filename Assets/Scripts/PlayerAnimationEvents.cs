using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public CombatController combatController;
    public PlayerController playerController;
    
    public void CanAttack()
    {
        combatController.CanAttack();
    }

    public void Dash(float dashPower)
    {
        playerController.Dash(dashPower);
    }

    public void Attack(int index)
    {
        combatController.Attack(index);
    }
}

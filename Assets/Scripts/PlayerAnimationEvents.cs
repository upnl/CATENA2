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
    

    public void Dash(int index)
    {
        combatController.Dash(index);
    }

    public void Attack(int index)
    {
        combatController.HitBoxCheck();
    }
}

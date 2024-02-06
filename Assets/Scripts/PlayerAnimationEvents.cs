using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public CombatController combatController;
    
    public void CanAttack()
    {
        combatController.CanAttack();
    }
}

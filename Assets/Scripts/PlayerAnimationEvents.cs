using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public HeroKnightCombatController combatController;
    
    public void CanAttack()
    {
        combatController.canAttack = true;
    }
}

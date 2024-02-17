using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationEvents : MonoBehaviour
{
    public MonsterCombatController combatController;
    
    public void CanAttack()
    {
        combatController.CanAttack();
    }

    public void Attack(int index)
    {
        combatController.Attack(index);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public CombatController combatController;
    public PlayerController playerController;
    
    // distinguish normal attack and attack that cause slow motion;
    [SerializeField] private bool _causeSlowMotion;
    [SerializeField] private float _slowMotionRate;
    [SerializeField] private float _slowMotionTime;
    
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
        if (!_causeSlowMotion)
        {
            combatController.Attack(index);
        }
        else
        {
            combatController.Attack(index, _slowMotionRate, _slowMotionTime);
        }
    }

    public void SetSlowMotionOption(bool slowMotion, float slowMotionRate, float slowMotionTime)
    {
        if (_causeSlowMotion = slowMotion)
        {
            _slowMotionRate = slowMotionRate;
            _slowMotionTime = slowMotionTime;
        }
    }
}

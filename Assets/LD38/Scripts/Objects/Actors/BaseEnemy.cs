using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseEnemy : BaseActor
{
    protected const float DAMAGE_FEEDBACK_DELAY = 0.25f;

    public override void TakeDamage(int amount, BaseObject damageSource)
    {
        base.TakeDamage(amount, damageSource);

        DOVirtual.DelayedCall(DAMAGE_TIME + DAMAGE_FEEDBACK_DELAY, () =>
        {
            if (m_currentHP > 0)
            {
                if (Vector3.Dot(damageSource.transform.up, transform.forward) > 0.9f)
                {
                    // attack back
                    BaseActor attacker = damageSource as BaseActor;
                    if(attacker != null)
                    {
                        Attack(attacker);
                    }
                }
                else
                {
                    // turn
                    FaceDir(damageSource.transform.up);
                }
            }
            else
            {
                // spawn item on other side
                

                // flip/die
                m_tile.m_side.Flip();
            }
        });
    }

}

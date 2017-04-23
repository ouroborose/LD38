using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseEnemy : BaseActor
{
    public static readonly List<BaseEnemy> s_allEnemies = new List<BaseEnemy>();

    protected const float DAMAGE_FEEDBACK_DELAY = 0.25f;

    protected override void Awake()
    {
        base.Awake();
        s_allEnemies.Add(this);
    }

    protected override void OnDestroy()
    {
        s_allEnemies.Remove(this);
        base.OnDestroy();
    }

    public override void TakeDamage(int amount, BaseObject damageSource = null)
    {
        IncrementBusyCounter();
        base.TakeDamage(amount, damageSource);

        DOVirtual.DelayedCall(DAMAGE_TIME + DAMAGE_FEEDBACK_DELAY, () =>
        {
            DecrementBusyCounter();
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
                Main.Instance.World.SpawnRandomPrefab(m_tile.m_side, Main.Instance.World.m_currentBiomeData.m_enemyDropPrefabs);
            }
        });
    }

}

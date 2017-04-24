using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseEnemy : BaseActor
{
    public static readonly List<BaseEnemy> s_allEnemies = new List<BaseEnemy>();


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
    public override void Reset()
    {
        if (Main.Instance.m_currentLevel > 0)
        {
            float scaler = Mathf.Pow(1.15f, Main.Instance.m_currentLevel);
            m_maxHpBonus = Mathf.FloorToInt(m_baseHP * scaler);
            m_atkBonus = Mathf.FloorToInt(m_baseAttack * scaler);
        }
        else
        {
            m_maxHpBonus = 0;
            m_atkBonus = 0;
        }
        base.Reset();
    }

    public override void TakeDamage(int amount, BaseObject damageSource = null)
    {
        IncrementBusyCounter();
        base.TakeDamage(amount, damageSource);

        DOVirtual.DelayedCall(DAMAGE_TIME + POST_DAMAGE_FEEDBACK_DELAY, () =>
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
                if(Main.Instance.Player.m_numKeys <= 0 && BaseChest.s_allChests.Count > 0 && BaseKey.s_allKeys.Count <= 0)
                {
                    // always spawn a key if player has none and there is a chest in the world
                    Main.Instance.World.SpawnObject(m_tile.m_side, Main.Instance.m_keyPrefab);
                }
                else
                {
                    Main.Instance.World.SpawnRandomPrefab(m_tile.m_side, Main.Instance.World.m_currentBiomeData.m_enemyDropPrefabs);
                }
                
            }
        });
    }

}

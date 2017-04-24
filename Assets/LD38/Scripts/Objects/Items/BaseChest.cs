using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseChest : BaseActor {
    public static readonly List<BaseChest> s_allChests = new List<BaseChest>();

    [SerializeField] private Transform m_itemSpawnPos;
    [SerializeField] private Transform m_keyPivot;
    [SerializeField] private Transform[] m_sides;

    [SerializeField] private AudioClip[] m_keyAppearSound;
    [SerializeField] private AudioClip[] m_keyTurnSound;
    [SerializeField] private AudioClip[] m_chestOpenSound;

    protected override void Awake()
    {
        m_keyPivot.gameObject.SetActive(false);
        s_allChests.Add(this);

        m_baseHP = Main.Instance.Player.CalculateAttackDamage() * 3;

        base.Awake();
    }

    protected override void OnDestroy()
    {
        s_allChests.Remove(this);
        base.OnDestroy();
    }

    public override void TakeDamage(int amount, BaseObject damageSource = null)
    {
        BasePlayer player = damageSource as BasePlayer;
        if (player != null && player.m_numKeys > 0)
        {
            player.RemoveKey();

            IncrementBusyCounter();
            m_keyPivot.gameObject.SetActive(true);

            AudioManager.Instance.PlayOneShot(m_keyAppearSound);
            m_keyPivot.DOLocalMoveY(0.0f, 0.25f).SetDelay(0.25f).SetEase(Ease.InOutBack).OnComplete(()=>
            {
                AudioManager.Instance.PlayOneShot(m_keyTurnSound);
            });
            m_keyPivot.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.25f, RotateMode.LocalAxisAdd).SetDelay(0.5f).SetEase(Ease.InOutBack);

            DOVirtual.DelayedCall(0.8f, () =>
            {
                AudioManager.Instance.PlayOneShot(m_chestOpenSound);
            });

            for (int i = 0; i < m_sides.Length; ++i)
            {
                Transform side = m_sides[i];
                side.DOLocalMoveY(0.5f, 0.75f).SetDelay(0.75f).SetEase(Ease.InOutBack);
                side.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f, RotateMode.LocalAxisAdd).SetDelay(1.0f).SetEase(Ease.OutBack);
                side.DOScale(Vector3.zero, 0.5f).SetDelay(1.0f);
            }

            DOVirtual.DelayedCall(1.0f, () =>
            {
                DetachFromTile();
                GameObject prefab = Main.Instance.GetCurrentBiomeGroup().m_chestDrops.GetNextDrop();
                if (prefab != null)
                {
                    GameObject dropObj = Instantiate(prefab);
                    BaseItem item = dropObj.GetComponent<BaseItem>();
                    item.SetTile(m_tile);
                    item.Model.position = m_itemSpawnPos.position;
                    item.Model.rotation = transform.rotation;
                    EventManager.OnItemSpawned.Dispatch(item);
                }
                Destroy(gameObject, 0.5f);
            });
        }
        else
        {
            // give key needed feedback
            base.TakeDamage(amount, damageSource);

            if (m_currentHP <= 0)
            {
                Destroy(gameObject, DAMAGE_TIME);
            }
        }
    }
}

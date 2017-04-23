﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseChest : BaseObject {
    [SerializeField] private Transform m_itemSpawnPos;
    [SerializeField] private Transform m_keyPivot;
    [SerializeField] private Transform[] m_sides;

    protected override void Awake()
    {
        base.Awake();
        m_keyPivot.gameObject.SetActive(false);
    }

    public void Open(BasePlayer player)
    {
        player.Attack(null);
        if (player.m_numKeys > 0)
        {
            player.RemoveKey();

            IncrementBusyCounter();
            m_keyPivot.gameObject.SetActive(true);


            m_keyPivot.DOLocalMoveY(0.0f, 0.25f).SetDelay(0.25f).SetEase(Ease.InOutBack);
            m_keyPivot.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.25f, RotateMode.LocalAxisAdd).SetDelay(0.5f).SetEase(Ease.InOutBack);

            for(int i = 0; i < m_sides.Length; ++i)
            {
                Transform side = m_sides[i];
                side.DOLocalMoveY(0.5f, 0.75f).SetDelay(0.75f).SetEase(Ease.InOutBack);
                side.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f, RotateMode.LocalAxisAdd).SetDelay(1.0f).SetEase(Ease.OutBack);
                side.DOScale(Vector3.zero, 0.5f).SetDelay(1.0f);
            }

            DOVirtual.DelayedCall(1.0f, ()=>
            {
                BiomeData data = Main.Instance.World.m_currentBiomeData;
                if(data.m_chestDropPrefabs.Length > 0)
                {
                    GameObject prefab = data.m_chestDropPrefabs[UnityEngine.Random.Range(0, data.m_chestDropPrefabs.Length)];
                    if(prefab != null)
                    {
                        GameObject dropObj = Instantiate(prefab);
                        BaseItem item = dropObj.GetComponent<BaseItem>();
                        item.SetTile(m_tile);
                        item.Model.position = m_itemSpawnPos.position;
                        item.Model.forward = m_tile.transform.up;
                    }
                }
                Destroy(gameObject, 0.5f);
            });
        }
        else
        {
            // give key needed feedback
            Shake(0.25f, 0.5f);
        }
    }
}

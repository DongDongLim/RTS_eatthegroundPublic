using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, Damaged
{
    Unit Aowner;

    [SerializeField]
    BattleMng battleMng;

    int m_hp;

    float scaleZ;
    private void Awake()
    {
        Aowner = GetComponent<Unit>();
        m_hp = Aowner.m_Data.hp[0];
        scaleZ = transform.localScale.z;
    }

    public void Damaged(int apk)
    {
        float z = transform.localScale.z - scaleZ / m_hp;
        if (z > 0)
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        else
        {
            StartCoroutine(Lose());
        }
    }

    IEnumerator Lose()
    {
        battleMng.EndBattle();
        if (gameObject.layer == LayerMask.NameToLayer("BattleUnit"))
        {
            battleMng.EnemyEffect1.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            battleMng.EnemyEffect.SetActive(true);
            battleMng.EnemyEffect1.SetActive(false);
            yield return new WaitForSeconds(5f);
            if (gameObject.scene.name == "Battle")
            {
                GameMng.instance.isAttackWin = true;
                SceneMng.instance.SceneUnStreaming("Battle");
            }
            else
            {
                GameMng.instance.isDefanceWin = true;
                SceneMng.instance.SceneUnStreaming("BattleDefance");
            }
        }
        else
        {
            battleMng.PlayerEffect1.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            battleMng.PlayerEffect.SetActive(true);
            battleMng.PlayerEffect1.SetActive(false);
            yield return new WaitForSeconds(5f);
            if (gameObject.scene.name == "Battle")
            {
                GameMng.instance.isAttackWin = false;
                SceneMng.instance.SceneUnStreaming("Battle");
            }
            else
            {
                GameMng.instance.isDefanceWin = false;
                SceneMng.instance.SceneUnStreaming("BattleDefance");
            }
        }
    }
}

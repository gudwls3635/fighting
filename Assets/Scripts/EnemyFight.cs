using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFight : MonoBehaviour
{
    public GameObject battleSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void endAni()
    {
        battleSystem.GetComponent<BattleSystem>().endEnemyAnimation();
    }
    public void attack()
    {
        Debug.Log("Enemy attack!");
        battleSystem.GetComponent<BattleSystem>().attackEnemy();
    }
    public void setIdle()
    {
        transform.GetComponent<Animator>().SetInteger("action", 0);
    }
}

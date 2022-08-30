using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userFight : MonoBehaviour
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
        battleSystem.GetComponent<BattleSystem>().endUserAnimation();
    }
    public void attack()
    {
        Debug.Log("user attack!");
        battleSystem.GetComponent<BattleSystem>().attackUser();
    }
    public void setIdle()
    {
        transform.GetComponent<Animator>().SetInteger("action", 0);
    }
}

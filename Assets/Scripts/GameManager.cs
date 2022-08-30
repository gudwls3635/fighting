using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance; // 싱글톤
    [HideInInspector] public GameData.UserInfo userInfo;
    [HideInInspector] public GameData.EnemyInfo enemyInfo;
    [HideInInspector] public int userTurn,turnNum,stage;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);

    }
    public void createUserInfo(int id)
    {
        userInfo = new GameData.UserInfo(id);
    }
    public void createGame( int level)
    {
        Debug.Log("게임 시작 level: " + level);
        stage = level;
        createEnemy(level);
        userTurn = GameData.ATTACK;
        userInfo.hp = userInfo.hpLimit;
        turnNum = 1;
    }
    public void createEnemy(int level)
    {
        if (level >= GameData.stageLimit) level = GameData.stageLimit - 1;
        enemyInfo = new GameData.EnemyInfo(level);
        Debug.Log("적 캐릭터 생성 id: " + enemyInfo.id);
    }
}

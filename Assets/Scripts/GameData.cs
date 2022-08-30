using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    const string GAME_ID = "4174447";
    [HideInInspector] public static GameData instance; // 싱글톤
    
    public Sprite[] gameBackSprite = new Sprite[3];
    public int limitStatus,diceLimit;
    public Sprite emptySprite;
    public Sprite[] cardSprite= new Sprite[3];
    public Sprite question;
    public Sprite[] userCardUISprite = new Sprite[3];
    public Sprite randomBtnSprite;
    public Sprite btnBoxSprite;
    public const int ATTACK = 0;
    public const int DEFENCE = 1;

    public List<List<int>>[] enemyCardLists;

    public string[] attackName = new string[] { "상", "중", "하" };
    public Sprite questionCharacter;
    public Sprite[] characterImages = new Sprite[6];
    public RuntimeAnimatorController[] chrAniIdle = new RuntimeAnimatorController[2];
    public Sprite[] selectCharacterImages = new Sprite[6];
    public Sprite[] selectCharacterShadowImages = new Sprite[6];
    public int RandomRewardlimit;
    public string[] RandomRewardStr;
    public int[] RandomRewardNum;
    public double[] RandomRewardPer;

    public static Hashtable stageInfo = new Hashtable();
    public static CharacterInfo[] chrInfos;
    public static int stageLimit,chrLimit;
    [HideInInspector] public List<Dictionary<string, object>> dic;

    public static UserGameData userGameData;
    public class UserGameData
    {
        public bool[] characterLock = new bool[GameData.chrLimit];
    }

    public class CharacterInfo
    {
        public int id;
        public string name;
        public string condition;
        public int healthPoint;
        public int high_stat;
        public int high_card;
        public int mid_stat;
        public int mid_card;
        public int low_stat;
        public int low_card;

    }
    public class UserInfo
    {
        public int hp;
        public int hpLimit;
        public int chrId; // 캐릭터 id
        public int[] status = new int[3]; // up mid down
        public List<int>[] enemyLists = new List<int>[GameData.chrLimit];
        public int checkCardN;
        public int[] cardN = new int[3]; // 상, 중, 하 카드 몇개 더 갖고 있는지
        public List<int> cardList = new List<int>();
        public RuntimeAnimatorController animator;
        public UserInfo(int chrId)
        {
            this.chrId = chrId;
            this.status[0] = chrInfos[chrId].high_stat;
            this.status[1] = chrInfos[chrId].mid_stat;
            this.status[2] = chrInfos[chrId].low_stat;
            for (int i = 0; i < GameData.chrLimit; i++) this.enemyLists[i] = new List<int>();
            this.cardN[0] = chrInfos[chrId].high_card;
            this.cardN[1] = chrInfos[chrId].mid_card;
            this.cardN[2] = chrInfos[chrId].low_card;
            this.checkCardN = 1;
            this.hpLimit = chrInfos[chrId].healthPoint;
            this.hp = this.hpLimit;
            this.cardList = new List<int>();
            this.animator = GameData.instance.chrAniIdle[chrId];
        }
        public UserInfo(UserInfo pre)
        {
            this.chrId = pre.chrId;
            for(int i = 0; i<3;i++) this.status[i] = pre.status[i];
            for (int i = 0; i < 5; i++) this.enemyLists[i] = new List<int>(pre.enemyLists[i]);
            for (int i = 0; i < 3; i++) this.cardN[i] = pre.cardN[i];
            this.checkCardN = pre.checkCardN;
            this.hpLimit = pre.hpLimit;
            this.hp = pre.hp;
            this.cardList = new List<int>();
            for (int i = 0; i < 3; i++) this.cardList.Add(pre.cardList[i]);
            this.animator = pre.animator;
        }
    }
    public class StatInfo
    {
        public int[] status = new int[3];
    }
    public class EnemyInfo
    {
        public int[] status = new int[3]; // up mid down
        public int hp;
        public int hpLimit;
        public int id;
        public List<int> cardList = new List<int>();
        public List<List<int>> cardLists = new List<List<int>>();
        public int currentCardList;
        public RuntimeAnimatorController animator;
        public EnemyInfo(int level)
        {
            this.id = GameData.getStageInfo<int>("Character", level - 1) - 1;
            string[] tmp = new string[] { "High_", "Mid_", "Low_" };
            for (int i = 0; i < 3; i++)
            {
                int statusRand = UnityEngine.Random.Range(GameData.getStageInfo<int>(tmp[0] + "min", level-1), GameData.getStageInfo<int>(tmp[0] + "max", level-1));
                this.status[i] = statusRand;
            }
            this.hpLimit = GameData.getStageInfo<int>("HealthPoint", level - 1);
            this.hp = this.hpLimit;

            this.cardLists = GameData.instance.enemyCardLists[this.id];
            this.animator = GameData.instance.chrAniIdle[this.id];
            this.cardList = new List<int>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        stageLimit = 10; chrLimit = 6;
        limitStatus = 30;
        diceLimit = 15;
        //datas
        dic = CSVReader.Read("datas");
        stageInfo = new Hashtable();
        stageInfo.Add("Character", new List<int>());
        stageInfo.Add("HealthPoint", new List<int>());
        stageInfo.Add("High_min", new List<int>());stageInfo.Add("High_max", new List<int>());
        stageInfo.Add("Mid_min", new List<int>());stageInfo.Add("Mid_max", new List<int>());
        stageInfo.Add("Low_min", new List<int>()); stageInfo.Add("Low_max", new List<int>());
        for (int i = 1; i <= stageLimit; i++)
        {
            (stageInfo["Character"] as List<int>).Add(int.Parse(dic[i - 1]["Character"].ToString()));
            (stageInfo["HealthPoint"] as List<int>).Add(int.Parse(dic[i - 1]["HealthPoint"].ToString()));
            (stageInfo["High_min"] as List<int>).Add(int.Parse(dic[i - 1]["High_min"].ToString()));
            (stageInfo["High_max"] as List<int>).Add(int.Parse(dic[i - 1]["High_max"].ToString()));
            (stageInfo["Mid_min"] as List<int>).Add(int.Parse(dic[i - 1]["Mid_min"].ToString()));
            (stageInfo["Mid_max"] as List<int>).Add(int.Parse(dic[i - 1]["Mid_max"].ToString()));
            (stageInfo["Low_min"] as List<int>).Add(int.Parse(dic[i - 1]["Low_min"].ToString()));
            (stageInfo["Low_max"] as List<int>).Add(int.Parse(dic[i - 1]["Low_max"].ToString()));
        }
        enemyCardLists = new List<List<int>>[stageLimit];
        for (int i = 0; i < stageLimit; i++)
        {
            enemyCardLists[i] = new List<List<int>>();
            string cardStr = dic[i]["Cards"].ToString();
            for(int j = 1; j <= (int)Char.GetNumericValue(cardStr[0]); j++)
            {
                enemyCardLists[i].Add(new List<int>() { (int)Char.GetNumericValue(cardStr[1 + 4*(j-1)]), (int)Char.GetNumericValue(cardStr[2 + 4 * (j - 1)]), (int)Char.GetNumericValue(cardStr[3 + 4 * (j - 1)]) });
            }
        }
        //chrDatas
        chrInfos = new CharacterInfo[chrLimit];
        dic = CSVReader.Read("chrDatas");
        for (int i = 1; i <= chrLimit; i++)
        {
            chrInfos[i - 1] = new CharacterInfo();
            chrInfos[i - 1].id = int.Parse(dic[i - 1]["Id"].ToString());
            chrInfos[i - 1].name = dic[i - 1]["Name"].ToString();
            //chrInfos[i].condition = dic[i - 1]["Character"].ToString();
            chrInfos[i - 1].healthPoint = int.Parse(dic[i - 1]["HealthPoint"].ToString());
            chrInfos[i - 1].high_stat = int.Parse(dic[i - 1]["High_stat"].ToString());
            chrInfos[i - 1].high_card = int.Parse(dic[i - 1]["High_card"].ToString());
            chrInfos[i - 1].mid_stat = int.Parse(dic[i - 1]["Mid_stat"].ToString());
            chrInfos[i - 1].mid_card = int.Parse(dic[i - 1]["Mid_card"].ToString());
            chrInfos[i - 1].low_stat = int.Parse(dic[i - 1]["Low_stat"].ToString());
            chrInfos[i - 1].low_card = int.Parse(dic[i - 1]["Low_card"].ToString());
        }
        //RandomReward
        RandomRewardStr = new string[] { "상 +1", "상 +2", "상 +3", "중 +1", "중 +2", "중 +3", "하 +1", "하 +2", "하 +3", "상 카드 +1", "중 카드 +1", "하 카드 +1", "카드 확인권 +1" };
        RandomRewardNum = new int[] { 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 1, 1, 1 };
        RandomRewardPer = new double[] { 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f, 7.69f };
        RandomRewardlimit = RandomRewardStr.Length;

        Advertisement.Initialize(GAME_ID, true);
        DontDestroyOnLoad(this);
        loadData();
        SceneManager.LoadScene("StartScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (GameData.instance == null)
        {
            GameData.instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);

    }
    
    public static void ShowRewardedAD(Action<ShowResult> handleShowResult )
    {
        if (Advertisement.IsReady("Rewarded_Android"))
        {
            var options = new ShowOptions { resultCallback = handleShowResult };
            Advertisement.Show("Rewarded_Android", options);
        }
    }
   
    public static void saveData()
    {

    }
    public void loadData()
    {
        userGameData = new UserGameData();
        for(int i = 0; i< 4; i++) userGameData.characterLock[i] = true;

    }
    public static T getStageInfo<T>(string name, int stage)
    {
        return (T)Convert.ChangeType((stageInfo[name] as List<T>)[stage], typeof(T));
    }
}

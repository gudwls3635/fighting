using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class SelectSystem : MonoBehaviour
{
    public Image gameBack;
    public Text topTitle;
    public GameObject selectObj, batteSystem;
    public GameObject upBtns, downBtns;

    public GameObject nextBtn;
    public GameObject enemyInfo, enemyInfoContent, enemyInfoObj;

    public GameObject cardCheckObj;
    public Text cardCheckObjText;
    public GameObject enemyCards;
    public GameObject msgObj;
    

    bool[] enemyCardCheck = new bool[3];

    public int[] cardList = new int[3];
    public int[] cardLimit = new int[3];
    public int[] cardN = new int[3];

    bool advCheck;
    public Sprite sword, shield;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.createGame(GameManager.instance.stage); // level 1 시작
        selectCard();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void selectCard()
    {
        initSystem();
        if (GameManager.instance.userTurn == GameData.ATTACK)
        {
            topTitle.text = GameManager.instance.turnNum + "번째 턴 - \"공격\"";
        }else
        {
            topTitle.text = GameManager.instance.turnNum + "번째 턴 - \"방어\"";
        }
        
        selectObj.SetActive(true);
    }
    public void selectCardBtn(Button btn)
    {
        int cardIndex = btn.transform.GetSiblingIndex();
        int userCardIndex=-1;
        for(int i = 0; i<3;i++)
        {
            if(cardList[i]== -1)
            {
                userCardIndex = i;
                break;
            }
        }
        if(userCardIndex != -1)
        {
            // 빈공간 있음
            if(cardN[cardIndex] > 0)
            {
                cardN[cardIndex]--;
                changeCardUI();

                upBtns.transform.GetChild(userCardIndex).GetComponent<Image>().sprite = GameData.instance.cardSprite[cardIndex];
                cardList[userCardIndex] = cardIndex;
            }
            
        }
    }
    public void userCardBtn(Button btn)
    {
        int cardIndex = btn.transform.GetSiblingIndex();
        if(cardList[cardIndex] != -1)
        {
            // 빈공간이 아닐 떄
            cardN[cardList[cardIndex]] += 1;
            changeCardUI();

            upBtns.transform.GetChild(cardIndex).GetComponent<Image>().sprite = GameData.instance.userCardUISprite[cardIndex];
            cardList[cardIndex] = -1;
        }
    }
    public void initSystem()
    {
        // 게임 배경 설정
        Debug.Log("stage: " + GameManager.instance.stage);
        if(GameManager.instance.stage < 4)
            gameBack.sprite = GameData.instance.gameBackSprite[GameManager.instance.stage - 1];
        // 적 카드 초기화
        for(int i = 0; i < 3;i++)
        {
            enemyCards.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.question;
        }
        for (int i = 0; i < 3; i++)
            upBtns.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.userCardUISprite[i];
        cardList = new int[3] { -1,-1,-1};

        for(int i  = 0; i < 3; i++)
        {
            cardLimit[i] = GameManager.instance.userInfo.cardN[i];
            cardN[i] = GameManager.instance.userInfo.cardN[i];
            
        }
        changeCardUI();

        for(int i=0; i<3; i++)
        {
            enemyCardCheck[i] = false;
        }

        setEnemyCardList();
        // 광고 초기화
        advCheck = false;
    }
    public void startBattleBtn(Button btn)
    {
        if(btn.transform.GetChild(0).GetComponent<Text>().text.Equals("닫기"))
        {
            // 닫기 
            for (int i = enemyInfoContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(enemyInfoContent.transform.GetChild(i).gameObject);
            }
            enemyInfo.SetActive(false);
            btn.transform.GetChild(0).GetComponent<Text>().text = "전투 시작";
        }
        else
        {
            bool check = true;
            for (int i = 0; i < 3; i++)
            {
                if (cardList[i] == -1)
                {
                    check = false;
                    break;
                }
            }
            if (check)
            {
                startBattle();
            }
        }
        
    }
    public void startBattle()
    {
        setCardList();
        selectObj.SetActive(false);
        batteSystem.GetComponent<BattleSystem>().startBattle();
    }
    public void setCardList()
    {
        GameManager.instance.userInfo.cardList.Clear();
        for (int i = 0; i < 3; i++) GameManager.instance.userInfo.cardList.Add(cardList[i]);
    }
    public void setEnemyCardList()
    {
        GameManager.instance.enemyInfo.cardList.Clear();
        int rand = UnityEngine.Random.Range(0, GameManager.instance.enemyInfo.cardLists.Count);
        GameManager.instance.enemyInfo.currentCardList = rand;
        for(int i = 0; i< GameManager.instance.enemyInfo.cardLists[rand].Count; i++)
        {
            GameManager.instance.enemyInfo.cardList.Add(GameManager.instance.enemyInfo.cardLists[rand][i]);
        }
    }
    int enemyCheckCardIndex;
    public void enemyCardClick(Button btn)
    {
        int btnIndex = btn.transform.GetSiblingIndex();
        if (enemyCardCheck[btnIndex] == false)
        {
            enemyCheckCardIndex = btnIndex;
            cardCheckObj.transform.GetChild(1).GetComponent<Text>().text = (btnIndex  +1) + "번째 카드를 확인하시겠습니까?\n(카드 확인:"
                + GameManager.instance.userInfo.checkCardN + "회 남음)";
            openCardCheckObj();
        }
        
    }
    public void openCardCheckObj()
    {
        cardCheckObjText.text = "확인";
        cardCheckObjText.color = new Color(0f, 0f, 0f);
        if (GameManager.instance.userInfo.checkCardN == 0)
        {
            if(advCheck)
            {
                //광고 봤음
                cardCheckObjText.color = new Color(0.5f, 0.5f, 0.5f);
            }else
            {
                // 광고 x
                cardCheckObjText.text = "광고 시청";
            }
        }
        cardCheckObj.SetActive(true);
    }
    public void enemyCardOKbtn()
    {
        if(cardCheckObjText.color.r != 0.5f)
        {
            if (cardCheckObjText.text.Equals("광고 시청"))
            {
                //  광고 시작
                Debug.Log("광고 시작");
                GameData.ShowRewardedAD(handleShowResult);
            }else
            {
                if (GameManager.instance.userInfo.checkCardN > 0)
                {
                    GameManager.instance.userInfo.checkCardN--;
                    enemyCardCheck[enemyCheckCardIndex] = true;
                    enemyCards.transform.GetChild(enemyCheckCardIndex).GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.enemyInfo.cardList[enemyCheckCardIndex]];
                }
                else
                {
                    openMsg("소지한 카드 확인권이 부족합니다");
                }
            }
            cardCheckObj.SetActive(false);
        }
    }
    
    public void handleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("AD Finished");
                advCheck = true;
                enemyCardCheck[enemyCheckCardIndex] = true;
                enemyCards.transform.GetChild(enemyCheckCardIndex).GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.enemyInfo.cardList[enemyCheckCardIndex]];
                break;
            case ShowResult.Skipped:
                Debug.Log("AD Skipped");
                break;
            case ShowResult.Failed:
                Debug.Log("AD Failed");
                break;
        }
    }
    

    void openMsg(string str)
    {
        msgObj.transform.GetChild(2).GetComponent<Text>().text = str;
        msgObj.SetActive(true);
    }
    public void msgOk()
    {
        msgObj.SetActive(false);
    }
    public void enemyCardCancelBtn()
    {
        cardCheckObj.SetActive(false);
    }
    public void enemyInfoBtn()
    {
        for(int i = 0; i< GameManager.instance.enemyInfo.cardLists.Count; i++)
        {
            GameObject tmp = Instantiate(enemyInfoObj, enemyInfoContent.transform, false);
            for (int k = 1; k <= 3; k++)
            {
                tmp.transform.GetChild(k).GetComponent<Image>().sprite = GameData.instance.question;
            }
            for (int j = 0; j< GameManager.instance.userInfo.enemyLists[GameManager.instance.enemyInfo.id].Count; j++)
            {
                if (GameManager.instance.userInfo.enemyLists[GameManager.instance.enemyInfo.id][j] == i)
                {
                    // 알고 있는 셋
                    tmp.transform.GetChild(1).GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.enemyInfo.cardLists[i][0]];
                    tmp.transform.GetChild(2).GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.enemyInfo.cardLists[i][1]];
                    tmp.transform.GetChild(3).GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.enemyInfo.cardLists[i][2]];
                }
            }
        }

        nextBtn.transform.GetChild(0).GetComponent<Text>().text = "닫기";
        enemyInfo.SetActive(true);
    }

    void changeCardUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (GameManager.instance.userTurn == GameData.ATTACK)
                downBtns.transform.GetChild(i).transform.GetChild(2).GetComponent<Image>().sprite = sword;
            else
                downBtns.transform.GetChild(i).transform.GetChild(2).GetComponent<Image>().sprite = shield;
            downBtns.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = "남은 카드: " + cardN[i];
        }
    }
}

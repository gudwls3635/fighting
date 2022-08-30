using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public Text topTitle, userDmgText, enemyDmgText;
    public GameObject  battleObj;
    public GameObject userGage, enemyGage;
    public GameObject selectSystem;
    public GameObject userCard, enemyCard,userObj,enemyObj;
    public GameObject nextBtn;
    public GameObject endContents;
    [HideInInspector] public int turn;
    [HideInInspector] public bool endBattle;
    int userAttackN, userDefenceN,enemyAttackN,enemyDefenceN;
    // Start is called before the first frame update
    void Start()
    {
       
        changeGage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeGage()
    {
        userGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount = GameManager.instance.userInfo.hp / (float)GameManager.instance.userInfo.hpLimit;
        userGage.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.userInfo.hp.ToString();
        if(userGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount >= 0.5f)
        {
            userGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else if(userGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount >= 0.1f)
        {
            userGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.65f, 0);
        }
        else
        {
            userGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0, 0);
        }
        for (int i = 0; i < 3; i++)
        {
            userGage.transform.GetChild(i + 1).transform.GetChild(0).GetComponent<Image>().fillAmount = GameManager.instance.userInfo.status[i] / (float)GameData.instance.limitStatus;
            userGage.transform.GetChild(i + 1).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.userInfo.status[i].ToString();
        }
        enemyGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount = GameManager.instance.enemyInfo.hp / (float)GameManager.instance.enemyInfo.hpLimit;
        enemyGage.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.enemyInfo.hp.ToString();
        if (enemyGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount >= 0.5f)
        {
            enemyGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else if (enemyGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount >= 0.1f)
        {
            enemyGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.65f, 0);
        }
        else
        {
            enemyGage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0, 0);
        }
        for (int i = 0; i < 3; i++)
        {
            enemyGage.transform.GetChild(i + 1).transform.GetChild(0).GetComponent<Image>().fillAmount = GameManager.instance.enemyInfo.status[i] / (float)GameData.instance.limitStatus;
            enemyGage.transform.GetChild(i + 1).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.enemyInfo.status[i].ToString();
        }
        
    }
    public void startBattle()
    {
        initBattle();
        battleObj.SetActive(true);
        
        nextTurn();
    }
    public void selectCard()
    {
        GameManager.instance.turnNum += 1;
        GameManager.instance.userTurn = 1 - GameManager.instance.userTurn;
        selectSystem.GetComponent<SelectSystem>().selectCard();
    }
    public void nextTurn()
    {
        setTitle();
        StartCoroutine(openCard());
    }
    IEnumerator openCard()
    {
        userCard.GetComponent<Image>().sprite = GameData.instance.emptySprite;
        enemyCard.GetComponent<Image>().sprite = GameData.instance.emptySprite;
        yield return new WaitForSeconds(1);

        userCard.GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.userInfo.cardList[turn]];
        enemyCard.GetComponent<Image>().sprite = GameData.instance.cardSprite[GameManager.instance.enemyInfo.cardList[turn]];
        // 카드 애니메이션 실행
        StartCoroutine(cardAnimation());
    }
    bool actionSuccess;
    IEnumerator cardAnimation()
    {
        // 유저와 적 애니메이션 진행
        aniDone[0] = 0; aniDone[1] = 0;
        if (GameManager.instance.userTurn == GameData.DEFENCE)
        {
            userObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.userInfo.cardList[turn]+4);
            enemyObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.enemyInfo.cardList[turn]+1);
        }
        else
        {
            userObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.userInfo.cardList[turn] + 1);
            enemyObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.enemyInfo.cardList[turn]+4);
        }
            
        actionSuccess= false;
        if(GameManager.instance.userInfo.cardList[turn] == GameManager.instance.enemyInfo.cardList[turn])
        {
            // 방어
            if (GameManager.instance.userTurn == GameData.DEFENCE)
            {
                // 유저 성공
                float timer=0f;
                Vector3 originalCardPos = userCard.transform.position;
                while(timer < 0.7f)
                {
                    userCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                    timer += Time.deltaTime * 2;
                    yield return new WaitForSeconds(Time.deltaTime*0.5f);
                }
                timer = 0.7f;
                while(timer>0)
                {
                    userCard.transform.position -= new Vector3(0, Time.deltaTime , 0);
                    timer -= Time.deltaTime * 2;
                    yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                }
                userCard.transform.position = originalCardPos;
                actionSuccess = true;
            }
            else
            {
                // 적 성공
                float timer = 0f;
                Vector3 originalCardPos = enemyCard.transform.position;
                while (timer < 0.7f)
                {
                    enemyCard.transform.position += new Vector3(0, Time.deltaTime , 0);
                    timer += Time.deltaTime * 2;
                    yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                }
                timer = 0.7f;
                while (timer > 0)
                {
                    enemyCard.transform.position -= new Vector3(0, Time.deltaTime , 0);
                    timer -= Time.deltaTime * 2;
                    yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                }
                enemyCard.transform.position = originalCardPos;
            }
        }
        else
        {
            // 클린 히트
            if (GameManager.instance.userTurn == GameData.ATTACK)
            {
                // 유저 성공
                float timer = 0f;
                Vector3 originalCardPos = userCard.transform.position;
                while (timer < 1)
                {
                    userCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                    timer += Time.deltaTime * 2;
                    yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                }
                timer = 0.7f;
                while (timer > 0)
                {
                    userCard.transform.position -= new Vector3(0, Time.deltaTime, 0);
                    timer -= Time.deltaTime *2;
                    yield return new WaitForSeconds(Time.deltaTime* 0.5f);
                }
                userCard.transform.position = originalCardPos;
                actionSuccess = true;
            }
            else
            {
                // 적 성공
                float timer = 0f;
                Vector3 originalCardPos = enemyCard.transform.position;
                while (timer < 1)
                {
                    enemyCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                    timer += Time.deltaTime *2 ;
                    yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                }
                timer = 0.7f;
                while (timer > 0)
                {
                    enemyCard.transform.position -= new Vector3(0, Time.deltaTime, 0);
                    timer -= Time.deltaTime * 2;
                    yield return new WaitForSeconds(Time.deltaTime*0.5f);
                }
                enemyCard.transform.position = originalCardPos;
            }
        }
        StartCoroutine(cardExecution(actionSuccess));
        //cardExecution(actionSuccess);
    }
    int[] aniDone = new int[2]; // 0: user, 1:enemy
    // 카드 애니메이션 실행 끝나면
    IEnumerator cardExecution(bool success)
    {
        while(true)
        {
            if (aniDone[0] == 1 && aniDone[1] == 1)
                break;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        StartCoroutine(doAction(GameManager.instance.userTurn, success));
        
    }
    public void endUserAnimation()
    {
        userObj.GetComponent<Animator>().SetInteger("action", 0);
        aniDone[0] = 1;
    }
    public void endEnemyAnimation()
    {
        enemyObj.GetComponent<Animator>().SetInteger("action",0);
        aniDone[1] = 1;
    }
    public void attackEnemy()
    {
        if(GameManager.instance.userInfo.cardList[turn] != GameManager.instance.enemyInfo.cardList[turn])
        {
            Debug.Log("User damage!");
            userObj.GetComponent<Animator>().SetTrigger("dm");
        }
    }
    public void attackUser()
    {
        if (GameManager.instance.userInfo.cardList[turn] != GameManager.instance.enemyInfo.cardList[turn])
        {
            Debug.Log("Enemy damage!");
            enemyObj.GetComponent<Animator>().SetTrigger("dm");
        }
    }
    IEnumerator doAction(int action,bool success)
    {
        yield return new WaitForSeconds(1);
        int plusAction = 0; // 1: 유저공격3, 2:적방어3, 3:유저방어3, 4:적공격3
        switch (action)
        {
            case GameData.ATTACK:
                if(success)
                {
                    enemyDamage(GameManager.instance.userInfo.status[GameManager.instance.userInfo.cardList[turn]]);
                    userAttackN += 1; if (userAttackN == 3) plusAction = 1;
                }
                else
                {
                    int damage = GameManager.instance.userInfo.status[GameManager.instance.userInfo.cardList[turn]] - GameManager.instance.enemyInfo.status[GameManager.instance.enemyInfo.cardList[turn]];
                    if (damage < 0) damage = 0;
                    enemyDamage(damage);
                    enemyDefenceN += 1; if (enemyDefenceN == 3) plusAction = 2;
                }
                break;
            case GameData.DEFENCE:
                if(success)
                {
                    int damage = GameManager.instance.enemyInfo.status[GameManager.instance.enemyInfo.cardList[turn]] - GameManager.instance.userInfo.status[GameManager.instance.userInfo.cardList[turn]];
                    if (damage < 0) damage = 0;
                    userDamage(damage);
                    userDefenceN += 1; if (userDefenceN == 3) plusAction = 3;
                }
                else
                {
                    userDamage(GameManager.instance.enemyInfo.status[GameManager.instance.enemyInfo.cardList[turn]]);
                    enemyAttackN += 1; if (enemyAttackN == 3) plusAction = 4;
                }
                break;
        }
        if ( !endBattle )
        {
            aniDone[0] = 0;  aniDone[1] = 0;
            // 연속 공/ 방 성공시 확인권 획득
            switch (plusAction)
            {
                case 1: // 유저 공격 3번 성공
                    GameManager.instance.userInfo.checkCardN += 1;
                    break;
                case 3: //유저 방어 3번 성공
                    GameManager.instance.userInfo.checkCardN += 1;
                    break;
            }
            execTurn();
            
            /* 반격이나 추가공격 했을 때 
            switch (plusAction)
            {
                case 0: // 일반 진행
                    execTurn();
                    break;
                case 1: // 유저 추가 공격
                    // 애니메이션
                    userObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.userInfo.cardList[turn] + 1);

                    // 카드 애니메이션                   
                    float timer = 0f;
                    Vector3 originalCardPos = userCard.transform.position;
                    while (timer < 1)
                    {
                        userCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                        timer += Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    timer = 0.7f;
                    while (timer > 0)
                    {
                        userCard.transform.position -= new Vector3(0, Time.deltaTime, 0);
                        timer -= Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    userCard.transform.position = originalCardPos;

                    while (true)
                    {
                        if (aniDone[0] == 1)
                            break;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    yield return new WaitForSeconds(1);
                    enemyDamage(10);
                    execTurn();
                    break;
                case 2: // 적 반격

                    // 애니메이션
                    enemyObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.enemyInfo.cardList[turn] + 1);

                    // 카드 애니메이션
                    originalCardPos = enemyCard.transform.position;
                    timer = 0;
                    while (timer < 1)
                    {
                        enemyCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                        timer += Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    timer = 0.7f;
                    while (timer > 0)
                    {
                        enemyCard.transform.position -= new Vector3(0, Time.deltaTime, 0);
                        timer -= Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    enemyCard.transform.position = originalCardPos;

                    while (true)
                    {
                        if ( aniDone[1] == 1)
                            break;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    yield return new WaitForSeconds(1);
                    execTurn();
                    userDamage(10);
                    break;
                case 3: //유저 반격
                    // 애니메이션
                    userObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.userInfo.cardList[turn] + 1);
                    // 카드 애니메이션
                    timer = 0;
                    originalCardPos = userCard.transform.position;
                    while (timer < 1)
                    {
                        userCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                        timer += Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    timer = 0.7f;
                    while (timer > 0)
                    {
                        userCard.transform.position -= new Vector3(0, Time.deltaTime, 0);
                        timer -= Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    userCard.transform.position = originalCardPos;

                    while (true)
                    {
                        if (aniDone[0] == 1 )
                            break;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    yield return new WaitForSeconds(1);
                    enemyDamage(10);
                    execTurn();
                    break;
                case 4: // 적 추가 공격
                    // 애니메이션
                    enemyObj.GetComponent<Animator>().SetInteger("action", GameManager.instance.enemyInfo.cardList[turn] + 1);
                    // 카드 애니메이션
                    originalCardPos = enemyCard.transform.position;
                    timer = 0;
                    while (timer < 1)
                    {
                        enemyCard.transform.position += new Vector3(0, Time.deltaTime, 0);
                        timer += Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    timer = 0.7f;
                    while (timer > 0)
                    {
                        enemyCard.transform.position -= new Vector3(0, Time.deltaTime, 0);
                        timer -= Time.deltaTime * 2;
                        yield return new WaitForSeconds(Time.deltaTime * 0.5f);
                    }
                    enemyCard.transform.position = originalCardPos;

                    while (true)
                    {
                        if (aniDone[1] == 1)
                            break;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    yield return new WaitForSeconds(1);
                    userDamage(10);
                    execTurn();
                    break;
            }
            */
        }
    }
    void enemyDamage(int dmg)
    {
        StartCoroutine(enemyDmgShow(dmg));
        GameManager.instance.enemyInfo.hp -= dmg;
        if (GameManager.instance.enemyInfo.hp <= 0)
        {
            GameManager.instance.enemyInfo.hp = 0;
            // 승리
            StartCoroutine(battleResult(1));
            endBattle = true;
        }
        changeGage();
    }
    IEnumerator enemyDmgShow(int dmg)
    {
        enemyDmgText.text = "-" + dmg.ToString();
        enemyDmgText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        enemyDmgText.gameObject.SetActive(false);
    }
    void userDamage(int dmg)
    {
        StartCoroutine(userDmgShow(dmg));
        GameManager.instance.userInfo.hp -= dmg;
        if (GameManager.instance.userInfo.hp <= 0)
        {
            GameManager.instance.userInfo.hp = 0;
            // 패배
            StartCoroutine(battleResult(0));
            endBattle = true;
        }
        changeGage();
    }
    IEnumerator userDmgShow(int dmg)
    {
        userDmgText.text = "-" + dmg.ToString();
        userDmgText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        userDmgText.gameObject.SetActive(false);
    }
    void execTurn()
    {
        turn += 1;
        if (turn == 3)
        {
            endBattle = true;
            setTitle();
            nextBtn.SetActive(true);
        }
        else
        {
            nextTurn();
        }
    }
    IEnumerator battleResult(int result) // 0: 패배 1: 승리
    {
        endContents.SetActive(true);
        switch (result)
        {
            case 0:
                // 패배
                userObj.GetComponent<Animator>().SetInteger("winDie", 2);
                enemyObj.GetComponent<Animator>().SetInteger("winDie",1);
                endContents.transform.GetChild(1).GetComponent<Text>().text = "패배";
                nextBtn.transform.GetChild(0).GetComponent<Text>().text = "Main Menu";
                break;
            case 1:
                // 승리
                userObj.GetComponent<Animator>().SetInteger("winDie", 1);
                enemyObj.GetComponent<Animator>().SetInteger("winDie", 2);
                endContents.transform.GetChild(1).GetComponent<Text>().text = "승리";
                nextBtn.transform.GetChild(0).GetComponent<Text>().text = "Next Stage";
                break;
        }
        nextBtn.SetActive(true);
        yield return null;
    }
    public void setTitle()
    {
        if(endBattle ==false)
        {
            switch (turn)
            {
                case 0:
                    topTitle.text = GameManager.instance.turnNum + "번째 턴 - \"첫번째 카드\"";
                    break;
                case 1:
                    topTitle.text = GameManager.instance.turnNum + "번째 턴 - \"두번째 카드\"";
                    break;
                case 2:
                    topTitle.text = GameManager.instance.turnNum + "번째 턴 - \"세번째 카드\"";
                    break;
            }
        }else
        {
            topTitle.text = GameManager.instance.turnNum + "번째 턴 - \"전투 종료\"";
        }
        
    }
    public void initBattle()
    {
        nextBtn.SetActive(false);
        endContents.SetActive(false);
        endBattle = false;
        turn = 0;
        userCard.GetComponent<Image>().sprite = GameData.instance.emptySprite;
        enemyCard.GetComponent<Image>().sprite = GameData.instance.emptySprite;

        userAttackN = 0; userDefenceN = 0; enemyAttackN = 0; enemyDefenceN = 0;


        userObj.GetComponent<Animator>().runtimeAnimatorController = GameManager.instance.userInfo.animator;
        enemyObj.GetComponent<Animator>().runtimeAnimatorController = GameManager.instance.enemyInfo.animator;
    }
    public void nextBtnClick()
    {
        // 적 카드 셋 등록
        int ch = 0;
        List<int> userKnowList = GameManager.instance.userInfo.enemyLists[GameManager.instance.enemyInfo.id];
        for(int i = 0; i< userKnowList.Count; i++)
        {
            if(userKnowList[i] == GameManager.instance.enemyInfo.currentCardList)
            {
                ch = 1;
                break;
            }
        }
        if( ch == 0 )
        {
            GameManager.instance.userInfo.enemyLists[GameManager.instance.enemyInfo.id].Add(GameManager.instance.enemyInfo.currentCardList);
        }
        // 버튼 클릭 이벤트
        if(nextBtn.transform.GetChild(0).GetComponent<Text>().text.Equals("Main Menu"))
        {
            // main menu
            SceneManager.LoadScene("StartScene");
        }else if(nextBtn.transform.GetChild(0).GetComponent<Text>().text.Equals("Next Stage"))
        {
            SceneManager.LoadScene("RandomScene");
        }
        else
        {
            // next round
            battleObj.SetActive(false);
            selectCard();
        }
    }
    public void testBtn()
    {
        SceneManager.LoadScene("RandomScene");
    }
}

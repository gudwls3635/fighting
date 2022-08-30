using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiceSystem : MonoBehaviour
{
    public GameData.UserInfo userInfo;
    GameData.StatInfo originalInfo,plusInfo;
    public int lastDice;
    public Button clickDice;
    public Text diceNumText;
    public Image upStatus, midStatus, downStatus;
    public Image diceResult;
    public bool canDice, canStatus;
    public int plusStatus;
    int statusIndex;
    bool diceResultShow;
    bool initCount;
    public GameObject explain,diceResultText;
    public GameObject diceObject;
    bool diceAnimationDone;
    
    
    // Start is called before the first frame update
    void Start()
    {
        originalInfo = new GameData.StatInfo();
        plusInfo = new GameData.StatInfo();
        initCount = false;
        diceObject.GetComponent<Animator>().SetInteger("diceResult", 0);
        diceAnimationDone = true;

        userInfo = GameManager.instance.userInfo;
        for(int i =0;i<3;i++) originalInfo.status[i] = userInfo.status[i];
        
        lastDice = 10;
        diceResultShow = false;
        canStatus = true;
        canDice = false;
        if (!checkStatus())
        {
            lastDice = 0;
            canStatus = false;
            canDice = true;
            changeStart();
        }else
        {
            diceResult.gameObject.SetActive(true);
            //clickStatusScreen();
            updateExplain("강화할 항목을 터치");
            explain.gameObject.SetActive(true);
           
        }
        changeDiceNum();
        changeStatus();
    }

    // Update is called once per frame
    bool mouseClick;
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            
            if (diceResultShow == true)
            {
                mouseClick = true;
                
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(mouseClick==true)
            {
                mouseClick = false;
                diceResultShow = false;

                diceResult.gameObject.SetActive(false);
                diceResultText.SetActive(false);
                if (!checkStatus() || lastDice == 0)
                {
                    lastDice = 0;
                    changeDiceNum();
                    changeStart();
                }
                else
                {
                    canDice = false; canStatus = true;
                }
            }
        }*/
    }
    public void dice()
    {
        Debug.Log("canDice : " + canDice + string.Format(" 남은 횟수: {0:D2}", lastDice) );
        if(canDice && diceAnimationDone)
        {
            canDice = false;
            explain.SetActive(false);
            if (lastDice == 0)
            {
                // 게임 시작 & 레벨 설정
                GameManager.instance.userInfo = userInfo;
                GameManager.instance.stage = 1;
                SceneManager.LoadScene("FightScene");
            }
            else
            {
                // 주사위 굴리기
                lastDice -= 1;
                changeDiceNum();
                diceAnimationDone = false;
                diceObject.GetComponent<Animator>().SetInteger("diceResult", -1);
                

            }
        }
        
    }
    public void diceAnimationDoneFun()
    {
        int rand;
        rand = Random.Range(1, 7);
        plusStatus = rand;
        //userInfo.status[statusIndex] += plusStatus;
        if (plusInfo.status[statusIndex] + plusStatus > GameData.instance.diceLimit)
        {
            //userInfo.status[statusIndex] = GameData.instance.diceLimit;
            //plusInfo.status[statusIndex] = GameData.instance.diceLimit - originalInfo.status[statusIndex];
            userInfo.status[statusIndex] += GameData.instance.diceLimit - plusInfo.status[statusIndex];
            plusInfo.status[statusIndex] = GameData.instance.diceLimit;
        }
        else
        {
            plusInfo.status[statusIndex] += plusStatus;
            userInfo.status[statusIndex] += plusStatus;
        }
        changeStatus();

        statusColor(-1);


        if (!checkStatus() || lastDice == 0)
        {
            canStatus = false;
            canDice = true;
            lastDice = 0;
            changeDiceNum();
            changeStart();
        }
        else
        {
            canStatus = true;
        }
        
        diceObject.GetComponent<Animator>().SetInteger("diceResult", rand);
        diceAnimationDone = true;
        //diceResultTextUpdate(plusStatus.ToString());
        //diceResult.gameObject.SetActive(true);
        //diceResultText.SetActive(true);
        //diceResultShow = true;
    }
    public void clickStatus(Button btn)
    {
        statusIndex = btn.transform.GetSiblingIndex();
        if (canStatus && plusInfo.status[statusIndex] < GameData.instance.diceLimit)
        {
            
            canDice = true;
            diceResult.gameObject.SetActive(false);
            statusColor(statusIndex);
            if(initCount == false)
            {
                updateExplain("주사위를 굴려 강화");
                initCount = true;
            }
            
            /*
            if(userInfo.status[index]!= GameData.instance.limitStatus)
            {
                userInfo.status[index] += plusStatus;
                if (userInfo.status[index] > GameData.instance.limitStatus)
                {
                    userInfo.status[index] = GameData.instance.limitStatus;
                    plusInfo.status[index] =GameData.instance.limitStatus - originalInfo.status[index] ;
                }
                else
                {
                    plusInfo.status[index] += plusStatus;
                }

                changeStatus();

                diceResult.gameObject.SetActive(false);
                canDice = true; canStatus = false;

                if(!checkStatus() || lastDice == 0)
                {
                    changeStart();
                }

            }*/
        }

    }
    public bool checkStatus()
    {
        int limit = GameData.instance.diceLimit;
        if (plusInfo.status[0] ==limit && plusInfo.status[1] == limit && plusInfo.status[2] == limit)
        {
            return false;
        }
        else
        {
            return true ;
        }
    }
    public void changeStart()
    {
        clickDice.transform.GetChild(0).GetComponent<Text>().text = "전투 시작";
    }
    public void changeDiceNum()
    {
        diceNumText.text = string.Format("남은 횟수: {0:D2}", lastDice);
    }
    public void changeStatus()
    {
        StartCoroutine(fillStatus());
        
        upStatus.transform.GetChild(1).GetComponent<Text>().text = originalInfo.status[0].ToString(); upStatus.transform.GetChild(2).GetComponent<Text>().text = " + " + plusInfo.status[0];
        if ( plusInfo.status[0] >= GameData.instance.diceLimit) upStatus.transform.GetChild(2).GetComponent<Text>().text += " (MAX)";
        midStatus.transform.GetChild(1).GetComponent<Text>().text = originalInfo.status[1].ToString(); midStatus.transform.GetChild(2).GetComponent<Text>().text = " + " + plusInfo.status[1];
        if ( plusInfo.status[1] >= GameData.instance.diceLimit) midStatus.transform.GetChild(2).GetComponent<Text>().text += " (MAX)";
        downStatus.transform.GetChild(1).GetComponent<Text>().text = originalInfo.status[2].ToString(); downStatus.transform.GetChild(2).GetComponent<Text>().text = " + " + plusInfo.status[2];
        if ( plusInfo.status[2] >= GameData.instance.diceLimit) downStatus.transform.GetChild(2).GetComponent<Text>().text += " (MAX)";
    }
    IEnumerator fillStatus()
    {
        float timer = 0;
        while (plusInfo.status[0] / (float)GameData.instance.diceLimit - upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount > 0.001)
        {
            timer += Time.deltaTime;
            upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount, plusInfo.status[0] / (float)GameData.instance.diceLimit, timer);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = plusInfo.status[0] / (float)GameData.instance.diceLimit;

        timer = 0;
        while (plusInfo.status[1] / (float)GameData.instance.diceLimit - midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount > 0.001)
        {
            timer += Time.deltaTime;
            midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount, plusInfo.status[1] / (float)GameData.instance.diceLimit, timer);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = plusInfo.status[1] / (float)GameData.instance.diceLimit;

        timer = 0;
        while (plusInfo.status[2] / (float)GameData.instance.diceLimit - downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount > 0.001)
        {
            timer += Time.deltaTime;
            downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount, plusInfo.status[2] / (float)GameData.instance.diceLimit, timer);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = plusInfo.status[2] / (float)GameData.instance.diceLimit;
    }
    public void testBtn()
    {
        userInfo.status[0] = 15;
        userInfo.status[1] = 15;
        userInfo.status[2] = 15;
        GameManager.instance.userInfo = userInfo;
        GameManager.instance.stage = 1;
        SceneManager.LoadScene("FightScene");
    }
    void diceResultTextUpdate(string str)
    {
        diceResultText.GetComponent<Text>().text = str;
    }
    void updateExplain(string str)
    {
        explain.GetComponent<Text>().text = str;
    }
    void clickStatusScreen()
    {
        int limit = GameData.instance.diceLimit;
        if (plusInfo.status[0] == limit) {
            upStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            upStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
        }
        if (plusInfo.status[1] == limit) {
            midStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
            midStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
        }
        if (plusInfo.status[2] == limit) {
            downStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f,0.4f);
            downStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
        }
    }
    void statusColor(int kind)
    {
        switch (kind)
        {
            case 0:
                upStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                upStatus.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                midStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                midStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
                downStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                downStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
                break;
            case 1:
                upStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                upStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
                midStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                midStatus.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                downStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                downStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
                break;
            case 2:
                upStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                upStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
                midStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                midStatus.transform.GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f);
                downStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                downStatus.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                break;
            case -1:
                upStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(1f,1f, 1f);
                upStatus.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                midStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                midStatus.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                downStatus.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                downStatus.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                break;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandomSystem : MonoBehaviour
{
    public GameData.UserInfo userInfo;
    GameData.StatInfo originalInfo, plusInfo;
    public Button nextBtn;
    public Image upStatus, midStatus, downStatus;
    public bool canNext;
    public GameObject RandomBack,enemy;
    int[,] randomResult = new int[3,2];
    Coroutine[] cor = new Coroutine[3];
    int btnIndex;
    bool enemyCheckCardSelect;
    // Start is called before the first frame update
    void Start()
    {
        // 카드 확인권 리워드 초기화
        enemyCheckCardSelect = false;

        nextBtn.transform.GetChild(0).GetComponent<Text>().text = "결정";
        originalInfo = new GameData.StatInfo();
        plusInfo = new GameData.StatInfo();
        userInfo = GameManager.instance.userInfo;

        for (int i = 0; i < 3; i++) originalInfo.status[i] = userInfo.status[i];
        upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = userInfo.status[0] / (float)GameData.instance.limitStatus;
        midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = userInfo.status[1] / (float)GameData.instance.limitStatus;
        downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = userInfo.status[2] / (float)GameData.instance.limitStatus;

        btnIndex = -1;
        changeStatus();
        createRandomBtn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void createRandomBtn()
    {
        for(int i = 0; i < 3; i++)
        {
            int selected = -1;
            do
            {
                int randIndex = Random.Range(0, GameData.instance.RandomRewardlimit);
                double rand = Random.Range(0.00f, 100.00f);
                if(rand < GameData.instance.RandomRewardPer[randIndex])
                {
                    if(randIndex == 12)
                    {
                        // 카드 확인권 
                        if(!enemyCheckCardSelect)
                        {
                            selected = randIndex;
                            enemyCheckCardSelect = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    selected = randIndex;
                    break;
                }
            } while (true);
            
            RandomBack.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = GameData.instance.RandomRewardStr[selected];
            RandomBack.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.randomBtnSprite;
            randomResult[i, 0] = selected; randomResult[i, 1] = GameData.instance.RandomRewardNum[selected];
        }
    }
    public void btnClick(Button btn)
    {
        if(btnIndex != -1) RandomBack.transform.GetChild(btnIndex).GetComponent<Image>().sprite = GameData.instance.randomBtnSprite;
        btnIndex = int.Parse(btn.name.Substring(3));
        for (int i = 0; i < 3; i++) plusInfo.status[i] = 0;

        if(randomResult[btnIndex,0] < 9)
        {
            // 스탯 올리는 보상
            plusInfo.status[randomResult[btnIndex, 0] / 3] = randomResult[btnIndex, 1];
        }
        
        btn.GetComponent<Image>().sprite = GameData.instance.btnBoxSprite;
        changeStatus();
    }
    public void nextBtnClick()
    {
        if(nextBtn.transform.GetChild(0).GetComponent<Text>().text.Equals("결정"))
        {
            if(btnIndex != -1)
            {
                if (randomResult[btnIndex, 0] > 8 && randomResult[btnIndex, 0] < 12)
                {
                    // 카드 증가 보상
                    userInfo.cardN[randomResult[btnIndex, 0] - 9] += randomResult[btnIndex, 1];
                }
                else if (randomResult[btnIndex, 0] == 12)
                {
                    // 카드 확인권 획득
                    userInfo.checkCardN++;
                }
                GameManager.instance.userInfo = userInfo;
                nextBtn.transform.GetChild(0).GetComponent<Text>().text = "Next Stage";
                RandomBack.SetActive(false);
                enemy.SetActive(true);
            }
        }
        else
        {
            // 레벨 증가
            GameManager.instance.stage += 1;
            SceneManager.LoadScene("FightScene");
        }
            
    }
    public void changeStatus()
    {
        for (int i = 0; i < 3; i++)
        {
            userInfo.status[i] = originalInfo.status[i] + plusInfo.status[i];
            if (userInfo.status[i] > GameData.instance.limitStatus)
            {
                userInfo.status[i] = GameData.instance.limitStatus;
                plusInfo.status[i] = GameData.instance.limitStatus - originalInfo.status[i];
            }
        }
        for(int i = 0;i < 3; i++)
        {
            if (cor[i] != null) StopCoroutine(cor[i]);
            cor[i] = StartCoroutine(fillStatus(i));
        }
        

        upStatus.transform.GetChild(1).GetComponent<Text>().text = originalInfo.status[0].ToString(); upStatus.transform.GetChild(2).GetComponent<Text>().text = " + " + plusInfo.status[0];
        midStatus.transform.GetChild(1).GetComponent<Text>().text = originalInfo.status[1].ToString(); midStatus.transform.GetChild(2).GetComponent<Text>().text = " + " + plusInfo.status[1];
        downStatus.transform.GetChild(1).GetComponent<Text>().text = originalInfo.status[2].ToString(); downStatus.transform.GetChild(2).GetComponent<Text>().text = " + " + plusInfo.status[2];
    }
    IEnumerator fillStatus(int kind)
    {

        float timer = 0;
        switch (kind)
        {
            case 0:
                while (Mathf.Abs(userInfo.status[0] / (float)GameData.instance.limitStatus - upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount) > 0.001)
                {
                    timer += Time.deltaTime;
                    upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount, userInfo.status[0] / (float)GameData.instance.limitStatus, timer);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                upStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = userInfo.status[0] / (float)GameData.instance.limitStatus;
                break;
            case 1:
                while (Mathf.Abs(userInfo.status[1] / (float)GameData.instance.limitStatus - midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount) > 0.001)
                {
                    timer += Time.deltaTime;
                    midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount, userInfo.status[1] / (float)GameData.instance.limitStatus, timer);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                midStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = userInfo.status[1] / (float)GameData.instance.limitStatus;
                break;
            case 2:
                while (Mathf.Abs(userInfo.status[2] / (float)GameData.instance.limitStatus - downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount) > 0.001)
                {
                    timer += Time.deltaTime;
                    downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount, userInfo.status[2] / (float)GameData.instance.limitStatus, timer);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                downStatus.transform.GetChild(0).GetComponent<Image>().fillAmount = userInfo.status[2] / (float)GameData.instance.limitStatus;
                break;
        }  
    }
}

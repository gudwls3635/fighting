using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ChrSelectSystem : MonoBehaviour
{
    int chrId;
    public GameObject chrImages;
    public GameObject statText, cardNText;
    int[] chrArray = new int[3];
    Camera main;

    public GameObject chrContent;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i< GameData.chrLimit; i++) cellSpaceList.Add( i * cellspace);
        main = GameObject.Find("Main Camera").GetComponent<Camera>();
        chrArray[0] = -1; chrArray[1] = 0; chrArray[2] = 1;
        updataUI();
    }

    // Update is called once per frame
    Vector3 startPoint;
    float moveDistance = 180f;
    public Text a;

    int currentSelectIndex;
    float cellspace = 344; // viewRect Cell size + Spacing
    Coroutine selectCor;
    List<float> cellSpaceList = new List<float>();
    void Update()
    {
        // 마우스
        /*if (Input.GetMouseButtonDown(0) == true)
        {
            startPoint = Input.mousePosition;
            //main.ScreenToWorldPoint(Input.mousePosition)
        }
        else if (Input.GetMouseButton(0) == true)
        {
            if (Mathf.Abs(Input.mousePosition.x - startPoint.x) >= moveDistance)
            {
                if (Input.mousePosition.x > startPoint.x)
                {
                    //right
                    changeImages(0);
                }
                else
                {
                    //left
                    changeImages(1);
                }
                startPoint = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            Debug.Log(Input.mousePosition.x - startPoint.x);
            if (Mathf.Abs(Input.mousePosition.x - startPoint.x) >= moveDistance)
            {
                if (Input.mousePosition.x > startPoint.x)
                {
                    //right
                    changeImages(0);
                }
                else
                {
                    //left
                    changeImages(1);
                }
                startPoint = Input.mousePosition;
            }
        }
        // 터치
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPoint = touch.position;
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                case TouchPhase.Moved:
                    Debug.Log(touch.position.x - startPoint.x);
                    a.text = (touch.position.x - startPoint.x).ToString();
                    if (Mathf.Abs(touch.position.x - startPoint.x) >= moveDistance)
                    {
                        if(touch.position.x > startPoint.x)
                        {
                            //right
                            changeImages(0);
                        }
                        else
                        {
                            //left
                            changeImages(1);
                        }
                        startPoint = touch.position;
                    }
                    break;
            }
        }*/


        // 마우스
        if (Input.GetMouseButtonDown(0) == true)
        {
            StopAllCoroutines();
        }
        else if (Input.GetMouseButton(0) == true)
        {
        
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            float currentPos = chrContent.GetComponent<RectTransform>().anchoredPosition.x;
            float min = 9999;
            int minIndex=0;
            for(int i = 0; i< cellSpaceList.Count; i++)
            {
                if (Mathf.Abs(Mathf.Abs(currentPos) - Mathf.Abs(cellSpaceList[i])) < min )
                {
                    min = Mathf.Abs(Mathf.Abs(currentPos) - Mathf.Abs(cellSpaceList[i]));
                    minIndex = i;
                    
                }
            }
            StopAllCoroutines();
            selectCor = StartCoroutine(cellSelectCor(-1 * cellSpaceList[minIndex]));
        }
        // 터치
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StopAllCoroutines();
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    float currentPos = chrContent.GetComponent<RectTransform>().anchoredPosition.x;
                    float min = 9999;
                    int minIndex = 0;
                    for (int i = 0; i < cellSpaceList.Count; i++)
                    {
                        if (Mathf.Abs(Mathf.Abs(currentPos) - Mathf.Abs(cellSpaceList[i])) < min)
                        {
                            min = Mathf.Abs(Mathf.Abs(currentPos) - Mathf.Abs(cellSpaceList[i]));
                            minIndex = i;

                        }
                    }
                    StopAllCoroutines();
                    selectCor = StartCoroutine(cellSelectCor(-1 * cellSpaceList[minIndex]));
                    break;
                case TouchPhase.Moved:
                    break;
                   
            }
        }
    }
    IEnumerator cellSelectCor(float target)
    {
        float e = 1f;
        float time = Time.deltaTime;
        while(Mathf.Abs(chrContent.GetComponent<RectTransform>().anchoredPosition.x - target) > e)
        {
            chrContent.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(chrContent.GetComponent<RectTransform>().anchoredPosition, new Vector3(target, chrContent.GetComponent<RectTransform>().anchoredPosition.y), time);
            time += 2 * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        chrContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(target, chrContent.GetComponent<RectTransform>().anchoredPosition.y);
    }
    void changeImages(int kind)
    {

        if(kind == 0 )
        {
            //right
            if(chrArray[0] > -1 )
            {
                for (int i = 0; i < 3; i++)
                {
                    chrArray[i] = chrArray[i] - 1;
                    //if (chrArray[i] < 0) chrArray[i] = GameData.chrLimit - 1;
                }
            }
            
        }
        else
        {
            //left
            if(chrArray[2] < GameData.chrLimit)
            {
                for (int i = 0; i < 3; i++)
                {
                    chrArray[i] = chrArray[i] + 1;
                    //if (chrArray[i] >= GameData.chrLimit) chrArray[i] = 0;
                }
            }
            
        }
        for (int i = 0; i < 3; i++)
            Debug.Log(chrArray[i]);
        updataUI();
    }
    public void startButtonClick()
    {
        if (GameData.userGameData.characterLock[currentSelectIndex] == true)
        {
            GameManager.instance.createUserInfo(currentSelectIndex);
            SceneManager.LoadScene("EnhanceScene");
        }
            
    }
    public void backBtnClick()
    {
        SceneManager.LoadScene("StartScene");
    }
    void updataUI()
    {
        /*for (int i = 0; i< chrImages.transform.childCount; i++)
        {
            if(chrArray[i] == -1 || chrArray[i] == GameData.chrLimit)
            {
                chrImages.transform.GetChild(i).GetComponent<Image>().sprite = null;
                chrImages.transform.GetChild(i).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
            else
            {
                chrImages.transform.GetChild(i).GetComponent<Animator>().runtimeAnimatorController = null;
                if (GameData.userGameData.characterLock[chrArray[i]] == true)
                {
                    chrImages.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    chrImages.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.selectCharacterImages[chrArray[i]];
                    //if (i == 1) chrImages.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController = GameData.instance.chrAniIdle[chrArray[1]];
                }
                else
                {
                    chrImages.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    chrImages.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.selectCharacterShadowImages[chrArray[i]];
                }
            }
           
        }*/

        //updateTextUI(chrArray[1]);
        for (int i = 0; i < chrContent.transform.childCount; i++)
        {
            if ( i == 0 || i == 7)
            {
                chrContent.transform.GetChild(i).GetComponent<Image>().sprite = null;
                chrContent.transform.GetChild(i).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
            else
            {
                chrContent.transform.GetChild(i).GetComponent<Animator>().runtimeAnimatorController = null;
                if (GameData.userGameData.characterLock[i-1] == true)
                {
                    chrContent.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    chrContent.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.selectCharacterImages[i - 1];
                    //if (i == 1) chrImages.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController = GameData.instance.chrAniIdle[chrArray[1]];
                }
                else
                {
                    chrContent.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    chrContent.transform.GetChild(i).GetComponent<Image>().sprite = GameData.instance.selectCharacterShadowImages[i - 1];
                }
            }

        }
        updateTextUI(0);
    }
    void updateTextUI(int id)
    {
        statText.transform.GetChild(0).GetComponent<Text>().text = GameData.chrInfos[id].high_stat.ToString();
        statText.transform.GetChild(1).GetComponent<Text>().text = GameData.chrInfos[id].mid_stat.ToString();
        statText.transform.GetChild(2).GetComponent<Text>().text = GameData.chrInfos[id].low_stat.ToString();
        cardNText.transform.GetChild(0).GetComponent<Text>().text = "X " + GameData.chrInfos[id].high_card.ToString();
        cardNText.transform.GetChild(1).GetComponent<Text>().text = "X " + GameData.chrInfos[id].mid_card.ToString();
        cardNText.transform.GetChild(2).GetComponent<Text>().text = "X " + GameData.chrInfos[id].low_card.ToString();
    }
    public void updateSelectTextUI()
    {
        float currentPos = chrContent.GetComponent<RectTransform>().anchoredPosition.x;
        float min = 9999;
        int minIndex = 0;
        for (int i = 0; i < cellSpaceList.Count; i++)
        {
            if (Mathf.Abs(Mathf.Abs(currentPos) - Mathf.Abs(cellSpaceList[i])) < min)
            {
                min = Mathf.Abs(Mathf.Abs(currentPos) - Mathf.Abs(cellSpaceList[i]));
                minIndex = i;

            }
        }
        updateTextUI(minIndex);
        currentSelectIndex = minIndex;
    }
}

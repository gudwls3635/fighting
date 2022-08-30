using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public GameObject optionObj,creditObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void optionBtn()
    {
        if(checkWindow())
            optionObj.SetActive(true);
    }
    public void startBtn()
    {
        if (checkWindow())
        {
            GameData.saveData();
            SceneManager.LoadScene("SelectScene");
        }
            
    }
    public bool checkWindow()
    {
        if(optionObj.activeSelf == false)
        {
            return true;
        }else
        {
            return false;
        }
    }
    public void exitOption()
    {
        optionObj.SetActive(false);
    }
    public void openCredit()
    {
        creditObj.SetActive(true);
    }
    public void bgmBtn()
    {

    }
    public void effectBtn()
    {

    }

}

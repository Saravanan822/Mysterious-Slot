using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    public Image loadingImage;
    float fRandomLoadingTime;
    float fCurrentLoadingTime;

    public GameObject[] welcomePanel;
    public GameObject gamePanel;
  
    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        fRandomLoadingTime = UnityEngine.Random.Range(5f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        fCurrentLoadingTime += Time.deltaTime;
        if (fCurrentLoadingTime > fRandomLoadingTime)
        {
            bool welcomePanelCheck = (PlayerPrefs.GetInt("WelcomePanel", 0) == 1) ? false : true ;

            if (welcomePanelCheck)
            {
                foreach (var panel in welcomePanel)
                {
                    panel.SetActive(true);
                }
            }

            gamePanel.SetActive(true);
            
            gameObject.SetActive(false);
        }

        loadingImage.fillAmount = fCurrentLoadingTime / fRandomLoadingTime;
    }
   
}

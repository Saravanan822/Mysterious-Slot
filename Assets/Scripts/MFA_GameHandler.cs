using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MFA_GameHandler : MonoBehaviour
{
    public Sprite[] symbolSprites;
    public static MFA_GameHandler instance;
    public bool bSpinning;
    public List<ReelsHandler> reels;
    public Image[] sCurrentSymbolIds = new Image[15];
    public PayLineData payLineData;
    public PayTableData payTableData;
    Dictionary<string, float[]> payTableAmts;
    public PayLineShowHandler payLineShowHandler;

    // money Handling start
    public float fCurrentBalance;
    public float fCurrentWin;
    public float[] fBetAmounts = {0.10f,1f,5f,10f,20f,25f};
    int iCurrentBetIndex;


    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI betText;
    // end


    // Welcom panel info start

    public GameObject[] welcomePanels;
    public Sprite tick;
    public Sprite unTick;
    public bool tickChecked;
    public Image tickImage;

    // end

    public TextMeshProUGUI slotInfo;
    public float fSpinTimer;


    // Constants Start

    const string TOTAL_WIN= "CurrentWin";
    const string TOTAL_BALANCE = "CurrentBalance";
    const string CURRENT_BET_INDEX = "CurrentBetIndex";
    const string WILD = "WILD";
    const string BONUS = "BONUS";
    const string SCATTER = "SCATTER";

    // end

    // Win Animation start

    public List<Image> animSymbols;
    bool makeAnim = true;

    public GameObject winImage;
    // end

    // special symbol handling

    bool freeSpins;
    int bonusMultiplier;
    public Image spinImage;
    public Sprite freeSpinSprite;
    public Sprite nrmSpinSprite;

    bool landscapeOrt = true;
    public GameObject bottomBar;

    // Help handling start

    public List<GameObject> disableForHelp;
    public List<GameObject> enableForHelp;
    public List <Sprite> helpHints;
    public Image hintImage;
    int currHelpIndex = 0;
    bool helpShown;
    // end


    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SoundHandler.instance.PlayMainAudio();

        fCurrentBalance = PlayerPrefs.GetFloat(TOTAL_BALANCE, 20000f);
        iCurrentBetIndex = PlayerPrefs.GetInt(CURRENT_BET_INDEX, 2);
        fCurrentWin = PlayerPrefs.GetFloat(TOTAL_WIN, 0);

        betText.text = fBetAmounts[iCurrentBetIndex].ToString();
        balanceText.text = fCurrentBalance.ToString();
        winText.text = fCurrentWin.ToString();

        payTableAmts = new Dictionary<string, float[]>();
        animSymbols = new List<Image>();

        // store paytable amount from the scrpitable to dictionary for payout calculations
        foreach(var symbol in payTableData.data)
        {
            payTableAmts[symbol.sSymbolName] = symbol.fAmount;
        }

        // change position of gameobjects mentioned in ppt during orientation changes by layout group
        if(Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            ChangeOrientation(false);
        }
        else ChangeOrientation(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (reels[reels.Count - 1].bSpinEnd && bSpinning)
        {
            makeAnim = true;
            bSpinning = false;
            SoundHandler.instance.StopReelsSpinAudio();
            SoundHandler.instance.PlayMainAudio();

            foreach (ReelsHandler reels in reels) //handle for collect symbols for each reel for payout calculations
            {
                int j = 0;
                for(int i = reels.iColumnId * 3; i < (reels.iColumnId * 3)+3 ; i++)
                {
                    sCurrentSymbolIds[i] = reels.sCurrentSymbolIds[j++];
                }
            }

            var currWin = CalculateWinAmount();

            if (currWin != 0)  // Handle for store and show animations for the results
            {
                if (bonusMultiplier > 0) currWin *= bonusMultiplier;

                slotInfo.text = "Won " + currWin.ToString() + (bonusMultiplier != 0 ? (" With " + bonusMultiplier + "X Bonus") : "") + (freeSpins ? " And Got Free Spin" : "");
                fCurrentWin += currWin;
                winText.text = fCurrentWin.ToString("0.0");
                PlayerPrefs.SetFloat(TOTAL_WIN, fCurrentWin);

                fCurrentBalance += currWin;
                balanceText.text = fCurrentBalance.ToString("0.0");
                PlayerPrefs.SetFloat(TOTAL_BALANCE, fCurrentBalance);

                winImage.SetActive(true);
                SoundHandler.instance.PlayWinAudio();
                Invoke("DisableWinImage", 2f);
            }
            else if (freeSpins)
            {
                SoundHandler.instance.PlayScatterAudio();
                slotInfo.text = "Got Free Spin";
            }
            else
            {
                SoundHandler.instance.PlayLoseAudio();
                slotInfo.text = "TRY AGAIN";
            }

            if (freeSpins)
            {
                spinImage.sprite = freeSpinSprite;
                spinImage.GetComponent<Animator>().SetBool("Blink", true);
            }


            
        }
        if(!makeAnim && animSymbols.Count > 0)
        {
            foreach (Image sym in animSymbols)
            {
                sym.GetComponent<Animator>().SetBool("Blink", false);
            }
            animSymbols.Clear();
        }
        if ((Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) && landscapeOrt)
        {
            ChangeOrientation(false);
        }
        else if((Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft) && !landscapeOrt) ChangeOrientation(true);

    }
    public void DisableWinImage()
    {
        winImage.SetActive(false);
        payLineShowHandler.ShowLines();
    }

    public void MakeSpin()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        if (!bSpinning)
        {

            if (!CheckAmtValidForSpin())
            {
                slotInfo.text = "Insufficent Balance";
                return;
            }

            slotInfo.text = "GOOD LUCK";

            fSpinTimer = Random.Range(5f, 7f);

            if(!freeSpins) ChangeBalanceAmt(fBetAmounts[iCurrentBetIndex] * 20f);

            freeSpins = false;
            spinImage.sprite = nrmSpinSprite;
            spinImage.GetComponent<Animator>().SetBool("Blink", false);

            bonusMultiplier = 0;
            bSpinning = true;
            SoundHandler.instance.PlayReelsSpinAudio();

            foreach (ReelsHandler reels in reels) { reels.MakeSpin(); }

            payLineShowHandler.Reset();

            makeAnim = false;
            
        }
    }

    public void IncreaseBetAmt()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        if (iCurrentBetIndex == fBetAmounts.Length - 1 || bSpinning) return;

        iCurrentBetIndex++;
        betText.text = fBetAmounts[iCurrentBetIndex].ToString();
        PlayerPrefs.SetInt(CURRENT_BET_INDEX, iCurrentBetIndex);
    }
    public void DecreaseBetAmt()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        if (iCurrentBetIndex == 0 || bSpinning) return;

        iCurrentBetIndex--;
        betText.text = fBetAmounts[iCurrentBetIndex].ToString();
        PlayerPrefs.SetInt(CURRENT_BET_INDEX, iCurrentBetIndex);
    }

    void ChangeBalanceAmt(float amt)
    {
        fCurrentBalance = fCurrentBalance - amt;
        balanceText.text = fCurrentBalance.ToString();
        PlayerPrefs.SetFloat(TOTAL_BALANCE, fCurrentBalance);
    }

    bool CheckAmtValidForSpin()
    {
        if(fBetAmounts[iCurrentBetIndex]*20f < fCurrentBalance || freeSpins) return true;
        else return false;
    }

    float CalculateWinAmount()
    {
        float returnAmt = 0;

        int scatterCount = 0;
        int bonusCount = 0;

        foreach (var symbol in sCurrentSymbolIds) { // handle for check special symbols availablity
            if (symbol.sprite.name == BONUS)
            {
                bonusCount++;
                animSymbols.Add(symbol);
            }
            else if (symbol.sprite.name == SCATTER)
            {
                scatterCount++;
                animSymbols.Add(symbol);
            }
        }

        if (scatterCount >= 3)
        {
            foreach (var symbol in animSymbols)
            {
                if(symbol.sprite.name == SCATTER) symbol.GetComponent<Animator>().SetBool("Blink", true);
            }
            freeSpins = true;
        }
        

        foreach (var line in payLineData.Payline) // handle for check symbols match with any of the paylines
        {
            string mainSymbol = "";
            List<Transform> transforms = new List<Transform>();
            for (int i = 0; i < line.pattern.Length; i++)
            {
                string currSymbol = sCurrentSymbolIds[line.pattern[i]].sprite.name;
                transforms.Add(sCurrentSymbolIds[line.pattern[i]].transform);

                if ((mainSymbol == "" || i < 3) && (currSymbol == BONUS || currSymbol == SCATTER)) break;

                if(mainSymbol == "" && currSymbol != WILD && currSymbol != BONUS && currSymbol != SCATTER) mainSymbol = currSymbol;
                if(mainSymbol != "" && currSymbol != WILD && mainSymbol != currSymbol)
                {
                    if (i > 2) // 3 and 4 symbol combinations
                    {
                        returnAmt += fBetAmounts[iCurrentBetIndex] * 20f * payTableAmts[mainSymbol][i - 3];
                        transforms.RemoveAt(transforms.Count - 1);
                        payLineShowHandler.SetLine(transforms);
                        
                    }
                    break;
                }
                if (i == 4) // 5 symbol combination
                {
                    returnAmt += fBetAmounts[iCurrentBetIndex] * 20f * payTableAmts[mainSymbol][2];
                    payLineShowHandler.SetLine(transforms);
                    break;
                }
            }
        }

        if (bonusCount > 0 && returnAmt > 0)
        {
            foreach (var symbol in animSymbols)
            {
                if (symbol.sprite.name == BONUS) symbol.GetComponent<Animator>().SetBool("Blink", true);
            }
            bonusMultiplier = bonusCount + 1;
        }

        return returnAmt;

    }

    public void CloseWelocomePanel()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        foreach (var panel in welcomePanels)
        {
            panel.SetActive(false);
        }
    }

    public void CheckWelcomePanel()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        if (!tickChecked)
        {
            tickChecked = true;
            tickImage.sprite = tick;
        }
        else
        {
            tickChecked = false;
            tickImage.sprite = unTick;
        }

        PlayerPrefs.SetInt("WelcomePanel",tickChecked ? 1 : 0);
    }

    public void ChangeOrientation(bool landScape)  
    {
        landscapeOrt = landScape;
        if(landScape && bottomBar.GetComponent<HorizontalLayoutGroup>() == null)
        {
            Destroy(bottomBar.GetComponent<VerticalLayoutGroup>());
            Invoke("AddHorizontalLayOut", 0.05f);

        }
        if (!landScape && bottomBar.GetComponent<VerticalLayoutGroup>() == null)
        {
            Destroy(bottomBar.GetComponent<HorizontalLayoutGroup>());
            Invoke("AddVerticalLayOut", 0.05f);
            

        }
    }
    public void AddHorizontalLayOut()
    {
        var grp = bottomBar.AddComponent<HorizontalLayoutGroup>();
        grp.childForceExpandHeight = false;
        grp.childForceExpandWidth = false;
        grp.childControlHeight = false;
        grp.childControlWidth = false;
        grp.childScaleHeight = false;
        grp.childScaleWidth = false;

        var rect = spinImage.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(350f,-25f,0);
        rect.sizeDelta = new Vector2(100f, 100f);

        payLineShowHandler.lineRenderer.startWidth = 0.13f;
        payLineShowHandler.lineRenderer.endWidth = 0.13f;
        foreach (var line in payLineShowHandler.lines)
        {
            line.GetComponent<LineRenderer>().startWidth = 0.13f;
            line.GetComponent<LineRenderer>().endWidth = 0.13f;
        }
    }
    public void AddVerticalLayOut()
    {
        var grp = bottomBar.AddComponent<VerticalLayoutGroup>();
        grp.childForceExpandHeight = false;
        grp.childForceExpandWidth = false;
        grp.childControlHeight = false;
        grp.childControlWidth = false;
        grp.childScaleHeight = false;
        grp.childScaleWidth = false;

        var rect = spinImage.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(240f, -175f, 0);
        rect.sizeDelta = new Vector2(200f, 200f);

        payLineShowHandler.lineRenderer.startWidth = 0.06f;
        payLineShowHandler.lineRenderer.endWidth = 0.06f;
        foreach (var line in payLineShowHandler.lines)
        {
            line.GetComponent<LineRenderer>().startWidth = 0.06f;
            line.GetComponent<LineRenderer>().endWidth = 0.06f;
        }
    }

    public void ShowHelp()
    {
        SoundHandler.instance.PlayBtnClickAudio();

        if (bSpinning) return;
        
        if (!helpShown)
        {
            hintImage.sprite = helpHints[0];
            currHelpIndex = 0;
            foreach (var obj in disableForHelp)
            {
                obj.SetActive(false);
            }
            foreach (var obj in enableForHelp)
            {
                obj.SetActive(true);
            }
            payLineShowHandler.HideLines();
            helpShown = true;
        }
        else
        {
            HideHelp();
        }
        
    }
    public void HideHelp()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        foreach (var obj in disableForHelp)
        {
            obj.SetActive(true);
        }
        foreach (var obj in enableForHelp)
        {
            obj.SetActive(false);
        }
        payLineShowHandler.ShowLines();
        helpShown = false;
    }
    public void NextHelpHint()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        if (currHelpIndex >= 3) return;

        currHelpIndex++;
        hintImage.sprite = helpHints[currHelpIndex];
    }
    public void PrevHelpHint()
    {
        SoundHandler.instance.PlayBtnClickAudio();
        if (currHelpIndex <= 0) return;

        currHelpIndex--;
        hintImage.sprite = helpHints[currHelpIndex];
    }
}

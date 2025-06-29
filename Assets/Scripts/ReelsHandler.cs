using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReelsHandler : MonoBehaviour
{
    float[] fSymbolPos = {209.6f, 104.6f, -0.4f, -105.4f, -210.4f };
    public GameObject[] gSymbols;
    public float fSpinSpeed; // random spin spped

    public float fSpinDuration; // constant spin time vary from other reels
    float fCurrentSpinDuration;
    public bool bSpinEnd;
    public int iColumnId;

    public Image[] sCurrentSymbolIds = new Image[3];
    // Start is called before the first frame update
    void Start()
    {
        bSpinEnd = true;
        foreach (var symbol in gSymbols) {
            var randSym = Random.Range(0,MFA_GameHandler.instance.symbolSprites.Length);
            symbol.GetComponent<Image>().sprite = MFA_GameHandler.instance.symbolSprites[randSym];
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (bSpinEnd) return;

        int iTopSym = 0;

        foreach (GameObject symbol in gSymbols)
        {
            symbol.transform.localPosition += (Vector3.down * Time.deltaTime * (fSpinSpeed));

            if(symbol.transform.localPosition.y <= fSymbolPos[fSymbolPos.Length - 1])
            {
                var diff = -symbol.transform.localPosition.y;
                diff = (diff - 210.4f); // Handle for position sync when rotate speed increases
                
                symbol.transform.localPosition = new Vector3(symbol.transform.localPosition.x, fSymbolPos[0]-diff, 0);

                var randSym = Random.Range(0, MFA_GameHandler.instance.symbolSprites.Length);
                symbol.GetComponent<Image>().sprite = MFA_GameHandler.instance.symbolSprites[randSym];

                if (fCurrentSpinDuration <= 0)
                {
                    iTopSym = int.Parse(symbol.transform.name);
                    
                    
                    bSpinEnd = true;
                }
            }
        }

        if (bSpinEnd)
        {
            // Handle for Reel stops perfectly when spin duration ends
            for (int i = 0; i < 4; i++)
            {
                if (iTopSym == 5) iTopSym = 1;
                gSymbols[iTopSym - 1].transform.localPosition = new Vector3(gSymbols[iTopSym - 1].transform.localPosition.x, fSymbolPos[i], 0);
                

                if(i != 0)
                {
                    sCurrentSymbolIds[i - 1] = gSymbols[iTopSym - 1].GetComponent<Image>(); // handle for storing resulting symbols for calculations
                }

                iTopSym++;
            }

            SoundHandler.instance.PlayReelsStopAudio();
        }

        fCurrentSpinDuration -= Time.deltaTime;
    }

    public void MakeSpin()
    {
        bSpinEnd = false;
        fCurrentSpinDuration = fSpinDuration+MFA_GameHandler.instance.fSpinTimer;
    }
}

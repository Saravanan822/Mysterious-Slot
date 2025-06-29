using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PayLineShowHandler : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<GameObject> lines;
    bool bShowLine;
    float bShowTimer = 1f;
    // Start is called before the first frame update
    void Start()
    {
        lines = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bShowLine && lines.Count > 0)
        {
            if (lines.Count > 1)
            {
                if (!lines[0].gameObject.activeSelf) lines[0].gameObject.SetActive(true);
                bShowTimer -= Time.deltaTime;
                if (bShowTimer < 0)
                {
                    bShowTimer = 1f;
                    var line = lines[0];
                    line.gameObject.SetActive(false);
                    lines.Remove(line);
                    lines.Add(line);
                }
            }
            else
            {
                bShowTimer -= Time.deltaTime;
                if (bShowTimer < 0)
                {
                    bShowTimer = 0.5f;
                    if (!lines[0].gameObject.activeSelf) lines[0].gameObject.SetActive(true);
                    else lines[0].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetLine(List<Transform> points)
    {
        LineRenderer line = Instantiate(lineRenderer);
        lines.Add(line.gameObject);
        line.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            line.SetPosition(i, points[i].position);
            points[i].GetComponent<Animator>().SetBool("Blink", true);
            MFA_GameHandler.instance.animSymbols.Add(points[i].GetComponent<UnityEngine.UI.Image>());
        }
        line.gameObject.SetActive(false);
    }
    public void Reset()
    {
        bShowLine = false;
        foreach (GameObject t in lines)
        {
            Destroy(t);
        }
        lines.Clear();
    }
    public void ShowLines()
    {
        bShowLine = true;
    }
    public void HideLines()
    {
        bShowLine = false;
        foreach (GameObject t in lines)
        {
            t.gameObject.SetActive(false);
        }
    }
}

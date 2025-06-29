using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PayLineData", menuName = "SlotGame/Paylines")]
public class PayLineData : ScriptableObject
{
    public List<Payline> Payline;
}

[System.Serializable]
public class Payline
{
    public int[] pattern; // List of Pattern having index of combinations
}

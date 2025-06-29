using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PayTableData", menuName = "SlotGame/PayTables")]
public class PayTableData : ScriptableObject
{
    public List<SymbolData> data;
}
[System.Serializable]
public class SymbolData
{
    public string sSymbolName;
    public float[] fAmount;
    
}

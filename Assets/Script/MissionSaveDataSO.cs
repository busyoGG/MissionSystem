using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MissionSaveDataSO")]
public class MissionSaveDataSO : ScriptableObject
{
    public List<MissionSaveData> _missionSaveDatas;

    public MissionSaveDataSO() { 
        _missionSaveDatas = new List<MissionSaveData>();
    }
}

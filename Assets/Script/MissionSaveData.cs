using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSaveData
{
    public int id { get; set; }

    public int m_id { get; set; }

    public Dictionary<int, float> complete_num { get; set; }

    public bool isDone { get; set; }

    public MissionSaveData()
    {
        complete_num = new Dictionary<int, float>();
    }
}

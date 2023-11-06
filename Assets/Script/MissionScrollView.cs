using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionScrollView : ScrollViewScript<MissionTree>
{
    public List<string> _subData;

    void Start()
    {
        base.Start();
        actions.Add(UpdateItemDataMission);
        actions.Add(UpdateItemDataType);
    }

    protected override void InitItemType()
    {
        List<int[]> type = new List<int[]>();

        List<List<int[]>> missionBucket = new List<List<int[]>>();

        for (int i = 0, len = _subData.Count; i < len; i++)
        {
            //type.Add(new int[] { 1, i });
            missionBucket.Add(new List<int[]>());
            //MissionFilter filter = MissionManager.Instance().GetMissionFileter(_subData[i]);
            //for(int j = 0,len2 = data.Count; j < len2; j++)
            //{

            //}
        }

        for (int i = 0, len = data.Count; i < len; i++)
        {
            //type.Add(new int[] { 0, i });
            missionBucket[(int)data[i].filter].Add(new int[] { 0, i });
        }

        for (int i = 0, len = missionBucket.Count; i < len; i++)
        {
            type.Add(new int[] { 1, i });
            for(int j = 0,len2 = missionBucket[i].Count; j < len2; j++)
            {
                type.Add(missionBucket[i][j]);
            }
        }

        itemType = type;
    }

    protected void UpdateItemDataMission(Button cell, int index)
    {
        TextMeshProUGUI content = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = data[index].title;

        cell.onClick.AddListener(() =>
        {
            EventManager.TriggerEventSticky("refresh_mission_panel", new ArrayList { data[index] });
        });
    }

    protected void UpdateItemDataType(Button cell, int index)
    {
        TextMeshProUGUI content = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = _subData[index];

        cell.onClick.AddListener(() =>
        {
            EventManager.TriggerEventSticky("refresh_mission_panel", new ArrayList { data[index] });
        });
    }
}

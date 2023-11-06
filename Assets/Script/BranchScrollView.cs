using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BranchScrollView : ScrollViewScript<MissionTree>
{
    public List<MissionTree> inputData;
    private void Start()
    {
        Debug.Log("初始化branch");
        base.Start();
        actions.Add(UpdateItemDataMission);
        this.data = inputData;
    }
    protected void UpdateItemDataMission(ItemTransformData cell, int index)
    {
        TextMeshProUGUI content = cell.item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = data[index].title;

        cell.item.onClick.RemoveAllListeners();
        cell.item.onClick.AddListener(() =>
        {
            EventManager.TriggerEventSticky("refresh_mission_panel", new ArrayList { data[index] });
            Debug.Log("展开任务面板");
        });
    }
}

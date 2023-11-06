using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BranchScrollView : ScrollViewScript<MissionTree>
{
    private bool _inited = false;
    public List<MissionTree> inputData
    {
        get
        {
            return _inputData;
        }
        set
        {
            _inputData = value;
            if(_inited ) { 
                this.data = value;
            }
        }
    }
    public List<MissionTree> _inputData;

    private void Start()
    {
        Debug.Log("初始化branch");
        base.Start();
        actions.Add(UpdateItemDataMission);
        this.data = _inputData;
        _inited = true;
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

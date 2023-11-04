using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionScrollView : ScrollViewScript<MissionTree>
{
    protected override void UpdateItemData(Button cell, int index)
    {
        TextMeshProUGUI content = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content.text = data[index].title;

        cell.onClick.AddListener(() =>
        {
            EventManager.TriggerEventSticky("refresh_mission_panel", new ArrayList { data[index] });
        });
    }
}

using System.Collections;
using TMPro;
using UnityEngine.UI;

public class AwardScrollView : ScrollViewScript<int>
{
    private void Start()
    {
        base.Start();
        actions.Add(UpdateItemData);
        EventManager.AddListening("target", "refresh_target", (ArrayList data) =>
        {
            RefreshList(this.data);
        });
    }
    protected void UpdateItemData(ItemTransformData cell, int index)
    {
        TextMeshProUGUI content = cell.item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        int id = (int)data[0];

        MissionTree mission = MissionManager.Instance().GetUnlockedMissionById(id);
        if (mission == null)
        {
            mission = MissionManager.Instance().GetUnlockedMissionById(id);
        }

        content.text = mission.award[index] + " ÊýÁ¿£º" + mission.award_num[index];
    }
}

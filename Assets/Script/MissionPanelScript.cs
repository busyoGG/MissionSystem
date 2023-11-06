using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanelScript : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public TargetScrollView ScrollTarget;
    public AwardScrollView ScrollAward;
    public Button Collect;
    public Button Complete;

    private Vector3 _defPos;

    private MissionTree _mission;

    void Start()
    {
        Debug.Log("³õÊ¼»¯panel");
        _defPos = transform.localPosition;

        Vector3 newPos = _defPos;
        newPos.x += 1000;
        transform.position = newPos;

        InitButtonEvent();
        InitEventListener();
    }

    private void InitButtonEvent()
    {
        Collect.onClick.AddListener(() =>
        {
            MissionManager.Instance().SetCompleteNum(_mission.m_id, 0, 1);
            EventManager.TriggerEvent("refresh_target", null);
        });

        Complete.onClick.AddListener(() =>
        {
            bool complete = MissionManager.Instance().CheckComplete(_mission);
            if (complete)
            {
                MissionManager.Instance().Next(_mission);
                EventManager.TriggerEvent("refresh_mission_list", null);
                Vector3 newPos = _defPos;
                newPos.x += 1000;
                transform.position = newPos;
            }
        });
    }

    private void InitEventListener()
    {

        EventManager.AddListening("panel", "refresh_mission_panel", (ArrayList data) =>
        {
            transform.localPosition = _defPos;
            MissionTree mission = (MissionTree)data[0];
            _mission = mission;

            Title.text = mission.title;
            Description.text = mission.describe;

            List<int> targets = new List<int>();
            for (int i = 0, len = mission.target_describe.Count; i < len; i++)
            {
                targets.Add(mission.m_id);
            }
            //Scroll.data = mission.target_describe;
            ScrollTarget.data = targets;

            RectTransform scrollRect = ScrollTarget.transform.GetComponent<RectTransform>();
            float posY = ScrollTarget.transform.parent.parent.localPosition.y - scrollRect.rect.height - 20;
            Vector3 pos = Description.rectTransform.localPosition;
            pos.y = posY;
            Description.rectTransform.localPosition = pos;

            List<int> awards = new List<int>();
            for (int i = 0, len = mission.award.Count; i < len; i++)
            {
                awards.Add(mission.m_id);
            }
            ScrollAward.data = awards;

            posY = Complete.transform.localPosition.y + ScrollAward.transform.GetComponent<RectTransform>().rect.height + 20;
            pos.y = posY;
            ScrollAward.transform.parent.parent.localPosition = pos;
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ScrollViewScript<T> : MonoBehaviour
{
    public Button PrefabItem;

    public GameObject Stack;

    public ScrollRect Scroll;

    public float SpaceX;

    public float SpaceY;

    public float OffsetX;

    public float OffsetY;

    public int RepeatX;

    public int RepeatY;

    public List<T> data
    {
        get
        {
            return _data;
        }
        set
        {
            RefreshList(value);
        }
    }

    private List<T> _data;

    private int _length;

    private List<Button> _cells = new List<Button>();

    private List<Button> _items = new List<Button>();

    private Stack<Button> _itemStack = new Stack<Button>();

    private float _itemHeight;

    private float _itemWidth;

    private Vector2 _tempVec2 = new Vector2();

    private RectTransform _rectTransform;

    private float _contentHeight;

    private float _viewHeight;

    private int _repeatX;

    private int _head;

    private int _tail = -1;

    private Vector2 _lastScrollPos = new Vector2();

    private int _scrollDirection = 0;

    public void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        for (int i = 0; i < 10; i++)
        {
            Button item = Instantiate(PrefabItem);
            item.transform.SetParent(Stack.transform);
            _itemStack.Push(item);
            _cells.Add(item);
            if (i == 0)
            {
                RectTransform rectTransform = item.GetComponent<RectTransform>();
                _itemHeight = rectTransform.rect.height;
                _itemWidth = rectTransform.rect.width;
                _repeatX = Mathf.FloorToInt(_rectTransform.rect.width / _itemWidth);
            }
        }

        Scroll.onValueChanged.AddListener(OnScroll);
    }

    private void OnScroll(Vector2 v)
    {
        //Debug.Log(v);
        float diff = _rectTransform.localPosition.y - _lastScrollPos.y;
        if (diff > 0)
        {
            _scrollDirection = 1;
        }
        else if (diff < 0)
        {
            _scrollDirection = -1;
        }
        else
        {
            _scrollDirection = 0;
        }

        if (_scrollDirection == 1)
        {
            Button item = _items[0];
            RectTransform rect = item.GetComponent<RectTransform>();
            float y = rect.localPosition.y - _itemHeight - SpaceY + (int)_rectTransform.localPosition.y;
            //Debug.Log(y);
            if (y > 0)
            {
                int index = _tail;
                if (index < _length)
                {
                    item.name = index + "";
                    _items.Remove(item);
                    _items.Add(item);
                    CalculatePosition(index);
                    rect.localPosition = _tempVec2;

                    UpdateItemData(item, index);
                    item.gameObject.SetActive(true);

                    _tail++;
                    _head++;
                }
            }
        }
        else if (_scrollDirection == -1)
        {
            Button item = _items[_items.Count - 1];
            RectTransform rect = item.GetComponent<RectTransform>();
            float y = rect.localPosition.y + (int)_rectTransform.localPosition.y + (int)_viewHeight;
            //Debug.Log(secondY);
            if (y < 0)
            {
                int index = _head;
                if (index >= 0)
                {
                    item.name = index + "";
                    _items.Remove(item);
                    _items.Insert(0, item);
                    CalculatePosition(index);
                    rect.localPosition = _tempVec2;

                    UpdateItemData(item, index);
                    item.gameObject.SetActive(true);

                    _head--;
                    _tail--;
                }
            }
        }

        _lastScrollPos.x = _rectTransform.localPosition.x;
        _lastScrollPos.y = _rectTransform.localPosition.y;
    }
    private Button CreateItem()
    {
        Button item;
        if (_itemStack.Count > 0)
        {
            item = _itemStack.Pop();
        }
        else
        {
            item = Instantiate(PrefabItem);
        }

        item.transform.SetParent(transform);
        _items.Add(item);

        return item;
    }

    /// <summary>
    /// 回收所有单元格
    /// </summary>
    private void RecycleAllCells()
    {
        _items.Clear();
        _itemStack.Clear();
        for (int i = 0, len = _cells.Count; i < len; i++)
        {
            Button item = _cells[i];
            item.transform.SetParent(Stack.transform);
            _itemStack.Push(item);
        }
    }

    private void CalculatePosition(int index)
    {
        float indX;
        float indY;
        if (RepeatX != 0)
        {
            indX = index % RepeatX;
            indY = Mathf.Floor(index / RepeatX);
        }
        else if (RepeatY != 0)
        {
            indX = Mathf.Floor(index / RepeatY);
            indY = index % RepeatY;
        }
        else
        {
            indX = index % _repeatX;
            indY = Mathf.Floor(index / _repeatX);
        }

        _tempVec2.x = _itemWidth * indX;
        _tempVec2.y = -_itemHeight * indY;

        _tempVec2.x += OffsetX + SpaceX * indX;
        _tempVec2.y -= OffsetY + SpaceY * indY;
    }

    protected void RefreshList(List<T> data)
    {
        RecycleAllCells();
        _data = data;
        int count = _data.Count;

        CalculatePosition(count);
        float maxH = -_tempVec2.y;
        _rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, maxH);

        _contentHeight = _rectTransform.rect.height;
        _viewHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        if (_contentHeight < _viewHeight)
        {
            _viewHeight = _contentHeight;
        }

        for (int i = 0, len = _cells.Count; i < len; i++)
        {
            if (i <= count)
            {
                Button item = CreateItem();
                CalculatePosition(i - 1);
                item.GetComponent<RectTransform>().localPosition = _tempVec2;
                item.name = (i - 1) + "";

                if (i - 1 >= 0)
                {
                    UpdateItemData(item, i - 1);
                }
                if (i == 0)
                {
                    item.gameObject.SetActive(false);
                }
                _tail++;
            }
        }

        _head = -2;

        _length = count;
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    protected virtual void UpdateItemData(Button cell, int index)
    {
        Debug.Log("更新数据" + index);
    }
}

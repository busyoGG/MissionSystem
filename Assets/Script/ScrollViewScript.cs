using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ScrollViewScript<T> : MonoBehaviour
{
    /// <summary>
    /// 单元格预制体
    /// </summary>
    public List<Button> PrefabItem;
    /// <summary>
    /// 单元格回收节点
    /// </summary>
    public GameObject Stack;
    /// <summary>
    /// 滚动节点
    /// </summary>
    public ScrollRect Scroll;
    /// <summary>
    /// 自适应大小
    /// </summary>
    public bool FitSize;
    /// <summary>
    /// X方向单元格间隔
    /// </summary>
    public float SpaceX;
    /// <summary>
    /// Y方向单元格间隔
    /// </summary>
    public float SpaceY;
    /// <summary>
    /// X方向偏移
    /// </summary>
    public float OffsetX;
    /// <summary>
    /// Y方向偏移
    /// </summary>
    public float OffsetY;
    /// <summary>
    /// X方向重复单元格个数
    /// </summary>
    public int RepeatX;
    /// <summary>
    /// Y方向重复单元格个数
    /// </summary>
    public int RepeatY;
    /// <summary>
    /// 默认列表数据
    /// </summary>
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
    /// <summary>
    /// 列表单元格类型
    /// </summary>
    protected List<int[]> itemType
    {
        get
        {
            return _itemType;
        }
        set
        {
            _itemType = value;
        }
    }

    protected List<Action<Button, int>> actions
    {
        get
        {
            return _actions;
        }
    }

    private List<int[]> _itemType = new List<int[]>();
    /// <summary>
    /// 默认列表数据
    /// </summary>
    private List<T> _data;
    /// <summary>
    /// 单元格虚拟数量最大值
    /// </summary>
    private int _length;
    /// <summary>
    /// 显示的单元格列表
    /// </summary>
    private List<ItemTransformData> _items = new List<ItemTransformData>();
    /// <summary>
    /// 回收的单元格列表
    /// </summary>
    private List<Stack<ItemTransformData>> _itemStack = new List<Stack<ItemTransformData>>();
    /// <summary>
    /// content面板transform
    /// </summary>
    private RectTransform _rectTransform;
    /// <summary>
    /// content视图高度
    /// </summary>
    private float _viewHeight;
    /// <summary>
    /// content视图宽度
    /// </summary>
    private float _viewWidth;

    //private int _repeatX;

    private ItemTransformData _head;

    private ItemTransformData _tail;

    private Vector2 _lastScrollPos = new Vector2();

    private List<Vector2> _itemPos = new List<Vector2>();

    private int _scrollDirection = 0;

    private List<Action<Button, int>> _actions = new List<Action<Button, int>>();

    public void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        _viewWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        _viewHeight = transform.parent.GetComponent<RectTransform>().rect.height;

        for (int i = 0, len = PrefabItem.Count; i < len; i++)
        {
            //创建回收栈节点对应类型的回收节点
            GameObject obj = new GameObject();
            obj.name = "ItemType_" + i;
            obj.transform.SetParent(Stack.transform);

            Button item = Instantiate(PrefabItem[i]);
            item.transform.SetParent(obj.transform);
            RectTransform itemRect = item.GetComponent<RectTransform>();

            ItemTransformData itd = new ItemTransformData();
            itd.item = item;
            itd.width = itemRect.rect.width;
            itd.height = itemRect.rect.height;
            itd.pos = new Vector2();
            itd.item_type = i;

            if (_itemStack.Count <= i)
            {
                _itemStack.Add(new Stack<ItemTransformData>());
            }
            _itemStack[i].Push(itd);

            for (int j = 0, len2 = (int)MathF.Ceiling(_viewHeight / itd.height); j < len2; j++)
            {
                Button item2 = Instantiate(PrefabItem[i]);
                item2.transform.SetParent(obj.transform);
                RectTransform itemRect2 = item.GetComponent<RectTransform>();

                ItemTransformData itd2 = new ItemTransformData();
                itd2.item = item2;
                itd2.width = itemRect2.rect.width;
                itd2.height = itemRect2.rect.height;
                itd2.pos = new Vector2();
                itd2.item_type = i;

                _itemStack[i].Push(itd2);
            }
        }

        Scroll.onValueChanged.AddListener(OnScroll);
    }

    private void OnScroll(Vector2 v)
    {
        ////Debug.Log(v);
        float rectY = _rectTransform.localPosition.y;
        float diff = rectY - _lastScrollPos.y;
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
            if (_head.pos.y - _head.height + rectY > 0)
            {
                if (_head.next != null)
                {
                    //Debug.Log("回收头" + _head.cell_index + " == " + _head.next.cell_index);
                    RecycleCell(_head.item_type, _head);
                    _head = _head.next;
                }
            }

            //Debug.Log(_tail.pos.y + rectY + " : " + _viewHeight);
            if (_tail.pos.y + rectY > -_viewHeight)
            {
                int index = _tail.cell_index + 1;
                if (index < _itemType.Count)
                {
                    int[] itemType = _itemType[index];
                    ItemTransformData item = CreateItem(itemType[0]);
                    Vector2 pos = _itemPos[index];
                    item.cell_index = index;
                    item.item_index = itemType[1];
                    item.parent = _tail;
                    _tail.next = item;
                    item.pos = pos;
                    item.item.transform.localPosition = pos;
                    item.item.name = index + "";
                    _tail = item;
                    Debug.Log("创建尾" + _tail.cell_index + " " + pos + " " + index);
                    _actions[itemType[0]](item.item, item.item_index);
                }
            }
        }
        else if (_scrollDirection == -1)
        {
            //Debug.Log(_head.pos.y - _head.height + rectY);
            if (_head.pos.y - _head.height + rectY < 0)
            {
                int index = _head.cell_index - 1;
                if (index >= 0)
                {
                    int[] itemType = _itemType[index];
                    ItemTransformData item = CreateItem(itemType[0]);
                    Vector2 pos = _itemPos[index];
                    item.cell_index = index;
                    item.item_index = itemType[1];
                    item.next = _head;
                    _head.parent = item;
                    item.pos = pos;
                    item.item.transform.localPosition = pos;
                    item.item.name = index + "";
                    _head = item;
                    Debug.Log("创建头" + _head.cell_index + " " + pos + " " + index);
                    _actions[itemType[0]](item.item, item.item_index);
                }
            }

            //Debug.Log(_tail.pos.y + rectY + " : " + -_viewHeight);
            if (_tail.pos.y + rectY < -_viewHeight)
            {
                if (_tail.parent != null)
                {
                    //Debug.Log("回收尾" + _tail.cell_index);
                    RecycleCell(_tail.item_type, _tail);
                    _tail = _tail.parent;
                }
            }
        }

        _lastScrollPos.x = _rectTransform.localPosition.x;
        _lastScrollPos.y = _rectTransform.localPosition.y;
    }

    /// <summary>
    /// 创建ItemTransformData
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    private ItemTransformData CreateItem(int itemType, bool init = true)
    {
        ItemTransformData item;
        if (_itemStack[itemType].Count > 0)
        {
            item = _itemStack[itemType].Pop();
        }
        else
        {
            Button button = Instantiate(PrefabItem[itemType]);
            item = new ItemTransformData();
            item.item = button;
        }

        if (init)
        {
            item.item.transform.SetParent(transform);
            _items.Add(item);
        }
        else
        {
            _itemStack[itemType].Push(item);
        }

        return item;
    }

    /// <summary>
    /// 回收所有单元格
    /// </summary>
    private void RecycleAllCells()
    {
        for (int i = 0, len = _items.Count; i < len; i++)
        {
            ItemTransformData item = _items[i];
            item.item.transform.SetParent(Stack.transform.GetChild(item.item_type));
            _itemStack[item.item_type].Push(item);
        }
        _items.Clear();
        _itemPos.Clear();
    }

    /// <summary>
    /// 回收一个单元格
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itd"></param>
    private void RecycleCell(int itemType, ItemTransformData itd)
    {
        itd.item.transform.SetParent(Stack.transform.GetChild(itemType));
        //itd.next = null;
        //itd.parent = null;
        _items.Remove(itd);
        _itemStack[itemType].Push(itd);
    }

    private Vector2 CalculatePosition(ItemTransformData last, ItemTransformData current)
    {

        float bonusX = 0;
        float bonusY = 0;
        if (RepeatX != 0)
        {
            if (current.cell_index == 0)
            {
                bonusX = OffsetX;
            }
            else if (current.cell_index / RepeatX == 0)
            {
                bonusX = last.width + SpaceX + OffsetX;
            }
            else
            {
                bonusY = last.height + SpaceY + OffsetY;
            }
        }
        else if (RepeatY != 0)
        {
            if (current.cell_index == 0)
            {
                bonusY = OffsetY;
            }
            else if (current.cell_index / RepeatY == 0)
            {
                bonusY = last.height + SpaceY + OffsetY;
            }
            else
            {
                bonusX = last.width + SpaceX + OffsetX;
            }
        }
        else
        {
            if (current.cell_index == 0)
            {
                bonusX = OffsetX;
            }
            else if (last.pos.x + last.width + SpaceX + current.width <= _viewWidth)
            {
                bonusX = last.width + SpaceX + OffsetX;
            }
            else
            {
                bonusY = last.height + SpaceY + OffsetY;
            }
        }

        //_tempVec2.x = last.pos.x + bonusX;
        //_tempVec2.y = last.pos.y - bonusY;
        Vector2 pos = new Vector2(last.pos.x + bonusX, last.pos.y - bonusY);
        return pos;
    }

    protected void RefreshList(List<T> data)
    {
        Debug.Log("设置数据");
        RecycleAllCells();
        _data = data;

        InitItemType();

        Resize();

        //_head = -2;

        //_length = count;

    }

    private void Resize()
    {
        if (FitSize)
        {
            _viewWidth = transform.parent.GetComponent<RectTransform>().rect.width;
            _viewHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        }

        ItemTransformData last = new ItemTransformData();
        int i = 0;
        float maxY = 0;
        float maxHeight = 0;
        while (i < itemType.Count)
        {
            int type = itemType[i][0];

            bool isInit = maxY > -_viewHeight;
            ItemTransformData itd = CreateItem(type, isInit);
            itd.cell_index = i;
            itd.item_index = itemType[i][1];
            itd.item.name = i + "";

            if (isInit)
            {
                _actions[type](itd.item, itd.item_index);
            }

            Vector2 pos = CalculatePosition(last, itd);
            _itemPos.Add(pos);
            itd.item.gameObject.GetComponent<RectTransform>().localPosition = pos;
            itd.pos = pos;
            last.next = itd;
            if (i != 0)
            {
                itd.parent = last;
            }
            last = itd;

            maxY = pos.y;
            maxHeight = pos.y - itd.height;

            i++;
            Debug.Log("设置item " + (i - 1) + "  " + maxHeight + " " + pos.y + " " + itd.height);
        }


        _head = _items[0];
        _tail = _items[_items.Count - 1];


        //更新UI大小
        _rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, -maxHeight);

        if (FitSize)
        {
            float contentHeight = _rectTransform.rect.height;

            if (contentHeight < _viewHeight)
            {
                _viewHeight = contentHeight;
            }
        }
    }

    /// <summary>
    /// 初始化单元格类型
    /// </summary>
    /// <param name="itemType">int[2] 0为button类型 1为对应类型的数据索引</param>
    protected virtual void InitItemType()
    {
        List<int[]> type = new List<int[]>();
        for (int i = 0, len = _data.Count; i < len; i++)
        {
            type.Add(new int[] { 0, i });
        }

        itemType = type;
    }
}

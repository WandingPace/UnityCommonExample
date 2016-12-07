using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIControlPVPScrollItem : EventTrigger
{
    public Callback<Vector2> onDrag;
    public Callback onEndDrag;

    private int curveOffSetIndex = 0;
    public int CurveOffSetIndex
    {
        get { return this.curveOffSetIndex; }
        set { this.curveOffSetIndex = value; }
    }

    private float dCurveCenterOffset = 0.0f;
    public float CenterOffSet
    {
        get { return this.dCurveCenterOffset; }
        set { dCurveCenterOffset = value; }
    }

    private bool _isSelect = false;
    public bool isSelect 
    {
        get { return _isSelect; }
        set
        {
            _isSelect = value;
        }
    }
    private PVPScrollItemInfo _pvpBattleResultInfo = null;
    public PVPScrollItemInfo pvpBattleResultInfo
    {
        get
        {
            return _pvpBattleResultInfo;
        }
        set
        {
            _pvpBattleResultInfo = value;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI Event
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (onDrag != null)
            onDrag(eventData.delta);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (onEndDrag != null)
            onEndDrag();
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIControlPVPScrollBattleRecord : MonoBehaviour
{
    private const string UI_CONTROL_PVP_SCROLLITEM = "UIControlPVPScrollItem";
    [SerializeField]
    private AnimationCurve _curvePosX;
    [SerializeField]
    private  AnimationCurve _curvePosY;
    [SerializeField]
    private  AnimationCurve _curveScale;


    [SerializeField]
    private RectTransform _rtfScrollRoot;

    [SerializeField]
    private List<UIControlPVPScrollItem> uiControlPVPScrollItems = new List<UIControlPVPScrollItem>();

    private UIControlPVPScrollItem curRecordItem = null;
    private UIControlPVPScrollItem preRecordItem = null;

    private float dFactor = 0.2f;

    // Lerp duration
    public float lerpDuration = 0.2f;
    private float mCurrentDuration = 0.0f;
    private int mCenterIndex = 0;
    public bool enableLerpTween = true;

    // originHorizontalValue Lerp to horizontalTargetValue
    private float originHorizontalValue = 0.1f;
    public float curHorizontalValue = 0.5f;

    // Use this for initialization
    void Start()
    {
        int count = uiControlPVPScrollItems.Count;
         dFactor = (Mathf.RoundToInt((1f / count) * 10000f)) * 0.0001f;
        mCenterIndex = count / 2;
        if (count % 2 == 0)
            mCenterIndex = count / 2 - 1;
        int index = 0;
        for (int i = count - 1; i >= 0; i--)
        {
            uiControlPVPScrollItems[i].CurveOffSetIndex = i;
            uiControlPVPScrollItems[i].CenterOffSet = dFactor * (mCenterIndex - index);
            uiControlPVPScrollItems[i].onDrag = OnDragViewMove;
            uiControlPVPScrollItems[i].onEndDrag = OnDragEnhanceViewEnd;
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enableLerpTween)
        {
            mCurrentDuration += Time.deltaTime;
            if (mCurrentDuration > lerpDuration)
                mCurrentDuration = lerpDuration;

            float percent = mCurrentDuration / lerpDuration;
            float value = Mathf.Lerp(originHorizontalValue, curHorizontalValue, percent);
            UpdateScrollView(value);
            if (mCurrentDuration >= lerpDuration)
            {
                enableLerpTween = false;
            }
        }
    }
    private float GetPosXValue(float sliderValue, float added)
    {
        float evaluateValue = _curvePosX.Evaluate(sliderValue + added)*500;//to do

        return evaluateValue;
    }
    private float GetPosYValue(float sliderValue, float added)
    {
        float evaluateValue = _curvePosY.Evaluate(sliderValue + added)*500; // to do

        return evaluateValue;
    }
    private float GetScaleValue(float sliderValue, float added)
    {
        float evaluateValue = _curveScale.Evaluate(sliderValue + added);

        return evaluateValue;
    }

    private void LerpTweenToTarget(float originValue, float targetValue, bool needTween = false)
    {
        if (!needTween)
        {
            originHorizontalValue = targetValue;
            UpdateScrollView(targetValue);
        }
        else
        {
            originHorizontalValue = originValue;
            curHorizontalValue = targetValue;
            mCurrentDuration = 0.0f;
        }
        enableLerpTween = needTween;
    }
    public void UpdateScrollView(float fValue)
    {
        for (int i = 0; i < uiControlPVPScrollItems.Count; i++)
        {
            UIControlPVPScrollItem itemScript = uiControlPVPScrollItems[i];
            float yValue = GetPosYValue(fValue, itemScript.CenterOffSet);
            float xValue = GetPosXValue(fValue, itemScript.CenterOffSet);
            float scaleValue = GetScaleValue(fValue, itemScript.CenterOffSet);

            Vector3 targetPos = Vector3.one;
            Vector3 targetScale = Vector3.one;
            // position
            targetPos.x = xValue;
            targetPos.y = yValue;
            //scale
            targetScale.x = targetScale.y = scaleValue;

            itemScript.transform.localPosition = targetPos;
            itemScript.transform.localScale = targetScale;

        }
    }

    #region Interface

    public void SetTargetItemIndex(UIControlPVPScrollItem selectItem)
    {
 
    }

    public void Init(List<PVPScrollItemInfo> infos)
    {
        int count = infos.Count;
        if (uiControlPVPScrollItems.Count > count)
        {
            for (int i = count; i < uiControlPVPScrollItems.Count; i++)
            {
                Destroy(uiControlPVPScrollItems[i]);
            }
            uiControlPVPScrollItems.RemoveRange(count, uiControlPVPScrollItems.Count - count);
        }
        else if (uiControlPVPScrollItems.Count < count)
        {
            uiControlPVPScrollItems.Add(this.CreateScrollItem());
        }

        for (int i = 0; i < infos.Count; i++)
        {
            uiControlPVPScrollItems[i].pvpBattleResultInfo = infos[i];
        }
   
    }
    private UIControlPVPScrollItem CreateScrollItem()
    {
        GameObject inst = null;
        UIControlPVPScrollItem item = null;
        GameObject template = Resources.Load<GameObject>(UI_CONTROL_PVP_SCROLLITEM);
        inst = GameObject.Instantiate(template);
        item = inst.GetComponent<UIControlPVPScrollItem>();
        item.GetComponent<RectTransform>().SetParent(_rtfScrollRoot, false);
        inst.gameObject.SetActive(true);
        return item;
    }
    
    #endregion

    #region Event
    public void OnDragViewMove(Vector2 delta)
    {
        // In developing
        if (Mathf.Abs(delta.x) > 0.0f)
        {
            curHorizontalValue += delta.x * 0.001f;
            LerpTweenToTarget(0.0f, curHorizontalValue, false);
        }
    }

    public void OnDragEnhanceViewEnd()
    {
        // find closed item to be centered
        int closestIndex = 0;
        float value = (curHorizontalValue - (int)curHorizontalValue);
        float min = float.MaxValue;
        float tmp = 0.5f * (curHorizontalValue < 0 ? -1 : 1);
        for (int i = 0; i < uiControlPVPScrollItems.Count; i++)
        {
            float dis = Mathf.Abs(Mathf.Abs(value) - Mathf.Abs((tmp - uiControlPVPScrollItems[i].CenterOffSet)));
            if (dis < min)
            {
                closestIndex = i;
                min = dis;
            }
        }
        originHorizontalValue = curHorizontalValue;
        float target = ((int)curHorizontalValue + (tmp - uiControlPVPScrollItems[closestIndex].CenterOffSet));
        LerpTweenToTarget(originHorizontalValue, target, true);
    } 
    #endregion
}

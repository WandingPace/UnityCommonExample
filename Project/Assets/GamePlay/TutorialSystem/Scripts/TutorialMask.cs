using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class TutorialMask : MonoBehaviour 
{
    [SerializeField]
    private RectTransform leftBorder;
    [SerializeField]
    private RectTransform rightBorder;
    [SerializeField]
    private RectTransform topBorder;
    [SerializeField]
    private RectTransform bottomBorder;
    [SerializeField]
    private RectTransform window;

    [SerializeField]
    private Vector4 location;
    public Vector4 Location
    {
        get { return location; }
        set
        {
            location = new Vector4(value.x, value.y, Mathf.Max(0f, value.z), Mathf.Max(0f, value.w));
            leftBorder.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x / 2f + location.x - location.z / 2f);
            rightBorder.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x / 2f - location.x - location.z / 2f);
            topBorder.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, rectTransform.sizeDelta.x / 2f + location.x - location.z / 2f, location.z);
            bottomBorder.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, rectTransform.sizeDelta.x / 2f + location.x - location.z / 2f, location.z);
            topBorder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y / 2f - location.y - location.w / 2f);
            bottomBorder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y / 2f + location.y - location.w / 2f);
            window.localPosition = new Vector3(location.x, location.y);
            window.sizeDelta = new Vector2(location.z, location.w);
        }
    }

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    
    [SerializeField]
    private float alpha;
    public float Alpha
    {
        get { return alpha; }
        set { alpha = Mathf.Clamp01(value); canvasGroup.alpha = alpha; }
    }

    [SerializeField]
    private bool blockRaycast;
    public bool BlockRaycast
    {
        get { return blockRaycast; }
        set 
        {
            blockRaycast = value;  
            canvasGroup.blocksRaycasts = blockRaycast;
            canvasGroup.interactable = blockRaycast;
        }
    }

	// Use this for initialization
	void Start () 
    {
        rectTransform = this.GetComponent<RectTransform>();
        canvasGroup = this.GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isEditor)
        {
            Location = location;
            Alpha = alpha;
            BlockRaycast = blockRaycast;
        }
	}
}

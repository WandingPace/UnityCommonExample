using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class testRectTransfrom : BaseMeshEffect
{

    private RectTransform rtf;
    private Image _img;
    public Transform[] pos = new Transform[5];
	// Use this for initialization
	void Start () {
        rtf = this.gameObject.GetComponent<RectTransform>();
       // Debug.Log(rtf.offsetMax);
        //anchoredPosition为pivot相对anchor的位置
        //recttransform两个set size 方法
        //rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,100);
        //rtf.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 100, 200);
        // transform的position 经过装箱操作 使用set无效果
        //rtf.transform.position.Set();
        //Vector3 vet = new Vector3(1, 1, 1);
        //vet.Set(2,2,2);
        //vet.x = 10;
        //rtf.transform.localPosition = vet;
	}
	
	// Update is called once per frame
    // 自己手动刷新
    void Update()
    {
        //SetNativeSize();
    }
    //protected override void OnPopulateMesh(VertexHelper vh)
    //{
    //    Color32 color32 = new Color32(1,0,0,1);
    //    vh.Clear();
    //    // 这里我用5对GameObject的坐标来与该Image对象的五个顶点绑定起来
    //    // AddVert的最后一个参数是UV值

    //    vh.AddVert(pos[0].position, color32, new Vector2(0f, 0f));
    //    vh.AddVert(pos[1].position, color32, new Vector2(0f, 1f));
    //    vh.AddVert(pos[2].position, color32, new Vector2(1f, 1f));
    //    vh.AddVert(pos[3].position, color32, new Vector2(1f, 0f));
    //    vh.AddVert(pos[4].position, color32, new Vector2(0.5f, 0f));

    //    vh.AddTriangle(0, 1, 2);
    //    vh.AddTriangle(2, 3, 4);
    //    vh.AddTriangle(2, 4, 0);
    //}
    public override void ModifyMesh(VertexHelper vh)
    {
        Color32 color32 = new Color32(255, 0, 0, 25);
        vh.Clear();
        // 这里我用5对GameObject的坐标来与该Image对象的五个顶点绑定起来
        // AddVert的最后一个参数是UV值

        vh.AddVert(pos[0].localPosition, color32, new Vector2(0f, 0f));
        vh.AddVert(pos[1].localPosition, color32, new Vector2(0f, 1f));
        vh.AddVert(pos[2].localPosition, color32, new Vector2(1f, 1f));
        vh.AddVert(pos[3].localPosition, color32, new Vector2(1f, 0f));
        vh.AddVert(pos[4].localPosition, color32, new Vector2(0.5f, 0f));

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 4);
        vh.AddTriangle(2, 4, 0);
    }
}

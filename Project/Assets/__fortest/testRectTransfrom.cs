using UnityEngine;
using System.Collections;

public class testRectTransfrom : MonoBehaviour {

    private RectTransform rtf;
	// Use this for initialization
	void Start () {
        rtf = this.gameObject.GetComponent<RectTransform>();
        Debug.Log(rtf.offsetMax);
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
	void Update () {
	
	}
}

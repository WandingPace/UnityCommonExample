using UnityEngine;
using System.Collections;

public class GuiTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    bool toggle = true;
    int toolBarIndex = 0;
    void OnGUI()
    {
        //GUI.Changed判断是否对GUI作出操作

        //文本域
        string text = "textField";
        //文本区域 多行
        text = GUI.TextField(new Rect(25,25,100,30),text);
        

        toggle = GUI.Toggle(new Rect(25,50,100,30),toggle,"Toggle");
        //工具栏
        toolBarIndex = GUI.Toolbar(new Rect(25,80,250,30),toolBarIndex,new string[]{"Tool1","Tool2","Tool3"});
        //工具栏表格 SelectionGrid

        //滑动条 HorizontalSlider VerticalSlider 

        //滚动视图 ScrollView 

        GUILayout.BeginArea(new Rect(25,120,200,60));
        GUILayout.BeginHorizontal();

         GUILayout.BeginVertical();
         GUILayout.Box("Test1");
         GUILayout.Space(10);
         GUILayout.Box("Test2");
         GUILayout.EndVertical();

         GUILayout.Space(20);

         GUILayout.BeginVertical();
         GUILayout.Box("Test3");
         GUILayout.Space(10);
         GUILayout.Box("Test4");
         GUILayout.EndVertical();
         text = GUILayout.TextArea(text, 200);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}

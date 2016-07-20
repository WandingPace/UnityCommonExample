using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;
public class TestManager : MonoBehaviour {
    void Awake()
    {
        
    }
	// Use this for initialization
	void Start () {
        EventDispatcher.Instance.RegistEventListener(EventNames.Event1, OnListener);
	}
	
	// Update is called once per frame
	void Update () {
        EventDispatcher.Instance.OnTick();
	}
    bool isShowWindow = false;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 150, 400));
        GUILayout.BeginVertical();
        if (GUILayout.Button("Event"))
        {
            EventDispatcher.Instance.DispatchEvent(EventNames.Event1, 0);
        }
        GUILayout.ExpandHeight(true);
        if (GUILayout.Button("Messagebox"))
        {
            StartCoroutine(EnterProc());
        }
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Load"))
        {
            UserProfile profile = UserProfileManager.instance.Load("testPlayer1");

            if (profile == null)
            {
                profile = new UserProfile();
                UserProfile.current = profile;
                UserProfile.current.account.username = "null";
                UserProfile.current.account.id = "testPlayer1";
                UserProfileManager.instance.Save();
            }
            Debug.Log(string.Format("<color=orange>Load User Name:{0}</color>",UserProfile.current.account.username));
        }
        if (GUILayout.Button("Set"))
        {
            isShowWindow = true;
        }
        if (GUILayout.Button("Delete"))
        {

        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reflect"))
        {
            /*获取type三种常见方式
             * typeof(string) 
             * 对象.GetType()
             * Type.GetType("");
             * 
             * 
             */
            Type t = Type.GetType("ReflectTest");
            ConstructorInfo[] ci = t.GetConstructors();
            MemberInfo[] mis = t.GetMembers();
            //public 字段
            FieldInfo[] fi = t.GetFields();
            MethodInfo[] mei = t.GetMethods();
            //private 属性
            PropertyInfo[] pi = t.GetProperties();
            object o = Activator.CreateInstance(t, 1); 

            //Debug.Log(string.Format("ConstructorInfo:{0} para: {1}| MemberInfo:{2} FieldInfo:{3} MethodInfo:{4} PropertyInfo{5}"
            //                         , ci[0].Name,ci[0].GetParameters()[0],mis[0].Name,fi[0].Name,mei[0].Name,pi[0].Name));

            MethodInfo mi = typeof(TestManager).GetMethod("OnReflectFunc", BindingFlags.Public | BindingFlags.Static);
            mi.Invoke(null, null);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (isShowWindow)
            GUILayout.Window(0, new Rect(50, 50, 400, 300), OnGUIWindowFunc, "mywindow");

    }
    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventNames.Event1, OnListener);
    }

    void OnListener(EventBase eb)
    {
        Debug.Log("DisPatch event1");
    }
    #region UI Process
    IEnumerator EnterProc()
    {
        AsyncDialogResult dr = new AsyncDialogResult();
        MessageBox mb = MessageBox.Show("Message", string.Format("Connection Failed, Do you want to play offline?"), (int)(MBButton.Option1 | MBButton.Option2), "OK", "Retry");
        mb.onResult = (MessageBox box, MBButton btnId) =>
        {
            dr.clicked = btnId;
        };
        yield return StartCoroutine(dr);
        if (dr.clicked == MBButton.Option1)
        {
            Debug.Log("Click");
        }
    }
    #endregion
    #region UI Event
    void OnClickEventBtn()
    {
        EventDispatcher.Instance.DispatchEvent(EventNames.Event1, 0);
    }

    string saveText = "";
    void OnGUIWindowFunc(int windowID)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();  
        if (GUILayout.Button("close"))
        {
            isShowWindow = false;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("user name:");
        saveText = GUILayout.TextField(saveText,GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("save"))
        {
            UserProfile.current.account.username = saveText;
            UserProfileManager.instance.Save();
        }
    }
    public static void OnReflectFunc()
    {
        Debug.Log("OnReflectFunc");
    }
    #endregion
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Test()
    {
        string json = "{\"score\":1}";
        LitJson.JsonData data = LitJson.JsonMapper.ToObject(json);
        Debug.Log("JsonData"+(int)data["score"]);
    }
}
public class ReflectTest
{
    public ReflectTest(int i)
    {
        Debug.Log(" ReflectTest instance para:" + i);
    }

    private string _testprivate = "private";
    public string _testPublic = "public";
    public string testprivate
    {
        get { return _testprivate; }
        set { _testprivate = value; }
    }
    public void Method(int i)
    {
 
    }
}

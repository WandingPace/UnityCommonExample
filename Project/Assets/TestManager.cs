using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using System;
using System.IO;
using UnityEditor;
using System.Text;
using System.Linq;
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
        #region user Profile
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
            Debug.Log(string.Format("<color=orange>Load User Name:{0}</color>", UserProfile.current.account.username));
        }
        if (GUILayout.Button("Set"))
        {
            isShowWindow = true;
        }
        if (GUILayout.Button("Delete"))
        {

        } 
        GUILayout.EndHorizontal();
        #endregion
        #region Attribute
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
            //有参构造
            object o = Activator.CreateInstance(t, 1);//type.Assembly.CreateInstance()
            //跨程序集
            //object o1 = Assembly.Load(".DLL")| Assembly.GetExecutingAssembly().CreateInstance("namespace.class");


            MethodInfo mi = typeof(TestManager).GetMethod("OnReflectFunc", BindingFlags.Public | BindingFlags.Static);
            mi.Invoke(null, null);
        }
        if (GUILayout.Button("exception"))
        {
            try
            {
                throw new System.DivideByZeroException();
            }
            catch (DivideByZeroException e)
            {
                Debug.Log("Attempted divide by zero.");
            }

        } 
        #endregion
        #region File IO
        if (GUILayout.Button("FileIO"))
        {

            string path = Path.Combine(Application.persistentDataPath, "new");
            Directory.CreateDirectory(path);
            if (Directory.Exists(path))
            {
                //Directory.Delete(path);

            }

            foreach (var item in Directory.GetFileSystemEntries(Application.persistentDataPath))
            {
                Debug.Log(item);
            }
            foreach (var item in Directory.GetFiles(Application.persistentDataPath))
            {
                Debug.Log(item);
            }
            //write
            string filepath = Path.Combine(path, "new.txt");
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("1123123");
                sw.WriteLine("2123123");
            }
            //read
            string ret = "";
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(filepath))
            {
                
                //1
                //string readbyte = sr.ReadLine();
                //while(readbyte!=null)
                //{
                //    ret += readbyte;
                //    readbyte = sr.ReadLine();

                //}
                //2
                //char[] buffer = new char[1024];
                //int readbyte = 0;
                //while ((readbyte = sr.Read(buffer, 0, buffer.Length)) != 0)
                //{
                //    sb.Append(buffer);
                //}
                //*3*
                using (BinaryReader br = new BinaryReader(sr.BaseStream))
                {
                    byte[] buffer = new byte[5];
                    int readbyte = 0;
                    while ((readbyte = br.Read(buffer, 0, buffer.Length)) != 0)
                    {  
                        sb.Append(buffer);               
                    }
                }
            }
            //Encoding.Default.GetString(sb);
            Debug.Log(string.Format("{0}", sb));
        } 
        #endregion
        #region Simple test
        if (GUILayout.Button("test"))
        {
            string s1 = "qwe";
            string s2 = s1;
            s2 = "1";
            Debug.Log(s1+"|"+s2);
        }
        #endregion
        #region WWW
        if(GUILayout.Button("WWW"))
        {
            StartCoroutine(StartWWWGet());
        }
        #endregion
        #region TempTest
        if (GUILayout.Button("TempTest"))
        {
            
        }
        #endregion

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
    void Gen()
    {
 
    }
    #region UI Process
    IEnumerator EnterProc()
    {
        AsyncDialogResult dr = new AsyncDialogResult();
        MessageBox mb = MessageBox.Show("Message", string.Format("Connection Failed, Do you want to play offline?"), (int)(MBButton.Option1 | MBButton.Option2), "OK", "Retry");
        //mb.onResult = (MessageBox box, MBButton btnId) =>
        //{
        //    //dr.clicked = btnId;
        //};
        //yield return StartCoroutine(dr);
        //if (dr.clicked == MBButton.Option1)
        //{
        //    Debug.Log("Click");
        //}
        yield break;
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
 
    IEnumerator StartWWWGet()
    {
        string url = "http://pic.sc.chinaz.com/files/pic/pic9/201508/apic14052.jpg";
        WWW www = new WWW(url);
        yield return www;
        //Renderer renderer = UIManager.instance._imgbg.GetComponent<Renderer>();

        UIManager.instance._imgbg.sprite = Sprite.Create(www.texture, new Rect(0,0,www.texture.width,www.texture.height), new Vector2(0.5f, 0.5f));
        //renderer.material.mainTexture = www.texture;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Test()
    {
        string json = "{\"score\":1}";
        LitJson.JsonData data = LitJson.JsonMapper.ToObject(json);
        Debug.Log("JsonData"+(int)data["score"]);
    }

    #region MenuItem
    [MenuItem("Folder/Open")]
    public static void OpenFile()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
        //File.Open(Application.persistentDataPath, FileMode.Open);
    }

    #endregion
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
        RectTransform rect = null;
        
    }
}

public class General
{
    public General()
    {
        name = "";
        age = -1;
    }
   public string name ;
    public int age;
}

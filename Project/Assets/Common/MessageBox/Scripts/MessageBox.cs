using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AsyncDialogResult : IEnumerator
{
    public MBButton clicked = MBButton.None;

    public AsyncDialogResult()
    {

    }

    public object Current { get { return null; } }

    public bool MoveNext() { return clicked == MBButton.None; }
    public void Reset() { clicked = MBButton.None; }
}

public enum MBButton : int
{
    None = 0,
    Close = 1,
    Option1 = 2,
    Option2 = 4,
    Option3 = 8,
    Default = Option1 | Option2,
}
public delegate void Callback();
public delegate void Callback<T0>(T0 arg0);
public delegate void Callback<T0, T1>(T0 arg0, T1 arg1);
public delegate void Callback<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
public delegate void Callback<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);
public delegate void Callback<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
/// <summary>
/// Common message box.
/// </summary>
public class MessageBox : MonoBehaviour
{
    public Callback<MessageBox, MBButton> onResult = null;



    [SerializeField]
    private Button _btnClose = null;
    [SerializeField]
    private Button _btnOption1 = null;
    [SerializeField]
    private Button _btnOption2 = null;
    [SerializeField]
    private Button _btnOption3 = null;

    [SerializeField]
    private Text _txtTitle = null;
    [SerializeField]
    private Text _txtMessage = null;
    [SerializeField]
    private Text _txtOption1 = null;
    [SerializeField]
    private Text _txtOption2 = null;
    [SerializeField]
    private Text _txtOption3 = null;

    /// <summary>
    /// Content of title.
    /// </summary>
    public string title
    {
        get
        {
            return _txtTitle.text;
        }
        set
        {
            _txtTitle.text = value;
        }
    }
    /// <summary>
    /// Content of message box.
    /// </summary>
    public string message
    {
        get
        {
            return _txtMessage.text;
        }
        set
        {
            _txtMessage.text = value;
        }
    }

    public string option1
    {
        get
        {
            return _txtOption1.text;
        }
        set
        {
            _txtOption1.text = value;
        }
    }

    public string option2
    {
        get
        {
            return _txtOption2.text;
        }
        set
        {
            _txtOption2.text = value;
        }
    }

    public string option3
    {
        get
        {
            return _txtOption3.text;
        }
        set
        {
            _txtOption3.text = value;
        }
    }

    /// <summary>
    /// Message box style.  eg.Option1|Option2 means show only Option1 and Option2 button.
    /// </summary>
    private int _style = (int)MBButton.Default;
    public int style
    {
        get
        {
            return _style;
        }
        set
        {
            _style = value;
            _btnClose.gameObject.SetActive((_style & (int)MBButton.Close) != 0);
            List<Button> buttons = new List<Button>();

            if ((_style & (int)MBButton.Option1) != 0)
            {
                _btnOption1.gameObject.SetActive(true);
                buttons.Add(_btnOption1);
            }
            else
            {
                _btnOption1.gameObject.SetActive(false);
            }
            if ((_style & (int)MBButton.Option2) != 0)
            {
                _btnOption2.gameObject.SetActive(true);
                buttons.Add(_btnOption2);
            }
            else
            {
                _btnOption2.gameObject.SetActive(false);
            }
            if ((_style & (int)MBButton.Option3) != 0)
            {
                _btnOption3.gameObject.SetActive(true);
                buttons.Add(_btnOption3);
            }
            else
            {
                _btnOption3.gameObject.SetActive(false);
            }

            float startX = -(buttons.Count - 1) * 260.0f * 0.5f;
            for (int i = 0; i < buttons.Count; ++i)
            {
                RectTransform rtf = buttons[i].GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(startX + 260.0f * i, 0.0f);
            }
        }
    }

    /// <summary>
    /// Auto close message box when option button clicked.
    /// </summary>
    private bool _closeClickClose = true;
    public bool closeClickClose
    {
        get
        {
            return _closeClickClose;
        }
        set
        {
            _closeClickClose = value;
        }
    }

    /// <summary>
    /// Auto close message box when option button clicked.
    /// </summary>
    private bool _closeClickOption = true;
    public bool closeClickOption
    {
        get
        {
            return _closeClickOption;
        }
        set
        {
            _closeClickOption = value;
        }
    }

    private void Awake()
    {
        _btnClose.onClick.AddListener(this.OnClickClose);
        _btnOption1.onClick.AddListener(this.OnClickOption1);
        _btnOption2.onClick.AddListener(this.OnClickOption2);
        _btnOption3.onClick.AddListener(this.OnClickOption3);
    }

    private void OnClickClose()
    {
        if (_closeClickClose)
            this.Close();
        if (this.onResult != null)
            this.onResult(this, MBButton.Close);
    }

    private void OnClickOption1()
    {
        if (_closeClickOption)
            this.Close();
        if (this.onResult != null)
            this.onResult(this, MBButton.Option1);
    }

    private void OnClickOption2()
    {
        if (_closeClickOption)
            this.Close();
        if (this.onResult != null)
            this.onResult(this, MBButton.Option2);
    }

    private void OnClickOption3()
    {
        if (_closeClickOption)
            this.Close();
        if (this.onResult != null)
            this.onResult(this, MBButton.Option2);
    }

    private void Close()
    {
        GameObject.Destroy(this.gameObject);
    }

    public static MessageBox Show(string title, string message, int style, bool closeClickClose, bool closeClickOption, string op1, string op2, string op3)
    {
        MessageBox mb = Create();
        mb.title = title;
        mb.message = message;
        mb.style = style;
        mb.closeClickClose = closeClickClose;
        mb.closeClickOption = closeClickOption;
        mb.option1 = op1;
        mb.option2 = op2;
        mb.option3 = op3;
        return mb;
    }

    public static MessageBox Show(string title, string message, int style, string op1, string op2, string op3)
    {
        return Show(title, message, style, true, true, op1, op2, op3);
    }

    public static MessageBox Show(string title, string message, int style, string op1, string op2)
    {
        return Show(title, message, style, true, true, op1, op2, "");
    }

    public static MessageBox Show(string title, string message, int style, string op1)
    {
        return Show(title, message, style, true, true, op1, "", "");
    }

    public static MessageBox Show(string title, string message, int style)
    {
        return Show(title, message, style, true, true, "", "", "");
    }


    private static MessageBox Create()
    {
        GameObject go = Resources.Load<GameObject>("MessageBox");
        GameObject inst = GameObject.Instantiate(go) as GameObject;
        RectTransform rtf = inst.GetComponent<RectTransform>();
        rtf.SetParent(FindRoot(), false);
        rtf.SetAsLastSibling();
        return inst.GetComponent<MessageBox>();
    }

    private static RectTransform FindRoot()
    {
        GameObject go = GameObject.Find("Canvas");
        if (go == null)
        {
            go = GameObject.Find("UIManager");
        }
        if (go == null)
            return null;

        Transform tf = go.transform.FindChild("MessageBoxContainer");
        if (tf == null)
        {
            GameObject c = new GameObject("MessageBoxContainer");
            c.transform.SetParent(go.transform, false);
            tf = c.transform;
        }

        RectTransform rtf = tf.GetComponent<RectTransform>();
        if (rtf == null)
        {
            rtf = tf.gameObject.AddComponent<RectTransform>();
            rtf.SetAsLastSibling();
            rtf.pivot = Vector2.one * 0.5f;
            rtf.anchorMin = Vector2.zero;
            rtf.anchorMax = Vector2.one;
            rtf.offsetMin = Vector2.zero;
            rtf.offsetMax = Vector2.zero;
        }
        return rtf;
    }
}

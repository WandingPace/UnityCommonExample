using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EScreenType
{
    NA,
    MainScene,
    

}

public class UIContextManager : MonoSingleton<UIContextManager> {
    [SerializeField]
    private Canvas mainCanvas;
    public Canvas MainCanvas
    {
        get
        {
            if (mainCanvas == null)
            {
                mainCanvas = GameObject.FindObjectOfType<Canvas>();
            }
            return mainCanvas;
        }
    }

	//Context
	private Stack<GameObject> _uiContextStack = new Stack<GameObject>();
    //log
    private Stack<EScreenType> _uiScreenTypeStack = new Stack<EScreenType>();
    void Awake()
    {
    }

	public void Push(GameObject nextUIGO)
	{
		if (_uiContextStack.Count != 0) {
			GameObject curUIGO = _uiContextStack.Peek();
			UIViewControlBase curView = curUIGO.GetComponent<UIViewControlBase>();
			curView.OnPause();
		}
		_uiContextStack.Push(nextUIGO);

		UIViewControlBase nextView = nextUIGO.GetComponent<UIViewControlBase>();
		//todo need separate the context and ui
		nextView.Appear();
        Debug.Log("/*/*UIManager*/*/ =================Push:" + nextUIGO.ToString());
	}
	public void Pop()
	{
		if (_uiContextStack.Count != 0) {
			GameObject curUIGO = _uiContextStack.Peek ();
			_uiContextStack.Pop();
			UIViewControlBase curView = curUIGO.GetComponent<UIViewControlBase>();
			curView.DisAppear();
            Debug.Log("[UIManager] =================Pop:" + curUIGO.ToString());


		}
		if (_uiContextStack.Count != 0) {
			GameObject lastUIGO = _uiContextStack.Peek();
			UIViewControlBase curView = lastUIGO.GetComponent<UIViewControlBase>();
			curView.OnResume();
		}


	}
    public void PopAll()
    {
        while (_uiContextStack.Count != 0)
        {
            GameObject curUIGO = _uiContextStack.Peek();
            _uiContextStack.Pop();
            UIViewControlBase curView = curUIGO.GetComponent<UIViewControlBase>();
            curView.DisAppear();
            Debug.Log("[UIManager] =================Pop:" + curUIGO.ToString());
        }
    }
	public GameObject PeekorNull()
	{
		if (_uiContextStack.Count != 0) {
			return _uiContextStack.Peek();
		}
		return null;
	}
    public EScreenType TopScreenType
    {
        get
        {
            if (_uiScreenTypeStack.Count != 0)
            {
                return _uiScreenTypeStack.Peek();
            }
            else
            {
                return EScreenType.NA;
            }
        }
        set
        {
            if (_uiScreenTypeStack.Count != 0)
            {
                _uiScreenTypeStack.Pop();
                _uiScreenTypeStack.Push(value);
            }
        }

    }

}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Test : MonoBehaviour {
    [SerializeField]
    private Button _btnEnter;
	// Use this for initialization
	void Start () {
        _btnEnter.onClick.AddListener(OnClickEnter);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnClickEnter()
    {
        StartCoroutine(EnterProc());
    }
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
}

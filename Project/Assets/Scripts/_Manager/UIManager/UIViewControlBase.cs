using UnityEngine;
using System.Collections;

public class UIViewControlBase : MonoBehaviour {

	// Use this for initialization
	protected virtual void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public virtual void Appear()
    {
        gameObject.SetActive(true);      
    }
    public virtual void DisAppear()
    {
        gameObject.SetActive(false);   
    }
	public virtual void OnPause()
	{

	}
	public virtual void OnResume()
	{

	}

}

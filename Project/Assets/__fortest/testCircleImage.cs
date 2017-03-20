using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class testCircleImage : Image {

    [SerializeField]
    private int curSegments;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);

    }
}

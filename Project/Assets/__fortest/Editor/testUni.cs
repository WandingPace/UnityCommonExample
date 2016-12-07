using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

[TestFixture]
[Category("Uni Tests")]
public class testUni {

	// Use this for initialization
    [Test]
    [Category("Test1")]
	public void Test1 () {
        
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic["1"] = "1";
        dic["2"] = "2";
        dic["3"] = "3";
        dic["4"] = "4";
        dic["5"] = "5";
        dic = dic.Take(3).ToDictionary(s => s.Key, s => s.Value);
        foreach (var item in dic)
        {
            Debug.Log(item.Value);
        }

	}

}

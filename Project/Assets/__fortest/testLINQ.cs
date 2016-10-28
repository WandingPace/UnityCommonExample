using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class testLINQ : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Hashtable table = new Hashtable();
        table["1"] = "a";
        table["2"] = "b";
        table["3"] = "c";
        List<string> keys = new List<string>() {"1","2"};
        //"select" reference to "table"  use clone or new to break reference 延迟执行
        var select = from s in (new Hashtable(table)) .Cast<DictionaryEntry>() where keys.Contains(s.Key.ToString()) select s;
        table.Clear();
        foreach (var item in select)
        {
            table[item.Key] = item.Value;
        }
        string result = URL_JSON.JsonEncode(table);

        string sentence = "the quick brown fox jumps over the lazy dog";
        // Split the string into individual words to create a collection.
        string[] words = sentence.Split(' ');

        // Using query expression syntax.
        var query = from word in words
                    group word.ToUpper() by word.Length into gr
                    orderby gr.Key
                    select new { Length = gr.Key, Word = gr };

        //dictionary
        Dictionary<int, string> dic = new Dictionary<int, string>();
        dic[1] = "a";
        dic[2] = "b";
        var ret = from s in dic select s.Key;
        
        foreach (var item in (new List<int>(ret)))
        {
           // Debug.Log(item);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SpriteSet for dynamic load sprite.
/// </summary>
public class SpriteSet : ScriptableObject//, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<Sprite> _sprites = null;
    private Dictionary<string, Sprite> _map = new Dictionary<string, Sprite>();

    /// <summary>
    /// Get sprite instance from sprite name. if the name is not exists will return null.
    /// </summary>
    /// <param name="name">sprite name</param>
    /// <returns></returns>
    public Sprite this[string name]
    {
        get
        {
            Sprite sprite = null;
            this.TryGet(name, out sprite);
            return sprite;
        }
    }
    /// <summary>
    /// Check sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Exists(string name)
    {
        Sprite sprite = null;
        return this.TryGet(name, out sprite);
    }
    /// <summary>
    /// Try to get sprite instance by sprite name, if the sprite is not exists will return false, and out sprite will be null.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public bool TryGet(string name, out Sprite sprite)
    {
        sprite = null;
        if (_sprites == null)
            return false;

        if (!_map.TryGetValue(name, out sprite)) 
        {
            for (int i = 0; i < _sprites.Count; ++i) 
            {
                Sprite sp = _sprites[i];
                if(sp != null && sp.name == name)
                {
                    sprite = sp;
                    _map[name] = sprite;
                    break;
                }
            }
        }

        if (sprite == null)
            return false;
        return true;
        //return _map.TryGetValue(name, out sprite);
    }

    /// <summary>
    /// Do not try to call this function in script.
    /// </summary>
    public void OnBeforeSerialize()
    {
        
    }
    /// <summary>
    /// Do not try to call this function in script.
    /// </summary>
    public void OnAfterDeserialize()
    {
//         _map.Clear();
//         if (_sprites == null)
//             return;
// 
//         for (int i = 0; i < _sprites.Count; ++i)
//         {
//             try
//             {
//                 Sprite sprite = _sprites[i];
//                 string fullname = sprite.ToString();
//                 string spname = fullname.Substring(0, fullname.Length - 21);
//                 if (sprite == null || string.IsNullOrEmpty(spname))
//                     continue;
//                 if (_map.ContainsKey(spname))
//                     continue;
//                 _map.Add(spname, sprite);
//             }
//             catch 
//             {
// 
//             }
//         }
    }

    private void Awake() 
    {
        if (_sprites == null)
            return;

        if (_sprites.Count != _map.Count) 
        {
            for (int i = 0; i < _sprites.Count; ++i)
            {
                Sprite sprite = _sprites[i];
                if (sprite != null && !string.IsNullOrEmpty(sprite.name))
                    _map[sprite.name] = sprite;
            }
        }
    }
}

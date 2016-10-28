using UnityEditor;
using System.Reflection;
using UnityEditor.Sprites;

public class MyEditor : Editor
{


    [MenuItem("GameObject/SearchAtlas", false, 0)]
    static void StartInitializeOnLoadMethod1()
    {
        //需要Sprite Packer界面定位的图集名称
        string spriteName = "atlas_name2";
        //设置使用采取图集的方式
        EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOn;
        //打包图集
        Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
        //打开SpritePack窗口
        EditorApplication.ExecuteMenuItem("Window/Sprite Packer");

        //反射遍历所有图集
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.Sprites.PackerWindow");
        var window = EditorWindow.GetWindow(type);
        FieldInfo infoNames = type.GetField("m_AtlasNames", BindingFlags.NonPublic | BindingFlags.Instance);
        string[] infoNamesArray = (string[])infoNames.GetValue(window);

        if (infoNamesArray != null)
        {
            for (int i = 0; i < infoNamesArray.Length; i++)
            {
                if (infoNamesArray[i] == spriteName)
                {
                    //找到后设置索引
                    FieldInfo info = type.GetField("m_SelectedAtlas", BindingFlags.NonPublic | BindingFlags.Instance);
                    info.SetValue(window, i);
                    break;
                }
            }
        }
    }
}
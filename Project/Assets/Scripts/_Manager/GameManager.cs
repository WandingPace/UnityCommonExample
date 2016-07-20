using UnityEngine;
using System.Collections;
using QFramework;
/*
 *  1.是游戏的入口
 *  2.不同功能模块的通信的中转站（当然用发消息的模式可能更好一些)
 *  3.处理游戏的一些挂起,进入后台等待特殊事件。
 *  4.一些资源的预加载,释放资源
 *  5.游戏的主线程 控制网络消息和逻辑消息的调用(只负责调用不负责处理,当然这种写法也不是很好)
 *  6.大模块跳转,HOME->GAME,GAME->HOME
 */

/// <summary>
/// 管理所有的控制器,资源的加载 场景的切换都在左立做
/// </summary>

public class GameManager : Singleton<GameManager>
{
    public LocalizationManager localizationManager = new LocalizationManager();
    private GameManager() { }

    /// <summary>
    /// 初始化
    /// </summary>
    public IEnumerator Init()
    {
        // 加载配置表数据
        //yield return TableManager.Instance().Init();

        // 初始化内存数据,可更改的数据
        //yield return DataManager.Instance().Init();

        // 音频资源加载
        //yield return SoundManager.Instance().Init();

        yield break;
    }


    /// <summary>
    /// 启动游戏
    /// </summary>
    public IEnumerator Launch()
    {
        //SceneManager.Instance().EnterHomeScene();
        yield return null;
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    void OnApplicationQuit()
    {
        //DataManager.Instance().Save(); // 数据加载
    }
}

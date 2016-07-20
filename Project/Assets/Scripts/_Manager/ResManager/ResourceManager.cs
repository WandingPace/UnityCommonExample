using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using Assets.ResourceManagement;

/// <summary>
/// This class manages externally acquired resources (by HTTP transfer or AssetBundle system)
/// </summary>
public class ResourceManager : MonoSingleton<ResourceManager>
{
    private const float FILE_UPDATER_POLL_INTERVAL = 0.03f;
    private const float ASSET_BUNDLE_PRELOAD_POLL_INTERVAL = 0.03f;
    private FileUpdater mainFileUpdater = null;

    /// <summary>
    /// A direct access dictionary which records all assets (including asset bundles and direct assets that are not stored in asset bundles).
    /// </summary>
    private Dictionary<string, System.Object> assetDict = new Dictionary<string, System.Object>();
    /// <summary>
    /// A dictionary that records the asset bundle one asset belongs to (if it does belong to an asset bundle).
    /// </summary>
    private Dictionary<Type, Dictionary<string, AssetBundle>> assetLocations = new Dictionary<Type, Dictionary<string, AssetBundle>>();
    /// <summary>
    /// A dictionary for fast-access of any direct/indirect asset.
    /// </summary>
    private Dictionary<string, System.Object> cachedAssets = new Dictionary<string, System.Object>();

    private Action onUpdateComplete = null;

    #region Update Logic
    /// <summary>
    /// Initiate the update operation.
    /// </summary>
    /// <param name="onUpdateComplete">Action to be executed when updates are completed.</param>
    /// <param name="isOffline">True if the game is not connected to PlayFab service.</param>
    public void InitiateUpdate(Action onUpdateComplete = null, bool isOffline = false)
    {
        if (onUpdateComplete != null)
            this.onUpdateComplete = onUpdateComplete;
        CreateFileUpdater();

        if (isOffline == false)
        {
            Debug.Log("<color=orange>ResourceManager: Start checking updates...</color>");
            UpdateChecker.CheckUpdate(this.OnUpdateCheckComplete);
        }
        else
        {
            Debug.Log("<color=orange>ResourceManager: Start off-line updates...</color>");
            UpdateChecker.LoadLocalUpdates(this.OnUpdateCheckComplete);
        }
    }
    
    /// <summary>
    /// Create a file updater if there's no existing updater.
    /// Currently we only create one FileUpdater so that the update process is single-tasking.
    /// </summary>
    private void CreateFileUpdater()
    {
        mainFileUpdater = this.gameObject.GetComponent<FileUpdater>();
        if (mainFileUpdater == null)
            mainFileUpdater = this.gameObject.AddComponent<FileUpdater>();
    }

    /// <summary>
    /// Called when update info list is acquired.
    /// It will starts to assign update tasks to FileUpdater(s).
    /// </summary>
    /// <param name="updateInfoList">The acquired update info list.</param>
    private void OnUpdateCheckComplete(Dictionary<string, UpdateInfo> updateInfoList)
    {
        Debug.Log("ResourceManager: Start assigning update tasks... (Total: " + updateInfoList.Count.ToString() + ")");
        StartCoroutine(AssignTasksToFileUpdater(updateInfoList));
    }

    /// <summary>
    /// Assign update tasks to the mainFileUpdater. (There might be multiple file updaters later if needed)
    /// </summary>
    /// <param name="updateInfoList">All tasks are in this dictionary.</param>
    private IEnumerator AssignTasksToFileUpdater(Dictionary<string, UpdateInfo> updateInfoList)
    {
        bool taskCompleted = false;
        foreach (var kv in updateInfoList)
        {
            taskCompleted = false;
            Debug.Log("<color=yellow>Task: " + kv.Value.name + "</color>");
            Debug.Log("Url: " + kv.Value.url);
            Debug.Log("Version: " + kv.Value.version);
            Debug.Log("MD5: " + kv.Value.hash);
            Debug.Log("KnownSize: " + kv.Value.size.ToString());

            mainFileUpdater.Initialize(kv.Value);
            mainFileUpdater.UpdateFile((UpdateInfo updateInfo, WWW www) => 
            {
                OnUpdatedFileLoaded(updateInfo, www);
                taskCompleted = true; 
            });
            while (taskCompleted == false)
            {
                yield return new WaitForSeconds(FILE_UPDATER_POLL_INTERVAL);
            }
            Debug.Log("<color=green>Task completed.</color>");
        }

        Debug.Log("<color=yellow>All update tasks are completed.</color>");
        
        // Need to clear cache because we might have new resource sources.
        cachedAssets.Clear();

        if(this.onUpdateComplete != null)
        {
            this.onUpdateComplete();
            this.onUpdateComplete = null;
        }
        yield break;
    }
    #endregion

    #region Resource Management
    /// <summary>
    /// Handles all loaded update files here.
    /// </summary>
    /// <param name="updateInfo">The update info of the file.</param>
    /// <param name="www">The WWW object for loading this file.</param>
    private void OnUpdatedFileLoaded(UpdateInfo updateInfo, WWW www)
    {
        if (www.assetBundle != null)
        {
            assetDict[updateInfo.name] = www.assetBundle;

            StartCoroutine(PreloadAssetsInAssetBundle(www.assetBundle));
        }
        else
        {
            assetDict[updateInfo.name] = www.bytes;
        }
    }

    /// <summary>
    /// Scan all assets in asset bundle and register them.
    /// The asset bundle will be unloaded after the scan to reduce memory usage.
    /// </summary>
    /// <param name="ab">The asset bundle to be scanned.</param>
    private IEnumerator PreloadAssetsInAssetBundle(AssetBundle ab)
    {
        string[] assetPaths = ab.GetAllAssetNames();

        foreach (string assetPath in assetPaths)
        {
            AssetBundleRequest abr = ab.LoadAssetAsync(assetPath);
            while (abr.isDone == false)
            {
                yield return new WaitForSeconds(ASSET_BUNDLE_PRELOAD_POLL_INTERVAL);
            }
            RegisterAsset(assetPath, abr.asset, ab);
        }
        ab.Unload(true);
    }

    /// <summary>
    /// Register the asset into "assetLocations" which records what AssetBundle an asset belongs to.
    /// </summary>
    /// <param name="obj">The asset.</param>
    /// <param name="ab">The asset bundle it belongs to.</param>
    private void RegisterAsset(string assetPath, UnityEngine.Object obj, AssetBundle ab)
    {
        Type type = obj.GetType();
        if (assetLocations.ContainsKey(type) == false)
        {
            assetLocations.Add(type, new Dictionary<string, AssetBundle>());
        }

        string clampedPath = assetPath.Substring(assetPath.LastIndexOf("resources") + 10);
        clampedPath = clampedPath.Substring(0, clampedPath.LastIndexOf('.'));
        assetLocations[type][clampedPath] = ab;
    }

    /// <summary>
    /// Check what asset bundle the specified asset belongs to.
    /// </summary>
    /// <param name="type">Type of the asset.</param>
    /// <param name="name">The unique name of the asset.</param>
    /// <returns>The asset bundle it belongs to.</returns>
    private AssetBundle GetAssetBundleForAsset(Type type, string name)
    {
        if (assetLocations.ContainsKey(type))
        {
            if (assetLocations[type].ContainsKey(name.ToLower()))
            {
                return assetLocations[type][name.ToLower()];
            }
            return null;
        }
        return null;
    }

    /// <summary>
    /// Get an asset from an asset-bundle (which is downloaded from CDN server).
    /// </summary>
    /// <typeparam name="T">Type of the asset.</typeparam>
    /// <param name="name">The unique name of the asset.</param>
    /// <param name="fallbackToResources">Whether it falls back to use Resources.Load() function if the asset is not found in asset bundles.</param>
    /// <returns>The specified asset.</returns>
    public T GetAsset<T>(string name, bool fallbackToResources = true) where T : UnityEngine.Object
    {
        if (cachedAssets.ContainsKey(name) == false)
        {
            AssetBundle assetBundle = GetAssetBundleForAsset(typeof(T), name.ToLower());
            if(assetBundle != null)
            {
                string[] assetNames = assetBundle.GetAllAssetNames();
                for(int i = 0; i < assetNames.Length; ++i)
                {
                    string clampedPath = assetNames[i].Substring(assetNames[i].LastIndexOf("resources") + 10);
                    clampedPath = clampedPath.Substring(0, clampedPath.LastIndexOf('.'));
                    if(name.ToLower() == clampedPath)
                    {
                        T result = assetBundle.LoadAsset<T>(assetNames[i]);
                        cachedAssets[name] = result;
                        assetBundle.Unload(false);
                        return result;
                    }
                }

                if (fallbackToResources)
                {
                    T result = Resources.Load<T>(name);
                    if(result != null)
                    {
                        cachedAssets[name] = result;
                        return result;
                    }
                }
                return null;
            }
            else
            {
                if(fallbackToResources)
                {
                    T result = Resources.Load<T>(name);
                    if (result != null)
                    {
                        cachedAssets[name] = result;
                        return result;
                    }
                }
                return null;
            }
        }
        return (T)cachedAssets[name];
    }

    /// <summary>
    /// Get the asset directly from file downloaded from CDN server.
    /// </summary>
    /// <typeparam name="T">Type of the asset.</typeparam>
    /// <param name="name">The unique name of the asset.</param>
    /// <returns>The specified asset.</returns>
    public T GetAssetDirectly<T>(string name) where T : class
    {
        // FIXME_YanFang: Remove folder structures in "name", which is not elegant.
        name = name.Substring(name.LastIndexOf('/') + 1);

        if (cachedAssets.ContainsKey(name) == false)
        {
            if (assetDict.ContainsKey(name))
            {
                if(typeof(T) == typeof(string))
                {
                    string result = Encoding.UTF8.GetString(assetDict[name] as byte[]);
                    cachedAssets[name] = result;
                    return result as T;
                }
                else
                {
                    T result = (T)Convert.ChangeType(assetDict[name], typeof(T));
                    cachedAssets[name] = result;
                    return result;
                }
            }
            else
            {
                return null;
            }
        }
        return (T)cachedAssets[name];
    }
    #endregion

    void Start()
    {

    }

    void Update()
    {

    }
}

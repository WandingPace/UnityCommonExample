using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Assets.ResourceManagement
{
    public class UpdateInfo
    {
        public string name;
        public string version;
        public string url;
        public string hash;
        public long size;

        public UpdateInfo(string name, string version, string url, string hash, long size)
        {
            this.name = name;
            this.version = version;
            this.url = url;
            this.hash = hash;
            this.size = size;
        }
    }

    public class UpdateChecker
    {
        private const string cdnDataInfoList = "CDNData";
        private const string cdnDataInfoHash = "CDNDataChecksum";

        private static int retryCounter = 0;
        private static int maxRetryCount = 3;

        private static Dictionary<string, UpdateInfo> updateInfoList = new Dictionary<string, UpdateInfo>();
        public static Dictionary<string, UpdateInfo> GetUpdateInfoList() { return updateInfoList; }
        private static Action<Dictionary<string, UpdateInfo>> onUpdateCheckComplete = null;

        public static void CheckUpdate(Action<Dictionary<string, UpdateInfo>> onUpdateCheckComplete = null)
        {
            //updateInfoList.Clear();
            //if(onUpdateCheckComplete != null)
            //    UpdateChecker.onUpdateCheckComplete = onUpdateCheckComplete;
            //PlayFab.ClientModels.GetTitleDataRequest request = new PlayFab.ClientModels.GetTitleDataRequest();
            //request.Keys = new List<string>();
            //request.Keys.Add("CDNData");
            //PlayFab.PlayFabClientAPI.GetTitleData(request, OnReceiveUpdateData, OnReceiveUpdateDataFailure);
        }

        private static void OnReceiveUpdateData(string jsonStr/*PlayFab.ClientModels.GetTitleDataResult result*/)
        {
            //string jsonStr = result.Data["CDNData"];

            // Do we need to store a salted MD5 hash for it as well?
            string hash = MD5Helper.GetHash(Encoding.UTF8.GetBytes("DOBBLE" + jsonStr + "DOBBLE"));
            File.WriteAllText(Path.Combine(Application.persistentDataPath, cdnDataInfoHash), hash);
            
            // We need to save the CDNDataInfoList locally (with AES encryption).
            string saltedHashofHash = MD5Helper.GetHash(Encoding.UTF8.GetBytes("DOBBLE" + hash + "DOBBLE"));

            byte[] key = AESHelper.CreateKey(new string(saltedHashofHash.Reverse().ToArray()));
            byte[] iv = AESHelper.CreateVector(saltedHashofHash);
            string encrypted = AESHelper.Encrypt(jsonStr, key, iv);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, cdnDataInfoList), encrypted);
            
            ParseUpdateData(jsonStr);
        }

        private static void ParseUpdateData(string data)
        {
            Hashtable table = URL_JSON.JsonDecode(data) as Hashtable;
            foreach(object k in table.Keys)
            {
                string key = (string)k;
                
                Hashtable subTable = table[key] as Hashtable;
                string version = subTable["Version"] as string;
                string url = subTable["CDNUrl"] as string;
                string hash = subTable["Hash"] as string;
                long size = -1;
                if(subTable.ContainsKey("Size"))
                    size = Convert.ToInt64(subTable["Size"].ToString());

                // Hack: We don't want those "Dobble" prefixes or "CDN" postfixes.
                if (key.EndsWith("CDN"))
                    key = key.Substring(0, key.Length - 3);
                if (key.StartsWith("Dobble"))
                    key = key.Substring(6);
                
                updateInfoList.Add(key, new UpdateInfo(key, version, url, hash, size));
            }
            if(onUpdateCheckComplete != null)
                onUpdateCheckComplete(updateInfoList);
        }

        private static void OnReceiveUpdateDataFailure(/*PlayFab.PlayFabError error*/)
        {
            retryCounter++;
            if (retryCounter <= maxRetryCount)
            {
                // We need to retry here.
                Debug.LogWarning("<color=orange>Failed to receive update list. (Retry: " + retryCounter.ToString() + "/" + maxRetryCount.ToString() + ")</color>");
                CheckUpdate();
            }
            else
            {
                // We should warn player that the network connection might have problems.
                Debug.LogWarning("<color=red>Failed to receive update list.</color>");
            }
        }

        /// <summary>
        /// Used when the game is in off-line mode.
        /// </summary>
        public static void LoadLocalUpdates(Action<Dictionary<string, UpdateInfo>> onUpdateCheckComplete = null)
        {
            updateInfoList.Clear();
            if (onUpdateCheckComplete != null)
                UpdateChecker.onUpdateCheckComplete = onUpdateCheckComplete;
            string filePath = Path.Combine(Application.persistentDataPath, cdnDataInfoList);
            if(File.Exists(filePath))
            {
                // Do we need to check MD5 hash of this update info file?
                string jsonStr = File.ReadAllText(filePath);
                string storedHash = File.ReadAllText(Path.Combine(Application.persistentDataPath, cdnDataInfoHash));
                string saltedHashofHash = MD5Helper.GetHash(Encoding.UTF8.GetBytes("DOBBLE" + storedHash + "DOBBLE"));

                // We need to decrypt the CDNDataInfoList (AES).
                byte[] key = AESHelper.CreateKey(new string(saltedHashofHash.Reverse().ToArray()));
                byte[] iv = AESHelper.CreateVector(saltedHashofHash);
                string decrypted = AESHelper.Decrypt(jsonStr, key, iv);

                string hash = MD5Helper.GetHash(Encoding.UTF8.GetBytes("DOBBLE" + decrypted + "DOBBLE"));

                if (storedHash == hash)
                {
                    ParseUpdateData(decrypted);
                }
                else
                {
                    // The local update info list has been modified.
                    Debug.LogWarning("<color=red>Corrupted local update info list.</color>");
                    // We just use the base version instead?
                    //if (onUpdateCheckComplete != null)
                    //    onUpdateCheckComplete(updateInfoList);
                    
                    // We need to notify the player to login the game to get into online mode.

                }
            }
            else
            {
                // There is no update info list. We don't need to update anything.
                // Still we need to send back an empty list.
                if (onUpdateCheckComplete != null)
                    onUpdateCheckComplete(updateInfoList);
            }
        }
    }
}

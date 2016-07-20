using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Assets.ResourceManagement
{
    /// <summary>
    /// A class that can update files using WWW.
    /// Can only handle one file simultaneously.
    /// </summary>
    public class FileUpdater : MonoBehaviour
    {
        private const float DOWNLOAD_POLL_INTERVAL = 0.03f;
        private const float LOAD_POLL_INTERVAL = 0.03f;

        private UpdateInfo updateInfo = null;
        private Action<UpdateInfo, WWW> onUpdateComplete = null;
        private bool isUpdating = false;
        public float progress { get { return (float)currentSize / fileSize; } }
        public long fileSize { get; private set; }
        public long currentSize { get; private set; }
        private WWW www = null;

        /// <summary>
        /// Set the file to be updated by this updater.
        /// </summary>
        /// <param name="updateInfo">The update info.</param>
        /// <returns>True if it's successfully set. False if the previous file's update process is not finished yet.</returns>
        public bool Initialize(UpdateInfo updateInfo)
        {
            if (isUpdating == true)
            {
                // The previous download is not done yet.
                return false;
            }
            this.updateInfo = updateInfo;
            this.onUpdateComplete = null;
            return true;
        }

        /// <summary>
        /// Start the update.
        /// </summary>
        /// <param name="onUpdateComplete">Action to be executed when this file is updated.</param>
        /// <returns>True if the update process is successfully started or if the file is already up-to-date. False if failed.</returns>
        public bool UpdateFile(Action<UpdateInfo, WWW> onUpdateComplete = null)
        {
            if (isUpdating) return false;
            if (updateInfo == null) return false;
            if (onUpdateComplete != null)
                this.onUpdateComplete = onUpdateComplete;

            bool needsDownload = CheckLocalFile();
            if(needsDownload)
            {
                Debug.Log("Start downloading...");
                Debugger.Log("Start downloading...");
                isUpdating = true;
                StartCoroutine(DownloadFile());
            }
            else
            {
                Debug.Log("Already up-to-date. Loading local file instead.");
                Debugger.Log("Already up-to-date. Loading local file instead.");
                isUpdating = true;
                StartCoroutine(LoadLocalFile());
            }
            return true;
        }

        private long GetFileSize(string url)
        {
            long fileSize = 0;
            System.Net.WebRequest request = System.Net.HttpWebRequest.Create(url);
            request.UseDefaultCredentials = true;
            try
            {
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                fileSize = response.ContentLength;
                response.Close();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error when determining file size: " + url);
                Debug.LogWarning(e.ToString());
            }

            return fileSize;
        }

        private IEnumerator DownloadFile()
        {
            www = new WWW(updateInfo.url);
            yield return null;
            if (updateInfo.size >= 0)
                fileSize = updateInfo.size;
            else
                fileSize = GetFileSize(updateInfo.url);

            while (www.isDone == false)
            {
                if (string.IsNullOrEmpty(www.error) == false)
                {
                    // Error occurs. We should retry.
                    isUpdating = false;
                    UpdateFile();
                    yield break;
                }

                // TODO: This step blocks the main thread, I don't know why...
                // Instead we use www.progress to predict the downloaded byte count.
                //currentSize = www.bytesDownloaded;
                currentSize = Mathf.FloorToInt(www.progress * fileSize);

                yield return new WaitForSeconds(DOWNLOAD_POLL_INTERVAL);
            }
            currentSize = www.bytesDownloaded;
                
            if (CheckFileContentHash(updateInfo.hash, www.bytes))
            {
                // Successful download.
                // Write to local file.
                string path = Path.Combine(Application.persistentDataPath, updateInfo.name);
                File.WriteAllBytes(path, www.bytes);
#if UNITY_IOS
                UnityEngine.iOS.Device.SetNoBackupFlag(path.Replace("\\", "/"));
#endif
                // Finishing the tasking.
                isUpdating = false;

                Debug.Log("Download completed.");
                Debugger.Log("Download completed.");

                if(onUpdateComplete != null)
                {
                    onUpdateComplete(updateInfo, www);
                    onUpdateComplete = null;
                }
            }
            else
            {
                // Something is wrong during download process. We should retry.
                Debug.LogWarning("<color=orange>Failed to update: " + updateInfo.name + ". Retrying...</color>");
                isUpdating = false;
                UpdateFile();
                yield break;
            }
        }

        /// <summary>
        /// Check the local file to see if we need to update it.
        /// </summary>
        /// <returns>True if update is needed. False if not.</returns>
        private bool CheckLocalFile()
        {
            string path = Path.Combine(Application.persistentDataPath, updateInfo.name);
            path = path.Replace("\\", "/");
            Debug.Log("Checking local file: " + path);
            Debugger.Log("Checking local file: " + path);
            if (File.Exists(path))
            {
                // Check MD5 hash.
                bool hashCheck = CheckFileContentHash(updateInfo.hash, File.ReadAllBytes(path));
                if (hashCheck)
                {
                    Debugger.Log("Hash check completed and passed.");
                    fileSize = new FileInfo(path).Length;
                }
                return !hashCheck;
            }
            else
            {
                // This is a new file.
                return true;
            }
        }

        private IEnumerator LoadLocalFile()
        {
            string path = Path.Combine(Application.persistentDataPath, updateInfo.name);
            path = path.Replace(" ", "%20").Replace("\\", "/");
            path = "file:///" + path;
            Debugger.Log("Loading local file: " + path);
            www = new WWW(path);
            yield return null;
            
            while (www.isDone == false)
            {
                if (string.IsNullOrEmpty(www.error) == false)
                {
                    // Error occurs. We should retry.
                    isUpdating = false;
                    UpdateFile();
                    yield break;
                }

                currentSize = www.bytesDownloaded;
                yield return null; // new WaitForSeconds(LOAD_POLL_INTERVAL);
            }
            currentSize = fileSize;

            // Finishing the tasking.
            Debug.Log("Load completed.");
            
            isUpdating = false;
            if (onUpdateComplete != null)
            {
                onUpdateComplete(updateInfo, www);
                onUpdateComplete = null;
            }
        }

        /// <summary>
        /// Check if md5 hash of the byte array matches the given hash.
        /// </summary>
        /// <param name="hash">The given hash.</param>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if hashes match. False if not.</returns>
        private bool CheckFileContentHash(string hash, byte[] bytes)
        {
            return string.Equals(hash, MD5Helper.GetHash(bytes));
        }
    }
}

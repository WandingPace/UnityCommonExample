        /* AssetBundleData         |      AssetData                              | AssetBundleCode
            -name                  |        -m_Guid                              |    -buildtarget     
            -variant               |        -m_Name                              |    -length
            -loadtype              |        -m_Length                            |    -hashcode
            -isPacked              |        -m_HashCode (util.convert)           |    -zipLength
            -List<AssetData>       |        -string[] m_DependencyAssetNames     |    -zipHashCode
            -List<AssetBundleCode> |                                             |    
        */

        /* Version
            -Path
            -Length
            -HashCode
            -ZipLength
            -ZipHashCode
            - 
        */
public class AssetBundleBuilderController
{
    SortedDictionary<string, AssetBundleData> m_AssetBundleDatas; //fullName(name.viriant)->
    //Build
    private void Build(AssetBundleBuild[] buildMap, BuildAssetBundleOptions buildOptions,bool zip,BuildTartGet target)
    {
        //clear path
        //build aseetbundles
         AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(workingPath, buildMap, buildOptions, buildTarget);
        //process assetbundle

        //prcoess package list 本地----version.dat (c)
        /*  VersionListHeader-PackageListVersion-encryptBytes-Lengh_applicableGameVersionBytes-InternalResourceVersion-
            m_AssetBundleDatas.Count(包数)-
            {
                Length_assetBundleData.Name(encrypt)-(Length_variantBytes)-
                assetBundleData.LoadType-assetBundleCode.Length-assetBundleCode.HashCode-
                assetNames.Length(资源数)-
                {
                    Length_assetNameBytes-
                    dependencyAssetNames.Length(依赖资源数)-
                    {
                        Length_dependencyAssetNameBytes
                    }
                }
            }-0   
        */

        //process fullpath(full) List------version1d123daaa.dat (CDN Sever)
        /*
            VersionListHeader-VersionListVersion-encryptBytes-Lengh_applicableGameVersionBytes-InternalResourceVersion-
            m_AssetBundleDatas.Count(包数)-
            {
                Length_assetBundleData.Name(encrypt)-(Length_variantBytes)-
                assetBundleData.LoadType-assetBundleCode.Length-assetBundleCode.HashCode-assetBundleCode.ZipLength-assetBundleCode.ZipHashCode
                assetNames.Length(资源数)-
                {
                    Length_assetNameBytes-
                    dependencyAssetNames.Length(依赖资源数)-
                    {  
                        Length_dependencyAssetNameBytes
                    }
                }
            }-0

        */  

        //Process ReadOnly(packed) List------list.dat (c)
        /*  ReadOnlyListHeader-ReadOnlyListVersion-encryptBytes-
            packedAssetBundleDatas.Count-
            {
                 Length_assetBundleData.Name(encrypt)-(Length_variantBytes)-
                 assetBundleData.LoadType-assetBundleCode.Length-assetBundleCode.HashCode     
            }

         */
    }
}


/* AssetBundle             Asset
    -name                  -guid
    -varient               -AssetBundle 
    -loadType              -Name = AssetDatabase.GUIDToAssetPath(Guid); ~ Assets/xxx/x.png
    -ispacked
   -List<Asset>             
*/
public class AssetBundleCollection
{
    private readonly SortedDictionary<string, AssetBundle> m_AssetBundles; //fullName->AssetBundle
    private readonly SortedDictionary<string, Asset> m_Assets; //guid->name
}


/*  DependencyData
    List<AssetBundle> m_DependencyAssetBundles
    List<Asset> m_DependencyAssets
    List<string> m_ScatteredDependencyAssetNames
 */
public class AssetBundleAnalyzeController
{
    private readonly Dictionary<string, DependencyData> m_DependencyDatas;//Asset.Name->

    private void AnalyzeAsset(string assetName, Asset asset,DependencyData denpendencyData)
    {
        //get denpendency asset pathname
        string[] dependencyAssetNames = AssetDatabase.GetDependencies(assetName, false);
        foreach (string dependencyAssetName in dependencyAssetNames)
        {
             string guid = AssetDatabase.AssetPathToGUID(dependencyAssetName);
             Asset asset = m_AssetBundleCollection.GetAsset(guid);
                if (asset != null)
                {
                    dependencyData.AddDependencyAsset(asset);
                }
                else
                {
                    dependencyData.AddScatteredDependencyAsset(dependencyAssetName);
                     AnalyzeAsset(dependencyAssetName, hostAsset, dependencyData, scriptAssetNames);
                }
        }
    }
}
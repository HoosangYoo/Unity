using UnityEngine;
using UnityEditor;
using System;
using System.IO;

#if UNITY_EDITOR
/// <summary>
/// A class that combines meshes into one mesh.
/// </summary>
[ExecuteInEditMode]
public class CombineMeshes : MonoBehaviour
{
    /// <summary>
    /// The directory where the mesh will be saved as an asset.
    /// </summary>
    [SerializeField]
    private string _assetDirectory = string.Empty;

    /// <summary>
    /// The name where the mesh will be saved as an asset.
    /// </summary>
    [SerializeField]
    private string _assetName = "mesh.asset";

    /// <summary>
    /// A material to apply to the combined mesh.
    /// </summary>
    [SerializeField]
    private Material _material = null;

    /// <summary>
    /// A flag that determines whether to generate lightmap UVs.
    /// </summary>
    [SerializeField]
    private bool _generateLightmapUV = true;

    /// <summary>
    /// Combines meshes into one mesh.
    /// This function can be executed through the inspector's context menu.
    /// </summary>
    [ContextMenu("Combine")]
    public void Combine()
    {
        // First, gather all mesh filters from childeren.
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();     
        if (meshFilters == null || meshFilters.Length == 0 )
        {
            Debug.Log("No meshes to combine.");
            return;
        }

        if (!transform.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (!transform.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        // Gather shared meshes and transforms and then deactivate them.
        CombineInstance[] instances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            instances[i].mesh = meshFilters[i].sharedMesh;
            instances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        
        // Combine them into new shared mesh.
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(instances);
        meshRenderer.material = _material;

        // Generate light map UVs if needed.
        if (_generateLightmapUV)
        {
            Unwrapping.GenerateSecondaryUVSet(meshFilter.sharedMesh);
        }

        // Finally, save generated mesh as an asset to the specified path.
        string path = Path.Combine("Assets", _assetDirectory, _assetName);
        AssetDatabase.CreateAsset(meshFilter.sharedMesh, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();

        gameObject.SetActive(true);

        Debug.Log($"Meshes has been successfully combined into {path}");
    }
}
#endif

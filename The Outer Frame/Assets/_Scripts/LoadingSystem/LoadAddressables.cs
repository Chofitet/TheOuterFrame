using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadAddressables
{
    public async Task LoadCommonAdressables()
    {
        var handle = Addressables.LoadResourceLocationsAsync("Common");

        handle.Completed += locationsHandle =>
        {
            if (locationsHandle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var location in locationsHandle.Result)
                {
                    var assetHandle = Addressables.LoadAssetAsync<ScriptableObject>(location);

                    assetHandle.Completed += assetOp =>
                    {
                        if (assetOp.Status == AsyncOperationStatus.Succeeded)
                        {
                            if (assetOp.Result is IReseteableScriptableObject resetable)
                            {
                                resetable.ResetScriptableObject();
                                Debug.Log($"Reset: {resetable}");
                            }
                        }
                    };
                }
            }
        };

    }
}

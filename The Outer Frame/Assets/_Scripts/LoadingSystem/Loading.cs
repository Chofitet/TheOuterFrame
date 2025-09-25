using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class Loading : MonoBehaviour
{
    LoadAddressables loadingAdressables = new LoadAddressables();
    [SerializeField] GameEvent OnChangeLevel;
    
    void Start()
    {
        LoadAsync();
    }

    async Task LoadAsync()
    {
        await loadingAdressables.LoadCommonAdressables();
        OnChangeLevel?.Invoke(this, null);
    }

}

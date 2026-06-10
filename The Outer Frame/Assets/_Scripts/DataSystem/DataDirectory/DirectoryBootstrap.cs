using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryBootstrap : MonoBehaviour
{
    [SerializeField] private DataDirectory directory;

    private void Awake()
    {
        directory.Initialize();
    }
}

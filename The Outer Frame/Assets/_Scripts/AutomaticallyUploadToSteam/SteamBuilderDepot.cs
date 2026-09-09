using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steam Builder/Steam Depot")]
public class SteamBuilderDepot :  ScriptableObject
{
    public string depotID;
    public string contentRoot;
    public string localPath = "*";
    public string depotPath = ".";
    public string recursive = "1";
    public string fileExclusion = "*.pdb";
}

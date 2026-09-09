using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steam Builder/Steam Builder Config")]
public class SteamBuilderConfig : ScriptableObject
{
    public string appId;
    public string unityBuildFolder = "..\\Builds";
    public string buildOutput = "output";
    public string contentRoot = "content";

    public List<string> branches;
    public int branchIndex;

    public List<SteamBuilderDepot> depots = new List<SteamBuilderDepot>();

}

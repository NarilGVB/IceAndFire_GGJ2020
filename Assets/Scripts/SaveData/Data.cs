using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    //general config
    public int language;
    public float masterVolume = 0.2f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    
    //game data
    public int maxScore;
}

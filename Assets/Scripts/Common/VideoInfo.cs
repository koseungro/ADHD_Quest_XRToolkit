using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVideoInfo", menuName="CreateVideoInfo/VideoInfo")]
public class VideoInfo : ScriptableObject {

    public string Id;
    public string videoPath;
    public float startFrame;
    public float endFrame;
    public string GetVideoPath { get { return videoPath + Id; } }
    public bool isLoop { get { return Id.Contains("Loop"); } }

}

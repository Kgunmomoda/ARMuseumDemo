using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NewExhibit_ShowInfo", menuName = "Exhibit/ShowInfo")]
public class ExhibitShowInfo_SO : ScriptableObject
{
    [SerializeField]
    public string text;
    [SerializeField]
    public Color textColor;
    [SerializeField]
    public int textSize;
    [SerializeField]
    public VideoClip videoClip;
    [SerializeField]
    public Sprite image1;
    [SerializeField]
    public Sprite image2;
    [SerializeField]
    public Sprite image3;
    [SerializeField]
    public Sprite image4;
    [SerializeField]
    public AudioClip audioClip;
}

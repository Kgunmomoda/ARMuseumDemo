using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using NRKernal;
using NRKernal.NRExamples;

public class Exhibitshower : MonoBehaviour
{

    public Canvas canvas;
    public int desiredExhibitId;
    private int exhibitId;
   // private ExhibitShowInfo showInfo;
    private bool isShowing;
    [Space]
    public Text showText;
    //public Image image1;
   // public Image image2;
   // public Image image3;
    //public Image image4;
    public VideoPlayer videoPlayer;
    public ScreenApapter screenApapter;
    public AudioSource audioSource;
    private Transform m_CenterCamera;
    private Transform CenterCamera
    {
        get
        {
            if (m_CenterCamera == null)
            {
                if (NRSessionManager.Instance.CenterCameraAnchor != null)
                {
                    m_CenterCamera = NRSessionManager.Instance.CenterCameraAnchor;
                }
                else if (Camera.main != null)
                {
                    m_CenterCamera = Camera.main.transform;
                }
            }
            return m_CenterCamera;
        }
    }
    void Awake()
    {
        NRHMDPoseTracker poseTracker = FindObjectOfType<NRHMDPoseTracker>();
        if (poseTracker)
        {
            canvas.worldCamera = poseTracker.leftCamera;
        }
        else
        {
            canvas.worldCamera = Camera.main;
        }
        //Init();
    }
   // public void Init(int _exhibitId, ExhibitShowInfo _showInfo)
   // {
    //    exhibitId = _exhibitId;
    //    showInfo = _showInfo;

    //    showText.text = showInfo.data.text;
     //   showText.fontSize = showInfo.data.textSize;
      //  showText.color = showInfo.data.textColor;
       // image1.sprite = showInfo.data.image1;
        //image2.sprite = showInfo.data.image2;
       // image3.sprite = showInfo.data.image3;
       // image4.sprite = showInfo.data.image4;

     //   if (showInfo.data.videoClip)
    //    {
    //        videoPlayer.clip = showInfo.data.videoClip;
    //    }
    //    if (showInfo.data.audioClip)
    //    {
    //        audioSource.clip = showInfo.data.audioClip;
    //    }
   // }
    //private void Init()
    //{
     //   ExhibitsManager exhibitsManager = FindObjectOfType<ExhibitsManager>();
      //  ExhibitShowInfo info = exhibitsManager.GetShowInfo(desiredExhibitId);
      //  if (info.IsValid())
     //   {
      //      Init(desiredExhibitId, info);
     //   }
   // }
    public void BeginShow()
    {

        canvas.gameObject.SetActive(true);
       // image1.gameObject.SetActive(image1.sprite);
       // image2.gameObject.SetActive(image2.sprite);
       // image3.gameObject.SetActive(image3.sprite);
      //  image4.gameObject.SetActive(image4.sprite);

        //if (videoPlayer.clip)
        //{
            videoPlayer.Play();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        //}
       // if (audioSource.clip)
       // {
            audioSource.Play();
       // }
        isShowing = true;
        videoPlayer.loopPointReached += StopShow;
    }

    public void StopShow(VideoPlayer source)
    {
        canvas.gameObject.SetActive(false);
        videoPlayer.Stop();
        screenApapter.leftRightScreen.SetActive(false);
        audioSource.Stop();
        isShowing = false;
    }
    public void StopShow()
    {
        canvas.gameObject.SetActive(false);
        videoPlayer.Stop();
        screenApapter.leftRightScreen.SetActive(false);
        audioSource.Stop();
        isShowing = false;
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        screenApapter.SetScreen();
        screenApapter.SetContent(source.texture);
    }

    void Update()
    {
        this.transform.forward = Camera.main.transform.forward;
        if (!isShowing) return;
        HandState rightHand = NRInput.Hands.GetHandState(HandEnum.RightHand);
        if (rightHand.currentGesture == HandGesture.Victory)
        {
            videoPlayer.Pause();
        }
        else if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
        if (rightHand.currentGesture == HandGesture.OpenHand)
        {
          //  image1.rectTransform.sizeDelta += Time.deltaTime * 30 * Vector2.one;
          //  image2.rectTransform.sizeDelta += Time.deltaTime * 30 * Vector2.one;
          //  image3.rectTransform.sizeDelta += Time.deltaTime * 30 * Vector2.one;
          //  image4.rectTransform.sizeDelta += Time.deltaTime * 30 * Vector2.one;
        }
        else if (rightHand.currentGesture == HandGesture.Grab)
        {
          //  image1.rectTransform.sizeDelta -= Time.deltaTime * 30 * Vector2.one;
          //  image2.rectTransform.sizeDelta -= Time.deltaTime * 30 * Vector2.one;
          //  image3.rectTransform.sizeDelta -= Time.deltaTime * 30 * Vector2.one;
          //  image4.rectTransform.sizeDelta -= Time.deltaTime * 30 * Vector2.one;
        }
    }
    void LateUpdate()
    {
        if (!isShowing) return;
       // canvas.transform.LookAt(CenterCamera.position);
    }

}

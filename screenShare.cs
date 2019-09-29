using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using UnityEngine.UI;
using System.Globalization;
using System.Runtime.InteropServices;
using System;

public class TestScreenCapture : MonoBehaviour
{



    //To create a virtual camera one will need
    //to convert the render texture to a texture 2d
    // https://stackoverflow.com/questions/44264468/convert-rendertexture-to-texture2d


    // Start is called before the first frame update
    Texture2D mTexture;
    Rect mRect;
    //IntPtr videoBytes;
    private static string appId = "be3e1b4145af48df897fa1c482c1a5e2";
	public IRtcEngine mRtcEngine;

	public WebCamTexture webCameraTexture;
	public WebCamDevice webCameraDevice;
    public RenderTexture videoTexture2D;
	public Vector2 cameraSize = new Vector2(1280, 720);
	public int cameraFPS = 30;
    int i = 100;
	void Start() {
        //StartCoroutine("InitCameraDevice");
        InitCameraDevice();
		Debug.Log("AgoraTestlllllll");
		mRtcEngine = IRtcEngine.getEngine (appId);
		// enable log
		mRtcEngine.SetLogFilter (LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
			// set callbacks (optional)
		mRtcEngine.SetParameters("{\"rtc.log_filter\": 65535}");

		mRtcEngine.SetExternalVideoSource(false, true);
		// enable video
		mRtcEngine.EnableVideo();
		// allow camera output callback
		mRtcEngine.EnableVideoObserver();
		// join channel
		mRtcEngine.JoinChannel( "unity3d", null, 0);

        mRect = new Rect(0, 0, Screen.width, Screen.height);
        mTexture = new Texture2D((int)mRect.width, (int)mRect.height,TextureFormat.RGBA32 ,false);  
    }

    void Update () {
		StartCoroutine(cutScreen());
        //StartCoroutine(pushVideoFrame());
	}

    public void InitCameraDevice()
	{	
		// yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
		// if (Application.HasUserAuthorization(UserAuthorization.WebCam))
		// {
        WebCamDevice[] devices = WebCamTexture.devices;
        webCameraTexture = new WebCamTexture(devices[0].name, (int)cameraSize.x, (int)cameraSize.y, cameraFPS);

        Renderer rend = GetComponent<Renderer> ();
        rend.material.mainTexture = webCameraTexture;
        webCameraTexture.Play();
        videoTexture2D = new Texture2D(webCameraTexture.width, webCameraTexture.height, TextureFormat.RGBA32, false);
	}

    
    
    //Screen Share
	IEnumerator cutScreen()
	{
		yield return new WaitForEndOfFrame();
        //videoBytes = Marshal.AllocHGlobal(Screen.width * Screen.height * 4);
        mTexture.ReadPixels(mRect, 0, 0);
		mTexture.Apply();  
		byte[] bytes = mTexture.GetRawTextureData();
		int size = Marshal.SizeOf(bytes[0]) * bytes.Length;	
        IRtcEngine rtc = IRtcEngine.QueryEngine();

        if (rtc != null)
        {
            ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
            externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_BGRA;
            externalVideoFrame.buffer = bytes;
            externalVideoFrame.stride = (int)mRect.width;
            externalVideoFrame.height = (int)mRect.height;
            externalVideoFrame.cropLeft = 10;
            externalVideoFrame.cropTop = 10;
            externalVideoFrame.cropRight = 10;
            externalVideoFrame.cropBottom = 10;
            externalVideoFrame.rotation = 90;
            externalVideoFrame.timestamp =i++;
            int a = rtc.PushExternVideoFrame(externalVideoFrame);
            Debug.Log(" pushVideoFrame =       "  + a);
        }
        }  
}

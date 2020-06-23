using UnityEngine;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using TMPro;

/* Created by voidzero-development 8/6/2020*/

public class BarcodeScanManager : MonoBehaviour
{
  public RawImage CameraDisplayPanel;
  public WebCamTexture webcamTexture;
  public bool camExists;
  public AspectRatioFitter fit;
  public IBarcodeReader reader;
  public Quaternion baseRotation;
  public SceneHandler sceneHandler;

  void Start()
  {
    /* Grab array of webcam devices. */
    WebCamDevice[] camDevices = WebCamTexture.devices;

    /* Check if we have an available webcam device */
    if (camDevices.Length == 0)
    {
      camExists = false;
    }

    /* Loop through array of webcam devices and find the front facing camera. */
    for (int i = 0; i < camDevices.Length; i++)
    {
      if (!camDevices[i].isFrontFacing)
      {
        webcamTexture = new WebCamTexture(camDevices[i].name);
      }
    }

    baseRotation = CameraDisplayPanel.transform.rotation;

    /* Start our rear facing camera. */
    webcamTexture.Play();

    /* Set our camera display panel texture equal to webcam texture */
    if (webcamTexture != null)
    {
      CameraDisplayPanel.texture = webcamTexture;
    }

    camExists = true;

    /* Instantiate ZXing barcode reader */
    reader = new BarcodeReader();
  }

  private void Update()
  {

    if (GameManager.Instance.isDebug)
    {
    //do debug code
    }

    /* Continue if camera has been found */
    if (!camExists)
      return;

    CameraDisplayPanel.transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.back);

    /* Convert image color32[] array to byte array and parse it to the barcode reader!*/
    byte[] rgbs = Color32ArrayToByteArray(webcamTexture.GetPixels32());
    var result = reader.Decode(rgbs, webcamTexture.width, webcamTexture.height, 0);

    if (result != null)
    {
      //do something with result here
    }
  }

  // convert color32[] array to byte[] array
  // losing my sanity over this 
  private static byte[] Color32ArrayToByteArray(Color32[] colors)
  {
    if (colors == null || colors.Length == 0)
      return null;

    int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
    int length = lengthOfColor32 * colors.Length;
    byte[] bytes = new byte[length];

    GCHandle handle = default(GCHandle);
    try
    {
      handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
      IntPtr ptr = handle.AddrOfPinnedObject();
      Marshal.Copy(ptr, bytes, 0, length);
    }
    finally
    {
      if (handle != default(GCHandle))
        handle.Free();
    }

    return bytes;
  }

}

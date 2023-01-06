using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    List<CinemachineFreeLook> cams = new List<CinemachineFreeLook>();
    public static CameraManager instance;
    [Header("these values reflect values in UI")]
    [SerializeField] float CurrentXSensitivity;
    [SerializeField] float CurrentYSensitivity;
    [Space]
    [SerializeField] float XAxisDamp;
    [SerializeField] float YAxisDamp;

    private void Awake()
    {
        instance = this;
    }

    //private IEnumerator Start()
    //{
    //    yield return null;
    //    SetCameraSensitivityX(CurrentXSensitivity);
    //    SetCameraSensitivityY(CurrentYSensitivity);
    //}
    [ContextMenu("update axis")]
    public void UpdateBothAxis()
    {
        SetCameraSensitivityX(CurrentXSensitivity);
        SetCameraSensitivityY(CurrentYSensitivity);
    }

    public void SetCameraSensitivityX(float value)
    {

        foreach(CinemachineFreeLook cam in cams)
        {
            //CurrentXSensitivity = value * .2f;
            //cam.m_XAxis.m_MaxSpeed = CurrentXSensitivity;
            CurrentXSensitivity = value;
            cam.m_XAxis.m_MaxSpeed = value * XAxisDamp;
        }
    }

    public void SetCameraSensitivityY(float value)
    {
        foreach (CinemachineFreeLook cam in cams)
        {
            CurrentYSensitivity = value;
            cam.m_YAxis.m_MaxSpeed = value * YAxisDamp;
        }
    }

    public void AddCamera(CinemachineFreeLook cam)
    {
        print("adding " + cam + " to camera list");
        cam.m_XAxis.m_MaxSpeed = CurrentXSensitivity * XAxisDamp;
        cam.m_YAxis.m_MaxSpeed = CurrentYSensitivity * YAxisDamp;
        cams.Add(cam);
    }

    public void RemoveCamera(CinemachineFreeLook cam)
    {
        print("removing " + cam + " to camera list");
        cams.Remove(cam);
    }
}

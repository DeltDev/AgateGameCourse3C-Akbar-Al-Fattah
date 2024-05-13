using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CameraState CameraState;
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineFreeLook tpsCamera;
    [SerializeField] private InputManager inputManager;
    private void Start(){
        inputManager.OnChangePOV += SwitchCamera;
    }
    private void OnDestroy(){
        inputManager.OnChangePOV -= SwitchCamera;
    }
    public void SetFPSClampedCamera(bool isClamped, Vector3 playerRotation){
        CinemachinePOV pov = fpsCamera.GetCinemachineComponent<CinemachinePOV>();

        if(isClamped){
            pov.m_HorizontalAxis.m_Wrap = false;
            pov.m_HorizontalAxis.m_MinValue = playerRotation.y-45;
            pov.m_HorizontalAxis.m_MaxValue = playerRotation.y+45;
        } else {
            pov.m_HorizontalAxis.m_MinValue = -180;
            pov.m_HorizontalAxis.m_MaxValue = 180;
            pov.m_HorizontalAxis.m_Wrap = true;
        }
    }

    private void SwitchCamera(){
        if(CameraState == CameraState.ThirdPerson){
            CameraState = CameraState.FirstPerson;
            tpsCamera.gameObject.SetActive(false);
            fpsCamera.gameObject.SetActive(true);
        } else {
            CameraState = CameraState.ThirdPerson;
            tpsCamera.gameObject.SetActive(true);
            fpsCamera.gameObject.SetActive(false);
        }
    }

    public void SetTPSFOV(float FOV){
        tpsCamera.m_Lens.FieldOfView = FOV;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMP.Unity;
using DataMesh.AR.Interactive;
using System;

/// <summary>
/// 完成整个场景的资源控制，
/// 1、比如该control相关界面的显示
/// 2、控制输入交互的层过滤信息，用来指定那些层可以进行交互操作。
/// </summary>
/// 
public interface IControllerInterface
{
     void SetLayerMask(int type);
}

public enum ObjectGestureType
{
    None,
    Fitting,
    Move,
    Rotate,
    Scale,
}

public enum StabilizationPlaneType
{
    FollowCamera,
    Normal,
    Customize,
}
public class GameController : Singleton<GameController>, IControllerInterface
{

    public GameObject uiPagePause;

    protected MultiInputManager inputManager;

    public GameObject currentActiveObject;
    [HideInInspector]
    public ObjectGestureType gestureType = ObjectGestureType.None;
    private System.Action cbTapEmpty;  //用于反馈没有点击到操作对象要做的记录和处理
    private bool isNavActive = false;




    [Tooltip("该控制器内进行被交互的对象所在的层级，该控制器激活后，交互输入只对该层级生效")]
    public int ControllerActiveLayer = 20;

    public void init()
    {
        inputManager = MultiInputManager.Instance;
    }


    #region layer_active_setting
    public bool NeedBlockOtherLayerInput = true; //清空其他活动层
    private int originInputLayer;
    public int controllerLayer
    {
        get
        {
            return 1 << ControllerActiveLayer;
        }
    }

    /// <summary>
    /// 记录开始操作之前的层
    /// </summary>
    public void SaveOriginInputLayer()
    {
        originInputLayer = inputManager.layerMask;
    }

    /// <summary>
    /// 设置新操作层
    /// </summary>
    /// <param name="type"></param>
    public void SetLayerMask(int type)
    {
        int t_originLayer = NeedBlockOtherLayerInput ? 0 : originInputLayer;
        switch(type)
        {
            case 1:  // 表示使用新层
                inputManager.layerMask = t_originLayer | controllerLayer | LayerMask.GetMask("UI");
                break;
            case 2: //清空所有层
                inputManager.layerMask = 0;
                break; 
        }
        
    }

    #endregion





    #region gesturemanager
    private void ActiveGestureManager(bool isActive)
    {
        if(isActive)
        {

            //点击手势
            inputManager.cbTap += OnTap;

            //导航手势
            inputManager.cbNavigationStart += OnNavigationStart;
            inputManager.cbNavigationUpdate += OnNavigationUpdate;
            inputManager.cbNavigationEnd += OnNavigationEnd;

            // 操控手势
            inputManager.cbManipulationStart += OnManipulationStart;
            inputManager.cbManipulationUpdate += OnManipulationUpdate;
            inputManager.cbManipulationEnd += OnManipulationEnd;
        }
        else
        {
            inputManager.cbTap -= OnTap;

            //导航手势
            inputManager.cbNavigationStart -= OnNavigationStart;
            inputManager.cbNavigationUpdate -= OnNavigationUpdate;
            inputManager.cbNavigationEnd -= OnNavigationEnd;

            // 操控手势
            inputManager.cbManipulationStart -= OnManipulationStart;
            inputManager.cbManipulationUpdate -= OnManipulationUpdate;
            inputManager.cbManipulationEnd -= OnManipulationEnd;

        }


    }
    private void OnTap(int obj)
    {
        //判断是否点击到了要操作的对象

        if(inputManager.FocusedObject != null)
        {
            Transform t_myCom = inputManager.FocusedObject.GetComponentInParent<Transform>();
            if( t_myCom != null)
            {
                //进行初始操作开始
            }
            if(t_myCom == null)
            {
                if(cbTapEmpty != null)
                {
                    cbTapEmpty();
                }
                return;
            }
        }
    }

    public  void ChangeGestureType(ObjectGestureType type)
    {
        gestureType = type;

        if(type == ObjectGestureType.Move)
        {
            inputManager.ChangeToManipulationRecognizer();
        }
        else if( type == ObjectGestureType.Rotate)
        {
            inputManager.ChangeToNavigationRecognizer();
        }


    }

    private void OnTriggerMoveGesture()
    {

    }

    private void OnNavigationStart(Vector3 obj)
    {
        if (currentActiveObject == null)
            return;

        if(gestureType == ObjectGestureType.Rotate)
        {
            isNavActive = true;
            // currentAnchorInfo.mark.HideTips();
        }

        if (gestureType == ObjectGestureType.Scale)
        {
            isNavActive = true;
            // currentAnchorInfo.mark.HideTips();
        }

    }
    private void OnNavigationUpdate(Vector3 delta)
    {
        if(gestureType == ObjectGestureType.Rotate)
        {
            if(isNavActive)
            {
                Vector3 deltaRot = new Vector3(delta.z, -delta.x, -delta.y);
                float m_rotateSpeed = 1.0f;
                currentActiveObject.transform.Rotate(deltaRot * m_rotateSpeed * Time.deltaTime,Space.Self);
            }
        }

        if(gestureType == ObjectGestureType.Scale)
        {
            if(isNavActive)
            {
                Vector3 deltaScale = new Vector3(delta.z, -delta.x, -delta.y);
                //float m_scaleSpeed = 1.0f;
                currentActiveObject.transform.localScale = new Vector3(0.5f, 1.0f, 1.0f);
            }
        }
    }

    private void OnNavigationEnd(Vector3 obj)
    {
        isNavActive = false;

        if (currentActiveObject == null)
            return;
        // currentAnchorInfo.mark.ShowTips();
    }

    private void OnManipulationStart(Vector3 obj)
    {
        throw new NotImplementedException();
    }
    private void OnManipulationUpdate(Vector3 obj)
    {
        throw new NotImplementedException();
    }

    private void OnManipulationEnd(Vector3 obj)
    {
        throw new NotImplementedException();
    }


    #endregion






}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEHoloClient.Utils;

#if true //UNITY_EDITOR || UNITY_STANDALONE_WIN ||UNITY_WSA
using DataMesh.AR.Anchor;
#endif

using DataMesh.AR.Account;
using DataMesh.AR.Library;
using DataMesh.AR.Interactive;
using DataMesh.AR.SpectatorView;
using DataMesh.AR.UI;
using DataMesh.AR.Network;
using DataMesh.AR.Utility;

namespace DataMesh.AR
{
    public class MEHoloEntrance : MonoBehaviour
    {
        public static MEHoloEntrance Instance { get; private set; }

        public string AppID = null;
#if true // UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WSA
        public GameObject AnchorPrefab;
#endif
        public GameObject AccountPrefab;
        public GameObject InputPrefab;
        public GameObject SpeechPrefab;
        public GameObject UIPerfab;
        public GameObject CollaborationPrefab;
        public GameObject LibraryPrefab;
        public GameObject LivePrefab;
        [HideInInspector]
        public bool NeedAccount = true;
#if true // UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WSA
        [HideInInspector]
        public bool NeedAnchor = true;
#endif

        [HideInInspector]
        public bool NeedInput = true;
        [HideInInspector]
        public bool NeedSpeech = true;
        [HideInInspector]
        public bool NeedUI = true;
        [HideInInspector]
        public bool NeedCollaboration = true;
        [HideInInspector]
        public bool NeedLibrary = true;
        [HideInInspector]
        public bool NeedLive = true;

        [HideInInspector]
        public bool HasInit = false;

        /// <summary>
        /// 系统菜单的数据资源
        /// </summary>
        public TextAsset systemMenuData;

        
        private AccountManager accountManager;

#if true // UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WSA
        private SceneAnchorController anchorController;
#endif
        private LiveController liveController;
        private MultiInputManager inputManager;
        private SpeechManager speechManager;
        private UIManager uiManager;
        private CollaborationManager collaborationManager;
        private LibraryManager libraryManager;

        private List<bool> moduleSwitch = new List<bool>();
        private List<MEHoloModule> moduleList = new List<MEHoloModule>();

        public delegate void DispatchDelegate(object param);
        private class DispatchMessage
        {
            public DispatchDelegate function;
            public object param;

            public DispatchMessage(DispatchDelegate f, object p)
            {
                function = f;
                param = p;
            }
        }


        private Queue _dispathMessageQueue = new Queue();
        private Queue dispathMessageQueue;

        private BlockMenu systemMenu;

        void Awake()
        {
            Instance = this;
            dispathMessageQueue = Queue.Synchronized(_dispathMessageQueue);
        }

        // Use this for initialization
        void Start()
        {
            StartCoroutine(CheckSystem());
        }

        /// <summary>
        /// 检查系统是否满足条件 
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckSystem()
        {
            // 检查应用名称是否已经设置 
            while (string.IsNullOrEmpty(AppID))
            {
                Debug.LogError("AppName can not be null! Please set AppName.");
                yield return new WaitForSeconds(1);
            }

            InitSystem();
        }

        /// <summary>
        /// 初始化系统 
        /// </summary>
        private void InitSystem()
        {
            accountManager = AccountManager.Instance;
#if true // UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WSA
            anchorController = SceneAnchorController.Instance;
#endif
            liveController = LiveController.Instance;
            inputManager = MultiInputManager.Instance;
            speechManager = SpeechManager.Instance;
            uiManager = UIManager.Instance;
            collaborationManager = CollaborationManager.Instance;
            libraryManager = LibraryManager.Instance;

            moduleSwitch.Add(NeedAccount);
#if true // UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WSA
            moduleSwitch.Add(NeedAnchor);
#endif
            moduleSwitch.Add(NeedInput);
            moduleSwitch.Add(NeedSpeech);
            moduleSwitch.Add(NeedUI);
            moduleSwitch.Add(NeedCollaboration);
            moduleSwitch.Add(NeedLibrary);
            moduleSwitch.Add(NeedLive);

            moduleList.Add(accountManager);
#if true // UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WSA
            moduleList.Add(anchorController);
#endif
            moduleList.Add(inputManager);
            moduleList.Add(speechManager);
            moduleList.Add(uiManager);
            moduleList.Add(collaborationManager);
            moduleList.Add(libraryManager);
            moduleList.Add(liveController);

            // 按需求启动模块 
            for (int i = 0; i < moduleSwitch.Count; i++)
            {
                if (moduleList[i] == null)
                    continue;

                if (moduleSwitch[i])
                {
                    moduleList[i].gameObject.SetActive(true);
                    moduleList[i].Init();
                }
                else
                {
                    moduleList[i].gameObject.SetActive(false);
                }
            }

            // 初始化系统菜单 
            InitSystemMenu();

            HasInit = true;

        }

        // Update is called once per frame
        void Update()
        {
            if (!HasInit)
                return;

            // 热键支持 
            if ((Input.GetKeyDown(KeyCode.M) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                || inputManager.controllerInput.GetButton(XBoxControllerButton.Menu))
            {
                OpenSystemMenu();
            }

            // 检查需要回到主线程的调用
            while (dispathMessageQueue.Count > 0)
            {
                DispatchMessage msg = (DispatchMessage)dispathMessageQueue.Dequeue();
                DispatchInvoke(msg);
            }
        }

        /// <summary>
        /// 检查消息队列，运行其中的代理函数
        /// </summary>
        /// <param name="msg"></param>
        private void DispatchInvoke(DispatchMessage msg)
        {
            if (msg.function != null)
            {
                msg.function(msg.param);
            }
        }

        /// <summary>
        /// 将一个方法延迟到主线程中运行
        /// </summary>
        /// <param name="function"></param>
        /// <param name="param"></param>
        public void Dispatch(DispatchDelegate function, object param = null)
        {
            dispathMessageQueue.Enqueue(new DispatchMessage(function, param));
        }


#region 系统菜单相关

        public void InitSystemMenu()
        {
            if (systemMenuData == null)
                return;

            BlockMenuData data = JsonUtil.Deserialize<BlockMenuData>(systemMenuData.text);

            uiManager.menuManager.CreateMenu(data);

            systemMenu = uiManager.menuManager.GetMenu(data.name);
            systemMenu.RegistButtonClick("SetAnchor", StartSetAnchor);

            uiManager.menuManager.cbMenuHide += OnMenuHide;


            // 开启语音菜单 
            if (speechManager.HasTurnOn())
                speechManager.TurnOff();

            speechManager.AddKeywords("Open Menu", OpenSystemMenu);
            speechManager.TurnOn();


            // 开启屏幕UI 

        }


        /// <summary>
        /// 开启系统菜单
        /// </summary>
        public void OpenSystemMenu()
        {
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            Vector3 pos = headPosition + gazeDirection * 2;

            uiManager.menuManager.ShowMenu(systemMenu, pos, gazeDirection);

            inputManager.layerMask = LayerMask.GetMask("UI");

        }

        private void OnMenuHide()
        {
            if (!startAnchorFitting)
                inputManager.layerMask = oldLayerMask;
        }


        private bool startAnchorFitting = false;
        private int oldLayerMask;
        /// <summary>
        /// 开始调整Anchor
        /// </summary>
        private void StartSetAnchor()
        {
            startAnchorFitting = true;
            Debug.Log("Start fit!");
            anchorController.AddCallbackFinish(AnchorMoveFinish);
            anchorController.TurnOn();

        }

        /// <summary>
        /// 调整Anchor结束
        /// </summary>
        private void AnchorMoveFinish()
        {
            startAnchorFitting = false;
            anchorController.RemoveCallbackFinish(AnchorMoveFinish);
            anchorController.TurnOff();

            inputManager.layerMask = oldLayerMask;
        }


        #endregion

    }
}
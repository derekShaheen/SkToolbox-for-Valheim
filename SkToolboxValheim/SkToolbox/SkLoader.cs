using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// Credit for some of the code in this class to wh0am15533, who can be found on Github. Your trainers have great example code. Thank you for posting your loaders and injectors!
/// Namespace: SkToolbox
/// Class: SkLoader
/// Method: Init or InitThreading
/// </summary>
namespace SkToolbox
{
    public class SkLoader : MonoBehaviour
    {
        private static GameObject _SkGameObject;

        private static bool FirstLoad = true;
        private static bool InitLogging = false;

        public static void Unload()
        {
            Destroy(_SkGameObject, 0f);
            SkLoader._SkGameObject = null; // https://answers.unity.com/questions/1186978/does-calling-destroy-on-a-gameobjectmonobehavior-d.html
        }
        public static void Reload()
        {
            Unload();
            Init();
        }

        public static GameObject Load
        {
            get
            {
                return SkLoader._SkGameObject;
            }
            set
            {
                SkLoader._SkGameObject = value;
            }
        }

        private void Start()
        {
            SkLoader.Init();
        }

        public static void Main(string[] args)
        {
            SkLoader.InitThreading();
        }

        public static void InitThreading()
        {
            new Thread(() =>
            {
                Thread.Sleep(5000); // 5 second sleep as initialization occurs *really* early

                Init();

            }).Start();
        }

        public static void InitWithLog()
        {
            InitLogging = true;
            Init();
        }

        public static void Init()
        {
            SkLoader._SkGameObject = new GameObject("SkToolbox");
            //SkLoader._SkGameObject.AddComponent<SkConsole>(); // Load the console first so output from the controller can be observed on the following frame

            if (InitLogging)
            {
                //SkConsole.writeToFile = true;
                InitLogging = false;
            }

            if (FirstLoad) SkUtilities.Logz(new string[] { "LOADER", "STARTUP" }, new string[] { "SUCCESS!" });

            CheckForUnknownInstance();

            SkLoader.Load.transform.parent = null;
            Transform root = SkLoader.Load.transform.parent;
            if (root != null)
            {
                if (root.gameObject != SkLoader.Load)
                {
                    root.parent = SkLoader.Load.transform;
                }
            }

            SkLoader._SkGameObject.AddComponent<SkMenuController>(); // Load the menu controller
            Object.DontDestroyOnLoad(SkLoader._SkGameObject);
            FirstLoad = false;
        }

        public static void CheckForUnknownInstance()
        {
            var OtherSkToolBoxs = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "SkToolbox");

            foreach (var Other in OtherSkToolBoxs)
            {
                if (Other != SkLoader._SkGameObject)
                {
                    Destroy(Other);
                    SkUtilities.Logz(new string[] { "LOADER", "DETECT" }, new string[] { "Other SkToolbox Destroyed." });
                }
            }
        }

        void OnDestroy()
        {
            Unload();
        }
    }

}


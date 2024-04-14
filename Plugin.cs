using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Video;
using Utilla;

namespace MonkeyPhone
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        string fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        List<string> videos = new List<string>();
        public GameObject _Phone;
        bool hold;
        int currentlyVideo = 0;

        void Awake()
        {
            Events.GameInitialized += GameInitialized;
            
            
        }

        void Update()
        {
            if(_Phone  != null)
            {
                if (ControllerInputPoller.instance.leftControllerPrimaryButton && !hold)
                {
                    var videoPlayer = _Phone.transform.Find("Video Player").GetComponent<VideoPlayer>();
                    hold = true;
                    currentlyVideo++;
                    if (currentlyVideo > videos.Count - 1)
                    {
                        currentlyVideo = 0;
                    }
                    videoPlayer.url = fileLocation + "\\Videos\\" + videos[currentlyVideo];
                    videoPlayer.Play();
                }
                else if (!ControllerInputPoller.instance.leftControllerPrimaryButton && hold)
                {
                    hold = false;
                }
            }
            
        }

        private void GameInitialized(object sender, EventArgs e)
        {
            InstantiatePhone();
            LookUpVideos();
        }
        void InstantiatePhone()
        {
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeyPhone.phone");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);

            GameObject Phone = bundle.LoadAsset<GameObject>("Phone");
            _Phone = Instantiate(Phone);

            Transform leftHand = GorillaLocomotion.Player.Instance.leftControllerTransform;
            _Phone.transform.SetParent(leftHand.transform, false);
            _Phone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            _Phone.transform.eulerAngles = new Vector3(90,-90,0);
            _Phone.transform.localPosition = new Vector3( 0.08f, 0.04f, -0.08f);
            _Phone.transform.Find("Video Player").GetComponent<VideoPlayer>().clip = null;

        }

        public void LookUpVideos()
        {
            DirectoryInfo directory = new DirectoryInfo(fileLocation + "\\Videos");
            foreach (var file in directory.GetFiles("*.mp4"))
            {
                videos.Add(file.Name);
                Debug.Log(file.Name);
            }
            var videoPlayer = _Phone.transform.Find("Video Player").GetComponent<VideoPlayer>();
            videoPlayer.url = fileLocation + "\\Videos\\" + videos[currentlyVideo];
            videoPlayer.Play();
        }


    }
}
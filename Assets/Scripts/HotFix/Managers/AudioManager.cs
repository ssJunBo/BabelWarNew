using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.Game.Base;
using Main.Game.ResourceFrame;
using UnityEngine;

namespace HotFix.Managers
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private AudioSource bgAudioSource;
        [SerializeField] private AudioSource uiAudioSource;

        private readonly Dictionary<string, AudioClip> _audioClipsDict = new Dictionary<string, AudioClip>();

        private const string BgPath = "Sound/Bg/";
        private const string UIPath = "Sound/UI/";
        private const string OtherPath = "Sound/Other/";

        private GameObject audioGameObj;

        protected override void Awake()
        {
            base.Awake();

            audioGameObj = new GameObject("AudioSourceObj");
            DontDestroyOnLoad(audioGameObj);

            if (bgAudioSource == null)
            {
                bgAudioSource = gameObject.AddComponent<AudioSource>();
            }

            if (uiAudioSource == null)
            {
                uiAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayBg(string clipName)
        {
            _audioClipsDict.TryGetValue(clipName, out var audioClip);
            if (audioClip == null)
            {
                audioClip = ResManager.Instance.LoadResource<AudioClip>(BgPath + clipName);
                _audioClipsDict.Add(clipName, audioClip);
            }

            bgAudioSource.clip = audioClip;
            bgAudioSource.Play();
            bgAudioSource.loop = true;
        }

        public void SetBgMusicState(bool isPlay)
        {
            if (isPlay)
            {
                bgAudioSource.Play();
            }
            else
            {
                bgAudioSource.Pause();
            }
        }

        public void PlayUI(string clipName)
        {
            _audioClipsDict.TryGetValue(clipName, out var audioClip);
            if (audioClip == null)
            {
                audioClip = ResManager.Instance.LoadResource<AudioClip>(UIPath + clipName);
                _audioClipsDict.Add(clipName, audioClip);
            }

            uiAudioSource.clip = audioClip;
            uiAudioSource.Play();
            uiAudioSource.loop = false;
        }

        private readonly Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();

        public void PlaySound(string clipName, float soundVolume = 1)
        {
            _audioClipsDict.TryGetValue(clipName, out var audioClip);
            if (audioClip == null)
            {
                audioClip = ResManager.Instance.LoadResource<AudioClip>(OtherPath + clipName);
                _audioClipsDict.Add(clipName, audioClip);
            }

            var audioSource = _audioSourcePool.Count > 0
                ? _audioSourcePool.Dequeue()
                : audioGameObj.AddComponent<AudioSource>();
            audioSource.enabled = true;
            audioSource.volume = soundVolume;
            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.loop = false;

            TimerEventManager.Instance.DelaySeconds(audioClip.length, () =>
            {
                audioSource.enabled = false;
                _audioSourcePool.Enqueue(audioSource);
            });
        }
    }
}
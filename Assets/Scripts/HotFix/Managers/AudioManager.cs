using System.Collections.Generic;
using Main.Game.Base;
using Main.Game.ResourceFrame;
using UnityEngine;

namespace HotFix.Managers
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private AudioSource _bgAudioSource;
        private AudioSource _uiAudioSource;

        private readonly Dictionary<string, AudioClip> _audioClipsDict = new Dictionary<string, AudioClip>();

        private const string BgPath = "Sound/Bg/";
        private const string UIPath = "Sound/UI/";
        private const string OtherPath = "Sound/Other/";

        private GameObject _audioGameObj;

        protected override void Awake()
        {
            base.Awake();

            _audioGameObj = new GameObject("AudioSourceObj");
            // DontDestroyOnLoad(_audioGameObj);

            if (_bgAudioSource == null)
            {
                _bgAudioSource = gameObject.AddComponent<AudioSource>();
            }

            if (_uiAudioSource == null)
            {
                _uiAudioSource = gameObject.AddComponent<AudioSource>();
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

            _bgAudioSource.clip = audioClip;
            _bgAudioSource.Play();
            _bgAudioSource.loop = true;
        }

        public void SetBgMusicState(bool isPlay)
        {
            if (isPlay)
            {
                _bgAudioSource.Play();
            }
            else
            {
                _bgAudioSource.Pause();
            }
        }

        public void SetBgSoundSize(float val)
        {
            _bgAudioSource.volume = val;
        }

        public float GetBgSoundSize()
        {
            return _bgAudioSource.volume;
        }

        public void PlayUI(string clipName)
        {
            _audioClipsDict.TryGetValue(clipName, out var audioClip);
            if (audioClip == null)
            {
                audioClip = ResManager.Instance.LoadResource<AudioClip>(UIPath + clipName);
                _audioClipsDict.Add(clipName, audioClip);
            }

            _uiAudioSource.clip = audioClip;
            _uiAudioSource.Play();
            _uiAudioSource.loop = false;
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
                : _audioGameObj.AddComponent<AudioSource>();
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
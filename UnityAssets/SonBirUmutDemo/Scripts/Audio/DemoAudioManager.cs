using System;
using System.Collections.Generic;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class DemoAudioManager : MonoBehaviour
    {
        private const string VolumeKey = "SonBirUmut.Release.Audio.Volume.v1";
        private const string MuteKey = "SonBirUmut.Release.Audio.Mute.v1";
        [SerializeField] private AudioSource source;
        private readonly Dictionary<SoundCue,AudioClip> clips = new Dictionary<SoundCue,AudioClip>();
        private bool dirty;
        private float nextPreview;
        public SoundPreferences Settings { get; private set; } = new SoundPreferences();
        public bool Available { get; private set; }
        public void Configure(AudioSource audioSource) => source = audioSource;

        private void Awake()
        {
            if (source == null) source = GetComponent<AudioSource>();
            float volume = PlayerPrefs.GetFloat(VolumeKey,.65f);
            if (float.IsNaN(volume) || float.IsInfinity(volume)) volume = .65f;
            Settings = new SoundPreferences(volume,PlayerPrefs.GetInt(MuteKey,0) != 0);
            source.playOnAwake = false; source.loop = false; source.spatialBlend = 0;
            Apply();
            try
            {
                foreach (SoundCue cue in Enum.GetValues(typeof(SoundCue)))
                {
                    var samples = SoundSynthesis.Generate(cue);
                    var clip = AudioClip.Create("SonBirUmut_"+cue,samples.Length,1,22050,false);
                    if (clip == null) throw new InvalidOperationException("AudioClip oluşturulamadı.");
                    clips.Add(cue,clip);
                    if (!clip.SetData(samples,0)) throw new InvalidOperationException("AudioClip verisi yüklenemedi.");
                }
                Available = true;
            }
            catch (Exception error)
            {
                Available = false;
                Debug.LogWarning("Sesler oluşturulamadı; oyun sessiz devam edecek. " + error.Message,this);
            }
        }

        public void Play(SoundCue cue)
        {
            if (!Available || source == null || Settings.EffectiveVolume <= 0) return;
            if (clips.TryGetValue(cue,out var clip)) source.PlayOneShot(clip,1f);
        }
        public void SetVolume(float volume)
        {
            if (Math.Abs(Settings.Volume-volume) < .000001) return;
            Settings.SetVolume(volume); Apply(); Store();
            if (Time.unscaledTime >= nextPreview) { Play(SoundCue.Menu); nextPreview = Time.unscaledTime + .12f; }
        }
        public void SetMuted(bool muted)
        {
            if (Settings.Muted == muted) return;
            Settings.SetMuted(muted); Apply(); Store(); SavePreferences(); Play(SoundCue.Menu);
        }
        private void Apply()
        { if (source != null) { source.mute = Settings.Muted; source.volume = (float)Settings.Volume; } }
        private void Store()
        { PlayerPrefs.SetFloat(VolumeKey,(float)Settings.Volume); PlayerPrefs.SetInt(MuteKey,Settings.Muted?1:0); dirty = true; }
        public void SavePreferences() { if (!dirty) return; PlayerPrefs.Save(); dirty = false; }
        public void StopEffects() { if (source != null) source.Stop(); }
        private void OnApplicationPause(bool paused) { if (paused) { StopEffects(); SavePreferences(); } }
        private void OnApplicationQuit() => SavePreferences();
        private void OnDisable() { StopEffects(); SavePreferences(); }
        private void OnDestroy()
        {
            foreach (var clip in clips.Values) if (clip != null) Destroy(clip);
            clips.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Audio
{
    [DefaultExecutionOrder(-90)]
    [RequireComponent(typeof(AudioSource))]
    public sealed class DemoAudioManager : MonoBehaviour
    {
        // Existing users keep both keys and master/mute defaults.
        private const string VolumeKey = "SonBirUmut.Release.Audio.Volume.v1";
        private const string MuteKey = "SonBirUmut.Release.Audio.Mute.v1";
        private const string MusicKey = "SonBirUmut.Release.Audio.Music.v1";
        private const string EffectsKey = "SonBirUmut.Release.Audio.Effects.v1";
        [SerializeField] private AudioSource source;
        [SerializeField] private bool atmosphereEnabled;
        [SerializeField] private DemoGameController game;
        [SerializeField] private AudioSource musicSource, windSource, tensionSource;
        [SerializeField] private AudioClip menuClip, windClip, tensionClip;
        private readonly Dictionary<SoundCue,AudioClip> clips = new Dictionary<SoundCue,AudioClip>();
        private readonly SoundEventGate events=new SoundEventGate();
        private readonly LoopPlayback[] transports={new LoopPlayback(),new LoopPlayback(),new LoopPlayback()};
        private bool dirty, focused=true, nativePaused, atmosphereReady;
        private float nextPreview, musicGain, windGain, tensionGain;
        public SoundPreferences Settings { get; private set; } = new SoundPreferences();
        public bool Available { get; private set; }
        public bool UsesAtmosphere => atmosphereEnabled;
        public bool AtmosphereAvailable => atmosphereReady;
        public bool AnyAudioAvailable => Available || atmosphereReady;
        public void Configure(AudioSource audioSource) => source = audioSource;
        public void ConfigureAtmosphere(DemoGameController owner,AudioSource music,AudioSource wind,AudioSource tension,
            AudioClip menu,AudioClip ambience,AudioClip crisis)
        {
            atmosphereEnabled=true;game=owner;musicSource=music;windSource=wind;tensionSource=tension;
            menuClip=menu;windClip=ambience;tensionClip=crisis;
            if(!ValidAtmosphereReferences()) throw new ArgumentException("Atmosphere needs three distinct loop sources and clips.");
        }
        public bool ValidAtmosphereReferences() => !atmosphereEnabled || (game!=null && source!=null &&
            musicSource!=null && windSource!=null && tensionSource!=null && menuClip!=null && windClip!=null && tensionClip!=null &&
            source!=musicSource && source!=windSource && source!=tensionSource && musicSource!=windSource &&
            musicSource!=tensionSource && windSource!=tensionSource);

        private static float Preference(string key,float fallback)
        {
            float value=PlayerPrefs.GetFloat(key,fallback);
            return float.IsNaN(value) || float.IsInfinity(value) ? fallback : Mathf.Clamp01(value);
        }
        private void Awake()
        {
            if (source == null) source = GetComponent<AudioSource>();
            focused=Application.isFocused;
            Settings = new SoundPreferences(Preference(VolumeKey,.65f),PlayerPrefs.GetInt(MuteKey,0)!=0,
                Preference(MusicKey,.55f),Preference(EffectsKey,1));
            source.playOnAwake=false;source.loop=false;source.spatialBlend=0;
            Apply();
            try
            {
                foreach (SoundCue cue in Enum.GetValues(typeof(SoundCue)))
                {
                    var samples=SoundSynthesis.Generate(cue);
                    var clip=AudioClip.Create("SonBirUmut_"+cue,samples.Length,1,22050,false);
                    if(clip==null) throw new InvalidOperationException("AudioClip oluşturulamadı.");
                    clips.Add(cue,clip);
                    if(!clip.SetData(samples,0)) throw new InvalidOperationException("AudioClip verisi yüklenemedi.");
                }
                Available=true;
            }
            catch(Exception error) { Available=false;Debug.LogWarning("Kısa sesler kullanılamıyor: "+error.Message,this); }
            if(atmosphereEnabled)
            {
                try
                {
                    if(!ValidAtmosphereReferences()) throw new InvalidOperationException("Atmosfer referansları eksik.");
                    SetupLoop(musicSource,menuClip);SetupLoop(windSource,windClip);SetupLoop(tensionSource,tensionClip);
                    atmosphereReady=true;
                }
                catch(Exception error) { atmosphereReady=false;StopLoops();Debug.LogWarning("Ses atmosferi kullanılamıyor: "+error.Message,this); }
            }
        }
        private static void SetupLoop(AudioSource target,AudioClip clip)
        {
            target.playOnAwake=false;target.loop=true;target.spatialBlend=0;target.volume=0;target.clip=clip;
            if(clip.loadState==AudioDataLoadState.Failed) throw new InvalidOperationException("WAV yüklenemedi.");
        }
        private void Update()
        {
            if(!atmosphereReady) return;
            var mix=AudioMix.Evaluate(game.Run,game.Menu!=null && game.Menu.MainMenuBackground,game.CardsOpen,focused,nativePaused);
            bool active=mix.Mood!=AudioMood.Silent && !Settings.Muted;
            if(!active) { StopLoops();return; }
            try
            {
                DriveLoop(musicSource,transports[0],true);DriveLoop(windSource,transports[1],true);DriveLoop(tensionSource,transports[2],true);
                // Very short smoothing; source mute/focus stop remains immediate.
                float step=Time.unscaledDeltaTime*3;
                musicGain=Mathf.MoveTowards(musicGain,(float)mix.Music,step);
                windGain=Mathf.MoveTowards(windGain,(float)mix.Ambience,step);
                tensionGain=Mathf.MoveTowards(tensionGain,(float)mix.Tension,step);
                Apply();
            }
            catch(Exception error) { atmosphereReady=false;StopLoops();Debug.LogWarning("Ses atmosferi durduruldu: "+error.Message,this); }
        }
        private static void DriveLoop(AudioSource target,LoopPlayback transport,bool active)
        {
            switch(transport.SetActive(active))
            { case LoopAction.Start:target.Play();break;case LoopAction.Stop:if(target!=null)target.Stop();break; }
        }
        private void StopLoops()
        {
            DriveLoop(musicSource,transports[0],false);DriveLoop(windSource,transports[1],false);DriveLoop(tensionSource,transports[2],false);
            musicGain=windGain=tensionGain=0;
            Apply();
        }
        public void Play(SoundCue cue)
        {
            if(!Available || source==null || !focused || nativePaused || Settings.EffectiveEffects<=0) return;
            if(!clips.TryGetValue(cue,out var clip)) return;
            if(!events.TryBegin(cue,Time.frameCount,Time.unscaledTime,clip.length,out bool interrupt)) return;
            if(interrupt) source.Stop();
            source.PlayOneShot(clip,1f);
        }
        public void SetVolume(float volume)
        {
            if(Math.Abs(Settings.Volume-volume)<.000001) return;
            Settings.SetVolume(volume);Apply();Store();Preview();
        }
        public void SetMusicVolume(float volume)
        {
            if(Math.Abs(Settings.MusicVolume-volume)<.000001) return;
            Settings.SetMusicVolume(volume);Apply();Store();
        }
        public void SetEffectsVolume(float volume)
        {
            if(Math.Abs(Settings.EffectsVolume-volume)<.000001) return;
            Settings.SetEffectsVolume(volume);Apply();Store();Preview();
        }
        private void Preview()
        { if(Time.unscaledTime>=nextPreview) { Play(SoundCue.Menu);nextPreview=Time.unscaledTime+.12f; } }
        public void SetMuted(bool muted)
        {
            if(Settings.Muted==muted) return;
            Settings.SetMuted(muted);Apply();Store();SavePreferences();
            if(muted) {StopEffects();StopLoops();} else Play(SoundCue.Menu);
        }
        private void Apply()
        {
            if(source!=null) {source.mute=Settings.Muted;source.volume=(float)Settings.EffectiveEffects;}
            ApplyLoop(musicSource,musicGain*.32f*(float)Settings.EffectiveMusic);
            ApplyLoop(windSource,windGain*.25f*(float)Settings.EffectiveEffects);
            ApplyLoop(tensionSource,tensionGain*.12f*(float)Settings.EffectiveEffects);
        }
        private void ApplyLoop(AudioSource target,float volume)
        { if(target!=null) {target.mute=Settings.Muted;target.volume=volume;} }
        private void Store()
        {
            PlayerPrefs.SetFloat(VolumeKey,(float)Settings.Volume);PlayerPrefs.SetInt(MuteKey,Settings.Muted?1:0);
            PlayerPrefs.SetFloat(MusicKey,(float)Settings.MusicVolume);PlayerPrefs.SetFloat(EffectsKey,(float)Settings.EffectsVolume);dirty=true;
        }
        public void SavePreferences() {if(!dirty)return;PlayerPrefs.Save();dirty=false;}
        public void StopEffects() {if(source!=null)source.Stop();events.Reset();}
        public void ResetSession() {StopEffects();tensionGain=0;Apply();}
        private void OnApplicationFocus(bool value)
        {focused=value;if(!value){StopEffects();StopLoops();SavePreferences();}}
        private void OnApplicationPause(bool paused)
        {nativePaused=paused;if(paused){StopEffects();StopLoops();SavePreferences();}}
        private void OnApplicationQuit() => SavePreferences();
        private void OnDisable() {StopEffects();StopLoops();SavePreferences();}
        private void OnDestroy()
        {
            foreach(var clip in clips.Values) if(clip!=null) Destroy(clip);
            clips.Clear(); // Imported loop assets are owned by the project, never destroyed here.
        }
    }
}

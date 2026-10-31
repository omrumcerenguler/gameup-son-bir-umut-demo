#if UNITY_EDITOR
using System;
using SonBirUmutDemo.Audio;
using SonBirUmutDemo.Core;
using UnityEditor;
using UnityEngine;

namespace SonBirUmutDemo.Editor
{
    public static class DemoPolishAudioBuilder
    {
        private const string Root="Assets/SonBirUmutDemo/Audio/";
        public static AudioClip[] TryImportClips()
        {
            try {return ImportClips();}
            catch(Exception error) {Debug.LogWarning("Arka plan sesi eklenmedi; sahne kısa efektlerle oluşturulacak. "+error.Message);return null;}
        }
        private static AudioClip[] ImportClips()
        {
            var names=new[]{"HopefulMenu.wav","VillageWind.wav","CrisisUndertone.wav"};
            foreach(string name in names)
                if(!System.IO.File.Exists(Root+name)) throw new InvalidOperationException("Audio klasörünü eksiksiz aktarın: "+name);
            var result=new AudioClip[names.Length];
            for(int i=0;i<names.Length;i++)
            {
                string path=Root+names[i];
                AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
                var importer=AssetImporter.GetAtPath(path) as AudioImporter;
                if(importer==null) throw new InvalidOperationException("Özgün WAV eksik: "+path+". Audio klasörünü de kopyalayın.");
                var sample=importer.defaultSampleSettings;
                sample.loadType=AudioClipLoadType.DecompressOnLoad;sample.compressionFormat=AudioCompressionFormat.PCM;
                sample.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;sample.preloadAudioData=true;
                importer.defaultSampleSettings=sample;importer.forceToMono=true;importer.loadInBackground=false;
                // Only these three new assets, never global Player/Audio/pipeline settings.
                importer.SaveAndReimport();
                result[i]=AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if(result[i]==null || result[i].channels!=1 || result[i].length>16)
                    throw new InvalidOperationException("WAV importunu kontrol edin: "+path);
            }
            return result;
        }
        public static void Configure(DemoGameController game,DemoAudioManager audio,AudioClip[] clips)
        {
            var root=new GameObject("Özgün müzik ve ortam - sabit üç kaynak");root.transform.SetParent(game.transform,false);
            var music=Loop(root,"Menu Music");var wind=Loop(root,"Village Wind");var tension=Loop(root,"Crisis Undertone");
            audio.ConfigureAtmosphere(game,music,wind,tension,clips[0],clips[1],clips[2]);
            if(!audio.ValidAtmosphereReferences()) throw new InvalidOperationException("Ses kaynakları ayrı ve referanslar dolu olmalı.");
        }
        private static AudioSource Loop(GameObject parent,string name)
        {
            var child=new GameObject(name);child.transform.SetParent(parent.transform,false);
            var source=child.AddComponent<AudioSource>();source.playOnAwake=false;source.loop=true;source.spatialBlend=0;source.volume=0;
            return source;
        }
    }
}
#endif

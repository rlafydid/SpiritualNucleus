using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager> {


		private Queue<string> audioQueue = new Queue<string>();
		private Queue<string> musicQueue = new Queue<string>();

	    private AudioSource audioSource;

		private AudioSource musicSource;

		private static bool isInited = false;

		private Dictionary<string, AudioClip> poolsMusic = new Dictionary<string, AudioClip> ();

    protected override void Constructor()
    {
        base.Constructor();
		musicSource = gameObject.AddComponent<AudioSource>();
		gameObject.AddComponent<AudioListener>();
	}

	protected override void Init()
        {
            base.Init();
        }

		public void PlayAudio(string path)
		{
		GameObject obj = new GameObject();
		audioSource = obj.AddComponent<AudioSource>();
				//audioSource = this.GetComponent<AudioSource> ();
				//if (audioSource == null) {
				//		audioSource = gameObject.AddComponent<AudioSource>();
				//}
				AudioClip clip = Facade.Asset.Instantiate<AudioClip>(path);
				audioSource.clip = clip;
				audioSource.Play ();
		}

		public void PlayMusic(string path)
		{
		//				Resources.UnloadAsset (musicSource.clip);
		//				musicSource.clip = null;
		//musicSource.Stop ();
		    AudioClip clip = Facade.Asset.Load<AudioClip>(path);
				musicSource.clip = clip;
				musicSource.Play ();
		}

        public void PlayQueueMusic(string path)
        {
		    musicQueue.Enqueue(path);
		    if (musicQueue.Count == 1)
		    {
			    StartCoroutine(PlayMusicCallBack());
		    }
	    }

		public void StopAudio(string path = null)
		{
				if (audioSource == null || audioSource.clip == null)
						return;
				
				if (path == null || path.IndexOf (audioSource.clip.name) >= 0) {

						audioSource.Stop ();
				}
		}

		public void LoadQueueAudio(string path)
		{
				audioQueue.Enqueue (path);
				if (audioQueue.Count == 1) {
						StartCoroutine (PlayCallBack());
				}
		}

		IEnumerator PlayCallBack()
		{
			yield return 0;
			if (audioQueue.Count > 0) {
					PlayAudio (audioQueue.Dequeue());
					yield return new WaitForSeconds (audioSource.clip.length);
					yield return PlayCallBack ();
			}

		}

	IEnumerator PlayMusicCallBack()
	{
		yield return 0;
		if (musicQueue.Count > 0)
		{
			PlayMusic(musicQueue.Dequeue());
			yield return new WaitForSeconds(musicSource.clip.length);
			yield return PlayMusicCallBack();
		}

	}

	public void ClearAudio()
		{
				if (audioSource != null) {
						audioSource.Stop ();
						audioSource.clip = null;
				}
				if (musicSource != null) {
						musicSource.Stop ();
						musicSource.clip = null;
				}
		}
}

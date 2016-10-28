/// <Licensing>
/// � 2011(Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License(the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    ///	Attach this component to any GameObject to automatically self-destruct (destroy, or 
	/// if pooling, despawn) it when its ParticleSystem or AudioSource are done.
    /// </summary>
    [AddComponentMenu("Path-o-logical/Common/Self-Despawn")]
    public class SelfDespawn : MonoBehaviour
    {
		protected ParticleSystem particles;
		protected AudioSource audioSource;
		
		protected string poolName = "";  // For use with PoolManager 
		
		protected void Awake()
		{
			this.particles = this.GetComponent<ParticleSystem>();
			this.audioSource = this.GetComponent<AudioSource>();
		}
		
		protected void OnEnable()
		{
			if (this.particles)
				this.StartCoroutine(this.ListenToDespawn(this.particles));

			if (this.audioSource)
				this.StartCoroutine(this.ListenToDespawn(this.audioSource));
		}
		
#if POOLMANAGER_INSTALLED
		/// <summary>
		/// If using PoolManager this will provide the pool name for despawn.
		/// </summary>
		protected void OnSpawned(SpawnPool spawnPool)
		{
			this.poolName = spawnPool.poolName;
		}
#endif
		
        protected IEnumerator ListenToDespawn(ParticleSystem emitter)
        {
            // Wait for the delay time to complete
            // Waiting the extra frame seems to be more stable and means at least one 
            //  frame will always pass
            yield return new WaitForSeconds(emitter.startDelay + 0.25f);
						
            while (emitter.IsAlive(true))
            {
                if (!emitter.gameObject.activeInHierarchy)
                {
                    emitter.Clear(true);
                    yield break;  // Do nothing, already despawned. Quit.
                }

                yield return null;
            }

            // Turn off emit before despawning
            InstanceManager.Despawn(this.poolName, emitter.transform);
        }

		protected IEnumerator ListenToDespawn(AudioSource src)
        {
            // Safer to wait a frame before testing if playing.
            yield return null;

            while (src.isPlaying)
                yield return null;

            InstanceManager.Despawn(this.poolName, src.transform);
        }
	}
}
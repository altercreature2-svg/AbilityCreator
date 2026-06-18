using System;
using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using Landfall.TABS.GameState;
using TFBGames;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000012 RID: 18
	public class BillyKeys : GameStateListener
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00005A4C File Offset: 0x00003C4C
		public override void OnEnterNewScene()
		{
			base.OnEnterNewScene();
			PlayerCamerasManager service = ServiceLocator.GetService<PlayerCamerasManager>();
			this.mainCam = ((service != null) ? service.GetMainCam(TFBGames.Player.One).transform : null);
			this.save = ServiceLocator.GetService<ISaveLoaderService>();
			bool flag = base.GetComponentInChildren<RotationShake>();
			if (flag)
			{
				this.rotationShake = base.GetComponentInChildren<RotationShake>();
			}
			bool flag2 = base.GetComponentInChildren<Rigidbody>();
			if (flag2)
			{
				this.secretObject = base.GetComponentInChildren<Rigidbody>();
			}
			this.loopSource = base.GetComponent<AudioSource>();
			List<BillyKeys.BillyKey> list = new List<BillyKeys.BillyKey>();
			bool flag3 = this.keys.Count > 0;
			if (flag3)
			{
				for (int i = 0; i < this.keys.Count; i++)
				{
					this.isUnlocking.Add(false);
					bool flag4 = this.save.HasUnlockedSecret(this.toBeUnlocked[i]);
					if (flag4)
					{
						this.keys[i].SetActive(true);
						this.keys[i].GetComponent<Animator>().Play("BillyKey");
						list.Add(new BillyKeys.BillyKey(this.keys[i], this.alreadyUnlocked[i], this.toBeUnlocked[i], this.isUnlocking[i]));
					}
				}
			}
			foreach (BillyKeys.BillyKey billyKey in list)
			{
				this.keys.Remove(billyKey.key);
				this.alreadyUnlocked.Remove(billyKey.alreadyUnlocked);
				this.toBeUnlocked.Remove(billyKey.toBeUnlocked);
				this.isUnlocking.Remove(billyKey.isUnlocking);
			}
			this.CheckUnlocks();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005C44 File Offset: 0x00003E44
		public override void OnEnterPlacementState()
		{
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005C47 File Offset: 0x00003E47
		public override void OnEnterBattleState()
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005C4C File Offset: 0x00003E4C
		public void Update()
		{
			bool flag = this.save.HasUnlockedSecret(this.finalUnlock);
			if (!flag)
			{
				float num = Vector3.Distance(this.secretObject.worldCenterOfMass, this.mainCam.position);
				float num2 = Vector3.Angle(this.mainCam.forward, this.secretObject.worldCenterOfMass - this.mainCam.position);
				float num3 = 1000f / (num * num2);
				this.loopSource.volume = Mathf.Pow(this.unlockValue * 0.25f, 1.3f);
				this.loopSource.pitch = 1f + 1f * this.unlockValue;
				bool flag2 = this.done;
				if (flag2)
				{
					bool flag3 = this.unlockValue > 0f || num3 > 10f;
					if (flag3)
					{
						this.SetColor();
					}
					bool flag4 = num > this.unlockDistance;
					if (flag4)
					{
						this.unlockValue -= Time.unscaledDeltaTime * 0.2f;
					}
					else
					{
						bool flag5 = num3 > 8f;
						if (flag5)
						{
							this.unlockValue += Time.unscaledDeltaTime * 0.2f;
							this.UnlockProgressFeedback();
							bool flag6 = this.unlockValue > 1f;
							if (flag6)
							{
								this.UnlockSelf();
							}
						}
						else
						{
							this.unlockValue -= Time.unscaledDeltaTime * 0.2f;
						}
					}
				}
				else
				{
					for (int i = 0; i < this.keys.Count; i++)
					{
						bool flag7 = this.save.HasUnlockedSecret(this.alreadyUnlocked[i]) && !this.save.HasUnlockedSecret(this.toBeUnlocked[i]) && !this.isUnlocking[i] && num3 > 8f;
						if (flag7)
						{
							base.StartCoroutine(this.UnlockKey(i));
						}
					}
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005E5B File Offset: 0x0000405B
		public IEnumerator UnlockKey(int index)
		{
			this.isUnlocking[index] = true;
			this.keys[index].SetActive(true);
			this.keys[index].GetComponent<Animator>().Play("BillyKey");
			yield return new WaitForSeconds(this.unlockDelay);
			this.save.UnlockSecret(this.toBeUnlocked[index]);
			this.loopSource.Stop();
			this.loopSource.volume = 1f;
			this.loopSource.PlayOneShot(this.hitClip);
			this.CheckUnlocks();
			yield break;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00005E74 File Offset: 0x00004074
		public void CheckUnlocks()
		{
			bool flag = this.toBeUnlocked.TrueForAll((string x) => this.save.HasUnlockedSecret(this.toBeUnlocked[this.toBeUnlocked.IndexOf(x)]));
			if (flag)
			{
				this.done = true;
				this.loopSource.Play();
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005EB4 File Offset: 0x000040B4
		public void UnlockSelf()
		{
			bool flag = this.save.HasUnlockedSecret(this.finalUnlock);
			if (!flag)
			{
				this.save.UnlockSecret(this.finalUnlock);
				ServiceLocator.GetService<ModalPanel>().OpenUnlockPanel(this.unlockDescription, this.unlockImage);
				PlacementUI placementUI = Object.FindObjectOfType<PlacementUI>();
				bool flag2 = placementUI != null;
				if (flag2)
				{
					placementUI.RedrawUI(this.finalUnlock);
				}
				this.loopSource.PlayOneShot(this.hitClip);
				this.unlockAllEvent.Invoke();
				base.StartCoroutine(this.ShrinkUnlockValue());
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005F4D File Offset: 0x0000414D
		public IEnumerator ShrinkUnlockValue()
		{
			while (this.unlockValue > 0f)
			{
				this.unlockValue -= Time.unscaledDeltaTime * 0.2f;
				this.SetColor();
				this.UnlockProgressFeedback();
				this.loopSource.volume = Mathf.Pow(this.unlockValue * 0.25f, 1.3f);
				this.loopSource.pitch = 1f + 1f * this.unlockValue;
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005F5C File Offset: 0x0000415C
		private void SetColor()
		{
			this.unlockValue = Mathf.Clamp(this.unlockValue, 0f, float.PositiveInfinity);
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				bool flag = !renderer.GetComponent<ParticleSystemRenderer>();
				if (flag)
				{
					Material[] materials = renderer.materials;
					for (int j = 0; j < materials.Length; j++)
					{
						bool flag2 = materials[j].HasProperty("_EmissionColor");
						if (flag2)
						{
							materials[j].EnableKeyword("_EMISSION");
							materials[j].SetColor("_EmissionColor", this.glowColor * this.unlockValue * 2f);
						}
					}
					renderer.materials = materials;
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00006040 File Offset: 0x00004240
		private void UnlockProgressFeedback()
		{
			bool flag = this.rotationShake;
			if (flag)
			{
				bool flag2 = this.unlockValue <= 0f;
				if (flag2)
				{
					this.rotationShake.AddForce(Random.onUnitSphere * 2f);
					this.unlockValue = 0f;
				}
				this.rotationShake.enabled = true;
				this.rotationShake.AddForce(Random.onUnitSphere * this.unlockValue * Time.deltaTime * 50f);
			}
		}

		// Token: 0x040000A3 RID: 163
		private Transform mainCam;

		// Token: 0x040000A4 RID: 164
		private ISaveLoaderService save;

		// Token: 0x040000A5 RID: 165
		private List<bool> isUnlocking = new List<bool>();

		// Token: 0x040000A6 RID: 166
		private RotationShake rotationShake;

		// Token: 0x040000A7 RID: 167
		private Rigidbody secretObject;

		// Token: 0x040000A8 RID: 168
		private AudioSource loopSource;

		// Token: 0x040000A9 RID: 169
		private bool done;

		// Token: 0x040000AA RID: 170
		private float unlockValue;

		// Token: 0x040000AB RID: 171
		[Header("Keys")]
		public List<GameObject> keys = new List<GameObject>();

		// Token: 0x040000AC RID: 172
		public List<string> alreadyUnlocked = new List<string>();

		// Token: 0x040000AD RID: 173
		public List<string> toBeUnlocked = new List<string>();

		// Token: 0x040000AE RID: 174
		[Header("Final Unlock")]
		public UnityEvent unlockAllEvent = new UnityEvent();

		// Token: 0x040000AF RID: 175
		public float unlockDelay = 2f;

		// Token: 0x040000B0 RID: 176
		public float unlockDistance = 4f;

		// Token: 0x040000B1 RID: 177
		public string finalUnlock;

		// Token: 0x040000B2 RID: 178
		public string unlockDescription;

		// Token: 0x040000B3 RID: 179
		public Sprite unlockImage;

		// Token: 0x040000B4 RID: 180
		public AudioClip hitClip;

		// Token: 0x040000B5 RID: 181
		public Color glowColor;

		// Token: 0x02000039 RID: 57
		private class BillyKey
		{
			// Token: 0x06000139 RID: 313 RVA: 0x0000D987 File Offset: 0x0000BB87
			public BillyKey(GameObject key, string alreadyUnlocked, string toBeUnlocked, bool isUnlocking)
			{
				this.key = key;
				this.alreadyUnlocked = alreadyUnlocked;
				this.toBeUnlocked = toBeUnlocked;
				this.isUnlocking = isUnlocking;
			}

			// Token: 0x040001E1 RID: 481
			public readonly GameObject key;

			// Token: 0x040001E2 RID: 482
			public readonly string alreadyUnlocked;

			// Token: 0x040001E3 RID: 483
			public readonly string toBeUnlocked;

			// Token: 0x040001E4 RID: 484
			public readonly bool isUnlocking;
		}
	}
}

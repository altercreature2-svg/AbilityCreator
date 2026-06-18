using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Landfall.TABS;
using Landfall.TABS.GameState;
using TFBGames;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000024 RID: 36
	public class SecretUnlockMultiple : GameStateListener
	{
		// Token: 0x060000CF RID: 207 RVA: 0x0000A5D4 File Offset: 0x000087D4
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_mainCamTransform == null;
			if (flag)
			{
				this.OnEnterNewScene();
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000A604 File Offset: 0x00008804
		private void Update()
		{
			bool flag = !(this.m_mainCamTransform != null) || !this.m_secretObject || this.done;
			if (!flag)
			{
				this.loopSource.volume = ((this.m_unlockValue <= 0f) ? 0f : Mathf.Pow(this.m_unlockValue * 0.25f, 1.3f));
				bool flag2 = float.IsNaN(this.loopSource.volume);
				if (flag2)
				{
					this.loopSource.volume = 0f;
				}
				float num = 1f + 1f * this.m_unlockValue;
				this.loopSource.pitch = ((num >= 0f) ? num : 0f);
				bool flag3 = this.m_unlockValue > 0f || this.m_lookValue > 10f;
				if (flag3)
				{
					this.SetColor();
				}
				float num2 = Vector3.Distance(this.m_secretObject.worldCenterOfMass, this.m_mainCamTransform.position);
				bool flag4 = num2 > this.distanceToUnlock;
				if (flag4)
				{
					this.m_unlockValue -= Time.unscaledDeltaTime * 0.2f;
				}
				else
				{
					float num3 = Vector3.Angle(this.m_mainCamTransform.forward, this.m_secretObject.worldCenterOfMass - this.m_mainCamTransform.position);
					this.m_lookValue = 1000f / (num2 * num3);
					bool flag5 = this.m_lookValue > 8f;
					if (flag5)
					{
						float num4 = 0.2f;
						this.m_unlockValue += num4 * Time.unscaledDeltaTime;
						this.UnlockProgressFeedback();
						bool flag6 = this.m_unlockValue > 1f;
						if (flag6)
						{
							base.StartCoroutine(this.UnlockSecret());
						}
					}
					else
					{
						this.m_unlockValue -= Time.unscaledDeltaTime * 0.2f;
					}
				}
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000A7F4 File Offset: 0x000089F4
		private void UnlockProgressFeedback()
		{
			bool flag = this.m_rotationShake;
			if (flag)
			{
				bool flag2 = this.m_unlockValue <= 0f;
				if (flag2)
				{
					this.m_rotationShake.AddForce(Random.onUnitSphere * 2f);
					this.m_unlockValue = 0f;
				}
				this.m_rotationShake.enabled = true;
				this.m_rotationShake.AddForce(Random.onUnitSphere * this.m_unlockValue * Time.deltaTime * 50f);
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000A88C File Offset: 0x00008A8C
		private void SetColor()
		{
			this.m_unlockValue = Mathf.Clamp(this.m_unlockValue, 0f, float.PositiveInfinity);
			Renderer[] componentsInChildren = this.m_secretObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] materials = componentsInChildren[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					bool flag = materials[j].HasProperty("_EmissionColor");
					if (flag)
					{
						materials[j].EnableKeyword("_EMISSION");
						materials[j].SetColor("_EmissionColor", this.glowColor * this.m_unlockValue * 2f);
					}
				}
				componentsInChildren[i].materials = materials;
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000A94F File Offset: 0x00008B4F
		private IEnumerator UnlockSecret()
		{
			bool flag = !base.enabled || string.IsNullOrWhiteSpace(this.secretKey);
			if (flag)
			{
				yield break;
			}
			bool flag2 = ScreenShake.Instance;
			if (flag2)
			{
				ScreenShake.Instance.AddForce(Vector3.up * 8f, this.m_secretObject.transform.position);
			}
			bool flag3 = this.unlockSparkEffect;
			if (flag3)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.unlockSparkEffect, this.m_secretObject.transform.position, this.m_secretObject.transform.rotation);
				gameObject.AddComponent<RemoveAfterSeconds>().seconds = 5f;
				MeshRenderer componentInChildren = this.m_secretObject.GetComponentInChildren<MeshRenderer>();
				bool flag4 = componentInChildren;
				if (flag4)
				{
					ParticleSystem.ShapeModule shape = gameObject.GetComponent<ParticleSystem>().shape;
					shape.meshRenderer = componentInChildren;
					shape = default(ParticleSystem.ShapeModule);
				}
				gameObject = null;
				componentInChildren = null;
			}
			this.m_secretObject.gameObject.SetActive(false);
			UnityEvent unityEvent = this.unlockEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.loopSource.Stop();
			this.loopSource.volume = 1f;
			this.loopSource.PlayOneShot(this.hitClip);
			this.done = true;
			ServiceLocator.GetService<ISaveLoaderService>().UnlockSecret(this.secretKey);
			int num;
			for (int i = 0; i < this.secretDescriptions.Count; i = num + 1)
			{
				ServiceLocator.GetService<ModalPanel>().OpenUnlockPanel(this.secretDescriptions[i], this.secretIcon);
				yield return new WaitForSeconds(0.1f);
				num = i;
			}
			yield break;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000A960 File Offset: 0x00008B60
		public override void OnEnterNewScene()
		{
			base.OnEnterNewScene();
			this.loopSource = base.GetComponent<AudioSource>();
			bool flag = this.loopSource;
			if (flag)
			{
				this.loopSource.volume = 0f;
			}
			this.m_rotationShake = base.GetComponentInChildren<RotationShake>();
			this.m_secretObject = base.GetComponentInChildren<Rigidbody>();
			bool flag2 = this.m_secretObject;
			if (flag2)
			{
				this.m_secretObject.isKinematic = true;
			}
			bool flag3 = !string.IsNullOrWhiteSpace(this.secretKey) && ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret(this.secretKey);
			if (flag3)
			{
				bool flag4 = this.m_secretObject;
				if (flag4)
				{
					this.m_secretObject.gameObject.SetActive(false);
				}
				base.enabled = false;
				UnityEvent unityEvent = this.hideEvent;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
			}
			PlayerCamerasManager service = ServiceLocator.GetService<PlayerCamerasManager>();
			MainCam mainCam = (service != null) ? service.GetMainCam(TFBGames.Player.One) : null;
			this.m_mainCamTransform = ((mainCam != null) ? mainCam.transform : null);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000AA69 File Offset: 0x00008C69
		public override void OnEnterPlacementState()
		{
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000AA6C File Offset: 0x00008C6C
		public override void OnEnterBattleState()
		{
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000AA70 File Offset: 0x00008C70
		public static void CheckAchievements()
		{
			AchievementService service = ServiceLocator.GetService<AchievementService>();
			SecretUnlockMultiple.<>c__DisplayClass24_0 CS$<>8__locals1;
			CS$<>8__locals1.secretService = ServiceLocator.GetService<ISaveLoaderService>();
			bool flag = SecretUnlockMultiple.<CheckAchievements>g__HasUnlockedFaction|24_0(874593522, ref CS$<>8__locals1);
			if (flag)
			{
				service.UnlockAchievement("UNLOCKED_ALL_SECRET");
			}
			bool flag2 = SecretUnlockMultiple.<CheckAchievements>g__HasUnlockedFaction|24_0(673578412, ref CS$<>8__locals1);
			if (flag2)
			{
				service.UnlockAchievement("UNLOCKED_ALL_LEGACY");
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000AAEC File Offset: 0x00008CEC
		[CompilerGenerated]
		internal static bool <CheckAchievements>g__HasUnlockedFaction|24_0(int factionId, ref SecretUnlockMultiple.<>c__DisplayClass24_0 A_1)
		{
			UnitBlueprint[] units = LandfallUnitDatabase.GetDatabase().GetFactionByGUID(new DatabaseID(-1, factionId)).Units;
			for (int i = 0; i < units.Length; i++)
			{
				string unlockKey = units[i].Entity.UnlockKey;
				bool flag = !string.IsNullOrEmpty(unlockKey) && !A_1.secretService.HasUnlockedSecret(unlockKey);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400014C RID: 332
		public string secretKey;

		// Token: 0x0400014D RID: 333
		public List<string> secretDescriptions = new List<string>();

		// Token: 0x0400014E RID: 334
		public Sprite secretIcon;

		// Token: 0x0400014F RID: 335
		public float distanceToUnlock = 5f;

		// Token: 0x04000150 RID: 336
		private RotationShake m_rotationShake;

		// Token: 0x04000151 RID: 337
		private Rigidbody m_secretObject;

		// Token: 0x04000152 RID: 338
		private float m_lookValue;

		// Token: 0x04000153 RID: 339
		private float m_unlockValue;

		// Token: 0x04000154 RID: 340
		public AudioClip hitClip;

		// Token: 0x04000155 RID: 341
		private AudioSource loopSource;

		// Token: 0x04000156 RID: 342
		private Transform m_mainCamTransform;

		// Token: 0x04000157 RID: 343
		public UnityEvent unlockEvent;

		// Token: 0x04000158 RID: 344
		public UnityEvent hideEvent;

		// Token: 0x04000159 RID: 345
		private bool done;

		// Token: 0x0400015A RID: 346
		public Color glowColor;

		// Token: 0x0400015B RID: 347
		public GameObject unlockSparkEffect;
	}
}

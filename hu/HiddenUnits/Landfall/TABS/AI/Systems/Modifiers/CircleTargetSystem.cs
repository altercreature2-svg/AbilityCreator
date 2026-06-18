using System;
using EzECS.Barriers;
using Landfall.TABS.AI.Components;
using Landfall.TABS.AI.Components.Modifiers;
using Landfall.TABS.AI.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Landfall.TABS.AI.Systems.Modifiers
{
	// Token: 0x0200000A RID: 10
	[UpdateAfter(typeof(UpdateBarrier))]
	[UpdateBefore(typeof(PreLateUpdateBarrier))]
	public class CircleTargetSystem : JobComponentSystem
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00004FE8 File Offset: 0x000031E8
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			return new CircleTargetSystem.Job
			{
				Entities = this.m_filter.Entities,
				Directions = this.m_filter.Directions,
				CircleTargets = this.m_filter.CircleTargets,
				TargetDatas = this.m_filter.TargetDatas,
				HasTargetTags = this.m_filter.HasTargetTags,
				CommandBuffer = this.m_barrier.CreateCommandBuffer().ToConcurrent()
			}.Schedule(this.m_filter.Length, 12, inputDeps);
		}

		// Token: 0x0400007B RID: 123
		[Inject]
		private CircleTargetSystem.Filter m_filter;

		// Token: 0x0400007C RID: 124
		[Inject]
		private PreLateUpdateBarrier m_barrier;

		// Token: 0x02000033 RID: 51
		private struct Filter
		{
			// Token: 0x040001B9 RID: 441
			public EntityArray Entities;

			// Token: 0x040001BA RID: 442
			public ComponentDataArray<Direction> Directions;

			// Token: 0x040001BB RID: 443
			[ReadOnly]
			public ComponentDataArray<CircleTarget> CircleTargets;

			// Token: 0x040001BC RID: 444
			[ReadOnly]
			public ComponentDataArray<HasTargetTag> HasTargetTags;

			// Token: 0x040001BD RID: 445
			[ReadOnly]
			public ComponentDataArray<TargetData> TargetDatas;

			// Token: 0x040001BE RID: 446
			public readonly int Length;
		}

		// Token: 0x02000034 RID: 52
		private struct Job : IJobParallelFor
		{
			// Token: 0x06000124 RID: 292 RVA: 0x0000D2F0 File Offset: 0x0000B4F0
			public void Execute(int index)
			{
				bool flag = this.HasTargetTags[index].Target == Entity.Null;
				if (!flag)
				{
					Entity entity = this.Entities[index];
					Direction direction = this.Directions[index];
					CircleTarget circleTarget = this.CircleTargets[index];
					bool flag2 = this.TargetDatas[index].DistanceToTarget <= circleTarget.CircleDistance;
					if (flag2)
					{
						float3 value = direction.Value;
						float3 @float;
						@float..ctor(value.x, 0f, value.z);
						float3 float2 = math.cross(math.normalize(@float), new float3(0f, 1f, 0f));
						float3 float3 = math.length(@float) * float2;
						direction.Value = new float3(float3.x, value.y, float3.z);
					}
					this.CommandBuffer.SetComponent<Direction>(index, entity, direction);
				}
			}

			// Token: 0x040001BF RID: 447
			public EntityArray Entities;

			// Token: 0x040001C0 RID: 448
			public ComponentDataArray<Direction> Directions;

			// Token: 0x040001C1 RID: 449
			[ReadOnly]
			public ComponentDataArray<CircleTarget> CircleTargets;

			// Token: 0x040001C2 RID: 450
			[ReadOnly]
			public ComponentDataArray<HasTargetTag> HasTargetTags;

			// Token: 0x040001C3 RID: 451
			[ReadOnly]
			public ComponentDataArray<TargetData> TargetDatas;

			// Token: 0x040001C4 RID: 452
			public EntityCommandBuffer.Concurrent CommandBuffer;
		}
	}
}

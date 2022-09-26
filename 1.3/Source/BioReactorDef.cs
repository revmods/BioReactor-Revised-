using RimWorld;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;
using Verse;

namespace BioReactor {
	public class BioReactorDef : ThingDef {
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req) {
			base.SpecialDisplayStats(req);

			yield return new StatDrawEntry(StatCategoryDefOf.Building, BioReactor_StatDefOf.MaxBodySize, bodySizeMax, StatRequest.ForEmpty());
		}

		/// <summary>
		/// 캐릭터 드로우 좌표. 리액터의 실좌표 중심을 기준으로 드로우.
		/// </summary>
		public Vector3 innerDrawOffset;
		/// <summary>
		/// 리액터 용액 드로우 중심 좌표. 리액터 실 좌표 중심을 기준으로 드로우
		/// </summary>
		public Vector3 liquidDrawCenter;
		/// <summary>
		/// 리액터 용액 그래픽 넓이
		/// </summary>
		public Vector2 liquidDrawSize;
		/// <summary>
		/// 수용 생명체 크기 최소 한도
		/// </summary>
		public float bodySizeMin;
		/// <summary>
		/// 수용 생명체 크기 최대 한도
		/// </summary>
		public float bodySizeMax;

		// was the default color, now defined as Common.liquidColorPreset[0]
		//public Color liquidColor = new Color(0.48f, 1, 0.91f, 0.29f);

		// was originally going to use this as a reference point for glower animations but i ditched that idea
		// now it is just used for "turning on" the glower when the bioreactor is occupied
		// it could be a member of Building_BioReactor, but its exposure is still useful
		public float maxGlowRadius;
	}
}

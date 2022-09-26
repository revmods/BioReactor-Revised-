using RimWorld;
using Verse;

namespace BioReactor {
	[DefOf]
	public static class Bio_JobDefOf {
		public static JobDef CarryToBioReactor;
		public static JobDef EnterBioReactor;
	}

	[DefOf]
	public static class BioReactorSoundDef {
		public static SoundDef Drowning;
	}

	[DefOf]
	public static class BioReactorThoughtDef {
		public static ThoughtDef KnowHistolysisHumanlike;
		public static ThoughtDef KnowHistolysisHumanlikeCannibal;
		public static ThoughtDef KnowHistolysisHumanlikePsychopath;

		public static ThoughtDef LivingBattery;
	}

	[DefOf]
	public static class CustomDefOf {
		public static WorkGiverDef CustomWorkRefuel;
	}

	[DefOf]
	public static class BioReactor_StatDefOf {
		public static StatDef MaxBodySize;

		static BioReactor_StatDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(BioReactor_StatDefOf));
		}
	}
}
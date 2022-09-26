using RimWorld;
using Verse;

namespace BioReactor {
	public class CompBioRefuelable : CompRefuelable, IStoreSettingsParent {
		private float consumptionRateReciprocal = 1f / 60000f;

		public StorageSettings inputSettings;
		CompFlickable flickComp;
		Building_BioReactor bioReactor;
		CompBioPowerPlant bioPowerPlantComp;

		public override void PostSpawnSetup(bool respawningAfterLoad) {
			base.PostSpawnSetup(respawningAfterLoad);
			flickComp = parent.GetComp<CompFlickable>();
			if (inputSettings == null) {
				inputSettings = new StorageSettings(this);
				if (parent.def.building.defaultStorageSettings != null) {
					inputSettings.CopyFrom(parent.def.building.defaultStorageSettings);
				}
			}
			bioReactor = (Building_BioReactor)parent;

			bioPowerPlantComp = parent.GetComp<CompBioPowerPlant>();

			CompMapRefuelable component = parent.Map.GetComponent<CompMapRefuelable>();
			if (component == null) {
				return;
			}

			component.comps.Add(this);
		}
		public override void PostDeSpawn(Map map) {
			base.PostDeSpawn(map);
			CompMapRefuelable component = map.GetComponent<CompMapRefuelable>();
			if (component == null) {
				return;
			}
			component.comps.Remove(this);
		}
		public override void PostExposeData() {
			base.PostExposeData();
			Scribe_Deep.Look(ref inputSettings, "inputSettings");

		}
		public override void CompTick() {
			if (
				!Props.consumeFuelOnlyWhenUsed && 
				// if you want a cryptosleep casket, build a cryptosleep casket
				//(this.flickComp == null || this.flickComp.SwitchIsOn) && 
				(bioReactor != null && bioReactor.InnerPawn != null) &&
				(bioPowerPlantComp != null && !bioPowerPlantComp.IsBrokenDown)
			) {
				ConsumeFuel(ConsumptionRatePerTick);
			}
		}
		private float ConsumptionRatePerTick {
			get {
				return Props.fuelConsumptionRate * consumptionRateReciprocal;
			}
		}
		public override void PostDestroy(DestroyMode mode, Map previousMap) {
		}

		public StorageSettings GetStoreSettings() {
			return inputSettings;
		}
		public StorageSettings GetParentStoreSettings() {
			return parent.def.building.fixedStorageSettings;
		}
		public bool StorageTabVisible {
			get {
				return true;
			}
		}
		public ThingFilter FuelFilter {
			get {
				return inputSettings.filter;
			}
		}

	}
}

using RimWorld;
using Verse;

namespace BioReactor {
	public class CompBioPowerPlant : CompPowerPlant {
		public Building_BioReactor building_BioReactor;
		public CompRefuelable compRefuelable;

		protected override float DesiredPowerOutput {
			get {
				return -Props.basePowerConsumption * ((Common.modSettings != null) ? (Common.modSettings.totalPowerOutputScale * Common.percentageReciprocal) : 1f);
			}
		}

		public bool IsBrokenDown {
			get {
				return breakdownableComp != null && breakdownableComp.BrokenDown;
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad) {
			base.PostSpawnSetup(respawningAfterLoad);
			building_BioReactor = (Building_BioReactor)parent;
			compRefuelable = parent.GetComp<CompRefuelable>();
		}

		public override void CompTick() {
			base.CompTick();
			this.UpdateDesiredPowerOutput();
		}

		public new void UpdateDesiredPowerOutput() {
			if (
				(building_BioReactor != null && !(building_BioReactor.state == Building_BioReactor.ReactorState.Full)) ||
				IsBrokenDown ||
				(refuelableComp != null && !refuelableComp.HasFuel) ||
				(this.flickableComp != null && !this.flickableComp.SwitchIsOn) ||
				!base.PowerOn
			) {
				PowerOutput = 0f;
			}
			else {
				Pawn pawn = building_BioReactor.ContainedThing as Pawn;
				if (pawn != null) {
					if (pawn.Dead) {
						PowerOutput = 0;
						return;
					}

					if (
						(pawn.RaceProps.Humanlike && Common.modSettings.disableNonHumanLike) ||
						(pawn.RaceProps.IsMechanoid && Common.modSettings.disableMechnoids) ||
						(pawn.RaceProps.Insect && Common.modSettings.disableInsectoids) ||
						((!pawn.RaceProps.Humanlike && !pawn.RaceProps.IsMechanoid && !pawn.RaceProps.Insect) && Common.modSettings.disableNonHumanLike)
					) {
						PowerOutput = 0;
						return;
					}

					if (pawn.RaceProps.IsMechanoid) {
						PowerOutput = DesiredPowerOutput * (Common.modSettings.mechanoidPowerOutputScale * Common.percentageReciprocal);
					}
					else if (pawn.RaceProps.Humanlike) {
						PowerOutput = DesiredPowerOutput * (Common.modSettings.humanLikePowerOutputScale * Common.percentageReciprocal);
					}
					else if (pawn.RaceProps.Insect) {
						PowerOutput = DesiredPowerOutput * (Common.modSettings.insectoidPowerOutputScale * Common.percentageReciprocal);
					}
					else {
						PowerOutput = DesiredPowerOutput * (Common.modSettings.nonHumanLikePowerOutputScale * Common.percentageReciprocal);
					}

					PowerOutput *= pawn.BodySize;
				}
			}
		}
	}
}

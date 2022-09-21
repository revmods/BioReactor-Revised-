using RimWorld;
using UnityEngine;
using Verse;


namespace BioReactor
{
	public class CompBioPowerPlant : CompPowerPlant
	{
		public Building_BioReactor building_BioReactor;
		public CompRefuelable compRefuelable;

		protected override float DesiredPowerOutput
		{
			get
			{
				return -Props.basePowerConsumption * ((Common.modSettings != null) ? ((float)Common.modSettings.powerOutputScale/100f) : 1f);
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			building_BioReactor = (Building_BioReactor)parent;
			compRefuelable = parent.GetComp<CompRefuelable>();
		}

		public override void CompTick()
		{
			base.CompTick();
			this.UpdateDesiredPowerOutput();
		}

		public new void UpdateDesiredPowerOutput()
		{
			if ((building_BioReactor != null && !(building_BioReactor.state == Building_BioReactor.ReactorState.Full)) || (breakdownableComp != null && breakdownableComp.BrokenDown) || (refuelableComp != null && !refuelableComp.HasFuel) || (this.flickableComp != null && !this.flickableComp.SwitchIsOn) || !base.PowerOn)
			{
				PowerOutput = 0f;
			}
			else
			{
				Pawn pawn = building_BioReactor.ContainedThing as Pawn;
				if (pawn != null)
				{
					if (pawn.Dead)
					{
						PowerOutput = 0;
						return;
					}
					// instead of mechanoids generating no power, make them generate 125% the normal output
					if (pawn.RaceProps.FleshType == FleshTypeDefOf.Mechanoid)
					{
						PowerOutput = DesiredPowerOutput * 1.25f;
					}
					else if ((pawn.RaceProps.Humanlike))
					{
						PowerOutput = DesiredPowerOutput;
					}
					// insects only generate 25% the normal output
					else if (pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid)
					{
						PowerOutput = DesiredPowerOutput * 0.25f;
					}
					else
					{
						PowerOutput = DesiredPowerOutput * 0.50f;
					}

					/*
                    if (pawn.Dead || (pawn.RaceProps.FleshType == FleshTypeDefOf.Mechanoid))
                    {
                        PowerOutput = 0;
                        return;
                    }
                    if ((pawn.RaceProps.Humanlike))
                    {
                        PowerOutput = DesiredPowerOutput;
                    }
                    else
                    {
                        PowerOutput = DesiredPowerOutput * 0.50f;
                    }
                    */

					PowerOutput *= pawn.BodySize;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using UnityEngine;
using HarmonyLib;
using RimWorld.Planet;

namespace BioReactor {
	public class Building_BioReactor : Building_Casket, ISuspendableThingHolder {
		/// <summary>
		/// 내부 캐릭터 드로우 좌표. 리액터 실좌표 중심으로 드로우.
		/// </summary>
		public Vector3 innerDrawOffset;
		public Vector3 liquidDrawCenter;
		public Vector2 liquidDrawSize;
		public float maxGlowRadius;

		public Color liquidColor;
		public int liquidColorPresetIndex = 0;

		public CompBioRefuelable compRefuelable;
		public CompForbiddable forbiddable;
		public CompGlower glower;

		//static Vector3 waterDrawY = new Vector3(0, 0.3f, 0);
		public enum ReactorState {
			Empty,//none
			StartFilling,//animating Filling
			Full,//Just Drawing
			HistolysisStarting,//Start Animating and Changing Color
			HistolysisEnding,
			HistolysisDone//Just Drawing
		}
		public ReactorState state = ReactorState.Empty;
		public float fillpct = 0;
		public float histolysisPct = 0;

		// store color values from Tick to be used in Draw during histolysis animation
		private Color currentHistolysisEndingColor;
		private Color currentHistolysisColor;

		private readonly PawnRenderFlags occupantRenderFlags = PawnRenderFlags.Cache | PawnRenderFlags.Clothes | PawnRenderFlags.Headgear;

		public bool IsContainingThingPawn {
			get {
				if (!HasAnyContents) {
					return false;
				}
				Pawn pawn = ContainedThing as Pawn;
				if (pawn != null) {
					return true;
				}
				return false;
			}
		}
		public Pawn InnerPawn {
			get {
				if (!HasAnyContents) {
					return null;
				}
				Pawn pawn = ContainedThing as Pawn;
				if (pawn != null) {
					return pawn;
				}
				return null;
			}
		}

		public void Initialize(BioReactorDef reactorDef) {
			// read values that are passed in via XML
			if (reactorDef != null) {
				innerDrawOffset = reactorDef.innerDrawOffset;
				liquidDrawCenter = reactorDef.liquidDrawCenter;
				liquidDrawSize = reactorDef.liquidDrawSize;
				maxGlowRadius = reactorDef.maxGlowRadius;
			}

			/*
			ColorForStuff c = new ColorForStuff() {
				null,
				new Color(0, 0, 1f, 1f)
			};

			reactorDef.colorPerStuff.Add();
			*/
		}

		public override void ExposeData() {
			base.ExposeData();

			int lcolor = Common.ColorToInt(liquidColor);

			Scribe_Values.Look(ref state, "state");
			Scribe_Values.Look(ref fillpct, "fillpct");
			Scribe_Values.Look(ref histolysisPct, "histolysisPct");
			Scribe_Values.Look(ref lcolor, "liquidColor", 0);

			if (lcolor != 0) {
				liquidColor = Common.IntToColor(lcolor);
			}
			else {
				liquidColorPresetIndex = 0;
				liquidColor = Common.liquidColorPreset[liquidColorPresetIndex];
			}

			// i feel as if ExposeData is called before components are initialized
			// similarly to how a GameObject constructor in unity is practically useless
			// a GameObject cant do much until OnAlive, SpawnSetup seems to be analogous to OnAlive
			// im basing all this on the fact that GetComp<CompGlower>() returns null here
			// and ExposeData is called before SpawnSetup from my testing

			//compRefuelable = GetComp<CompBioRefuelable>();
			//forbiddable = GetComp<CompForbiddable>();
			//glower = GetComp<CompGlower>();

			// even if components are not ready, you can still read from the Def
			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				Initialize(def as BioReactorDef);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad) {
			base.SpawnSetup(map, respawningAfterLoad);

			fillpct = 0;
			histolysisPct = 0;

			compRefuelable = GetComp<CompBioRefuelable>();
			forbiddable = GetComp<CompForbiddable>();
			glower = GetComp<CompGlower>();

			if (liquidColor.r == 0f && liquidColor.g == 0f && liquidColor.b == 0f && liquidColor.a == 0f) {
				liquidColorPresetIndex = 0;
				liquidColor = Common.liquidColorPreset[liquidColorPresetIndex];
			}

			CompProperties_Glower glowerProps = glower.props as CompProperties_Glower;
			glowerProps.glowColor = Common.ColorToColorInt(liquidColor);
			glowerProps.glowRadius = maxGlowRadius;
			glower.Initialize(glowerProps);
			glower.PostSpawnSetup(respawningAfterLoad);

			Initialize(def as BioReactorDef);
		}

		public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true) {
			if (base.TryAcceptThing(thing, allowSpecialEffects)) {
				if (allowSpecialEffects) {
					SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(new TargetInfo(base.Position, Find.CurrentMap, false));
				}
				state = ReactorState.StartFilling;
				Pawn pawn = thing as Pawn;
				if (pawn != null && pawn.RaceProps.Humanlike) {
					pawn.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef.LivingBattery, null);
				}
				return true;
			}
			return false;
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn) {
			foreach (FloatMenuOption o in base.GetFloatMenuOptions(myPawn)) {
				yield return o;
			}
			if (innerContainer.Count == 0) {
				if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, false, TraverseMode.ByPawn)) {
					FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
					yield return failer;
				}
				else {
					JobDef jobDef = Bio_JobDefOf.EnterBioReactor;
					string jobStr = "EnterBioReactor".Translate();
					Action jobAction = delegate () {
						Job job = new Job(jobDef, this);
						myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
					};
					yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, jobAction, MenuOptionPriority.Default, null, null, 0f, null, null), myPawn, this, "ReservedBy");
				}
			}
			yield break;
		}

		public void OnCycleColor() {
			++liquidColorPresetIndex;
			if (liquidColorPresetIndex >= Common.liquidColorPreset.Length) {
				liquidColorPresetIndex = 0;
			}

			liquidColor = Common.liquidColorPreset[liquidColorPresetIndex];

			SetGlowerColor(Common.ColorToColorInt(liquidColor), maxGlowRadius);
		}

		public void OnHistolysis() {
			BioReactorSoundDef.Drowning.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			state = ReactorState.HistolysisStarting;
		}

		public void OnHistolysisCancel() {
			state = ReactorState.Full;
			histolysisPct = 0f;
			fillpct = 1f;
		}

		public override IEnumerable<Gizmo> GetGizmos() {
			foreach (Gizmo c in base.GetGizmos()) {
				yield return c;
			}

			if (HasAnyContents) {
				Pawn pawn = ContainedThing as Pawn;
				if (pawn != null) {
					if (pawn.RaceProps.FleshType == FleshTypeDefOf.Normal || pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid) {
						if (state == ReactorState.Full) {
							yield return new Command_Action {
								defaultLabel = "Histolysis".Translate(),
								defaultDesc = "HistolysisDesc".Translate(),
								icon = ContentFinder<Texture2D>.Get("UI/Commands/Histolysis", true),
								action = OnHistolysis
							};
						}
						else if (state == ReactorState.HistolysisStarting) {
							yield return new Command_Action {
								defaultLabel = "Histolysis".Translate(),
								defaultDesc = "HistolysisDesc".Translate(),
								icon = ContentFinder<Texture2D>.Get("UI/Commands/CancelHistolysis", true),
								action = OnHistolysisCancel
							};
						}
					}

				}
			}

			yield return new Command_Action {
				defaultLabel = "CycleColor".Translate(),
				defaultDesc = "CycleColorDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get("UI/Commands/Colorwheel", true),
				action = OnCycleColor
			};

			// Assets/Resources/textures/things/building/furniture/LampStanding

			foreach (Gizmo gizmo2 in CopyPasteGizmosFor(compRefuelable.inputSettings)) {
				yield return gizmo2;
			}
			yield break;
		}

		public override void EjectContents() {
			ThingDef filth_Slime = ThingDefOf.Filth_Slime;
			foreach (Thing thing in ((IEnumerable<Thing>)this.innerContainer)) {
				Pawn pawn = thing as Pawn;
				if (pawn != null) {
					PawnComponentsUtility.AddComponentsForSpawn(pawn);
					pawn.filth.GainFilth(filth_Slime);
					if (pawn.RaceProps.IsFlesh) {
						pawn.health.AddHediff(HediffDefOf.CryptosleepSickness, null, null, null);

						if (!Common.modSettings.disableHypothermia) {
							// messing around with other health conditions that arise after being placed in the bioreactor
							HediffSet hediffs = pawn.health.hediffSet;

							// hypothermia seems to make since assuming the bioreactor uses the body heat of the occupant
							Hediff hypothermia = null;

							// adding hypothermia when it already exist doesnt seem to return the existing hediff 
							if (!hediffs.HasHediff(HediffDefOf.Hypothermia)) {
								hypothermia = pawn.health.AddHediff(HediffDefOf.Hypothermia, null, null, null);
							}
							else {
								// find the existing hypothermia hediff when it already exists
								hypothermia = hediffs.GetFirstHediffOfDef(HediffDefOf.Hypothermia);
							}

							// the bioreactor saps all the occupants body heat leaving them at the brink of death
							hypothermia.Severity = 0.99f;
						}
					}
				}
			}
			if (!base.Destroyed) {
				SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
			}
			state = ReactorState.Empty;
			fillpct = 0;
			base.EjectContents();
		}
		public override void PostMake() {
			base.PostMake();
		}
		public virtual void Histolysis() {
			if (HasAnyContents) {
				Pawn pawn = ContainedThing as Pawn;
				if (pawn != null) {
					pawn.Rotation = Rot4.South;
					compRefuelable.Refuel(35);
					DamageInfo d = new DamageInfo();
					d.Def = DamageDefOf.Burn;
					d.SetAmount(1000);
					pawn.Kill(d);

					try {
						CompRottable compRottable = ContainedThing.TryGetComp<CompRottable>();
						if (compRottable != null) {
							compRottable.RotProgress += 600000f;
						}

						if (Common.modSettings.makeFuelOnHistolysis) {
							MakeFuel();
						}
					}
					catch (Exception ee) {
						Log.Message("Building_BioReactor.Histolysis - Rot Error: " + ee);
					}

					if (pawn.RaceProps.Humanlike) {
						foreach (Pawn p in base.Map.mapPawns.SpawnedPawnsInFaction(Faction)) {
							if (p.needs != null && p.needs.mood != null && p.needs.mood.thoughts != null) {
								p.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef.KnowHistolysisHumanlike, null);
								p.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef.KnowHistolysisHumanlikeCannibal, null);
								p.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef.KnowHistolysisHumanlikePsychopath, null);
							}
						}
					}
				}
			}
		}
		public void MakeFuel() {
			ThingDef stuff = GenStuff.RandomStuffFor(ThingDefOf.Chemfuel);
			Thing thing = ThingMaker.MakeThing(ThingDefOf.Chemfuel, stuff);
			thing.stackCount = 35;
			GenPlace.TryPlaceThing(thing, Position, base.Map, ThingPlaceMode.Near, null, null);
		}

		public static Building_BioReactor FindBioReactorFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false) {
            if (p.RaceProps.IsMechanoid) {
                return null;
            }

			IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
											   where typeof(Building_BioReactor).IsAssignableFrom(def.thingClass)
											   select def;

			foreach (ThingDef singleDef in enumerable) {
				Building_BioReactor building_BioReactor = (Building_BioReactor)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell, TraverseParms.For(traveler, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, delegate (Thing x) {
					bool result;
					if (!((Building_BioReactor)x).HasAnyContents) {
						Pawn traveler2 = traveler;
						LocalTargetInfo target = x;
						bool ignoreOtherReservations2 = ignoreOtherReservations;
						result = traveler2.CanReserve(target, 1, -1, null, ignoreOtherReservations2);
					}
					else {
						result = false;
					}
					return result;
				}, null, 0, -1, false, RegionType.Set_Passable, false);
				if (building_BioReactor != null && !building_BioReactor.forbiddable.Forbidden) {
					if ((p.BodySize <= ((BioReactorDef)(building_BioReactor.def)).bodySizeMax) && (p.BodySize >= ((BioReactorDef)(building_BioReactor.def)).bodySizeMin)) {
						return building_BioReactor;
					}
				}
			}
			return null;
		}

		public void SetGlowerColor(ColorInt color, float radius) {
			CompProperties_Glower glowerProps = glower.props as CompProperties_Glower;

			ColorInt targetColor = new ColorInt(color.r, color.g, color.b, 255);
			if (glowerProps.glowColor.Equals(targetColor) && glowerProps.glowRadius == radius) {
				return;
			}

			glowerProps.glowColor = targetColor;
			glowerProps.glowRadius = radius;

			glower.UpdateLit(Map);
			Map.glowGrid.MarkGlowGridDirty(glower.parent.Position);
		}

		public override void Tick() {
			currentHistolysisEndingColor = new Color(
				0.7f,
				0.2f,
				0.2f,
				0.4f + (0.45f * histolysisPct)
			);

			currentHistolysisColor = Common.MixColor(liquidColor, currentHistolysisEndingColor, histolysisPct);

			base.Tick();

			switch (state) {
				case ReactorState.Empty:
					fillpct = 0;
					SetGlowerColor(Common.ColorToColorInt(liquidColor), 0f);
					break;
				case ReactorState.StartFilling:
					fillpct += 0.01f;
					if (fillpct >= 1) {
						state = ReactorState.Full;
						BioReactorSoundDef.Drowning.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
					}
					SetGlowerColor(Common.ColorToColorInt(liquidColor), maxGlowRadius);
					break;
				case ReactorState.Full:
					SetGlowerColor(Common.ColorToColorInt(liquidColor), maxGlowRadius);
					break;
				case ReactorState.HistolysisStarting:
					histolysisPct += 0.005f;
					if (histolysisPct >= 1) {
						state = ReactorState.HistolysisEnding;
						Histolysis();
					}
					SetGlowerColor(Common.ColorToColorInt(currentHistolysisColor), maxGlowRadius);
					break;
				case ReactorState.HistolysisEnding:
					histolysisPct -= 0.01f;
					if (histolysisPct <= 0) {
						histolysisPct = 0;
						state = ReactorState.HistolysisDone;
					}
					SetGlowerColor(Common.ColorToColorInt(currentHistolysisEndingColor), maxGlowRadius);
					break;
				case ReactorState.HistolysisDone:
					SetGlowerColor(Common.ColorToColorInt(currentHistolysisEndingColor), maxGlowRadius);
					break;
			}
		}

		public override void Draw() {
			switch (state) {
				case ReactorState.Empty:
					break;

				case ReactorState.StartFilling:
					foreach (Thing t in innerContainer) {
						if (t is Pawn pawn) {
							DrawInnerThing(pawn, DrawPos + innerDrawOffset, occupantRenderFlags);
							LiquidDraw(liquidColor, fillpct);
						}
					}
					break;

				case ReactorState.Full:
					foreach (Thing t in innerContainer) {
						if (t is Pawn pawn) {
							DrawInnerThing(pawn, DrawPos + innerDrawOffset, occupantRenderFlags);
							LiquidDraw(liquidColor, 1);
						}
					}
					break;

				case ReactorState.HistolysisStarting:
					foreach (Thing t in innerContainer) {
						if (t is Pawn pawn) {
							DrawInnerThing(pawn, DrawPos + innerDrawOffset, occupantRenderFlags);
							LiquidDraw(currentHistolysisColor, 1);
						}
					}
					break;

				case ReactorState.HistolysisEnding:
					foreach (Thing t in innerContainer) {
						t.DrawAt(DrawPos + innerDrawOffset);
						LiquidDraw(currentHistolysisEndingColor, 1);
					}
					break;

				case ReactorState.HistolysisDone:
					foreach (Thing t in innerContainer) {
						t.DrawAt(DrawPos + innerDrawOffset);
						LiquidDraw(currentHistolysisEndingColor, 1);
					}
					break;
			}

			//Graphic.Draw(GenThing.TrueCenter(Position, Rot4.South, def.size, 11.7f), Rot4.South, this, 0f);
			Comps_PostDraw();
		}

		public override void Print(SectionLayer layer) {
			Printer_Plane.PrintPlane(
				layer, 
				GenThing.TrueCenter(
					Position, 
					this.Rotation,
					this.RotatedSize,
					11.7f
				),
				Graphic.drawSize,
				Graphic.MatSingle, 
				0, 
				false, 
				null, 
				null, 
				0f, //0.01f, 
				0f
			);
		}

		public virtual void LiquidDraw(Color color, float fillPct) {
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			r.center = DrawPos + liquidDrawCenter;
			r.size = liquidDrawSize;

			r.fillPercent = fillPct;
			r.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(color, false);
			r.unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0, 0, 0, 0), false);
			r.margin = 0f;

			Rot4 rotation = Rotation;
			//rotation.Rotate(RotationDirection.Clockwise);
			if (rotation == Rot4.North || rotation == Rot4.South) {
				rotation.Rotate(RotationDirection.Clockwise);
			}
			r.rotation = rotation;

			GenDraw.DrawFillableBar(r);
		}
		public static MethodInfo pawnrender = AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new Type[]
			{
				typeof(Vector3),
				typeof(float),
				typeof(bool),
				typeof(Rot4),
				typeof(RotDrawMode),
				typeof(PawnRenderFlags)
			});
		public virtual void DrawInnerThing(Pawn pawn, Vector3 rootLoc, PawnRenderFlags renderFlags) {
			pawnrender.Invoke(pawn.Drawer.renderer, new object[]
					{
						rootLoc,
						0,
						true,
						Rotation, //Rot4.South,
                        RotDrawMode.Fresh,
						renderFlags
					});
		}

		public static IEnumerable<Gizmo> CopyPasteGizmosFor(StorageSettings s) {
			yield return new Command_Action {
				icon = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings", true),
				defaultLabel = "CommandCopyBioReactorSettingsLabel".Translate(),
				defaultDesc = "CommandCopyBioReactorSettingsDesc".Translate(),
				action = delegate () {
					SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
					Copy(s);
				},
				hotKey = KeyBindingDefOf.Misc4
			};
			Command_Action command_Action = new Command_Action();
			command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings", true);
			command_Action.defaultLabel = "CommandPasteBioReactorSettingsLabel".Translate();
			command_Action.defaultDesc = "CommandPasteBioReactorSettingsDesc".Translate();
			command_Action.action = delegate () {
				SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
				PasteInto(s);
			};
			command_Action.hotKey = KeyBindingDefOf.Misc5;
			if (!HasCopiedSettings) {
				command_Action.Disable(null);
			}
			yield return command_Action;
			yield break;
		}

		private static StorageSettings clipboard = new StorageSettings();

		private static bool copied = false;

		public static bool HasCopiedSettings {
			get {
				return copied;
			}
		}

		bool ISuspendableThingHolder.IsContentsSuspended {
			get {
				return true;
			}
		}

		public static void Copy(StorageSettings s) {
			clipboard.CopyFrom(s);
			copied = true;
		}

		public static void PasteInto(StorageSettings s) {
			s.CopyFrom(clipboard);
		}
	}
}
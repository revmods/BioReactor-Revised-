using RimWorld;
using UnityEngine;
using Verse;

namespace BioReactor {
	public class ITab_OccupantNeeds : ITab {
		private Vector2 thoughtsScrollPosition = Vector2.zero;

		private Pawn Occupant {
			get {
				Building_BioReactor bioreactor = Find.Selector.SingleSelectedThing as Building_BioReactor;
				if (bioreactor == null) {
					return null;
				}

				return bioreactor.ContainedThing as Pawn;
			}
		}

		public override void OnOpen() {
			base.OnOpen();
		}

		public override bool IsVisible {
			get {
				Pawn pawn = Occupant;
				return pawn != null && pawn.needs.AllNeeds.Count != 0;
			}
		}

		public ITab_OccupantNeeds() {
			size = Vector2.zero;
			labelKey = Translator.TranslateWithBackup("Needs", "Needs");
		}

		protected override void FillTab() {
			size = NeedsCardUtility.FullSize;
			Rect rect = GenUI.ContractedBy(new Rect(0f, 10f, size.x, size.y), 10f);
			NeedsCardUtility.DoNeedsMoodAndThoughts(rect, Occupant, ref thoughtsScrollPosition);

			//Rect rect = GenUI.ContractedBy(new Rect(0f, 10f, size.x, size.y), 10f);
			//HealthCardUtility.DrawPawnHealthCard(rect, Occupant, false, false, Occupant);
		}
		public override void Notify_ClickOutsideWindow() {
			base.Notify_ClickOutsideWindow();
		}
	}
}

using RimWorld;
using UnityEngine;
using Verse;

namespace BioReactor {
	public class ITab_OccupantHeatlth : ITab {
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

		public override bool IsVisible => Occupant is Pawn;

		public ITab_OccupantHeatlth() {
			size = new Vector2(630f, 430f);
			labelKey = Translator.TranslateWithBackup("Health", "Health");
		}

		protected override void FillTab() {
			Rect rect = GenUI.ContractedBy(new Rect(0f, 10f, size.x, size.y), 10f);
			HealthCardUtility.DrawPawnHealthCard(rect, Occupant, false, false, Occupant);
		}
		public override void Notify_ClickOutsideWindow() {
			base.Notify_ClickOutsideWindow();
		}
	}
}

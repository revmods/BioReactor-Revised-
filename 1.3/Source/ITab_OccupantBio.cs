using RimWorld;
using UnityEngine;
using Verse;

namespace BioReactor {
	public class ITab_OccupantBio : ITab {
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
				return pawn is Pawn && (pawn.IsWildMan() || !pawn.NonHumanlikeOrWildMan());
			}
		}

		public ITab_OccupantBio() {
			size = Vector2.zero;
			labelKey = Translator.TranslateWithBackup("Bio", "Bio");
		}

		protected override void FillTab() {
			Building_BioReactor bioreactor = Find.Selector.SingleSelectedThing as Building_BioReactor;
			if (bioreactor.ContainedThing is Pawn occupant) {
				Vector2 cardSize = CharacterCardUtility.PawnCardSize(Occupant);
				size = cardSize;
				Rect rect = GenUI.ContractedBy(new Rect(0f, 10f, cardSize.x, cardSize.y), 10f);
				CharacterCardUtility.DrawCharacterCard(rect, occupant);
			}
		}
		public override void Notify_ClickOutsideWindow() {
			base.Notify_ClickOutsideWindow();
		}
	}
}

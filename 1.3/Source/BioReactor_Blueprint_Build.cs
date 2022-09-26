using RimWorld;
using Verse;

namespace BioReactor {
	public class BioReactor_Blueprint_Build : Blueprint_Build {
		public override void Print(SectionLayer layer) {
			Printer_Plane.PrintPlane(
				layer,
				GenThing.TrueCenter(
					Position,
					Rotation,
					RotatedSize,
					0f
				),
				Graphic.drawSize,
				Graphic.MatSingle
			);
		}
	}
}

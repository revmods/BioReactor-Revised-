using System.Collections.Generic;
using Verse;

namespace BioReactor {
	public class CompMapRefuelable : MapComponent {
		public readonly List<CompBioRefuelable> comps = new List<CompBioRefuelable>();
		public CompMapRefuelable(Map map) : base(map) {
		}
	}
}

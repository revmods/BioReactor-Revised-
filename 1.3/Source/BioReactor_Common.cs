using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BioReactor {
	class Common {
		public static readonly float percentageReciprocal = 1f / 100f;

		public static BioReactor_Settings modSettings = LoadedModManager.GetMod<BioReactor_Mod>().GetSettings<BioReactor_Settings>();

		public static Color[] liquidColorPreset = new Color[] {
			new Color(0.48f,    1f,     0.91f,  0.29f),
			new Color(1f,       0.48f,  0.48f,  0.29f),
			new Color(0.48f,    1f,     0.48f,  0.29f),
			new Color(0.48f,    0.48f,  1f,     0.29f),
			new Color(0.91f,    1f,     0.4f,   0.29f),
			new Color(0.91f,    0.48f,  1f,     0.29f),
			new Color(1f,       1f,     1f,     0.29f),
		};

		public static int ColorToInt(Color color) {
			return
				((int)(color.r * 255f) & 0xFF) << 24 |
				((int)(color.g * 255f) & 0xFF) << 16 |
				((int)(color.b * 255f) & 0xFF) << 8 |
				((int)(color.a * 255f) & 0xFF)
			;
		}
		public static Color IntToColor(int color) {
			return new Color(
				(float)((color >> 24) & 0xFF) / 255f,
				(float)((color >> 16) & 0xFF) / 255f,
				(float)((color >> 8) & 0xFF) / 255f,
				(float)(color & 0xFF) / 255f
			);
		}

		public static ColorInt ColorToColorInt(Color color) {
			return new ColorInt(
				(int)(color.r * 255f),
				(int)(color.g * 255f),
				(int)(color.b * 255f),
				(int)(color.a * 255f)
			);
		}

		public static int ColorIntToInt(ColorInt color) {
			return color.r << 24 | color.g << 16 | color.b << 8 | color.a;
		}
		public static ColorInt IntToColorInt(int color) {
			return new ColorInt(
				(color >> 24) & 0xFF,
				(color >> 16) & 0xFF,
				(color >> 8) & 0xFF,
				color & 0xFF
			);
		}

		public static float MixFloat(float x, float y, float a) {
			return x * (1f - a) + y * a;
		}
		public static Color MixColor(Color color1, Color color2, float a) {
			return new Color(
				MixFloat(color1.r, color2.r, a),
				MixFloat(color1.g, color2.g, a),
				MixFloat(color1.b, color2.b, a),
				MixFloat(color1.a, color2.a, a)
			);
		}
	}
}

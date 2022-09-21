using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BioReactor {
	public class BioReactorDef : ThingDef {
		/// <summary>
		/// 캐릭터 드로우 좌표. 리액터의 실좌표 중심을 기준으로 드로우.
		/// </summary>
		public Vector3 innerDrawOffset;
		/// <summary>
		/// 리액터 용액 드로우 중심 좌표. 리액터 실 좌표 중심을 기준으로 드로우
		/// </summary>
		public Vector3 liquidDrawCenter;
		/// <summary>
		/// 리액터 용액 그래픽 넓이
		/// </summary>
		public Vector2 liquidDrawSize;
		/// <summary>
		/// 수용 생명체 크기 최소 한도
		/// </summary>
		public float bodySizeMin;
		/// <summary>
		/// 수용 생명체 크기 최대 한도
		/// </summary>
		public float bodySizeMax;

		//public Color liquidColor = new Color(0.48f, 1, 0.91f, 0.29f);

		public float maxGlowRadius;
	}

	public class BioReactor_Settings : ModSettings {
		public int powerOutputScale = 100;
		public bool makeFuelOnHistolysis = true;
		public bool disableHypothermia = false;

		public override void ExposeData() {
			Scribe_Values.Look(ref powerOutputScale, "powerOutputScale");
			Scribe_Values.Look(ref makeFuelOnHistolysis, "makeFuelOnHistolysis");
			Scribe_Values.Look(ref disableHypothermia, "disableHypothermia");
			base.ExposeData();
		}
	}

	public class BioReactor_Mod : Mod {
		BioReactor_Settings settings;

		string powerOutputScaleBuffer;

		public BioReactor_Mod(ModContentPack content) : base(content) {
			settings = GetSettings<BioReactor_Settings>();

			powerOutputScaleBuffer = settings.powerOutputScale.ToStringSafe();
		}

		public void drawPowerOutputScale(Listing_Standard listingStandard) {
			listingStandard.Label("Power Output Scale (%)");
			listingStandard.TextFieldNumeric<int>(ref settings.powerOutputScale, ref powerOutputScaleBuffer, 0, 1000);
			settings.powerOutputScale = (int)listingStandard.Slider(settings.powerOutputScale, 0, 1000);
			powerOutputScaleBuffer = settings.powerOutputScale.ToStringSafe();

			// just in case something weird happens
			settings.powerOutputScale = Mathf.Clamp(settings.powerOutputScale, 0, 1000);
		}

		public void drawMakeFuelOnHistolysis(Listing_Standard listingStandard) {
			listingStandard.CheckboxLabeled("Make Fuel On Histolysis", ref settings.makeFuelOnHistolysis);
		}

		public void drawDisableHypothermia(Listing_Standard listingStandard) {
			listingStandard.CheckboxLabeled("Disable Hypothermia On BioReactor Ejection", ref settings.disableHypothermia);
		}

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);

			drawPowerOutputScale(listingStandard);
			listingStandard.GapLine(4);
			drawMakeFuelOnHistolysis(listingStandard);
			listingStandard.GapLine(4);
			drawDisableHypothermia(listingStandard);

			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() {
			return "Bioreactor (Revised)";
		}
	}

	class Common {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BioReactor {
	public class BioReactor_Settings : ModSettings {
		public int totalPowerOutputScale = 100;

		public bool makeFuelOnHistolysis = true;

		public bool disableHypothermia = false;

		public bool disableHumanLike = false;
		public int humanLikePowerOutputScale = 100;

		public bool disableNonHumanLike = false;
		public int nonHumanLikePowerOutputScale = 50;

		public bool disableInsectoids = false;
		public int insectoidPowerOutputScale = 25;

		public bool disableMechnoids = true;
		public int mechanoidPowerOutputScale = 125;

		public bool disableMalnutrition = true;
		public int malnutritionSeverityScale = 50;

		public void ResetToDefault() {
			totalPowerOutputScale = 100;

			makeFuelOnHistolysis = true;

			disableHypothermia = false;

			disableHumanLike = false;
			humanLikePowerOutputScale = 100;

			disableNonHumanLike = false;
			nonHumanLikePowerOutputScale = 50;

			disableInsectoids = false;
			insectoidPowerOutputScale = 25;

			disableMechnoids = true;
			mechanoidPowerOutputScale = 125;

			disableMalnutrition = true;
			malnutritionSeverityScale = 50;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref totalPowerOutputScale, "powerOutputScale");

			Scribe_Values.Look(ref makeFuelOnHistolysis, "makeFuelOnHistolysis");

			Scribe_Values.Look(ref disableHypothermia, "disableHypothermia");

			Scribe_Values.Look(ref disableHumanLike, "disableHumanLike");
			Scribe_Values.Look(ref humanLikePowerOutputScale, "humanLikePowerOutputScale");

			Scribe_Values.Look(ref disableNonHumanLike, "disableNonHumanLike");
			Scribe_Values.Look(ref nonHumanLikePowerOutputScale, "nonHumanLikePowerOutputScale");

			Scribe_Values.Look(ref disableInsectoids, "disableInsectoids");
			Scribe_Values.Look(ref insectoidPowerOutputScale, "insectoidPowerOutputScale");

			Scribe_Values.Look(ref disableMechnoids, "disableMechnoids");
			Scribe_Values.Look(ref mechanoidPowerOutputScale, "mechanoidPowerOutputScale");

			Scribe_Values.Look(ref disableMalnutrition, "disableMalnutrition");
			Scribe_Values.Look(ref malnutritionSeverityScale, "malnutritionSeverityScale");

			base.ExposeData();
		}
	}

	public class BioReactor_Mod : Mod {
		private BioReactor_Settings settings;

		private const int minPowerScale = 0;
		private const int maxPowerScale = 1000;

		// string buffers that Slider widgets use
		private string totalPowerOutputScaleBuffer;
		private string humanLikePowerOutputScaleBuffer;
		private string nonHumanLikePowerOutputScaleBuffer;
		private string insectoidPowerOutputScaleBuffer;
		private string mechanoidPowerOutputScaleBuffer;
		private string malnutritionSeverityScaleBuffer;

		public static readonly float gapHeight = Text.LineHeight;

		public void ResetToDefault() {
			settings.ResetToDefault();

			totalPowerOutputScaleBuffer = settings.totalPowerOutputScale.ToStringSafe();
			humanLikePowerOutputScaleBuffer = settings.humanLikePowerOutputScale.ToStringSafe();
			nonHumanLikePowerOutputScaleBuffer = settings.nonHumanLikePowerOutputScale.ToStringSafe();
			insectoidPowerOutputScaleBuffer = settings.insectoidPowerOutputScale.ToStringSafe();
			mechanoidPowerOutputScaleBuffer = settings.mechanoidPowerOutputScale.ToStringSafe();
			malnutritionSeverityScaleBuffer = settings.malnutritionSeverityScale.ToStringSafe();
		}

		public BioReactor_Mod(ModContentPack content) : base(content) {
			settings = GetSettings<BioReactor_Settings>();
		}

		// all the stuff i was using before is here just for reference, will not be updated with new settings
		private static class UseListingStandard {
			public static void DrawLinkedIntFieldSlider(
				Listing_Standard listingStandard,
				string label,
				ref int settingsValue,
				ref string buffer,
				int min = minPowerScale,
				int max = maxPowerScale
			) {
				listingStandard.Label(label);
				listingStandard.TextFieldNumeric<int>(ref settingsValue, ref buffer, min, max);
				settingsValue = (int)listingStandard.Slider(settingsValue, min, max);

				// just in case something weird happens
				settingsValue = Mathf.Clamp(settingsValue, min, max);
				buffer = settingsValue.ToStringSafe();
			}

			public static void DrawEnabledLinkedIntFieldSlider(
				Listing_Standard listingStandard,
				string enableLabel,
				ref bool settingsEnabled,
				string sliderLabel,
				ref int settingsSliderValue,
				ref string sliderBuffer,
				int min = minPowerScale,
				int max = maxPowerScale
			) {
				listingStandard.CheckboxLabeled(enableLabel, ref settingsEnabled);

				if (!settingsEnabled) {
					DrawLinkedIntFieldSlider(listingStandard, sliderLabel, ref settingsSliderValue, ref sliderBuffer, min, max);
				}
			}

			public static void DrawPowerOutputScale(Listing_Standard listingStandard, ref int settingsValue, ref string buffer) {
				DrawLinkedIntFieldSlider(listingStandard,
					"Total Power Output Scale (%)", ref settingsValue, ref buffer
				);
			}

			public static void DrawMakeFuelOnHistolysis(Listing_Standard listingStandard, ref bool settingsValue) {
				listingStandard.CheckboxLabeled("Make Fuel On Histolysis", ref settingsValue);
			}

			public static void DrawDisableHypothermia(Listing_Standard listingStandard, ref bool settingsValue) {
				listingStandard.CheckboxLabeled("Disable Hypothermia", ref settingsValue);
			}

			public static void DrawHumanLikePowerOutput(Listing_Standard listingStandard, ref bool settingsEnabledValue, ref int settingsSliderValue, ref string sliderBuffer) {
				DrawEnabledLinkedIntFieldSlider(listingStandard,
					"Disable HumanLike", ref settingsEnabledValue,
					"HumanLike Power Output Scale (%)", ref settingsSliderValue, ref sliderBuffer
				);
			}

			public static void DrawNonHumanLikePowerOutput(Listing_Standard listingStandard, ref bool settingsEnabledValue, ref int settingsSliderValue, ref string sliderBuffer) {
				DrawEnabledLinkedIntFieldSlider(listingStandard,
					"Disable Non-HumanLike", ref settingsEnabledValue,
					"Non-HumanLike Power Output Scale (%)", ref settingsSliderValue, ref sliderBuffer
				);
			}

			public static void DrawInsectoidPowerOutput(Listing_Standard listingStandard, ref bool settingsEnabledValue, ref int settingsSliderValue, ref string sliderBuffer) {
				DrawEnabledLinkedIntFieldSlider(listingStandard,
					"Disable Insectoids", ref settingsEnabledValue,
					"Insectoid Power Output Scale (%)", ref settingsSliderValue, ref sliderBuffer
				);
			}

			public static void DrawMechanoidPowerOutput(Listing_Standard listingStandard, ref bool settingsEnabledValue, ref int settingsSliderValue, ref string sliderBuffer) {
				DrawEnabledLinkedIntFieldSlider(listingStandard,
					"Disable Mechanoids", ref settingsEnabledValue,
					"Mechanoid Power Output Scale (%)", ref settingsSliderValue, ref sliderBuffer
				);
			}

			public static void DrawSettings(Rect inRect, BioReactor_Mod mod) {
				Listing_Standard listingStandard = new Listing_Standard();

				listingStandard.Begin(inRect);
				DrawPowerOutputScale(listingStandard,
					ref mod.settings.totalPowerOutputScale, ref mod.totalPowerOutputScaleBuffer
				);

				listingStandard.GapLine(gapHeight);
				DrawMakeFuelOnHistolysis(listingStandard,
					ref mod.settings.makeFuelOnHistolysis
				);

				listingStandard.GapLine(gapHeight);
				DrawDisableHypothermia(listingStandard,
					ref mod.settings.disableHypothermia
				);

				listingStandard.GapLine(gapHeight);
				DrawHumanLikePowerOutput(listingStandard,
					ref mod.settings.disableHumanLike, ref mod.settings.humanLikePowerOutputScale, ref mod.humanLikePowerOutputScaleBuffer
				);

				listingStandard.GapLine(gapHeight);
				DrawNonHumanLikePowerOutput(listingStandard,
					ref mod.settings.disableNonHumanLike, ref mod.settings.nonHumanLikePowerOutputScale, ref mod.nonHumanLikePowerOutputScaleBuffer
				);

				listingStandard.GapLine(gapHeight);
				DrawInsectoidPowerOutput(listingStandard,
					ref mod.settings.disableInsectoids, ref mod.settings.insectoidPowerOutputScale, ref mod.insectoidPowerOutputScaleBuffer
				);

				listingStandard.GapLine(gapHeight);
				DrawMechanoidPowerOutput(listingStandard,
					ref mod.settings.disableMechnoids, ref mod.settings.mechanoidPowerOutputScale, ref mod.mechanoidPowerOutputScaleBuffer
				);

				listingStandard.End();
			}
		}

		// all the stuff im currenting using is in here
		private static class UseWidgets {
			private static Vector2 scrollPosition = Vector2.zero;

			public static float HeightOfLinkedIntFieldSliderWidget() {
				return Text.LineHeight * 3f;
			}
			public static float HeightOfLabeledCheckboxWidget() {
				return Text.LineHeight;
			}
			public static float HeightOfEnabledLinkedIntFieldSliderWidget(bool isEnabled) {
				if (!isEnabled) {
					return HeightOfLabeledCheckboxWidget();
				}

				return HeightOfLabeledCheckboxWidget() + HeightOfLinkedIntFieldSliderWidget();
			}

			public static void DrawGapLine(
				ref Rect inRect,
				float height,
				Color color,
				float lineWidth = 1f
			) {
				float lineOffsetY = height * 0.5f;
				Widgets.DrawLine(
					new Vector2(inRect.x, inRect.y + lineOffsetY),
					new Vector2(inRect.x + inRect.width, inRect.y + lineOffsetY),
					color,
					lineWidth
				);

				// update inRect height to include things added here
				inRect.y += height;
				inRect.height += height;
			}

			public static void DrawGap(
				ref Rect inRect,
				float height
			) {
				// update inRect height to include things added here
				inRect.y += height;
				inRect.height += height;
			}

			public static void DrawLinkedIntFieldSlider(
				ref Rect inRect,
				string label, ref int settingsValue, ref string buffer,
				int min = minPowerScale, int max = maxPowerScale
			) {
				Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, HeightOfLinkedIntFieldSliderWidget());

				// current y
				float y = 0f;

				Widgets.BeginGroup(outRect);

				Widgets.Label(new Rect(outRect.x, y, outRect.width, Text.LineHeight), label);
				y += Text.LineHeight;

				Widgets.TextFieldNumeric<int>(new Rect(outRect.x, y, outRect.width, Text.LineHeight), ref settingsValue, ref buffer, min, max);
				y += Text.LineHeight;

				settingsValue = (int)Widgets.HorizontalSlider(new Rect(outRect.x, y + (Text.LineHeight * 0.25f), outRect.width, Text.LineHeight), settingsValue, min, max);
				//y += Text.LineHeight;

				Widgets.EndGroup();

				// update inRect height to include things added here
				inRect.y += outRect.height;
				inRect.height += outRect.height;

				// just in case something weird happens
				settingsValue = Mathf.Clamp(settingsValue, min, max);
				buffer = settingsValue.ToStringSafe();
			}

			public static void DrawLabeledCheckboxWidget(
				ref Rect inRect,
				string label,
				ref bool settingsValue
			) {
				Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, Text.LineHeight);

				Widgets.BeginGroup(outRect);
				Widgets.CheckboxLabeled(new Rect(outRect.x, 0f, outRect.width, Text.LineHeight), label, ref settingsValue, false, null, null, false);
				Widgets.EndGroup();

				// update inRect height to include things added here
				inRect.y += outRect.height;
				inRect.height += outRect.height;
			}

			public static void DrawEnabledLinkedIntFieldSlider(
				ref Rect inRect,
				string enabledLabel, ref bool enabledSettingsValue, bool isEnabledInverted,
				string sliderLabel, ref int sliderSettingsValue, ref string sliderBuffer,
				int min = minPowerScale, int max = maxPowerScale
			) {
				DrawLabeledCheckboxWidget(ref inRect, enabledLabel, ref enabledSettingsValue);
				if ((isEnabledInverted && !enabledSettingsValue) || (!isEnabledInverted && enabledSettingsValue)) {
					DrawLinkedIntFieldSlider(ref inRect, sliderLabel, ref sliderSettingsValue, ref sliderBuffer, min, max);
				}
			}

			public static float DrawSettings(Rect inRect, BioReactor_Mod mod) {
				Rect resetToDefaultButtonRect = new Rect(0f, 0, inRect.width, Text.LineHeight);

				// draw the reset to defaults button
				{
					Widgets.BeginGroup(inRect);
					bool resetToDefault = Widgets.ButtonText(resetToDefaultButtonRect, "Reset To Default");
					Widgets.EndGroup();

					if (resetToDefault) {
						mod.ResetToDefault();
					}
				}

				Rect scrollViewRect = new Rect(inRect.x, inRect.y + resetToDefaultButtonRect.height, inRect.width, inRect.height - resetToDefaultButtonRect.height);

				float totalHeight = 0f;

				// tally up totalHeight
				{
					// add height of header gap
					totalHeight += gapHeight;
					// add height of Total Power Output Scale widget
					totalHeight += HeightOfLinkedIntFieldSliderWidget();
					// add height of gap line
					totalHeight += gapHeight;
					// add height of Make Fuel On Histolysis widget
					totalHeight += HeightOfLabeledCheckboxWidget();
					// add height of gap line
					totalHeight += gapHeight;
					// add height of Disable Hypothermia widget
					totalHeight += HeightOfLabeledCheckboxWidget();
					// add height of gap line
					totalHeight += gapHeight;
					// add height of HumanLike Power Output Scale widget
					totalHeight += HeightOfEnabledLinkedIntFieldSliderWidget(!mod.settings.disableHumanLike);
					// add height of gap line
					totalHeight += gapHeight;
					// add height of NonHumanLike Power Output Scale widget
					totalHeight += HeightOfEnabledLinkedIntFieldSliderWidget(!mod.settings.disableNonHumanLike);
					// add height of gap line
					totalHeight += gapHeight;
					// add height of Insectoid Power Output Scale widget
					totalHeight += HeightOfEnabledLinkedIntFieldSliderWidget(!mod.settings.disableInsectoids);
					// add height of gap line
					totalHeight += gapHeight;
					// add height of Mechnoid Power Output Scale widget
					totalHeight += HeightOfEnabledLinkedIntFieldSliderWidget(!mod.settings.disableMechnoids);
					// add height of gap line
					totalHeight += gapHeight;
					// add height of Mechnoid Power Output Scale widget
					totalHeight += HeightOfEnabledLinkedIntFieldSliderWidget(!mod.settings.disableMalnutrition);
					// add height of footer gap
					totalHeight += gapHeight;
				}

				Rect totalRect = new Rect(0f, 0f, scrollViewRect.width, totalHeight);

				bool hasVerticalScrollbar = totalRect.height > scrollViewRect.height;
				if (hasVerticalScrollbar) {
					// remove some width to account for scrollbar
					// just using line height because i dont know how to find the actual width but this seems good
					totalRect.width -= Text.LineHeight;
				}

				// draw the main scroll view that contains most settings
				{
					Widgets.BeginScrollView(scrollViewRect, ref scrollPosition, totalRect, hasVerticalScrollbar);
					Rect cursorRect = new Rect(totalRect.x, totalRect.y, totalRect.width, 0f);

					// draw a header gap
					DrawGap(ref cursorRect, gapHeight);

					DrawLinkedIntFieldSlider(ref cursorRect, "TotalPowerOutputScaleLabel".Translate(), ref mod.settings.totalPowerOutputScale, ref mod.totalPowerOutputScaleBuffer);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawLabeledCheckboxWidget(ref cursorRect, "MakeFuelOnHistolysisLabel".Translate(), ref mod.settings.makeFuelOnHistolysis);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawLabeledCheckboxWidget(ref cursorRect, "DisableHypothermiaLabel".Translate(), ref mod.settings.disableHypothermia);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawEnabledLinkedIntFieldSlider(ref cursorRect,
						"DisableHumanLikeLabel".Translate(), ref mod.settings.disableHumanLike, true,
						"HumanLikePowerOutputScaleLabel".Translate(), ref mod.settings.humanLikePowerOutputScale, ref mod.humanLikePowerOutputScaleBuffer
					);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawEnabledLinkedIntFieldSlider(ref cursorRect,
						"DisableNonHumanLikeLabel".Translate(), ref mod.settings.disableNonHumanLike, true,
						"NonHumanLikePowerOutputScaleLabel".Translate(), ref mod.settings.nonHumanLikePowerOutputScale, ref mod.nonHumanLikePowerOutputScaleBuffer
					);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawEnabledLinkedIntFieldSlider(ref cursorRect,
						"DisableInsectoidsLabel".Translate(), ref mod.settings.disableInsectoids, true,
						"InsectoidPowerOutputScaleLabel".Translate(), ref mod.settings.insectoidPowerOutputScale, ref mod.insectoidPowerOutputScaleBuffer
					);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawEnabledLinkedIntFieldSlider(ref cursorRect,
						"DisableMechnoidsLabel".Translate(), ref mod.settings.disableMechnoids, true,
						"MechnoidPowerOutputScaleLabel".Translate(), ref mod.settings.mechanoidPowerOutputScale, ref mod.mechanoidPowerOutputScaleBuffer
					);

					DrawGapLine(ref cursorRect, gapHeight, Color.gray);
					DrawEnabledLinkedIntFieldSlider(ref cursorRect,
						"DisableMalnutritionLabel".Translate(), ref mod.settings.disableMalnutrition, true,
						"MalnutritionSeverityScaleLabel".Translate(), ref mod.settings.malnutritionSeverityScale, ref mod.malnutritionSeverityScaleBuffer
					);

					// draw a footer gap
					DrawGap(ref cursorRect, gapHeight);

					Widgets.EndScrollView();
				}

				return totalHeight;
			}
		}

		public override void DoSettingsWindowContents(Rect inRect) {
			//UseListingStandard.DrawSettings(inRect, this);

			UseWidgets.DrawSettings(inRect, this);

			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory() {
			return "Bioreactor (Revised)";
		}
	}
}

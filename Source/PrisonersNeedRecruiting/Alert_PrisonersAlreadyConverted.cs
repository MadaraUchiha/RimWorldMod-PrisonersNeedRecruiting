using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonersNeedRecruiting
{
    public class Alert_PrisonersAlreadyConverted : Alert_Critical
    {
        // Wardens skip prisoners who already share their ideoligion, and the game never
        // switches them off Convert, so these prisoners sit idle until reassigned.
        private List<Pawn> ConvertedPrisoners => PawnsFinder.AllMaps_PrisonersOfColonySpawned
                    .Where(prisoner => prisoner.guest.IsInteractionEnabled(PrisonerInteractionModeDefOf.Convert)
                        && prisoner.guest.ideoForConversion != null
                        && prisoner.Ideo == prisoner.guest.ideoForConversion)
                    .ToList();

        private const int RIGHT_CLICK = 1;
        private List<Pawn> _prevConvertedPrisoners = new List<Pawn>();

        private bool _dismissed = false;

        public override string GetLabel()
        {
            if (ConvertedPrisoners.Count == 1)
            {
                return "PrisonerAlreadyConverted".Translate();
            }
            return "PrisonersAlreadyConverted".Translate();
        }

        public override TaggedString GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn item in ConvertedPrisoners)
            {
                stringBuilder.AppendLine("  - " + item.NameShortColored.Resolve());
            }
            return "PrisonersAlreadyConvertedDesc".Translate(stringBuilder);
        }

        public override AlertReport GetReport()
        {
            if (!ModsConfig.IdeologyActive)
            {
                return AlertReport.Inactive;
            }

            if (IsDirty())
            {
                _prevConvertedPrisoners = ConvertedPrisoners;
                _dismissed = false;
            }
            if (_dismissed)
            {
                return AlertReport.Inactive;
            }
            return AlertReport.CulpritsAre(ConvertedPrisoners);
        }

        private bool IsDirty()
        {
            if (_prevConvertedPrisoners.Count != ConvertedPrisoners.Count)
            {
                return true;
            }
            if (ConvertedPrisoners.Any(p => !_prevConvertedPrisoners.Contains(p)))
            {
                return true;
            }
            return false;
        }

        protected override void OnClick()
        {
            if (Event.current.button != RIGHT_CLICK)
            {
                base.OnClick();
                return;
            }

            _dismissed = true;
        }
    }
}

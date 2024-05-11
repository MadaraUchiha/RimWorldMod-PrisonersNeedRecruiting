using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonersNeedRecruiting
{
    public class Alert_PrisonersNonInteract : Alert_Critical
    {
        private List<Pawn> UnassignedPrisoners => PawnsFinder.AllMaps_PrisonersOfColonySpawned
                    .Where(prisoner => prisoner.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.MaintainOnly)
                    // TODO: Try this ingame 
                    .ToList();

        private const int RIGHT_CLICK = 1;
        private List<Pawn> _prevUnassignedPrisoners = new List<Pawn>();

        private bool _dismissed = false;

        public override string GetLabel()
        {
            if (UnassignedPrisoners.Count == 1)
            {
                return "PrisonerSetToNonInteract".Translate();
            }
            return "PrisonersSetToNonInteract".Translate();
        }

        public override TaggedString GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn item in UnassignedPrisoners)
            {
                stringBuilder.AppendLine("  - " + item.NameShortColored.Resolve());
            }
            return "PrisonersSetToNonInteractDesc".Translate(stringBuilder);
        }

        public override AlertReport GetReport()
        {

            if (IsDirty())
            {
                _prevUnassignedPrisoners = UnassignedPrisoners;
                _dismissed = false;
            }
            if (_dismissed)
            {
                return AlertReport.Inactive;
            }
            return AlertReport.CulpritsAre(UnassignedPrisoners);
        }

        private bool IsDirty()
        {
            if (_prevUnassignedPrisoners.Count != UnassignedPrisoners.Count)
            {
                return true;
            }
            if (UnassignedPrisoners.Any(p => !_prevUnassignedPrisoners.Contains(p)))
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

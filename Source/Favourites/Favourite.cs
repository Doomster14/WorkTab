// Favourite.cs
// Copyright Karel Kroeze, 2020-2020

using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab {
    public class Favourite: PriorityTracker {
        private Texture2D _icon;
        private string    _iconPath;
        private string    _label;
        private int       _loadId;

        public Texture2D Icon {
            get {
                if (_icon == null && !_iconPath.NullOrEmpty()) {
                    _icon = ContentFinder<Texture2D>.Get(_iconPath);
                }

                return _icon;
            }
            set {
                _icon = value;
                _iconPath = "UI/Icons/Various/" + _icon.name;
            }
        }

        public string Label {
            get => _label;
            set => _label = value;
        }

        public static Favourite From(Pawn pawn, bool apply = false) {
            // create from pawn
            Favourite favourite  = new Favourite();
            PriorityTracker priorities = PriorityManager.Get[pawn];
            foreach (WorkGiverDef workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading) {
                favourite.priorities[workgiver] = priorities[workgiver].Clone(favourite);
            }

            // open name/icon window
            Find.WindowStack.Add(new Dialog_CreateFavourite(favourite, pawn));

            return favourite;
        }

        public void Apply(Pawn pawn) {
            // apply to pawn
            PriorityTracker priorities = PriorityManager.Get[pawn];
            foreach (WorkGiverDef workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading) {
                priorities[workgiver] = this[workgiver].Clone(priorities);
            }
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref _loadId, "ID");
            Scribe_Values.Look(ref _label, "Label");
            Scribe_Values.Look(ref _iconPath, "IconPath");
        }

        protected override void OnChange() {
            if (Scribe.mode == LoadSaveMode.Inactive) {
                FavouriteManager.Save(this);
            }
        }
    }
}

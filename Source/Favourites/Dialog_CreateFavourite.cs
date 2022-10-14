// Dialog_CreateFavourite.cs
// Copyright Karel Kroeze, 2020-2020

using FluffyUI;
using RimWorld;
using UnityEngine;
using Verse;
using Widgets = FluffyUI.Widgets;

namespace WorkTab {
    public class Dialog_CreateFavourite: Dialog_Favourite {
        private readonly Pawn pawn;

        public Dialog_CreateFavourite(Favourite favourite, Pawn pawn) : base(favourite) {
            this.pawn = pawn;
        }

        public void ApplyAndClose() {
            FailReason valid = FavouriteManager.IsValidLabel(label, favourite.Label);
            if (!valid) {
                Messages.Message(valid.Reason, MessageTypeDefOf.RejectInput, false);
                return;
            }

            favourite.Icon = textureChooser.Choice;
            favourite.Label = label;

            FavouriteManager.Favourites.Add(favourite);
            FavouriteManager.Get[pawn] = favourite;
            FavouriteManager.Save(favourite);

            Close();
        }

        public override void DoWindowContents(Rect canvas) {
            System.Collections.Generic.List<FluffyUI.Grid> rows = new FluffyUI.Grid(canvas).Rows(HEADER_HEIGHT, FIELD_HEIGHT, BUTTON_HEIGHT);
            Rect titleRect = rows[0].Rect;
            FluffyUI.Grid nameRect = rows[1].Column(9);
            FluffyUI.Grid iconRect = rows[1].Column(3);
            rows[2].Column(4);
            FluffyUI.Grid cancelRect = rows[2].Column(4);
            FluffyUI.Grid okRect = rows[2].Column(4);

            // title and a little text
            Widgets.Label(titleRect, "Fluffy.WorkTab.CreateFavouritesDialogTitle".Translate(), font: GameFont.Medium);

            // favourites label
            FailReason valid = FavouriteManager.IsValidLabel(label);
            GUI.color = valid ? Color.white : Color.red;
            label = GUI.TextField(nameRect, label);
            GUI.color = Color.white;

            // icon tumbler
            textureChooser.DrawAt(iconRect);

            // ok/cancel button.
            if (Verse.Widgets.ButtonText(cancelRect, "CancelButton".Translate())) {
                Close();
            }

            if (Verse.Widgets.ButtonText(okRect, "AcceptButton".Translate())) {
                ApplyAndClose();
            }
        }

        public override void OnAcceptKeyPressed() {
            ApplyAndClose();
        }

        public override void PostClose() {
            base.PostClose();
            if (favourite.Label.NullOrEmpty() || favourite.Icon == null) {
                FavouriteManager.Remove(favourite);
            }
        }
    }
}

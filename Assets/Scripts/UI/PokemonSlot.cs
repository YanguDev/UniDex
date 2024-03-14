using System;
using System.Collections;
using System.Collections.Generic;
using UniDex.Pokemons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UniDex.UI
{
    public class PokemonSlot : VisualElement
    {
        public Image Image;
        public Label Name;

        public PokemonSlot(PokemonObject pokemonObject, Action<PokemonObject> clickAction)
        {
            Image = this.AddChild(new Image());
            Name = this.AddChild(new Label(pokemonObject.Name));
            name = $"{pokemonObject.Name}-Slot";

            Image.scaleMode = ScaleMode.ScaleToFit;
            Image.image = pokemonObject.Texture;
            style.opacity = 0;
            style.opacity = 1;

            RegisterCallback<ClickEvent>(clickEvent => clickAction(pokemonObject));
        }
    }
}

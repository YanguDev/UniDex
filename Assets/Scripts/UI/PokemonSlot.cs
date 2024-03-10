using System.Collections;
using System.Collections.Generic;
using UniDex.Pokemons;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.UI
{
    public class PokemonSlot : VisualElement
    {
        public Image Image;
        public Label Name;

        public PokemonSlot(PokemonData pokemonData)
        {
            Image = this.AddChild(new Image());
            Name = this.AddChild(new Label(pokemonData.Name));

            Image.image = pokemonData.Texture;
        }
    }
}

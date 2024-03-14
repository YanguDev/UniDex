using System.Collections.Generic;
using UniDex.Colors;
using UniDex.Pokemons;
using UniDex.UI.DocumentControllers;
using UnityEngine;

namespace UniDex.Menus
{
    public class DetailsMenu : Menu
    {
        [SerializeField]
        private PokemonDetailsDocumentController documentController;
        [SerializeField]
        private NamedColorValues pokemonColorValues;

        private PokemonObject lastPokemonObject;
        private List<PokemonObject> currentPokemonsContext;
        private int currentContextIndex;

        public override void Open()
        {
            base.Open();

            documentController.ExitButton.clicked += Exit;
            documentController.LeftButton.clicked += PreviousPokemon;
            documentController.RightButton.clicked += NextPokemon;

            RefreshNavigationButtons();
        }

        public void SetPokemonDetails(PokemonObject pokemonObject)
        {
            lastPokemonObject = pokemonObject;
            documentController.PokemonID.text = pokemonObject.ID.ToString();
            documentController.PokemonName.text = pokemonObject.Name;
            documentController.PokemonImage.image = pokemonObject.Texture;
            documentController.PokemonDescription.text = pokemonObject.FlavorText;
            documentController.PokemonGenus.text = pokemonObject.Genus;
            documentController.PokemonWeight.text = pokemonObject.Weight;
            documentController.PokemonHeight.text = pokemonObject.Height;
            documentController.Header.style.backgroundColor = pokemonColorValues.GetColorFromNamedColor(pokemonObject.NamedColor);
        }

        public void SetPokemonsContext(List<PokemonObject> pokemonsContext, int currentIndex)
        {
            currentPokemonsContext = pokemonsContext;
            currentContextIndex = currentIndex;
            RefreshNavigationButtons();
        }

        private void RefreshNavigationButtons()
        {
            if (currentPokemonsContext == null)
            {
                documentController.LeftButton.visible = false;
                documentController.RightButton.visible = false;
                return;
            }

            documentController.LeftButton.visible = currentContextIndex > 0;
            documentController.RightButton.visible = currentContextIndex < currentPokemonsContext.Count - 1;
        }

        private void Exit()
        {
            MenuManager.Instance.SwitchMenu<PokemonsMenu>()
                .ScrollTo(lastPokemonObject);
        }

        private void NextPokemon()
        {
            if (!documentController.RightButton.visible) return;

            SetPokemonDetails(currentPokemonsContext[++currentContextIndex]);
            RefreshNavigationButtons();
        }

        private void PreviousPokemon()
        {
            if (!documentController.LeftButton.visible) return;

            SetPokemonDetails(currentPokemonsContext[--currentContextIndex]);
            RefreshNavigationButtons();
        }
    }
}
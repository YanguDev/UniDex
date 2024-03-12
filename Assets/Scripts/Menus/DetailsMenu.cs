using UniDex.Pokemons;
using UniDex.UI.DocumentControllers;
using UnityEngine;

namespace UniDex.Menus
{
    public class DetailsMenu : Menu
    {
        [Header("UI")]
        [SerializeField]
        private PokemonDetailsDocumentController documentController;

        public override void Open()
        {
            base.Open();
            documentController.ExitButton.clicked += Exit;
        }

        public override void Close()
        {
            documentController.ExitButton.clicked -= Exit;
            base.Close();
        }

        private void Exit()
        {
            MenuManager.Instance.SwitchMenu<PokemonsMenu>();
        }

        public void SetPokemonDetails(PokemonObject pokemonObject)
        {
            documentController.PokemonID.text = pokemonObject.ID.ToString();
            documentController.PokemonName.text = pokemonObject.Name;
            documentController.PokemonImage.image = pokemonObject.Texture;
            documentController.PokemonDescription.text = pokemonObject.FlavorText;
            documentController.PokemonGenus.text = pokemonObject.Genus;
            documentController.PokemonWeight.text = pokemonObject.Weight;
            documentController.PokemonHeight.text = pokemonObject.Height;
        }
    }
}
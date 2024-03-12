using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.UI.DocumentControllers
{
    public class PokemonDetailsDocumentController : MonoBehaviour
    {
        [SerializeField]
        private UIDocument pokemonDetailsDocument;

        public Label PokemonID => Root.Q<Label>(nameof(PokemonID));
        public Label PokemonName => Root.Q<Label>(nameof(PokemonName));
        public Label PokemonGenus => Root.Q<Label>(nameof(PokemonGenus));
        public Label PokemonHeight => Root.Q<Label>(nameof(PokemonHeight));
        public Label PokemonWeight => Root.Q<Label>(nameof(PokemonWeight));
        public Label PokemonDescription => Root.Q<Label>(nameof(PokemonDescription));

        public Image PokemonImage => Root.Q<Image>(nameof(PokemonImage));
        public Button ExitButton => Root.Q<Button>(nameof(ExitButton));
        public Button LeftButton => Root.Q<Button>(nameof(LeftButton));
        public Button RightButton => Root.Q<Button>(nameof(RightButton));

        private VisualElement Root => pokemonDetailsDocument.rootVisualElement;
    }
}
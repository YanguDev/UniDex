namespace UniDex.Pokemons.API.Data
{
    [System.Serializable]
    public class Pokemon
    {
        public int id;
        public string name;
        public int weight;
        public int height;
        public PokemonSprites sprites;
        public PokemonSpecies species;

        [System.Serializable]
        public struct PokemonSprites
        {
            public string front_default;
            public string front_shiny;
        }

        [System.Serializable]
        public struct PokemonSpecies
        {
            public string name;
            public string url;
        }
    }
}
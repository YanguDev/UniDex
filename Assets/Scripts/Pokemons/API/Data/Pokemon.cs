namespace UniDex.Pokemons.API.Data
{
    public class Pokemon
    {
        public int id;
        public string name;
        public int weight;
        public int height;
        public PokemonSprites sprites;
        public PokemonSpecies species;

        public struct PokemonSprites
        {
            public string front_default;
            public string front_shiny;
        }

        public struct PokemonSpecies
        {
            public string name;
            public string url;
        }
    }
}
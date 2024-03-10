namespace UniDex.Pokemons
{
    public struct PokemonData
    {
        public int id;
        public string name;
        public PokemonSpriteURLs sprites;

        public override string ToString()
        {
            return $"{id}: {name} {sprites.front_default}";
        }

        public struct PokemonSpriteURLs
        {
            public string front_default;
            public string front_shiny;
        }
    }
}
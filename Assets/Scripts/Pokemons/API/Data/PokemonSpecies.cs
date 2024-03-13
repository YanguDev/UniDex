namespace UniDex.Pokemons.API.Data
{
    public struct PokemonSpecies
    {
        public uint id;
        public Name[] names;
        public FlavorText[] flavor_text_entries;
        public Genus[] genera;
        
        public struct FlavorText
        {
            public string flavor_text;
            public Language language;
        }

        public struct Genus
        {
            public string genus;
            public Language language;
        }

        public struct Name
        {
            public string name;
            public Language language;
        }
    }
}
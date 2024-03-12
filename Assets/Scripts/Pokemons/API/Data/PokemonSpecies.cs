namespace UniDex.Pokemons.API.Data
{
    public struct PokemonSpecies
    {
        public uint id;
        public Name[] names;
        public FlavorTextEntry[] flavor_text_entries;
        public Genus[] genera;
        
        public struct FlavorTextEntry
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
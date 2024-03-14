namespace UniDex.Pokemons.API.Data
{
    [System.Serializable]
    public struct PokemonSpecies
    {
        public uint id;
        public Name[] names;
        public FlavorText[] flavor_text_entries;
        public Genus[] genera;
        public NamedAPIResource color;
        
        [System.Serializable]
        public struct FlavorText
        {
            public string flavor_text;
            public Language language;
        }

        [System.Serializable]
        public struct Genus
        {
            public string genus;
            public Language language;
        }

        [System.Serializable]
        public struct Name
        {
            public string name;
            public Language language;
        }
    }
}
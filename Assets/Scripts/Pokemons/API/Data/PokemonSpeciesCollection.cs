using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniDex.Pokemons.API.Data
{
    public struct PokemonSpeciesCollection
    {
        public int count;
        public PokemonSpeciesResult[] results;

        public struct PokemonSpeciesResult
        {
            public string name;
            public string url;
        }
    }
}

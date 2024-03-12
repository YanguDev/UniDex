using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniDex.Pokemons.API.Data
{
    public struct PokemonCollection
    {
        public int count;
        public PokemonResult[] results;

        public struct PokemonResult
        {
            public string name;
            public string url;
        }
    }
}

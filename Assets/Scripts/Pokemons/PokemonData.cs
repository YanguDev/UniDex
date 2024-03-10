using System.Threading.Tasks;
using UniDex.Pokemons.API.Data;
using UnityEngine;

namespace UniDex.Pokemons
{
    public struct PokemonData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Texture Texture { get; private set; }

        public static async Task<PokemonData> FromPokemonSpecies(PokemonSpecies pokemonSpecies)
        {
            var texture = await TextureDownloader.DownloadFromURL(pokemonSpecies.sprites.front_default);
            return new PokemonData()
            {
                ID = pokemonSpecies.id,
                Name = pokemonSpecies.name,
                Texture = texture
            };
        }

        public override string ToString()
        {
            return $"{ID}: {Name}";
        }
    }
}
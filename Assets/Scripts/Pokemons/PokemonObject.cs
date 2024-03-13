using System.Linq;
using System.Threading.Tasks;
using UniDex.Pokemons.API;
using UniDex.Pokemons.API.Data;
using UnityEngine;

namespace UniDex.Pokemons
{
    public class PokemonObject
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Texture Texture { get; private set; }
        public string Weight { get; private set; }
        public string Height { get; private set; } 
        public string FlavorText { get; private set; }
        public string Genus { get; private set; }

        public PokemonObject(Pokemon pokemon, PokemonSpecies species, Texture texture)
        {
            ID = pokemon.id;
            Name = species.names.FirstOrDefault(name => name.language.name == "en").name;
            FlavorText = species.flavor_text_entries.FirstOrDefault(entry => entry.language.name == "en").flavor_text.Replace("", " ");
            Genus = species.genera.FirstOrDefault(genus => genus.language.name == "en").genus;
            Weight = $"{(float) pokemon.weight / 100} kg";
            Height = $"{(float) pokemon.height / 10} m";
            Texture = texture;
        }

        public override string ToString()
        {
            return $"{ID}: {Name}";
        }
    }
}
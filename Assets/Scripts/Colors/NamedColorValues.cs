using UnityEngine;

namespace UniDex.Colors
{
    [CreateAssetMenu(menuName = "Data/Named Color Values")]
    public class NamedColorValues : ScriptableObject
    {
        [field: SerializeField]
        public Color Black { get; private set; }
        [field: SerializeField]
        public Color Blue { get; private set; }
        [field: SerializeField]
        public Color Brown { get; private set; }
        [field: SerializeField]
        public Color Gray { get; private set; }
        [field: SerializeField]
        public Color Green { get; private set; }
        [field: SerializeField]
        public Color Pink { get; private set; }
        [field: SerializeField]
        public Color Purple { get; private set; }
        [field: SerializeField]
        public Color Red { get; private set; }
        [field: SerializeField]
        public Color White { get; private set; }
        [field: SerializeField]
        public Color Yellow { get; private set; }

        public Color GetColorFromNamedColor(NamedColor namedColor)
        {
            return namedColor switch
            {
                NamedColor.Black => Black,
                NamedColor.Blue => Blue,
                NamedColor.Brown => Brown,
                NamedColor.Gray => Gray,
                NamedColor.Green => Green,
                NamedColor.Pink => Pink,
                NamedColor.Purple => Purple,
                NamedColor.Red => Red,
                NamedColor.White => White,
                NamedColor.Yellow => Yellow,
                _ => White
            };
        }
    }
}

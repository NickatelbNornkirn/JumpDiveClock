using Raylib_cs;

namespace JumpDiveClock
{
    public class ColorManager
    {
        public Color Background { get; }
        public Color Base { get; }
        public Color AheadGaining { get; }
        public Color AheadLosing { get; }
        public Color BehindGaining { get; }
        public Color BehindLosing { get; }
        public Color Best { get; }
        public Color Separator { get; }

        /*
            The constructors receives #rrggbb colors.
        */
        public ColorManager(string backgroundColor, string baseColor, string aheadGainingColor,
                            string aheadLosingColor, string behindGainingColor, 
                            string behindLosingColor, string bestColor, string separatorColor)
        {
            Background = ToColor(backgroundColor);
            Base = ToColor(baseColor);
            AheadGaining = ToColor(aheadGainingColor);
            AheadLosing = ToColor(aheadLosingColor);
            BehindGaining = ToColor(behindGainingColor);
            BehindLosing = ToColor(behindLosingColor);
            Best = ToColor(bestColor);
            Separator = ToColor(separatorColor);
        }

        private Color ToColor(string hexColor)
        {
            // Colors are in the #rrggbb format.
            return new Color(
                Convert.ToInt32(hexColor.Substring(1, 2), 16),
                Convert.ToInt32(hexColor.Substring(3, 2), 16),
                Convert.ToInt32(hexColor.Substring(5, 2), 16),
                255
            );
        }
    }
}
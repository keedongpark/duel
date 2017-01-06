using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts
{
    public class Card
    {
        public enum Rank : byte
        {
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }

        public enum Suite : byte
        {
            Clubs,
            Diamonds,
            Spades,
            Hearts,
        }

        private string rankString = "23456789TJQKA";
        private string suiteString = "CDSH";
        private Suite suite;
        private Rank rank;

        public Card(Suite s, Rank v)
        {
            suite = s;
            rank = v;
        }

        public Suite S { get { return suite; } }
        public Rank R { get { return rank; } }

        public string ToString(Rank v)
        {
            return rankString[(byte)v].ToString();
        }

        public string ToString(Suite s)
        {
            return suiteString[(byte)s].ToString();
        }

        public override string ToString()
        {
            return ToString(S) + ToString(R);
        }
    } 
}

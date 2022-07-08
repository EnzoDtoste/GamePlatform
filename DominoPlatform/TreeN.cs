using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoPlatform
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> type of Ficha side </typeparam>
    /// <typeparam name="P"> return type of print </typeparam>
    public class TreeN<T, P>
    {

        Ficha<T, P> ficha;

        List<TreeN<T, P>>[] hijos;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ficha"></param>
        /// <param name="index_usedSide"> if it´s the first Ficha put -1 </param>
        public TreeN(Ficha<T, P> ficha, int index_usedSide)
        {

            this.ficha = ficha;

            hijos = new List<TreeN<T, P>>[ficha.sides.Count];

            for (int i = 0; i < hijos.Length; i++)
            {

                if (i != index_usedSide)
                    hijos[i] = new List<TreeN<T, P>>();

            }

        }

        /// <summary>
        /// Add a ficha to the Tree
        /// </summary>
        /// <param name="board_side"> side that you wich to play for </param>
        /// <param name="ficha"></param>
        /// <param name="index_side"> index side of the ficha that you wich to play for </param>
        /// <returns></returns>
        public bool AddFicha(T board_side, Ficha<T, P> ficha, int index_side)
        {

            //check if the play is correct
            if (!ficha.CanMatch(board_side).Contains(index_side))
                return false;

            //can I play here?
            if(this.ficha.sides.Contains(board_side))
            {

                //all matching sides
                List<int> indexof = new List<int>();

                for (int i = 0; i < this.ficha.sides.Count; i++)
                {
                    if(this.ficha.sides[i].Equals(board_side))
                        indexof.Add(i);
                }

                foreach (int i in indexof)
                {

                    //search for a spot
                    if(hijos[i] != null && hijos[i].Count < this.ficha.plays_bySide)
                    { 
                        hijos[i].Add(new TreeN<T, P>(ficha, index_side));
                        return true;

                    }

                }

            }

            foreach(var hijo in hijos)
            {

                if (hijo != null)
                {

                    foreach (var tree in hijo)
                    {

                        if (tree.AddFicha(board_side, ficha, index_side))
                            return true;

                    }

                }

            }

            return false;

        }

        /// <summary>
        /// List of available sides wich the player can play for
        /// </summary>
        /// <returns></returns>
        public List<T> AvailableSides()
        {

            List<T> avSides = new List<T>();

            for (int i = 0; i < hijos.Length; i++)
            {

                if(hijos[i] != null)
                {

                    if (hijos[i].Count < ficha.plays_bySide)
                        avSides.Add(ficha.sides[i]);

                    foreach (var tree in hijos[i])
                        avSides = new List<T>(avSides.Union(tree.AvailableSides()));

                }

            }

            return avSides;

        }

        /// <summary>
        /// List of fichas at the same level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<Ficha<T, P>> Level(int level)
        {

            if (level == 0)
                return new List<Ficha<T, P>>() { this.ficha };

            else
            {

                List<Ficha<T, P>> lev = new List<Ficha<T, P>>();

                foreach (var hijo in hijos)
                {

                    if(hijo != null)
                    {

                        if (hijo.Count == 0)
                            lev.Add(null);

                        foreach (var tree in hijo)
                            lev.AddRange(tree.Level(level - 1));

                    }

                }

                return lev;

            }

        }

        /// <summary>
        /// Depth of the Tree
        /// </summary>
        /// <returns></returns>
        public int Deep()
        {

            int deepest = 1;

            foreach(var hijo in hijos)
            {

                if (hijo != null)
                {

                    foreach (var tree in hijo)
                    {

                        deepest = Math.Max(deepest, 1 + tree.Deep());

                    }

                }

            }

            return deepest;

        }

    }
}

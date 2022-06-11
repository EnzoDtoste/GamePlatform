using GamePlatform;

namespace DominoPlatform
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> Type of Ficha´s Side </typeparam>
    /// <typeparam name="P"> return type of print </typeparam>
    public class Board<T, P> : Environment<TreeN<T, P>, DominoPlayer<T, P>, List<Ficha<T, P>>, P>
    {

        List<Ficha<T, P>> FichasAfuera;

        List<Ficha<T, P>>[] rounds;

        int pass;

        private Board(TreeN<T, P> Collection, IEnumerable<DominoPlayer<T, P>> Players, GenerateFichas<T, P> GenerateFichas, Distribute<T, P> initialDistribute, int initialHand, PassTurn<T, P> pass) : base(Collection, Players)
        {

            rounds = new List<Ficha<T, P>>[this.Players.Count];

            for (int i = 0; i < rounds.Length; i++)
            {

                rounds[i] = new List<Ficha<T, P>>();

            }

            this.pass = pass(rounds, ActualPlayer);

            FichasAfuera = new List<Ficha<T, P>>(GenerateFichas());

            foreach(var player in this.Players)
            {

                player.Collection.AddRange(initialDistribute(FichasAfuera, initialHand));

                foreach(Ficha<T, P> ficha in player.Collection)
                {

                    Remove(FichasAfuera, ficha);

                }

            }

        }

        public static void Remove(List<Ficha<T, P>> Collection, Element<P> e)
        {

            for (int i = 0; i < Collection.Count; i++)
            {

                if (Collection[i].EqualsTo(e))
                { Collection.RemoveAt(i); i--; }

            }

        }

        public static Board<T, P> StartGame(IEnumerable<DominoPlayer<T, P>> Players, GenerateFichas<T, P> GenerateFichas, Distribute<T, P> initialDistribute, int initialHand, PassTurn<T, P> pass)
        {

            return new Board<T, P>(null , Players, GenerateFichas, initialDistribute, initialHand, pass);

        }

        private bool AddFicha(T board_side, Ficha<T, P> ficha, int index_side)
        {

            if (this.Collection == null)
            { this.Collection = new TreeN<T, P>(ficha, -1); return true; }

            return this.Collection.AddFicha(board_side, ficha, index_side);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passTurn"></param>
        /// <param name="endConditions"></param>
        /// <returns> false if the game is over, otherwise true </returns>
        public bool PlayTurn(PassTurn<T, P> passTurn, bool robar, Winner<T, P> winner, params IConditions<T, P>[] endConditions)
        {

            if (Players[ActualPlayer].Enabled)
            {

                (T, Ficha<T, P>, int) ficha;

                if (this.Collection != null)
                    ficha = ((DominoPlayer<T, P>)Players[ActualPlayer]).Play(rounds, Collection.AvailableSides());

                else
                    ficha = ((DominoPlayer<T, P>)Players[ActualPlayer]).Play(rounds, null);

                if (ficha.Item2 != null)
                {

                    if (AddFicha(ficha.Item1, ficha.Item2, ficha.Item3))
                        rounds[ActualPlayer].Add(ficha.Item2);

                    else if (robar)
                    {

                        var f = PickFicha();

                        if (f != null)
                        { Players[ActualPlayer].Collection.Add(f); return true; }

                        else
                            rounds[ActualPlayer].Add(null);

                    }

                    else
                        rounds[ActualPlayer].Add(null);

                }

                else if (robar)
                {

                    var f = PickFicha();

                    if (f != null)
                    { Players[ActualPlayer].Collection.Add(f); return true; }

                    else
                        rounds[ActualPlayer].Add(null);

                }

                else
                    rounds[ActualPlayer].Add(null);

                if (CheckConditions(winner, endConditions))
                    return false;
                
            }

            pass *= passTurn(rounds, ActualPlayer);

            NextPlayer(pass);

            return true;

        }

        public Ficha<T, P> PickFicha()
        {

            if (FichasAfuera.Count == 0)
                return null;

            Random r = new Random();

            var f = FichasAfuera[r.Next(FichasAfuera.Count)];
            Remove(FichasAfuera, f);

            return f;

        }

        private void NextPlayer(int next)
        {

            next = next % Players.Count;

            if (next > 0)
            {

                if (next <= Players.Count - 1 - ActualPlayer)
                    ActualPlayer += next;

                else
                    ActualPlayer = next - (Players.Count - 1 - ActualPlayer) - 1;

            }

            else if(next < 0)
            {

                next *= -1;

                if (next <= ActualPlayer)
                    ActualPlayer -= next;

                else
                    ActualPlayer = Players.Count - (next - ActualPlayer);

            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endConditions"></param>
        /// <returns> true is the game must over </returns>
        private bool CheckConditions(Winner<T, P> winner, params IConditions<T, P>[] endConditions)
        {

            foreach(var condition in endConditions)
            {
                if(condition.isValid(in rounds, Players, ActualPlayer))
                {
                    if(condition.Finish(Players, ActualPlayer, winner))
                        return true;
                }
            }

            return false;

        }

    }
    
    public class Ficha<T, P> : Element<P>
    {

        public List<T> sides { get; private set; }
        public List<int> values { get; private set; }
        public int plays_bySide { get; private set; }

        public Ficha(List<T> sides, List<int> values, int plays_bySide)
        {
            this.sides = sides;
            this.values = values;
            this.plays_bySide = plays_bySide;
        }

        /// <summary>
        /// returns a List of matching sides´ indexes
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public virtual List<int> CanMatch(T side) 
        {
            throw new NotImplementedException();
        }

        public virtual int TotalValue()
        {
            throw new NotImplementedException();
        }

        public override bool EqualsTo(Element<P> other)
        {
            throw new NotImplementedException();
        }

        public override P Print(PrintParameters pp)
        {
            throw new NotImplementedException();
        }
    }

    public class DominoPlayer<T, P> : Player<List<Ficha<T, P>>, P>
    {

        PlayFicha<T, P> Jugador;

        public DominoPlayer(PlayFicha<T, P> Jugador) : base(new List<Ficha<T, P>>())
        { this.Jugador = Jugador; }

        public (T, Ficha<T, P>, int) Play(List<Ficha<T, P>>[] Rounds, List<T> sides)
        {

            (T, Ficha<T, P>, int) ficha = Jugador(Rounds, sides, Collection);

            if (ficha.Item2 != null)
                RemoveELement(Board<T, P>.Remove, ficha.Item2);

            return ficha;

        }

        public int TotalValues()
        {

            int total = 0;

            foreach(Ficha<T, P> ficha in Collection)
            {

                total += ficha.TotalValue();

            }

            return total;

        }

    }

    #region Delegates

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> Type of the ficha´s side </typeparam>
    /// <typeparam name="P"> return type of print </typeparam>
    public delegate IEnumerable<Ficha<T, P>> GenerateFichas<T, P>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> Type of the ficha´s side </typeparam>
    /// <typeparam name="P"> return type of print </typeparam>
    public delegate IEnumerable<Ficha<T, P>> Distribute<T, P>(List<Ficha<T, P>> FichasCollection, int total);

    /// <summary>
    /// return null if you can´t play 
    /// </summary>
    /// <typeparam name="T"> type of ficha´s side </typeparam>
    /// <typeparam name="P"> return type of print </typeparam>
    /// <param name="Rounds"> game history </param>
    /// <param name="sides"> sides available </param>
    /// <param name="hand"> hand of player </param>
    /// <returns> board side, ficha, index of ficha play side </returns>
    public delegate (T, Ficha<T, P>, int) PlayFicha<T, P>(List<Ficha<T, P>>[] Rounds, List<T> sides, List<Ficha<T, P>> hand);

    public delegate int PassTurn<T, P>(List<Ficha<T, P>>[] Rounds, int actualPlayer);

    public delegate int Winner<T, P>(in List<DominoPlayer<T, P>> players);

    #endregion

    public interface IConditions<T, P>
    {

        public bool isValid(in List<Ficha<T, P>>[] Rounds, in List<DominoPlayer<T, P>> players, int actualPlayer);

        /// <summary>
        ///  what to do at the end
        /// </summary>
        /// <returns> true is the game must over </returns>
        public bool Finish(in List<DominoPlayer<T, P>> players, int actualPlayer, Winner<T, P> winner);

    }

    public interface IBoardPrint<T, P>: IEnvironmentPrint<TreeN<T, P>, DominoPlayer<T, P>, List<Ficha<T, P>>, P>
    { }
    public interface IDominoPlayerPrint<T, P> : IPlayerPrint<List<Ficha<T, P>>, P>
    { }

}

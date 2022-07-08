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

        //play log
        List<Ficha<T, P>>[] rounds;
        
        //scalar that defines the direction of the game
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

                foreach (Ficha<T, P> ficha in player.Collection)
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

        /// <summary>
        /// Returns a new instance of Board
        /// </summary>
        /// <param name="Players"></param>
        /// <param name="GenerateFichas"> how to generate the fichas for the game </param>
        /// <param name="initialDistribute"> how to distribute the fichas at the beginning </param>
        /// <param name="initialHand"> amount of fichas at the start </param>
        /// <param name="pass"> direction of the game </param>
        /// <returns></returns>
        public static Board<T, P> StartGame(IEnumerable<DominoPlayer<T, P>> Players, GenerateFichas<T, P> GenerateFichas, Distribute<T, P> initialDistribute, int initialHand, PassTurn<T, P> pass)
        {

            return new Board<T, P>(null , Players, GenerateFichas, initialDistribute, initialHand, pass);

        }

        /// <summary>
        /// Add a Ficha to the Tree Collection
        /// </summary>
        /// <param name="board_side"> side you wich to play for </param>
        /// <param name="ficha"></param>
        /// <param name="index_side"> index side of the ficha that you wich to play for </param>
        /// <returns> if it was possible to play </returns>
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

            if (Players[ActualPlayer].Enabled && (Players[ActualPlayer].Collection.Count > 0 || robar))
            {

                (T, Ficha<T, P>, int) ficha;

                if (this.Collection != null)
                    ficha = Players[ActualPlayer].Play(rounds, Collection.AvailableSides());

                else
                    ficha = Players[ActualPlayer].Play(rounds, null);

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
                    {
                        rounds[ActualPlayer].Add(null);
                        
                    }
                        

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

        /// <summary>
        /// "Robar" a ficha from the non used ones at the outside
        /// </summary>
        /// <returns></returns>
        private Ficha<T, P> PickFicha()
        {

            if (FichasAfuera.Count == 0)
                return null;

            Random r = new Random();

            var f = FichasAfuera[r.Next(FichasAfuera.Count)];
            Remove(FichasAfuera, f);

            return f;

        }

        /// <summary>
        /// Designate the next player
        /// </summary>
        /// <param name="next"> scalar that defines the direction of the game </param>
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

        //Represents the value in a Int of each side
        public List<int> values { get; private set; }

        //top plays for each side
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

        /// <summary>
        /// returns the total value of the ficha
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual int TotalValue()
        {
            throw new NotImplementedException();
        }

        public override bool EqualsTo(Element<P> other)
        {

            Ficha<T,P> f = (Ficha<T,P>)other;

            List<T> f1Num = new List<T>();
            f1Num.AddRange(this.sides);

            List<T> f2Num = new List<T>();
            f2Num.AddRange(f.sides);

            for (int i = 0; i < f1Num.Count; i++)
            {
                T x = f1Num[i];

                if (f2Num.Contains(x))
                {
                    f1Num.RemoveAt(i);
                    f2Num.Remove(x);
                    i--;
                }
            }

            return f1Num.Count == 0 && f2Num.Count == 0;
           
        }

        public override P Print(PrintParameters pp)
        {
            throw new NotImplementedException();
        }
    }

    public class DominoPlayer<T, P> : Player<List<Ficha<T, P>>, P>
    {

        //how the domino player must play
        PlayFicha<T, P> Jugador;

        public DominoPlayer(PlayFicha<T, P> Jugador) : base(new List<Ficha<T, P>>())
        { this.Jugador = Jugador; }

        public (T, Ficha<T, P>, int) Play(List<Ficha<T, P>>[] Rounds, List<T> sides)
        {

            (T, Ficha<T, P>, int) ficha = Jugador(Rounds, sides, Collection);

            if (ficha.Item2 != null)
                Board<T, P>.Remove(Collection, ficha.Item2);

            return ficha;

        }

        /// <summary>
        /// returns the total value of the player´s hand
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// determine who are the winners
    /// </summary>
    /// <typeparam name="T"> type of ficha´s side </typeparam>
    /// <typeparam name="P"> return type of print </typeparam>
    /// <param name="players"></param>
    /// <returns></returns>
    public delegate List<int> Winner<T, P>(in List<DominoPlayer<T, P>> players);

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

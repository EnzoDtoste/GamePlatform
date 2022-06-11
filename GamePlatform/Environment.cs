

namespace GamePlatform
{

    /// <summary>
    /// Abstract class that defines the behavior of the Game
    /// </summary>
    /// <typeparam name="T"> Type of Board </typeparam>
    /// <typeparam name="B"> Type of Player Collection </typeparam>
    /// <typeparam name="P"> return type of Print </typeparam>
    public class Environment<T, B, BT, P> where B: Player<BT, P>
    {

        public T Collection { get; protected set; }
        public List<B> Players { get; protected set; }

        public int ActualPlayer { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Players"> Declare null if you want to create new instance </param>
        public Environment(T Collection, IEnumerable<B> Players)
        {

            this.Collection = Collection;
            
            this.Players = new List<B>();

            if (Players != null)
                this.Players.AddRange(Players);

            ActualPlayer = 0;

        }

        public void AddPlayer(BT Collection)
        { Players.Add((B)Activator.CreateInstance(typeof(B), Collection)); }

        public bool ContainsElement(EnvironmentContainsElement<P> ee, Element<P> e)
        {
            return ee(e);
        }

        public bool PlayerContainsElement(PlayerContainsElement<Player<BT, P>, P> pe, Element<P> e)
        {

            foreach (B p in this.Players)
            { 
            
                if(p.Contains(pe, e))
                    return true;
            
            }

            return false;

        }

        public P Print(IEnvironmentPrint<T, B, BT, P> print, PrintParameters pp)
        {
            return print.Print(this, pp);
        }
        public P Print(IEnvironmentPrint<T, B, BT, P> print, P Background, PrintParameters pp)
        {
            return print.Print(this, Background, pp);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> Type of Collection </typeparam>
    /// <typeparam name="P"> return type of Print </typeparam>
    public class Player<T, P>
    {

        public T Collection;
        public bool Enabled = true;

        public Player(T Collection)
        {
            this.Collection = Collection;
        }

        public void RemoveELement(RemoveElementFromCollection<T,P> ir, Element<P> e)
        { 
           ir(Collection, e);
        }
        public bool Contains(PlayerContainsElement<Player<T, P>, P> ie, Element<P> e)
        { return ie(this, e); }

        public P Print(IPlayerPrint<T, P> print, PrintParameters pp)
        { return print.Print(Collection, pp); }

    }

    public abstract class Element<P>
    {

        public abstract bool EqualsTo(Element<P> other);
        public abstract P Print(PrintParameters pp);

    }

    public abstract class PrintParameters
    { }

    #region Interfaces

    public interface IPlayerPrint<T, P>
    {
        P Print(T Collection, PrintParameters pp);
    }

    public interface IEnvironmentPrint<T, B, BT, P> where B: Player<BT, P>
    {
        P Print(in Environment<T, B, BT, P> environment, PrintParameters pp);
        P Print(in Environment<T, B, BT, P> environment, P Background, PrintParameters pp);
    }

    #endregion

    #region Delegates

    public delegate bool EnvironmentContainsElement<T>(Element<T> e);

    public delegate bool PlayerContainsElement<T, P>(T player, Element<P> e);

    public delegate void RemoveElementFromCollection<T, P>(T Collection, Element<P> e);

    #endregion

}
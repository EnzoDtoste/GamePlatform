

namespace GamePlatform
{

    /// <summary>
    /// Abstract class that defines the behavior of the Game
    /// </summary>
    /// <typeparam name="T"> Type of Board </typeparam>
    /// <typeparam name="B"> Type of Player </typeparam>
    /// <typeparam name="BT"> Type of Player Collection </typeparam>
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


}
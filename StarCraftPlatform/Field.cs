using GamePlatform;

namespace StarCraftPlatform
{

    public class Field<P> : Environment<Element<P>[,], StarPlayer<P>, Article<P>[], P>
    {

        public List<(int, int)> PlayersPositions { get; private set; }

        public Field(Element<P>[,] Collection, IEnumerable<StarPlayer<P>> Players) : base(Collection, Players)
        {
            
            PlayersPositions = new List<(int, int)>();

            for (int i = 0; i < Players.Count(); i++)
                PlayersPositions.Add((2, 2));
        
        }

        public bool NextPlayer()
        {

            for (int i = 0; i < Players.Count; i++)
            {
                
                Next();

                if (!Players[ActualPlayer].IsDead)
                    return true;

            }

            return false;

        }

        private void Next()
        {

            if (ActualPlayer == Players.Count - 1)
                ActualPlayer = 0;

            else
                ActualPlayer++;

        }

        public void Attack(PrintParameters pp)
        {

            if (Players[ActualPlayer].ActualWeapon != null && Players[ActualPlayer].Energy >= Players[ActualPlayer].ActualWeapon.EnergyWasted)
            {

                for (int i = 0; i < Players.Count; i++)
                {

                    if (i != ActualPlayer)
                    {

                        if (Math.Sqrt(Math.Pow(PlayersPositions[ActualPlayer].Item1 - PlayersPositions[i].Item1, 2) + Math.Pow(PlayersPositions[ActualPlayer].Item2 - PlayersPositions[i].Item2, 2)) <= ((StarPlayer<P>)Players[ActualPlayer]).ActualWeapon.Ratio)
                        {

                            Players[i].ReceiveDamage(Players[ActualPlayer].ActualWeapon.Strength);

                        }

                    }

                }

                Players[ActualPlayer].ReduceEnergy(Players[ActualPlayer].ActualWeapon.EnergyWasted);

                Players[ActualPlayer].ActualWeapon.attackEffect(Players[ActualPlayer].ActualWeapon.Ratio, pp);

            }

        }

        public bool MovePlayer((int , int) dir, int index)
        {

            if(PlayersPositions[index].Item1 + dir.Item1 >= 0 && PlayersPositions[index].Item1 + dir.Item1 < Collection.GetLength(0) && PlayersPositions[index].Item2 + dir.Item2 >= 0 && PlayersPositions[index].Item2 + dir.Item2 < Collection.GetLength(1))
            {
            
                PlayersPositions[index] = (PlayersPositions[index].Item1 + dir.Item1, PlayersPositions[index].Item2);
                PlayersPositions[index] = (PlayersPositions[index].Item1, PlayersPositions[index].Item2 + dir.Item2);

                for (int i = 0; i <= 4; i++)
                {

                    for (int j = 0; j <= 4; j++)
                    {

                        if (PlayersPositions[index].Item1 - (2 - i) >= 0 && PlayersPositions[index].Item1 - (2 - i) < Collection.GetLength(0) && PlayersPositions[index].Item2 - (2 - j) >= 0 && PlayersPositions[index].Item2 - (2 - j) < Collection.GetLength(1) && Collection[PlayersPositions[index].Item1 - (2 - i), PlayersPositions[index].Item2 - (2 - j)] != null && Collection[PlayersPositions[index].Item1 - (2 - i), PlayersPositions[index].Item2 - (2 - j)] is Article<P>)
                        {

                            if (Players[index].AddArticle((Article<P>)Collection[PlayersPositions[index].Item1 - (2 - i), PlayersPositions[index].Item2 - (2 - j)]))
                                Collection[PlayersPositions[index].Item1 - (2 - i), PlayersPositions[index].Item2 - (2 - j)] = null;

                        }

                    }

                }

                return true;
            }

            return false;

        }

    }

    public class StarPlayer<P> : Player<Article<P>[], P>
    {

        public string Name { get; protected set; }

        int topLife;
        int topEnergy;

        bool dead = false;

        public P print { get; protected set; }

        public int Life { get; protected set; }
        public int Energy { get; protected set; }
        public int CountArticles { 
            
            get
            {

                int count = 0;

                for(int i = 0; i < Collection.Length; i++)
                {
                    if (Collection[i] != null)
                        count++;
                }

                return count;

            }
            
            }

        public bool IsDead { get { return dead; } protected set { dead = value; } }
        
        public Weapon<P> ActualWeapon { get; protected set; }

        public StarPlayer(string name, Article<P>[] Collection, int life, int energy, P print) : base(Collection)
        {
            Name = name;
            topLife = life;
            topEnergy = energy;
            Life = life;
            Energy = energy;
            this.print = print;
        }

        public bool AddArticle(Article<P> article)
        {
           
            for (int i = 0; i < Collection.Length; i++)
            {
                if (Collection[i] == null)
                { Collection[i] = article; return true; }
            }

            return false;

        }

        public bool ApplyArticle(int index)
        {

            if (index < Collection.Length && Collection[index] != null)
            {

                if (Collection[index] is Potion<P>)
                {

                    Potion<P> potion = (Potion<P>)Collection[index];
                    RemoveELement(Remove(), potion);

                    return ApplyPotion(potion);

                }

                if(Life == 0)
                    dead = true;

                if (Collection[index] is Weapon<P>)
                    return ChangeWeapon((Weapon<P>)Collection[index]);

            }

            return false;

        }

        bool ApplyPotion(Potion<P> potion)
        {

            if (potion is ILife)
            { 
                Life = Top(topLife, Life, ((ILife)potion).Apply());
                
                if(Life == 0)
                    dead = true;

            }

            if (potion is IEnergy)
            { Energy = Top(topEnergy, Energy, ((IEnergy)potion).Apply()); }

            return true;

        }

        public bool ChangeWeapon(Weapon<P> weapon)
        {

            ActualWeapon = weapon;
            return true;

        }

        public void ReceiveDamage(int amount)
        {
            Life -= amount;
         
            if (Life <= 0)
            { Life = 0; dead = true; }
        
        }

        public void ReduceEnergy(int amount)
        {
            Energy = Top(topEnergy, Energy, -amount);
        }

        private int Top(int top, int actual, int amount)
        {

            if (actual + amount < 0)
                return 0;

            if (actual + amount > top)
                return top;

            return actual + amount;

        }

        public RemoveElementFromCollection<Article<P>[], P> Remove()
        {

            return (x, y) => {

                bool found = false;

                for (int i = 0; i < x.Length - 1; i++)
                {

                    if(!found && x[i].EqualsTo(y))
                        found = true;

                    if (found)
                        Collection[i] = Collection[i + 1];

                }

                Collection[Collection.Length - 1] = null;

            };

        }

    }

    public abstract class Article<P> : Element<P>
    {
        public string Name { get; protected set; }
        protected P print;
        public Func<P, string, string, PrintParameters, P> FuncPrint;

        public Article(string name, P print, Func<P, string, string, PrintParameters, P> funcPrint)
        {
            this.Name = name;
            this.print = print;
            this.FuncPrint = funcPrint;
        }

        public override bool EqualsTo(Element<P> other)
        {
            Article<P> a = (Article<P>)other;
            return this.Name == a.Name;
        }

    }

    public class Weapon<P> : Article<P>
    {

        public int Strength { get; protected set; }
        public int Ratio { get; protected set; }

        public int EnergyWasted { get; protected set; }

        internal AttackEffect attackEffect;

        public Weapon(string name, int strength, int ratio, int energyWasted, P print, Func<P, string, string, PrintParameters, P> funcPrint, AttackEffect attackEffect): base(name, print, funcPrint)
        {
            Strength = strength;
            Ratio = ratio;
            EnergyWasted = energyWasted;
            this.attackEffect = attackEffect;
        }

        public override P Print(PrintParameters pp)
        {
            return FuncPrint(print, Name, Strength + " " + Ratio + " " + EnergyWasted, pp);
        }

    }

    public abstract class Potion<P>: Article<P>
    {
        protected Potion(string name, int power, P print, Func<P, string, string, PrintParameters, P> funcPrint) : base(name, print, funcPrint)
        {
            Power = power;
        }

        public int Power { get; protected set; }
    }

    public class LifePotion<P> : Potion<P>, ILife
    {

        public LifePotion(string name, int power, P print, Func<P, string, string, PrintParameters, P> funcPrint) : base(name, power, print, funcPrint)
        { Power = power; }

        public int Apply()
        {
            return Power;
        }

        public override P Print(PrintParameters pp)
        {
            return FuncPrint(print, Name, "Life: " + Power, pp);
        }

    }

    public class EnergyPotion<P> : Potion<P>, IEnergy
    {

        public EnergyPotion(string name, int power, P print, Func<P, string, string, PrintParameters, P> funcPrint) : base(name, power, print, funcPrint)
        { Power = power; }

        public int Apply()
        {
            return Power;
        }

        public override P Print(PrintParameters pp)
        {
            return FuncPrint(print, Name, "Energy: " + Power, pp);
        }

    }

    public class DoublePotion<P> : Potion<P>, ILife, IEnergy
    {
        public DoublePotion(string name, int power, P print, Func<P, string, string, PrintParameters, P> funcPrint) : base(name, power, print, funcPrint)
        {
        }

        int ILife.Apply()
        {
            return Power;
        }

        int IEnergy.Apply()
        {
            return Power;
        }

        public override P Print(PrintParameters pp)
        {
            return FuncPrint(print, Name, "Life & Energy: " + Power, pp);
        }

    }

    public interface ILife
    {
        public int Apply();
    }
    public interface IEnergy
    {
        public int Apply();
    }

    public interface IFieldPrint<P>: IEnvironmentPrint<Element<P>[,], StarPlayer<P>, Article<P>[], P>
    { }
    public interface IStarPlayerPrint<P>: IPlayerPrint<Article<P>[], P>
    { }

    public delegate void AttackEffect(int ratio, PrintParameters pp);

}

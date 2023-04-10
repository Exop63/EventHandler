using System.Reflection;

namespace MyApp // Note: actual namespace depends on the project name.
{

    internal class Program
    {

        static void Main(string[] args)
        {
            var myGame = new Game();
            myGame.Invoke(new PlayerMoveEvent());
            myGame.Invoke(new PlayerDeadEvent());
        }

    }

    #region EventHandler
    public class MyArgs { }
    public class EventHandler
    {
        public delegate void MyEvent(MyArgs args);
        public Dictionary<Type, MyEvent> map = new Dictionary<Type, MyEvent>();

        public void Add(Type type, MyEvent value) { map.Add(type, value); }

        public void Remove(Type type) { map.Remove(type); }

        public void Invoke(MyArgs myEvent)
        {
            if (map.TryGetValue(myEvent.GetType(), out var ev))
            {
                ev.Invoke(myEvent);
            }
        }
    }
    #endregion
    #region GameEvents
    public class PlayerMoveEvent : MyArgs { }

    public class PlayerDeadEvent : MyArgs { }
    #endregion

    #region Game
    public class Game : EventHandler
    {
        public event MyEvent PlayerMove { add => Add(typeof(PlayerMoveEvent), value); remove => Remove(typeof(PlayerMoveEvent)); }
        public event MyEvent PlayerDead { add => Add(typeof(PlayerDeadEvent), value); remove => Remove(typeof(PlayerDeadEvent)); }

        public Game()
        {
            Console.WriteLine("Game Started!");
            PlayerMove += GameEvents_PlayerMove;
            PlayerDead += GameEvents_PlayerDead;
        }

        private void GameEvents_PlayerDead(MyArgs args)
        {
            Console.WriteLine("GameEvents_PlayerDead invoked.");
        }

        private void GameEvents_PlayerMove(MyArgs args)
        {
            Console.WriteLine("GameEvents_PlayerMove invoked.");
        }
    }
    #endregion
}

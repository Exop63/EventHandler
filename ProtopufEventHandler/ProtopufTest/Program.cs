using ProtoBuf;
using ProtopufTest;
using System.Reflection;

namespace MyApp // Note: actual namespace depends on the project name.
{

    internal class Program
    {

        static void Main(string[] args)
        {
            var myGame = new Game();

            // serilization on client
            var data1 = Magic.Serialize(new PlayerMoveEvent());
            var data2 = Magic.Serialize(new PlayerDeadEvent());
            // thing this byte array sended on the network

            // deserializing on the server
            var ev1 = Magic.Deserialize<MyArgs>(data1);   
            var ev2 = Magic.Deserialize<MyArgs>(data2);
            myGame.Invoke(ev1);
            myGame.Invoke(ev2);
        }

    }

    #region EventHandler
    [ProtoContract]
    [ProtoInclude(1,typeof(PlayerMoveEvent))]
    [ProtoInclude(2,typeof(PlayerDeadEvent))]

    public class MyArgs { }
    public class EventHandler
    {
        // delegateler c# da bir metodu temsil eder js deki Prototype benzeri bir yapı.
        public delegate void MyEvent(MyArgs args);

        // olabilecek tüm eventlerin Class tibine göre barındığı yer.
        public Dictionary<Type, MyEvent> map = new Dictionary<Type, MyEvent>();
        // map'e eleman ekler
        public void Add(Type type, MyEvent value) { map.Add(type, value); }
        // map'den eleman çıkarır.

        public void Remove(Type type) { map.Remove(type); }
        // bir eventin tetiklenmesini sağlar. Eğer bu event map'de varsa :D
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
    [ProtoContract]
    // Oyuncunun hareket ettiğini temsil eden event
    public class PlayerMoveEvent : MyArgs { }
    [ProtoContract]
    // Oyuncunun öldüğünü temsil eden event
    public class PlayerDeadEvent : MyArgs { }
    #endregion

    #region Game
    public class Game : EventHandler
    {
        // eventler bir delegete tetiklendiğinde bir method çağırmaya yarar.
        // eğer Game.Invoke()'e tipi PlayerMoveEvent olan bir argüman verilerek çağrı yapılırsa PlayerMove event'i tetiklenecektir.
        // buda PlayerMove eventine += ile bağlanmış olan tüm methodları çağırır. GameEvents_PlayerMove()
        public event MyEvent PlayerMove { add => Add(typeof(PlayerMoveEvent), value); remove => Remove(typeof(PlayerMoveEvent)); }
        public event MyEvent PlayerDead { add => Add(typeof(PlayerDeadEvent), value); remove => Remove(typeof(PlayerDeadEvent)); }

        public Game()
        {
            Console.WriteLine("Game Started!");
            // PlayerMove  eventi tetiklendiğinde çağırılacak method
            PlayerMove += GameEvents_PlayerMove;
            // PlayerDead  eventi tetiklendiğinde çağırılacak method
            PlayerDead += GameEvents_PlayerDead;
        }
        // oyuncunun öldüğünde yapılacak işlemler.
        private void GameEvents_PlayerDead(MyArgs args)
        {
            Console.WriteLine("GameEvents_PlayerDead invoked.");
        }
        // oyuncunun hareket ettiğinde yapılacak işlemler.

        private void GameEvents_PlayerMove(MyArgs args)
        {
            Console.WriteLine("GameEvents_PlayerMove invoked.");
        }
    }
    #endregion
}

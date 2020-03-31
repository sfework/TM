using NewLife.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.PagesModel
{
    public class RedisManage : LayoutComponent
    {
        private Redis DB { get; set; }
        public bool Connected { get; set; }
        public DataModel DataBase { get; set; } = new DataModel();
        public KeyModel Keys { get; set; } = new KeyModel();
        public Dictionary<string, string> ConnectStrs { get; set; } = new Dictionary<string, string>();


        protected override void Initialization()
        {
            ConnectStrs = new Dictionary<string, string>
            {
                { "text", "127.0.0.1:6379" },
                { "text1", "127.0.0.1:6380" }
            };
        }


        public void Connect(string ConnectStr)
        {
            DB = Redis.Create(ConnectStr, 0);
            Connected = DB.Pool.Get().Client.Connected;
            Refresh_Data();
        }
        public void CloseConnect()
        {
            DB.Dispose();
            Connected = DB.Pool.Get().Client.Connected;
            DataBase = new DataModel();
            Keys = new KeyModel();
        }
        private void Refresh_Data()
        {
            try
            {
                DataBase = new DataModel();
                var Result = DB.Pool.Get().Execute<NewLife.Data.Packet[]>("config", "get", "databases");
                var DBCount = int.Parse(Result[1].ToStr());
                DataBase.Count = DBCount;
                DataBase.Current = 0;
                for (int i = 0; i < DBCount; i++)
                {
                    DataBase.List.Add(i, i == 0);
                }
                Refresh_Keys(1);
            }
            catch (Exception)
            {

            }
        }
        public void Select_Data(int DataIndex)
        {
            if (DataBase.Current != DataIndex)
            {
                DataBase.List[DataBase.Current] = false;
                DataBase.List[DataIndex] = true;
                DataBase.Current = DataIndex;
                DB.Db = DataIndex;
                DB.Pool.Get().Select(DataIndex);
                Refresh_Keys(1);
            }
        }


        private void Refresh_Keys(int PageNum)
        {
            Keys = new KeyModel
            {
                PageNum = PageNum,
                Count = DB.Keys.Count()
            };
            foreach (var item in DB.Keys.Skip((Keys.PageNum - 1) * 25).Take(25))
            {
                Keys.List.Add(item, false);
            }
        }

        public void Select_Key(string Key)
        {
            if (Keys.Current != Key)
            {
                if (!string.IsNullOrWhiteSpace(Keys.Current))
                {
                    Keys.List[Keys.Current] = false;
                }
                Keys.List[Key] = true;
                Keys.Current = Key;
            }
        }



        public class DataModel
        {
            public int Current { get; set; }
            public Dictionary<int, bool> List { get; set; } = new Dictionary<int, bool>();
            public int Count { get; set; }
        }
        public class KeyModel
        {
            public int Count { get; set; }
            public int PageNum { get; set; } = 1;
            public string Current { get; set; }
            public Dictionary<string, bool> List { get; set; } = new Dictionary<string, bool>();
        }
    }
}

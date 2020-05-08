using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TestData
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Test> ts = new List<Test>
            {
 new Test { 评价人="宋四玉",评价人编码=20140,评价人部门="英国运营组",评价人部门编码=2096,被评价人="丁明明",被评价人编码=20032,被评价人部门="英国运营组",被评价人部门编码=2096,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=4.5M,合=5,筑堤坝=4.5M,抡大锤=5,带队伍=5,思经营=5,均分=4.88M ,评价=@"1:处事灵活,善于沟通协调,面对难沟通问题比较冷静
2:善于思考,处事果断,逻辑思维能力强
3:有整体和大局意识
4:对组内工作能较快做有全面布局
5:组组内成员关心,注重个人发展和指导
"  },
 new Test { 评价人="周徽",评价人编码=20310,评价人部门="第三方运营组",评价人部门编码=2030,被评价人="贾黎建",被评价人编码=20073,被评价人部门="产品6组",被评价人部门编码=1015,维度=0,考评日期="2020-4-20",勤=4.5M,诚=4.5M,精=4.5M,合=5,筑堤坝=4.5M,抡大锤=4.5M,带队伍=4,思经营=5,均分=4.56M,评价=@"1.做事认真负责，能很好地帮助推动问题解决
2.产品方面专业能力强，思路清晰"},
 new Test {评价人="周徽",评价人编码=20310,评价人部门="第三方运营组",评价人部门编码=2030,被评价人="王飞龙",被评价人编码=10057,被评价人部门="3D设计",被评价人部门编码=5040,维度=0,考评日期="2020-4-20",勤=4.5M,诚=4.5M,精=5,合=4.5M,筑堤坝=4,抡大锤=4.5M,带队伍=4.5M,思经营=5,均分=4.56M,评价=@"1.关注事件进展，能及时根据变化而做出调整
2.做事有条不紊，实事求是"},
 new Test { 评价人="黄瑜瑾",评价人编码=60961,评价人部门="运营5组",评价人部门编码=2014,被评价人="宋慧芳",被评价人编码=20139,被评价人部门="运营4组",被评价人部门编码=2013,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=5,合=5,筑堤坝=5,抡大锤=4.5M,带队伍=5,思经营=5,均分=4.94M,评价=@"1.将组内流程一点一滴的规范化，形成链条；2.考虑问题十分全面，做决定勇于决断，值得我学习；3.将工作经验倾囊相授，认真规划组内成员发展。"},
 new Test { 评价人="李晓净",评价人编码=20096,评价人部门="VC组",评价人部门编码=2050,被评价人="卜朦朦",被评价人编码=20006,被评价人部门="运营2组",被评价人部门编码=2011,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=5,合=5,筑堤坝=4.5M,抡大锤=4.5M,带队伍=5,思经营=4.5M,均分=4.81M,评价=@"行动力：完善组内利润汇总分析，毛利监控汇总和月完成进度复盘，组内把控更有力"},
 new Test { 评价人="徐玉青",评价人编码=20348,评价人部门="运营1组",评价人部门编码=2010,被评价人="张金蕊",被评价人编码=20217,被评价人部门="运营3组",被评价人部门编码=2012,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=5,合=5,筑堤坝=5,抡大锤=5,带队伍=5,思经营=4.5M,均分=4.94M,评价=@"最认可：
工作细致认真；热情度高

最需要改进、提升：
希望多提升组员之间的沟通分享"},
 new Test { 评价人="孙越",评价人编码=818139,评价人部门="视觉设计",评价人部门编码=5010,被评价人="王江萍",被评价人编码=818080,被评价人部门="视觉设计",被评价人部门编码=5010,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=4.5M,合=4.5M,筑堤坝=5,抡大锤=5,带队伍=5,思经营=5,均分=4.88M ,评价=@"新品数量激增的时候，反馈过后能及时分配出去，帮助及时消化，避免到港后没有图片"},
 new Test { 评价人="藤田",评价人编码=818200,评价人部门="日本分公司",评价人部门编码=84,被评价人="蔡海伟",被评价人编码=20007,被评价人部门="品管组",被评价人部门编码=3030,维度=0,考评日期="2020-4-17",勤=5,诚=5,精=4.5M,合=5,筑堤坝=4.5M,抡大锤=5,带队伍=5,思经营=4.5M,均分=4.81M  ,评价=@"海伟总体来看做事很快"},
 new Test { 评价人="王晓青",评价人编码=818083,评价人部门="视觉设计",评价人部门编码=5010,被评价人="王江萍",被评价人编码=818080,被评价人部门="视觉设计",被评价人部门编码=5010,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=4.5M,合=5,筑堤坝=4.5M,抡大锤=5,带队伍=5,思经营=5,均分=4.88M  ,评价=@"及时的给予帮助，带领组员提升，能够较好的经营团队"},
 new Test { 评价人="王晓婷",评价人编码=20305,评价人部门="英语客服组",评价人部门编码=4030,被评价人="陈小为",被评价人编码=818186,被评价人部门="英语客服组",被评价人部门编码=4030,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=4.5M,合=5,筑堤坝=5,抡大锤=5,带队伍=4.5M,思经营=5,均分=4.88M  ,评价=@"积极发现问题解决问题，关心团队成员发展"},
 new Test { 评价人="杨景景",评价人编码=20198,评价人部门="第三方运营组",评价人部门编码=2030,被评价人="张金蕊",被评价人编码=20217,被评价人部门="运营3组",被评价人部门编码=2012,维度=1,考评日期="2020-4-20",勤=5,诚=5,精=5,合=5,筑堤坝=5,抡大锤=5,带队伍=4.5M,思经营=4.5M,均分=4.88M  ,评价=@"1.专业能力很强，对各项目的掌握比较细致和全面；可以给下级在工作上提供很大的帮助；管理能力较强；2.各方面都比较优秀，暂没发现有需要改进的地方"},
 new Test { 评价人="FIONA LU",评价人编码=818172,评价人部门="美国分公司",评价人部门编码=82,被评价人="蔡海伟",被评价人编码=20007,被评价人部门="品管组",被评价人部门编码=3030,维度=0,考评日期="2020-4-17",勤=4,诚=4,精=4,合=5,筑堤坝=4,抡大锤=4,带队伍=4,思经营=4,均分=4.13M  ,评价=@"交流无障碍，能够及时回复美国仓库提出的问题和帮助"},
 new Test { 评价人="程磊",评价人编码=10006,评价人部门="设计部",评价人部门编码=50,被评价人="陈飞",被评价人编码=10003,被评价人部门="设计4组",被评价人部门编码=502040,维度=-1,考评日期="2020-4-21",勤=4.5M,诚=5,精=5,合=5,筑堤坝=4.5M,抡大锤=5,带队伍=5,思经营=5,均分=4.88M ,评价=@"处事成熟稳重，具有解决重大问题的能力"}
       };
            SQL sql = new SQL("Data Source=47.93.124.145,49468;Initial Catalog=EYA;Persist Security Info=True;User ID=sa;Password=ZiEl0PLMo1knTh9;Enlist=true;Pooling=true;Max Pool Size=30000;Min Pool Size=0;Connection Lifetime=300;packet size=30000");
            int index = 0;
            sql.RunTransaction(() => {
                foreach (var item in ts)
                {
                    index++;
                    var cmd1 = new SQLCmd("INSERT INTO Eya_360PersonRelationship VALUES (@AID,@UserCode,@UserName,@AssessTargetCode,@AssessTargetName,@TargetType,@Template,@DeptName,@DeptCode,@IsAssess)");
                    cmd1.Params.Add("AID", "artificial_" + index);
                    cmd1.Params.Add("UserCode", item.评价人编码);
                    cmd1.Params.Add("UserName", item.评价人);
                    cmd1.Params.Add("AssessTargetCode", item.被评价人编码);
                    cmd1.Params.Add("AssessTargetName", item.被评价人);
                    cmd1.Params.Add("TargetType", item.维度);
                    cmd1.Params.Add("Template", 2);
                    cmd1.Params.Add("DeptName", item.评价人部门);
                    cmd1.Params.Add("DeptCode", item.评价人部门编码);
                    cmd1.Params.Add("IsAssess", 1);
                    sql.Execute(cmd1);
                    var cmd2 = new SQLCmd("INSERT INTO Eya_360TestRecord VALUES(@AID,@Diligence,@Sincerity,@Spirit,@Cooperation,@DamConstruction,@SwingingHammer,@LeadingTerms,@ThinkingManagement,@UserCode,@TargetUserCode,@TargetType,@CreateTime,@ScoreAverage,@JsonInfo,@Template,@IsValid,@GiveUpMessage,@UpdateTime)");
                    cmd2.Params.Add("AID", "artificial_" + index);
                    cmd2.Params.Add("Diligence", item.勤);
                    cmd2.Params.Add("Sincerity", item.诚);
                    cmd2.Params.Add("Spirit", item.精);
                    cmd2.Params.Add("Cooperation", item.合);
                    cmd2.Params.Add("DamConstruction", item.筑堤坝);
                    cmd2.Params.Add("SwingingHammer", item.抡大锤);
                    cmd2.Params.Add("LeadingTerms", item.带队伍);
                    cmd2.Params.Add("ThinkingManagement", item.思经营);
                    cmd2.Params.Add("UserCode", item.评价人编码);
                    cmd2.Params.Add("TargetUserCode", item.被评价人编码);
                    cmd2.Params.Add("TargetType", item.维度);
                    cmd2.Params.Add("CreateTime", DateTime.Now);
                    cmd2.Params.Add("ScoreAverage", item.均分);
                    cmd2.Params.Add("JsonInfo", "[{\"Question\":\"您对被评价人最认可的地方有哪些？||(What areas do you think this person excels at?)\",\"Answer\":\"" + item.评价 + "\"},{\"Question\":\"您认为被评价人最需要改进、提升的地方有哪些？||(What do you think are the most important areas for improvement?)\",\"Answer\":\"无\"}]");
                    cmd2.Params.Add("Template", 2);
                    cmd2.Params.Add("IsValid", 1);
                    cmd2.Params.Add("GiveUpMessage", string.Empty);
                    cmd2.Params.Add("UpdateTime", DateTime.Now);
                    sql.Execute(cmd2);
                    Console.WriteLine(item.评价人);
                }
            });
            Console.ReadKey();
        }
    }

    public class Test
    {
        public string 评价人 { get; set; }
        public int 评价人编码 { get; set; }
        public string 评价人部门 { get; set; }
        public int 评价人部门编码 { get; set; }
        public string 被评价人 { get; set; }
        public int 被评价人编码 { get; set; }
        public string 被评价人部门 { get; set; }
        public int 被评价人部门编码 { get; set; }
        public int 维度 { get; set; }
        public string 考评日期 { get; set; }
        public decimal 勤 { get; set; }
        public decimal 诚 { get; set; }
        public decimal 精 { get; set; }
        public decimal 合 { get; set; }
        public decimal 筑堤坝 { get; set; }
        public decimal 抡大锤 { get; set; }
        public decimal 带队伍 { get; set; }
        public decimal 思经营 { get; set; }
        public decimal 均分 { get; set; }
        public string 评价 { get; set; }
    }
    public class SQLMessage : SQLCmd
    {
        public bool Success => string.IsNullOrWhiteSpace(Message);
        public string Message { get; set; }
    }

    public class SQLCmd
    {
        public SQLCmd()
        {

        }
        public SQLCmd(string SqlStr)
        {
            Text = SqlStr;
        }
        public SQLCmd(string SqlStr, params (string, object)[] SqlParams)
        {
            Text = SqlStr;
            foreach (var item in SqlParams)
            {
                Params.Add(item.Item1, item.Item2);
            }
        }
        public CommandType CommandType { get; set; } = CommandType.Text;
        public IDictionary<string, object> Params { get; set; } = new Dictionary<string, object>();
        public string Text { get; set; }
        public new string ToString()
        {
            var SQLText = Text;
            foreach (var item in Params)
            {
                var TP = item.Value.GetType();
                var Value = item.Value;
                if (TP == typeof(string))
                {
                    Value = "'" + Value.ToString() + "'";
                }
                else if (TP == typeof(DateTime))
                {
                    Value = "'" + Value.ToString() + "'";
                }
                else if (TP == typeof(bool))
                {
                    Value = (bool)Value ? 1 : 0;
                }
                else if (TP.IsEnum)
                {
                    Value = (int)Value;
                }
                SQLText = SQLText.Replace("@" + item.Key, Value.ToString());
            }
            return SQLText;
        }
    }
    public class SQLPageCmd : SQLCmd
    {
        public string OrderBy { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }
    public class SQL
    {
        private readonly SqlConnection Conn;
        private SqlTransaction Transaction;

        public SQL(string ConnectionStr)
        {
            Conn = new SqlConnection(ConnectionStr);
        }
        public void RunTransaction(Action Run)
        {
            try
            {
                Open();
                Transaction = Conn.BeginTransaction();
                Run();
                Transaction.Commit();
            }
            catch (Exception Ex)
            {
                Transaction.Rollback();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
                Close();
            }
        }
        public int Execute(SQLCmd Cmd)
        {
            SQLMessage Message = new SQLMessage
            {
                Text = Cmd.Text,
                CommandType = Cmd.CommandType,
                Params = Cmd.Params
            };
            try
            {
                Open();
                using SqlCommand Command = Create(Cmd);
                if (Transaction != null)
                {
                    Command.Transaction = Transaction;
                }
                var Result = Command.ExecuteNonQuery();
                CallBack?.Invoke(Message);
                return Result;
            }
            catch (Exception Ex)
            {
                Message.Message = Ex.Message;
                CallBack?.Invoke(Message);
                if (Transaction != null)
                {
                    throw Ex;
                }
                return default;
            }
            finally
            {
                Close();
            }
        }
        public T Query<T>(SQLPageCmd Cmd)
        {
            SQLMessage Message = new SQLMessage
            {
                Text = Cmd.Text,
                CommandType = Cmd.CommandType,
                Params = Cmd.Params
            };
            var Result = default(T);
            try
            {
                var TP = typeof(T);
                Open();
                string TotalCountSql = string.Format("select count(1) c from ({0}) t", Cmd.ToString());
                int TotalCount = Query<int>(new SQLCmd(TotalCountSql));
                string PageSql = string.Format(@"{0} {1} offset {2} rows fetch next {3} rows only;", Cmd.ToString(), Cmd.OrderBy, (Cmd.Page - 1) * Cmd.PageSize, Cmd.PageSize);
                Result = Query<T>(new SQLCmd(PageSql));
                CallBack?.Invoke(Message);
                return Result;
            }
            catch (Exception Ex)
            {
                Message.Message = Ex.Message;
                CallBack?.Invoke(Message);
                return Result;
            }
            finally
            {
                Close();
            }
        }
        public T Query<T>(SQLCmd Cmd)
        {
            SQLMessage Message = new SQLMessage
            {
                Text = Cmd.Text,
                CommandType = Cmd.CommandType,
                Params = Cmd.Params
            };
            var Result = default(T);
            try
            {
                var TP = typeof(T);
                Open();
                using SqlCommand Command = Create(Cmd);
                using var Reader = Command.ExecuteReader();
                if (IsList(TP))
                {
                    var FieldCache = new Dictionary<string, int>();
                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        FieldCache.Add(Reader.GetName(i).Trim(), i);
                    }
                    var IResult = Activator.CreateInstance(TP) as System.Collections.IList;
                    TP = TP.GenericTypeArguments.FirstOrDefault();
                    while (Reader.Read())
                    {
                        IResult.Add(QueryRow(Reader, TP, FieldCache));
                    }
                    Result = (T)IResult;
                }
                else
                {
                    if (Reader.Read())
                    {
                        return (T)QueryRow(Reader, TP);
                    }
                }
                CallBack?.Invoke(Message);
                return Result;
            }
            catch (Exception Ex)
            {
                Message.Message = Ex.Message;
                CallBack?.Invoke(Message);
                return Result;
            }
            finally
            {
                Close();
            }
        }
        private object QueryRow(SqlDataReader Reader, Type TP, Dictionary<string, int> FieldCache = null)
        {
            object Result = null;
            if (IsPrimitive(TP))
            {
                Result = ConvertValue(Reader.GetValue(0), TP);
            }
            else if (!IsClass(TP) && IsTuple(TP))
            {
                int Index = 0;
                var ResultRef = Activator.CreateInstance(TP);
                foreach (var item in TP.GetFields())
                {
                    var Value = Reader.GetValue(Index);
                    if (!IsNull(Value))
                    {
                        item.SetValue(ResultRef, ConvertValue(Value, item.FieldType));
                    }
                    Index++;
                }
                Result = ResultRef;
            }
            else if (IsClass(TP) && !IsTuple(TP))
            {
                foreach (var item in TP.GetProperties())
                {
                    if (FieldCache.TryGetValue(item.Name, out int Index))
                    {
                        var Value = Reader.GetValue(Index);
                        if (!IsNull(Value))
                        {
                            item.SetValue(Result, ConvertValue(Value, item.PropertyType), null);
                        }
                    }
                }
            }
            return Result;
        }

        private bool IsPrimitive(Type TP)
        {
            var Type = Nullable.GetUnderlyingType(TP);
            if (Type == null)
            {
                Type = TP;
            }
            if (Type.IsPrimitive)
            {
                return true;
            }
            if (Type == typeof(string) || Type == typeof(decimal) || Type == typeof(DateTime) || Type == typeof(char[]) || Type == typeof(byte[]) || Type.IsEnum)
            {
                return true;
            }
            return false;
        }
        private bool IsList(Type TP)
        {
            return TP.GetInterface("IList") != null;
        }
        private bool IsTuple(Type TP)
        {
            return TP.GetInterface("ITuple") != null;
        }
        private bool IsClass(Type TP)
        {
            return TP.IsClass;
        }
        private bool IsNull(object Obj)
        {
            return (Obj == null || (Obj is DBNull) || string.IsNullOrEmpty(Obj.ToString())) ? true : false;
        }
        private object ConvertValue(object Value, Type ConversionType)
        {
            if (IsNull(Value))
            {
                return null;
            }
            if (ConversionType.IsGenericType && ConversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                ConversionType = new System.ComponentModel.NullableConverter(ConversionType).UnderlyingType;
            }
            if (ConversionType.IsEnum)
            {
                return Enum.ToObject(ConversionType, Value);
            }
            return Convert.ChangeType(Value, ConversionType);
        }
        private SqlCommand Create(SQLCmd Cmd)
        {
            SqlCommand Command = new SqlCommand() { CommandText = Cmd.Text, Connection = Conn, CommandType = Cmd.CommandType };
            foreach (var item in Cmd.Params)
            {
                //Console.WriteLine(item.Key+"/"+ item.Value);
                Command.Parameters.AddWithValue("@" + item.Key, item.Value);
            }
            return Command;
        }

        private void Open()
        {
            if (Conn.State != ConnectionState.Open) Conn.Open();
        }
        private void Close()
        {
            if (Transaction == null && Conn.State == ConnectionState.Open) Conn.Close();
        }

        public Action<SQLMessage> CallBack { get; set; }
    }
    public class TData
    {
        private readonly string ConnectionString;
        public TData(string _ConnectionString)
        {
            ConnectionString = _ConnectionString;
        }
        public void Clear()
        {
            Values.Clear();
        }
        public void AddString(string FieldName, int Length)
        {
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            Values.Add((FieldName, result, true));
        }
        public void AddBool(string FieldName, bool? FixedValue = null)
        {
            if (FixedValue.HasValue)
            {
                Values.Add((FieldName, FixedValue.Value ? 1 : 0, false));
            }
            else
            {
                Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
                Values.Add((FieldName, random.Next(0, 2), false));
            }
        }
        public void AddNumber(string FieldName, int Length)
        {
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            Values.Add((FieldName, result, false));
        }
        public void AddDate(string FieldName)
        {
            DateTime Start = DateTime.Now.AddYears(-10);
            var ts = new TimeSpan(DateTime.Now.Ticks - DateTime.Now.AddYears(-10).Ticks).TotalSeconds;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            Values.Add((FieldName, Start.AddSeconds(random.NextDouble() * ts).ToString("yyyy-MM-dd HH:mm:ss"), true));
        }
        private readonly List<(string FieldName, object Value, bool IsQuotation)> Values = new List<(string FieldName, object Value, bool IsQuotation)>();

        public void BuildSQL(string TableName, int Count)
        {
            string SQL = $"INSERT INTO {TableName} () VALUES ()";
            string Fields = string.Empty;
            foreach (var item in Values)
            {

            }
        }
    }
}

using System;
using System.Data.SQLite;

await $"echo (1)";
var _conn = new SQLiteConnection();
_conn.ConnectionString = "Data Source=test.db3";
_conn.Open();
await $"echo (2)";
{
    SQLiteCommand command = _conn.CreateCommand();
    command.CommandText = "CREATE TABLE if not exists Test (id integer primary key AUTOINCREMENT, text varchar(100))";
    command.ExecuteNonQuery();
}
await $"echo (3)";
for (int i = 0; i < 10; i++)
{
    SQLiteCommand command = _conn.CreateCommand();
    command.CommandText = "INSERT INTO Test (text) VALUES (@1)";
    SQLiteParameter parameter = command.CreateParameter();
    parameter.ParameterName = "@1";
    parameter.Value = "this is " + i.ToString() + " text";
    command.Parameters.Add(parameter);
    command.ExecuteNonQuery();
}
await $"echo (4)";
{
    // 全データの取得
    SQLiteCommand command = _conn.CreateCommand();
    command.CommandText = "SELECT * FROM Test";
    var reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine(string.Format("ID = {0}, Name = {1}",
            reader.GetInt32(0),
            reader.GetString(1)
        ));
    }
}
await $"echo (5)";
//await "sqlitestudio test.db3";
{
    _conn.Close();
}
await $"echo (6)";

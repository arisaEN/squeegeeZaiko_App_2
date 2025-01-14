using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace zaiko.Database
{
    public class DatabaseManager
    {
        private readonly string connectionString;


        public DatabaseManager()
        {
            var configFile = "appsettings.json";

            // 設定ファイルが存在しない場合、エラーメッセージを表示して終了
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException($"設定ファイルが見つかりません: {configFile}");
            }

            // 設定ファイルから接続情報を読み込む
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .Build();

            // 接続情報を取得
            var host = configuration["Database:Host"];
            var port = configuration["Database:Port"];
            var username = configuration["Database:Username"];
            var password = configuration["Database:Password"];
            var database = configuration["Database:DatabaseName"];

            // 設定値が正しいかチェック
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(database))
            {
                throw new Exception("設定ファイルの内容が不完全です。全ての項目を正しく設定してください。");
            }

            // 接続文字列を構築
            connectionString = $"Host={host};" +
                               $"Port={port};" +
                               $"Username={username};" +
                               $"Password={password};" +
                               $"Database={database}";
        }



        //----------------------------------------------------------------------------------------------------------------------------------------------
        //データをアップデートする
        public void updateQuery(string query)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------
        // データを取得するメソッド //
        public DataTable ExecuteQuery(string query)
        {
            try

            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"データベース操作中にエラーが発生しました: {ex.Message}", ex);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        // パラメータ付きのExecuteQueryメソッドを追加
        //パラメータでスキージ名からスキージidを取得するSQL処理
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        // パラメータをコマンドに追加
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }

                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"データベース操作中にエラーが発生しました: {ex.Message}", ex);
            }
        }

        //---------------------------------------------------------------------------------------------------------------------
        //パラメータ化してSQL結果をそのまま返す
        public DataTable ExecuteQuery2(string query, DateTime hensuu)
        {
            // DataTableの作成
            DataTable dataTable = new DataTable();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        // プレースホルダにパラメータを設定
                        command.Parameters.AddWithValue("@Today", hensuu);

                        // クエリの実行
                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            // DataTableにクエリ結果を格納
                            adapter.Fill(dataTable);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"データベース操作中にエラーが発生しました: {ex.Message}", ex);
            }
            return dataTable;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //SQLそのまま実行
        public DataTable ExecuteQuery3(string query)
        {
            // DataTableの作成
            DataTable dataTable = new DataTable();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {


                        // クエリの実行
                        using (var adapter = new NpgsqlDataAdapter())
                        {
                            // DataTableにクエリ結果を格納
                            adapter.SelectCommand = command;
                            adapter.Fill(dataTable);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"データベース操作中にエラーが発生しました: {ex.Message}", ex);
            }
            return dataTable;
        }




        //---------------------------------------------------------------------------------------------------------------------
        //スキージ在庫登録
        public void InsertSqueegeeData(int squeegeeId, double measuredLength, DateTime todayFormBoot, int groupNum)
        {
            string query = @"
            INSERT INTO public.""T在庫調査"" (""スキージid"", ""長さ"",""フォーム起動日"",""グループ"")
            VALUES (@SqueegeeId, @MeasuredLength,@TodayFormBoot,@GroupNum);
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@SqueegeeId", squeegeeId },
                { "@MeasuredLength", measuredLength },
                { "@TodayFormBoot", todayFormBoot },
                { "@GroupNum", groupNum }
            };

            ExecuteNonQuery(query, parameters);
        }


        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new Npgsql.NpgsqlCommand(query, connection))
                {
                    // パラメータをコマンドに追加
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }

                    // クエリを実行
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateDatabase(int deletionId, int inventoryId)
        {
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new Npgsql.NpgsqlCommand(
                   "UPDATE T在庫調査 SET 削除フラグ = @DeletionId WHERE 在庫登録id = @InventoryId", connection))
                {
                    command.Parameters.AddWithValue("@DeletionId", deletionId);
                    command.Parameters.AddWithValue("@InventoryId", inventoryId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // エラー処理（ログ記録や例外再スロー）
                throw new Exception("データベース更新中にエラーが発生しました。", ex);
            }
        }



        public void Update_DatabaseDataGridSqueegee(string query)
        {
            
        }

        public string DatabaseConnection()
        {
            var configFile = "appsettings.json";

            // appsettings.json が存在しない場合、ユーザー入力で作成
            if (!File.Exists(configFile))
            {
                Console.WriteLine("初回実行です。接続情報を設定してください。");

                Console.Write("Host: ");
                var host = Console.ReadLine();

                Console.Write("Port: ");
                var port = Console.ReadLine();

                Console.Write("Username: ");
                var username = Console.ReadLine();

                Console.Write("Password: ");
                var password = Console.ReadLine();

                Console.Write("Database Name: ");
                var database = Console.ReadLine();

                var config = new
                {
                    Database = new
                    {
                        Host = host,
                        Port = port,
                        Username = username,
                        Password = password,
                        DatabaseName = database
                    }
                };

                // JSON形式で書き込む
                File.WriteAllText(configFile, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

                Console.WriteLine($"接続情報を {configFile} に保存しました。");
            }


            // 設定ファイルから接続情報を読み込む
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .Build();

            var hostFromConfig = configuration["Database:Host"];
            var portFromConfig = configuration["Database:Port"];
            var usernameFromConfig = configuration["Database:Username"];
            var passwordFromConfig = configuration["Database:Password"];
            var databaseFromConfig = configuration["Database:DatabaseName"];

            string connectionString = $"Host={hostFromConfig};" +
                               $"Port={portFromConfig};" +
                               $"Username={usernameFromConfig};" +
                               $"Password={passwordFromConfig};" +
                               $"Database={databaseFromConfig}";

            return connectionString;
        }


    }
}


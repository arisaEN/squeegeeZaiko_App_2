using System;
using System.Diagnostics;
using System.IO;
using Npgsql;
using zaiko.Database;


public class DatabaseInitializer
{
    //private const string ConnectionString = "Host=localhost;Port=5432;Username=arisa;Password=your_password;";
    private string ConnectionString;
    //dbマネージャーから接続文取得してこのクラスでも使う。
    private readonly DatabaseManager dbManager;


    public DatabaseInitializer() //コンストラクタ
    {
        dbManager = new DatabaseManager(); //ここで初期化処理としてDB接続文あり
        InitializeDatabase();
    }

    public void InitializeDatabase()
    {
        // データベース接続文字列
        var connectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=admin71994;"; // 適宜変更

        // 初回実行時かどうかを確認
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string firstRunMarker = Path.Combine(appDirectory, "firstRun.flag");

        if (!File.Exists(firstRunMarker))
        {   // 初回実行時の処理
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // ロール作成
                CreateRole(connectionString);

                // 対象のデータベースが無ければ作成
                CreateDatabase(connectionString);
            }

            Console.WriteLine("Database initialization completed.");


            
            Console.WriteLine("First run detected. Executing batch file...");
            //batファイルでデータベースをリストアし初期化する。
            string batchFilePath = Path.Combine(appDirectory, "postgresBackup", "postgresRes.bat");
            if (File.Exists(batchFilePath))
            {
                ExecuteBatchFile(batchFilePath);

                // 初回実行済みのフラグファイルを作成
                File.WriteAllText(firstRunMarker, "This file indicates the first run has been completed.");
            }
            else
            {
                Console.WriteLine($"Batch file not found: {batchFilePath}");
            }
        }

        
    }

    private void ExecuteBatchFile(string batchFilePath)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batchFilePath,
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };

            process.Start();
            process.WaitForExit();
            Console.WriteLine("Batch file executed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to execute batch file: {ex.Message}");
        }
    }

    //DB作成
    private void CreateDatabase(string connectionString)
    {
        try
        {
            // データベースの存在を確認するコマンド
            var checkDatabaseCommand = @"
            SELECT 1
            FROM pg_database
            WHERE datname = 'SqueegeeManagement';
            ";

            bool databaseExists;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(checkDatabaseCommand, connection))
                {
                    databaseExists = cmd.ExecuteScalar() != null;
                }
            }

            if (!databaseExists)
            {
                // データベースを作成するコマンド
                var createDatabaseCommand = @"
                    CREATE DATABASE ""SqueegeeManagement""
                    WITH
                    OWNER = arisa
                    ENCODING = 'UTF8'
                    LC_COLLATE = 'Japanese_Japan.932'
                    LC_CTYPE = 'Japanese_Japan.932'
                    TABLESPACE = pg_default
                    CONNECTION LIMIT = -1
                    IS_TEMPLATE = false;
                    ";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(createDatabaseCommand, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Database created successfully.");
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    //ユーザー作成
    private void CreateRole(string connectionString)
    {
        var roleCommand = @"
    DO $$
    BEGIN
        -- ロールが存在しない場合にのみ作成
        IF NOT EXISTS (
            SELECT 1
            FROM pg_roles
            WHERE rolname = 'arisa'
        ) THEN
            CREATE ROLE arisa WITH
              LOGIN
              SUPERUSER
              INHERIT
              CREATEDB
              CREATEROLE
              REPLICATION
              BYPASSRLS
              ENCRYPTED PASSWORD 'SCRAM-SHA-256$4096:lVbl3EIGQ6F53pIyL5Lm/Q==$9pktFco4ty90hZz1TKER39abiPnl5wL0Sq8XSCiOXfE=:94K5pD6e1xVTc2iQR20ppMFrGYsS3ytB3VZZWv412Nw=';
        END IF;
    END $$;
    ";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(roleCommand, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Role created successfully.");
    }

    //トリガ関数作成
    private  void CreateTriggerFunction(NpgsqlConnection connection)
    {
        var commandText = @"
        CREATE OR REPLACE FUNCTION public.update_timestamp()
        RETURNS trigger
        LANGUAGE 'plpgsql'
        COST 100
        VOLATILE NOT LEAKPROOF
        AS $BODY$
        BEGIN
            NEW.更新日 = CURRENT_TIMESTAMP;
            RETURN NEW;
        END;
        $BODY$;
        ";
        using (var cmd = new NpgsqlCommand(commandText, connection))
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Trigger function created successfully.");
        }
    }
    //テーブル作成
    private  void CreateTables(NpgsqlConnection connection)
    {
        var tableCommands = new[]
        {
            //Mスキージ
            @"
            CREATE TABLE IF NOT EXISTS public.""Mスキージ""
            (
                ""スキージid"" integer NOT NULL DEFAULT nextval('""Mスキージ_id_seq""'::regclass),
                ""名称"" character varying(100) NOT NULL,
                ""全長"" numeric(10,2) NOT NULL,
                ""廃棄ライン"" numeric(10,2) NOT NULL,
                ""登録日"" timestamp DEFAULT CURRENT_TIMESTAMP,
                ""更新日"" timestamp DEFAULT CURRENT_TIMESTAMP,
                ""削除フラグ"" integer DEFAULT 0,
                ""スキージ研磨減少量"" numeric(10,2) DEFAULT 0.00,
                CONSTRAINT ""Mスキージ_pkey"" PRIMARY KEY (""スキージid"")
            );
            CREATE OR REPLACE TRIGGER set_update_timestamp
                BEFORE UPDATE 
                ON public.""Mスキージ""
                FOR EACH ROW
                EXECUTE FUNCTION public.update_timestamp();
            ",
            @"
                CREATE TABLE IF NOT EXISTS public.""M削除""
                (
                    ""削除id"" integer NOT NULL DEFAULT nextval('""M削除_id_seq""'::regclass),
                    ""名称"" character varying(100) COLLATE pg_catalog.""default"" NOT NULL,
                    ""登録日"" timestamp DEFAULT CURRENT_TIMESTAMP,
                    ""更新日"" timestamp DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""M削除_pkey"" PRIMARY KEY (""削除id"")
                );

                ALTER TABLE IF EXISTS public.""M削除""
                    OWNER to arisa;

                CREATE OR REPLACE TRIGGER set_update_timestamp
                    BEFORE UPDATE 
                    ON public.""M削除""
                    FOR EACH ROW
                    EXECUTE FUNCTION public.update_timestamp();
                ",
                //M削除
                @"
                CREATE TABLE IF NOT EXISTS public.""M削除""
                (
                    ""削除id"" integer NOT NULL DEFAULT nextval('""M削除_id_seq""'::regclass),
                    ""名称"" character varying(100) COLLATE pg_catalog.""default"" NOT NULL,
                    ""登録日"" timestamp DEFAULT CURRENT_TIMESTAMP,
                    ""更新日"" timestamp DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""M削除_pkey"" PRIMARY KEY (""削除id"")
                );

                ALTER TABLE IF EXISTS public.""M削除""
                    OWNER to arisa;

                CREATE OR REPLACE TRIGGER set_update_timestamp
                    BEFORE UPDATE 
                    ON public.""M削除""
                    FOR EACH ROW
                    EXECUTE FUNCTION public.update_timestamp();
                ",
                //M在庫登録最新
                @"
                CREATE TABLE IF NOT EXISTS public.""M在庫登録最新""
                (
                    ""在庫登録状況id"" integer NOT NULL DEFAULT nextval('""M在庫登録最新_在庫登録状況id_seq""'::regclass),
                    ""名称"" character varying(10) COLLATE pg_catalog.""default"" NOT NULL,
                    ""登録日時"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""更新日時"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""削除フラグ"" integer DEFAULT 0,
                    CONSTRAINT ""M在庫登録最新_pkey"" PRIMARY KEY (""在庫登録状況id"")
                )
                TABLESPACE pg_default;
                ALTER TABLE IF EXISTS public.""M在庫登録最新""
                    OWNER to arisa;
                CREATE OR REPLACE TRIGGER set_update_timestamp
                    BEFORE UPDATE 
                    ON public.""M在庫登録最新""
                    FOR EACH ROW
                    EXECUTE FUNCTION public.update_timestamp();
                ",
                //M在庫登録状況
                @"
                CREATE TABLE IF NOT EXISTS public.""M在庫登録状況""
                (
                    ""在庫登録状況id"" integer NOT NULL DEFAULT nextval('""M在庫登録状況_在庫登録状況id_seq""'::regclass),
                    ""名称"" character varying(10) COLLATE pg_catalog.""default"" NOT NULL,
                    ""登録日時"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""更新日時"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""削除フラグ"" integer DEFAULT 0,
                    CONSTRAINT ""M在庫登録状況_pkey"" PRIMARY KEY (""在庫登録状況id"")
                )
                TABLESPACE pg_default;

                ALTER TABLE IF EXISTS public.""M在庫登録状況""
                    OWNER to arisa;
                CREATE OR REPLACE TRIGGER set_update_timestamp
                    BEFORE UPDATE 
                    ON public.""M在庫登録状況""
                    FOR EACH ROW
                    EXECUTE FUNCTION public.update_timestamp();

                ",
                //T在庫調査
                @"
                CREATE TABLE IF NOT EXISTS public.""T在庫調査""
                (
                    ""在庫登録id"" integer NOT NULL DEFAULT nextval('""T在庫調査_在庫登録id_seq""'::regclass),
                    ""スキージid"" integer NOT NULL,
                    ""長さ"" numeric(10,2) NOT NULL,
                    ""登録日"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""更新日"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""削除フラグ"" integer DEFAULT 1,
                    ""在庫登録状況フラグ"" integer DEFAULT 1,
                    ""在庫登録最新フラグ"" integer DEFAULT 1,
                    ""フォーム起動日"" date,
                    ""グループ"" integer NOT NULL DEFAULT 0,
                    CONSTRAINT ""T在庫調査_pkey"" PRIMARY KEY (""在庫登録id"")
                )

                TABLESPACE pg_default;

                ALTER TABLE IF EXISTS public.""T在庫調査""
                    OWNER to arisa;

                CREATE OR REPLACE TRIGGER set_update_timestamp
                    BEFORE UPDATE 
                    ON public.""T在庫調査""
                    FOR EACH ROW
                    EXECUTE FUNCTION public.update_timestamp();

                ",
                //T製品投入数
                @"
                CREATE TABLE IF NOT EXISTS public.""T製品投入数""
                (
                    ""製品投入id"" integer NOT NULL DEFAULT nextval('""T製品投入数_製品投入id_seq""'::regclass),
                    ""投入数"" integer NOT NULL,
                    ""登録日時"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""更新日時"" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
                    ""削除フラグ"" integer DEFAULT 0,
                    ""投入年月日"" character(6) COLLATE pg_catalog.""default"",
                    CONSTRAINT ""T製品投入数_pkey"" PRIMARY KEY (""製品投入id""),
                    CONSTRAINT ""unique_投入年月日"" UNIQUE (""投入年月日"")
                )

                TABLESPACE pg_default;

                ALTER TABLE IF EXISTS public.""T製品投入数""
                    OWNER to arisa;


                "
        };

        foreach (var commandText in tableCommands)
        {
            using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        Console.WriteLine("Tables created successfully.");
    }
}
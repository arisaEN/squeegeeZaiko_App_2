using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using zaiko.Database; // DatabaseManagerクラスを使用
using zaiko.anime;
using zaiko;
using Npgsql;
namespace zaiko;


public partial class WindowRegistration : Window
{

    
    private readonly DatabaseManager dbManager;
    private readonly FormAnimation formAni;
    private int groupNum;
    private WindowMain _mainWindow;


    public WindowRegistration(int groupNum,string judge, WindowMain mainWindow)
    {
        
        InitializeComponent();
        //groupName = groupName;
        dbManager = new DatabaseManager(); //ここで初期化処理としてDB接続文あり
        formAni = new FormAnimation();
        //フォーム起動日時
        LabelToday.Content = $"{DateTime.Now:yyyy/MM/dd}";
        LabelGroup.Content = groupNum;
        string judge_current_past = judge; //judgeからデータベースの在庫明細を過去のものに置き換えるか判断するフラグ
        Current_past_process(judge_current_past);
        // DatabaseManagerの初期化
        LoadData();
        formAni.ApplyFadeInAnimation(DataGridDatabase);
        LoadDataSqueegee();
        //DataTable deleteMaster =LoadDataDeleteMaster();
        _mainWindow = mainWindow;
        registrationNowSort_RadioButton.IsChecked = true;
    }

    //---------------------------------------------------------------------------------------------------------------------
    //もしjudge_current_pastが入荷後ならば、DBにあるスキージ在庫の状態を"過去"にする。
    private void Current_past_process(string judge_current_past )
    {
        if (judge_current_past == "入荷後")
        {
            string query = @"
                update ""T在庫調査""
                    set ""在庫登録最新フラグ"" = 2
                    where ""在庫登録最新フラグ"" =1;
                ";
            dbManager.updateQuery(query);
        }
    }

    private DataTable LoadDataDeleteMaster()
    {
        string query = @"
                SELECT  a.""削除id"" , a.""名称""
                    FROM public.""M削除"" as a
                    ORDER BY a.""削除id"" ASC 
                ";
         DataTable deleteMaster = dbManager.ExecuteQuery(query);
        return deleteMaster;
    }




    //-------------------------------------- -------------------------------------------------------------------------------
    //データグリッドに表示する在庫明細 入荷後からの記録全て
    private void LoadData()
    {
        //// SQLクエリ
        string query = @"
            SELECT 
                a.""在庫登録id"",
                b.""名称"" as ""スキージ名称"",
                a.""長さ"" - b.""廃棄ライン"" as ""廃棄までの長さ"",
                b.""廃棄ライン"",
	            d.""名称"" as ""登録最新状況"",
                TO_CHAR(a.""フォーム起動日"", 'YYYY/MM/DD') as ""登録日"",
	            c.""名称"" as ""削除状態"",
                a.""グループ""
	
            FROM ""T在庫調査"" as a 
            JOIN ""Mスキージ"" as b ON a.""スキージid"" = b.""スキージid""
            JOIN ""M削除"" as c ON a.""削除フラグ"" = c.""削除id""
            JOIN ""M在庫登録最新"" d ON a.""在庫登録最新フラグ"" = d.""在庫登録状況id""
            WHERE a.""削除フラグ"" = 1 
            and d.""名称""='現在'

            ORDER BY ""在庫登録id"" ASC;

            ";
        try
        {
            // データベースからデータを取得してDataGridにバインド
            var dataTable = dbManager.ExecuteQuery(query);
            DataGridDatabase.ItemsSource = dataTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"データの読み込み中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}");
        }

        //チェックボックスをONにする。
        //useNowSort_RadioButton
        //registrationNowSort_RadioButton.IsChecked = true;

    }

    //入荷後から今日の登録分のみ
    private void LoadData2()
    {
        DateTime todayFormBoot = DateTime.Parse(LabelToday.Content.ToString());

        //登録日が本日の情報のみでソート
        string query = @"
            SELECT 
                a.""在庫登録id"",
                b.""名称"" as ""スキージ名称"",
                a.""長さ"" - b.""廃棄ライン"" as ""廃棄までの長さ"",
                b.""廃棄ライン"",
	            d.""名称"" as ""登録最新状況"",
                TO_CHAR(a.""フォーム起動日"", 'YYYY/MM/DD') as ""登録日"",
	            c.""名称"" as ""削除状態"",
                a.""グループ""
	
            FROM ""T在庫調査"" as a 
            JOIN ""Mスキージ"" as b ON a.""スキージid"" = b.""スキージid""
            JOIN ""M削除"" as c ON a.""削除フラグ"" = c.""削除id""
            JOIN ""M在庫登録最新"" d ON a.""在庫登録最新フラグ"" = d.""在庫登録状況id""
            WHERE a.""削除フラグ"" = 1 
            and d.""名称""='現在'
            and a.""フォーム起動日"" = @Today

            ORDER BY ""在庫登録id"" ASC;

            ";
        try
        {
            // データベースからデータを取得してDataGridにバインド
            var dataTable = dbManager.ExecuteQuery2(query, todayFormBoot);
            DataGridDatabase.ItemsSource = dataTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"データの読み込み中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}");
        }

        //チェックボックスをONにする。
        //registrationNowSort_RadioButton.IsChecked = true;

    }

    //入荷後から削除フラグが経っている明細
    private void LoadData3()
    {
        //// SQLクエリ
        string query = @"
            SELECT 
                a.""在庫登録id"",
                b.""名称"" as ""スキージ名称"",
                a.""長さ"" - b.""廃棄ライン"" as ""廃棄までの長さ"",
                b.""廃棄ライン"",
	            d.""名称"" as ""登録最新状況"",
                TO_CHAR(a.""フォーム起動日"", 'YYYY/MM/DD') as ""登録日"",
	            c.""名称"" as ""削除状態"",
                a.""グループ""
	
            FROM ""T在庫調査"" as a 
            JOIN ""Mスキージ"" as b ON a.""スキージid"" = b.""スキージid""
            JOIN ""M削除"" as c ON a.""削除フラグ"" = c.""削除id""
            JOIN ""M在庫登録最新"" d ON a.""在庫登録最新フラグ"" = d.""在庫登録状況id""
            WHERE a.""削除フラグ"" = 2
            and d.""名称""='現在'

            ORDER BY ""在庫登録id"" ASC;

            ";
        try
        {
            // データベースからデータを取得してDataGridにバインド
            var dataTable = dbManager.ExecuteQuery(query);
            DataGridDatabase.ItemsSource = dataTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"データの読み込み中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}");
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------
    //コンボボックスにリストとして選択できるようにする
    private void LoadDataSqueegee()
    {
        // SQLクエリ
        string query = @"
            SELECT a.""名称""
            FROM public.""Mスキージ"" as a
            WHERE ""削除フラグ"" = 0
            ORDER BY ""スキージid"" ASC;
            ";

        try
        {
            // データベースからデータを取得してDataGridにバインド
            var dataTable = dbManager.ExecuteQuery(query);

            // ComboBoxにデータを追加
            foreach (DataRow row in dataTable.Rows)
            {
                ComboBoxSqueegeeNames.Items.Add(row["名称"].ToString());
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"データの読み込み中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}");
        }
    }


    //---------------------------------------------------------------------------------------------------------------------

    private void mainTransitionButton_Click(object sender, RoutedEventArgs e)
    {
        // 既にWindowMainが開いているか確認
        foreach (Window window in System.Windows.Application.Current.Windows)
        {
            if (window is WindowMain)
            {
                // 既存のウィンドウをアクティブにして終了
                window.Activate();
                formAni.ApplyFadeOutAndClose(this);
                return;
            }
        }


        
        // 開いていない場合、新しいウィンドウを開く
        var windowMain = new WindowMain();
        windowMain.Show();
        formAni.ApplyFadeOutAndClose(this);

    }
    //---------------------------------------------------------------------------------------------------------------------
    private void DatabaseRegistrationSqueegee_Click(object sender, EventArgs e)
    {

        DateTime todayFormBoot = DateTime.Parse(LabelToday.Content.ToString());
        //MessageBox.Show(todayFormBoot.ToString());

        try
        {
            // ComboBoxから選択された名称
            string selectedName = ComboBoxSqueegeeNames.SelectedItem?.ToString();
            
            if (string.IsNullOrEmpty(selectedName))
            {
                MessageBox.Show("スキージ名称を選択してください。");
                return;
            }

            // 測定長さを取得
            if (!double.TryParse(TextBoxMeasuredLength.Text, out double measuredLength))
            {
                MessageBox.Show("測定長さを正しく入力してください。");
                return;
            }

            // 選択された名称からスキージIDを取得するクエリ
            string query = @"
                    SELECT ""スキージid""
                    FROM public.""Mスキージ""
                    WHERE ""名称"" = @Name AND ""削除フラグ"" = 0;
                ";

            var parameters = new Dictionary<string, object> { { "@Name", selectedName } };
            var dataTable = dbManager.ExecuteQuery(query, parameters);

            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("選択されたスキージが見つかりません。");
                return;
            }
            formAni.ApplyFadeInAnimation(DataGridDatabase);

            int squeegeeId = Convert.ToInt32(dataTable.Rows[0]["スキージid"]);


            //groupNumをラベルから取得してデータベースに挿入する。
            string contentValue = LabelGroup.Content.ToString();
            // int型の変数に変換
            if (int.TryParse(contentValue, out int groupNum))
            {
                // 変換成功時、resultに値が入る
                Console.WriteLine($"変換成功: {groupNum}");
            }
            else
            {
                // 変換失敗時の処理
                Console.WriteLine("Contentの値が数値ではありません。");
            }


            // データベースに挿入
            dbManager.InsertSqueegeeData(squeegeeId, measuredLength,todayFormBoot, groupNum);

            //データグリッドに最新データを表示させる。
            LoadData2();
            formAni.ApplyFadeInAnimation(DataGridDatabase);

            //MessageBox.Show("データが正常に登録されました。");



            // WindowMain のメソッドを呼び出す
            _mainWindow.LoadGraphData();

            // PredictionSqueegee の結果を取得
            DataTable resultTable = _mainWindow.PredictionSqueegee();

            // WindowMain の DataGrid に結果を設定
            _mainWindow.resultDataGrid.ItemsSource = resultTable.DefaultView;



        }
        catch (Exception ex)
        {
            MessageBox.Show($"登録中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}");
        }
    }


    //---------------------------------------------------------------------------------------------------------------------
    //ラジオボタン
    private void useNowSort_RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        LoadData();
        formAni.ApplyFadeInAnimation(DataGridDatabase);
    }


    private void registrationNowSort_RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        LoadData2();
        formAni.ApplyFadeInAnimation(DataGridDatabase);

    }

    private void NowDeleteSort_RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        LoadData3();
        formAni.ApplyFadeInAnimation(DataGridDatabase);
    }
    //---------------------------------------------------------------------------------------------------------------------
    //削除フラグの変更ボタン
    //明細のid取得してそのidの削除フラグを変える
    private void ChangeFlagButton_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var row = button?.DataContext as DataRowView;

        if (row == null)
            return;

        int recordId = (int)row["在庫登録id"];

        string updateQuery = @"
                UPDATE  ""T在庫調査"" 
                SET ""削除フラグ"" = 
                    CASE 
                        WHEN ""削除フラグ"" = 1 THEN 2 
                        WHEN ""削除フラグ"" = 2 THEN 1 
                        ELSE ""削除フラグ"" 
                    END
                WHERE ""在庫登録id"" = @Id";

        var idParam = new NpgsqlParameter("@Id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = recordId };

        int rowsAffected = dbManager.ExecuteNonQuery(updateQuery, idParam);

        if (rowsAffected > 0)
        {
            // Sort状態に応じたメソッドを呼び出す
            if (registrationNowSort_RadioButton.IsChecked == true)
            {
                LoadData2(); // registrationNowSort_RadioButtonがtrueの場合
            }
            else if (NowDeleteSort_RadioButton.IsChecked == true)
            {
                LoadData3(); // NowDeleteSort_RadioButtonがtrueの場合
            }
            else if (useNowSort_RadioButton.IsChecked == true)
            {
                LoadData(); // NowDeleteSort_RadioButtonがtrueの場合
            }

            // WindowMainのメソッドを呼び出す
            _mainWindow.LoadGraphData();

            // PredictionSqueegeeの結果を取得
            DataTable resultTable = _mainWindow.PredictionSqueegee();

            // WindowMainのDataGridに結果を設定
            _mainWindow.resultDataGrid.ItemsSource = resultTable.DefaultView;

            //MessageBox.Show($"在庫登録ID {recordId} の削除フラグを変更しました。");
        }
        else
        {
            //MessageBox.Show($"在庫登録ID {recordId} の削除フラグの変更に失敗しました。");
        }
    }






    //---------------------------------------------------------------------------------------------------------------------
}





//private void LoadSqueegeeNames()
//{
//    try
//    {
//        // データベース接続
//        string connectionString = "Host=100.64.16.151;Username=arisa;Password=arisa;Database=スキージ管理";
//        using (var connection = new NpgsqlConnection(connectionString))
//        {
//            connection.Open();
//            string query = @"
//                SELECT  a.""名称""
//                FROM public.""Mスキージ"" as a
//                where ""削除フラグ"" = 0	
//                ORDER BY ""スキージid"" ASC;
//         "; 

//            using (var command = new NpgsqlCommand(query, connection))
//            {
//                using (var reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        ComboBoxSqueegeeNames.Items.Add(reader["名称"].ToString());
//                    }
//                }
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show($"データベースエラー: {ex.Message}");
//    }
//}

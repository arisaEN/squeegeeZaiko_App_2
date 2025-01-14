using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zaiko.Database;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Data;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.Statistics;
using LiveCharts;
using LiveCharts.Wpf;
using Npgsql;
using System.Globalization;
using static MaterialDesignThemes.Wpf.Theme;

namespace zaiko
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : System.Windows.Window
    {
        //グラフ用変数
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Dates { get; set; }

        private readonly DatabaseManager dbManager; //インスタンス用の変数 //コンストラクタのみで変数への代入可能
        private readonly DatabaseInitializer dbInitializer;
        //private readonly InventoryData inventoryData;

        public WindowMain() // コンストラクタ
        {
            InitializeComponent(); ///XAMLファイルのボタン情報などとリンクさせる
            
            dbManager = new DatabaseManager(); //ここで初期化処理としてDB接続文あり //クラスからインスタンス生成
            dbInitializer= new  DatabaseInitializer();

            //グラフ更新、グラフ明細更新(データグリッド)
            LoadGraphData();
            DataTable resultTable=PredictionSqueegee();
            resultDataGrid.ItemsSource = resultTable.DefaultView;
        }
        //-------------------------------------------------------------------------------------------------------
        public DataTable PredictionSqueegee()
        {
            //スキージの種類文ループ
            string query = @"
                SELECT ""名称"",
                            ""全長"",
                            ""廃棄ライン""
                FROM ""Mスキージ""
                ORDER BY ""スキージid"" ASC;
            ";
            // データ取得
            DataTable squeegeeMaster = dbManager.ExecuteQuery3(query);

            // 結果を格納するデータテーブルを作成
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("スキージ名称", typeof(string));
            resultTable.Columns.Add("入荷日付", typeof(string));
            resultTable.Columns.Add("入荷在庫", typeof(double));
            resultTable.Columns.Add("最新日付", typeof(string));
            resultTable.Columns.Add("最新在庫", typeof(double));
            //resultTable.Columns.Add("経過日数", typeof(int));
            resultTable.Columns.Add("1日あたりの消費量", typeof(double));
            //resultTable.Columns.Add("あと何日もつか", typeof(int));
            resultTable.Columns.Add("何日後か", typeof(string));
            //resultTable.Columns.Add("30日後何mm足りないか", typeof(string));
            resultTable.Columns.Add("30日後まで何本足りないか？", typeof(string));


            // 件数分ループ処理
            int r = squeegeeMaster.Rows.Count;
            for (int i = 0; i < r; i++)
            {
                string squeegeeName = squeegeeMaster.Rows[i]["名称"].ToString();
                double squeegeeLength = Convert.ToDouble(squeegeeMaster.Rows[i]["全長"]);
                double squeegeeDiscardLength = Convert.ToDouble(squeegeeMaster.Rows[i]["廃棄ライン"]);

                // スキージごとのデータを取得するクエリ
                string squeegeeQuery = $@"
                   SELECT 
                        TO_CHAR(a.""フォーム起動日"", 'YYYY/MM/DD') as ""登録日"",
                        b.""名称"" as ""スキージ名称"",
                        SUM(a.""長さ"" - b.""廃棄ライン"") as ""廃棄までの長さ""
                    FROM ""T在庫調査"" as a 
                    JOIN ""Mスキージ"" as b ON a.""スキージid"" = b.""スキージid""
                    JOIN ""M削除"" as c ON a.""削除フラグ"" = c.""削除id""
                    JOIN ""M在庫登録最新"" d ON a.""在庫登録最新フラグ"" = d.""在庫登録状況id""
                    WHERE a.""削除フラグ"" = 1 
                      AND d.""名称"" = '現在'
                      AND b.""名称"" = '{squeegeeName}'
                    GROUP BY 
                        a.""フォーム起動日"", 
                        b.""名称""
                    ORDER BY a.""フォーム起動日"" ASC;
                ";

                // スキージごとのデータを取得
                DataTable squeegeeData = dbManager.ExecuteQuery3(squeegeeQuery);

                // 新しい行を作成
                DataRow newRow = resultTable.NewRow();

                // スキージ名称を設定
                newRow["スキージ名称"] = squeegeeName;

                //resultDataを作ってスキージ在庫増減を予測する。
                if (squeegeeData.Rows.Count > 0)
                {
                    //squeegeeDataの最初の行と最後の行
                    DataRow firstRow = squeegeeData.Rows[0]; // squeegeeData の最初の行
                    DataRow lastRow = squeegeeData.Rows[squeegeeData.Rows.Count - 1]; // squeegeeData の最後の行
                    // squeegeeDataの一番上の明細を設定
                    newRow["入荷日付"] = firstRow["登録日"].ToString(); // 入荷日付
                    newRow["入荷在庫"] = Convert.ToDouble(firstRow["廃棄までの長さ"]); // 入荷在庫

                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    // squeegeeDataの一番下の明細を設定
                    newRow["最新日付"] = lastRow["登録日"].ToString(); // 最新日付
                    newRow["最新在庫"] = Convert.ToDouble(lastRow["廃棄までの長さ"]); // 最新在庫

                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    // 最新日付と入荷日付の差を計算して経過日数を設定
                    DateTime latestDate = DateTime.Parse(lastRow["登録日"].ToString());  // 最新日付
                    DateTime firstDate = DateTime.Parse(firstRow["登録日"].ToString());  // 入荷日付
                    TimeSpan difference = latestDate - firstDate;  // 日付差を計算

                    int daysDifference = difference.Days;  // 経過日数を取得
                    //newRow["経過日数"] = daysDifference;  // 経過日数を設定

                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    // 1日あたりの消費量を計算
                    double intakeStock = Convert.ToDouble(firstRow["廃棄までの長さ"]);
                    double latestStock = Convert.ToDouble(lastRow["廃棄までの長さ"]);
                    double dailyConsumption = 0;
                    // 1日あたりの消費量を計算（0で割ることを防ぐ）
                    if (daysDifference > 0)
                    {
                        dailyConsumption = (intakeStock - latestStock) / daysDifference;
                        newRow["1日あたりの消費量"] = dailyConsumption;
                    }
                    else
                    {
                        dailyConsumption = 0;
                        newRow["1日あたりの消費量"] = dailyConsumption;  // 経過日数が0の場合は消費量を0に設定
                    }

                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    double remainingDays =0 ;
             
                    //あと何日もつか計算 //最新在庫÷一に当たりの消費量
                    if (dailyConsumption <= 0)  //一日あたりの消費量が0以下の場合
                    {
                        //もし一日あたりの消費量がマイナスならば-1とする。
                        remainingDays = -1;
                        //newRow["あと何日もつか"] = remainingDays;
                    }
                    else 
                    {
                        remainingDays = Convert.ToDouble(lastRow["廃棄までの長さ"]) / dailyConsumption;
                        //newRow["あと何日もつか"] = remainingDays;
                    }

                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    // 何日後に0になるか計算  // 最新日付 + あと何日もつか
                    string whatDay = "";
                    // もし一日あたりの消費量が-1ならば”計測不能”とする。 ※在庫が入荷していないのに増えているため計測不能。
                    if (remainingDays == -1)
                    {
                        whatDay = "計測不能";
                        newRow["何日後か"] = whatDay;
                    }
                    else
                    {
                        // DateTime.Parseで最新日付をDateTime型に変換し、AddDaysでremainingDays日を加算
                        DateTime lastDate = DateTime.Parse(lastRow["登録日"].ToString());  // 最新日付
                        DateTime resultDate = lastDate.AddDays(remainingDays);  // 何日後かを計算

                        // 計算結果のDateTimeを文字列に変換して設定
                        whatDay = resultDate.ToString("yyyy/MM/dd");  // 日付を"yyyy/MM/dd"形式に変換
                        newRow["何日後か"] = whatDay;
                    }

                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    //目標日付まであと何ミリ足りないか計算
                    // 30日後何mm足りないか計算
                    double stockIn30Days = 0;

                    // 1日あたりの消費量が計算できていて、日数がプラスの場合に計算を行う
                    if (dailyConsumption > 0)
                    {
                        // 最新在庫から30日間消費する在庫を予測
                        stockIn30Days = Convert.ToDouble(lastRow["廃棄までの長さ"]) - (dailyConsumption * 30);
                        if (stockIn30Days < 0)
                        {
                            // もし30日後の在庫がマイナスになる場合、その差を足りないmmとして設定
                            //newRow["30日後何mm足りないか"] = Math.Abs(stockIn30Days).ToString("0.##");
                        }
                        else
                        {
                            // 30日後でも在庫が残る場合
                            //newRow["30日後何mm足りないか"] = "足りない量なし";
                        }
                    }
                    else
                    {
                        //newRow["30日後何mm足りないか"] = "計測不能"; // 1日あたりの消費量が0以下の場合
                    }
                    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    // 30日まで何本足りないか計算
                    if (stockIn30Days < 0)
                    {
                        double missingLength = Math.Abs(stockIn30Days);
                        double missingCount = missingLength / (squeegeeLength-squeegeeDiscardLength);
                        newRow["30日後まで何本足りないか？"] = missingCount.ToString("0.##");
                    }
                    else
                    {
                        newRow["30日後まで何本足りないか？"] = "足りない本数なし";
                    }


                    // 作成した行をresultTableに追加
                    resultTable.Rows.Add(newRow);
                }
            }
            return resultTable;
        }


        //-------------------------------------------------------------------------------------------------------
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //-------------------------------------------------------------------------------------------------------
        public void LoadGraphData()
        {
            string query = @"
                SELECT 
                    TO_CHAR(a.""フォーム起動日"" AT TIME ZONE 'Asia/Tokyo', 'YYYY/MM/DD') as ""登録日"",
                    b.""名称"" as ""スキージ名称"",
                    SUM(a.""長さ"" - b.""廃棄ライン"") as ""廃棄までの長さ""
                FROM ""T在庫調査"" as a 
                JOIN ""Mスキージ"" as b ON a.""スキージid"" = b.""スキージid""
                JOIN ""M削除"" as c ON a.""削除フラグ"" = c.""削除id""
                JOIN ""M在庫登録最新"" d ON a.""在庫登録最新フラグ"" = d.""在庫登録状況id""
                WHERE a.""削除フラグ"" = 1 
                --AND b.""名称""= 'DB青60'
                AND d.""名称""='現在'
                GROUP BY 
                    a.""フォーム起動日"", 
                    b.""名称""
                ORDER BY a.""フォーム起動日"" ASC;";

            // データ取得
            DataTable dataTable = dbManager.ExecuteQuery3(query);

            var data = new Dictionary<string, List<double>>();
            var dates = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                string squeegeeName = row["スキージ名称"].ToString();
                double lengthUntilDisposal = Convert.ToDouble(row["廃棄までの長さ"]);
                string date = row["登録日"].ToString();

                if (!data.ContainsKey(squeegeeName))
                    data[squeegeeName] = new List<double>();

                if (!dates.Contains(date))
                    dates.Add(date);

                data[squeegeeName].Add(lengthUntilDisposal);
            }

            // グラフデータ設定
            Dates = dates;
            SeriesCollection = new SeriesCollection();

            foreach (var item in data)
            {
                SeriesCollection.Add(new LineSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<double>(item.Value),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                });
            }

            LineChart.Series = SeriesCollection;
            LineChart.AxisX[0].Labels = Dates;

            // 縦軸の設定
            LineChart.AxisY.Clear();
            LineChart.AxisY.Add(new Axis
            {
                Title = "廃棄までの長さ",
                MinValue = 0 // 縦軸の最小値を0に設定
            });
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //入荷登録
        private void Arrival_Button_Click(object sender, RoutedEventArgs e)
        {
            //入荷後の登録で間違ってないかユーザーに問う
            MessageBoxResult result = MessageBox.Show(
                "入荷後の登録をすると、入荷前のスキージ在庫データは見れなくなります。よろしいですか？", // メッセージ
                "確認",                 // タイトル
                MessageBoxButton.YesNo, // ボタン
                MessageBoxImage.Question // アイコン
                );

            if (result == MessageBoxResult.Yes)
            {
                //何もしない
            }
            else if (result == MessageBoxResult.No)
            {
                //処理終了
                MessageBox.Show("登録をキャンセルしました。");
                return;
            }

            if (!WindowRegistration_Open_process())
            {
                return;
            }

            int groupNum = LoadGroup();
            groupNum = groupNum + 1; //入荷後は次のグループへ。
            // 開いていない場合、新しいウィンドウを開く

            string judge = "入荷後";

            //グラフ更新、グラフ明細更新(データグリッド)
            LoadGraphData();
            DataTable resultTable = PredictionSqueegee();
            resultDataGrid.ItemsSource = resultTable.DefaultView;

            var windowRegistration = new WindowRegistration(groupNum,judge, this);
            windowRegistration.Show();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //研磨後登録
        private void Polishing_Button_Click(object sender, RoutedEventArgs e)
        {
            //すでに本日在庫入力してあるかDBを調べる。
            DateTime today = DateTime.Today;

            if (!WindowRegistration_Open_process())
            {
                return;
            }

           int groupNum = LoadGroup();
            // 開いていない場合、新しいウィンドウを開く

            string judge = "研磨後";

            var windowRegistration = new WindowRegistration(groupNum, judge, this);
            windowRegistration.Show();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool WindowRegistration_Open_process()
        {
            //すでに本日在庫入力してあるかDBを調べる。
            DateTime today = DateTime.Today;
            //test用
            //DateTime today = new DateTime(2025, 1, 8);

            //SQL作成

            string query = @"
                SELECT* FROM public.""T在庫調査"" as a
                where a.""フォーム起動日"" =  @Today
            ";
            // var parameters = new Dictionary<string, object> { { "@Today", today } };
            var dataTable = dbManager.ExecuteQuery2(query, today);

            //してないなら何もしない
            if (dataTable.Rows.Count == 0)
            {
                return true;
            }
            //してあるならば警告を出してからフォームを開く

            else if (dataTable.Rows.Count >= 1)
            {
                MessageBoxResult result = MessageBox.Show(
                "本日分は既に登録されています。追加で登録しますか？", // メッセージ
                "確認",                 // タイトル
                MessageBoxButton.YesNo, // ボタン
                MessageBoxImage.Question // アイコン
                );

                if (result == MessageBoxResult.Yes)
                {
                    //何もしない
                }
                else if (result == MessageBoxResult.No)
                {
                    //処理終了
                    MessageBox.Show("登録をキャンセルしました。");
                    return false;
                }

            }
            // 既にWindowRegistrationが開いているか確認
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window is WindowRegistration)
                {
                    // 既存のウィンドウをアクティブにして終了
                    window.Activate();
                    return false;
                }
            }
            return true;
        }

        //----------------------------------------------------------------------------------------------------------------------
        // 在庫のグループ番号を取得する
        private int LoadGroup()
        {
            int groupNum = 0; // 初期値を0に設定
                              // SQLクエリ
            string query = @"
                SELECT 
                     ""グループ""
                     FROM public.""T在庫調査""
                     ORDER BY ""在庫登録id"" DESC
                     LIMIT 1
                ";
            try
            {
                // DataTableの作成
                var dataTable = dbManager.ExecuteQuery3(query);

                // DataTableから1行目を取得
                if (dataTable.Rows.Count > 0)
                {
                    var firstRow = dataTable.Rows[0]; // 1行目を取得
                    if (int.TryParse(firstRow["グループ"]?.ToString(), out int parsedGroupNum))
                    {
                        groupNum = parsedGroupNum; // 正しくパースできた場合に代入
                    }
                    else
                    {
                        MessageBox.Show("グループ列の値を整数に変換できませんでした。");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"グループの読み込み中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}");
            }

            return groupNum;

        }

        

        //----------------------------------------------------------------------------------------------------------------------
    }
}



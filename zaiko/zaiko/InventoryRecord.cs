using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zaiko
{
    public class InventoryRecord
    {
        public int 在庫登録id { get; set; }
        public string スキージ名称 { get; set; }
        public int 廃棄までの長さ { get; set; }
        public int 廃棄ライン { get; set; }
        public string 登録最新状況 { get; set; }
        public string 登録日 { get; set; }
        public string 削除状態 { get; set; }
        public string グループ { get; set; }
    }
}

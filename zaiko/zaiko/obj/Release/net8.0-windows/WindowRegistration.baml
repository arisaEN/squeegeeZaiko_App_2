   M S B A M L   `   `   `  ���� @  <zaiko, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:zaikozaiko      zaiko.WindowRegistration   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:zaiko       � Title$  WindowRegistration��$	��550��$
��1000�� � Triggers :�  :� RoutedEvent$ Window.Loaded��.[��� .���� .��W�  �� TargetProperty$ Opacity� W� From 0	 W� To 1 g� Duration$ 0:0:1D�.���  � RowDefinitions �� $
��Auto ��� $��* � � ColumnDefinitions �� $��* ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��	 �� Orientation$	 
Horizontal=�$��Center=�$��5q�.+��� -   $��LabelTodayTitle��$��12(�$��フォーム起動日時���� -   $��
LabelToday��$��12(��� $��0��$��1��$	 
Horizontal=�$��Center=�$��5q�.+��� -   $��LabelGrpupTitle��$��12(�$��グループ番号��$��0,0,5,0q��� -   $��
LabelGroup��$��12(��� $��0��$��2��$	 
Horizontal=�$��Right=�$��5q�.+�	� -   
 	�Name$
 useNowSort_RadioButton��$!��入荷後の登録ソート�� 	� 	GroupName$
 Sort��$��5,0,0,0q�	� -   $%
 registrationNowSort_RadioButton��$'��!入荷後本日登録分ソート��$
 Sort��$��5,0,0,0q�	� -   $
 NowDeleteSort_RadioButton��$��入荷後削除リスト��$
 Sort��$��5,0,0,0q��� $��1��$��3��$��5q�.+��� $	 
Horizontal=�$��3��$��5q�$��Stretch=�.+��� -   $��LabelDatabaseTitle��$��16(�$	��5,1q�$��Database出力��&   System.Windows.Controls.DataGrid  -	   $��DataGridDatabase��$��5q�   AutoGenerateColumns$
 True��$	��300��"   VerticalScrollBarVisibility$
 Auto=�   CanUserAddRows$ False��   
IsReadOnly$
 True��   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 削除フラグ変更��   CellTemplate �� .R��� -
   $��変更���� $	 
Horizontal=�$��5q�.+��� $��スキージ名称:���� -   $��ComboBoxSqueegeeNames��$	��180��$��0,0,5,0q��� $��測定長さ:���� -   $��TextBoxMeasuredLength��$	��180��$��0,0,5,0q��� -    ��Name$" DatabaseRegistrationSqueegee��$!��データベースに入力��$	��150��$��10,0,0,0q��� -   $ mainTransitionButton��$��100,10q�$��メイン画面へ��
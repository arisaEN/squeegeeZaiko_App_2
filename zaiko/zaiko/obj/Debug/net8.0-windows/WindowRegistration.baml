   M S B A M L   `   `   `  ����@  <zaiko, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:zaikozaiko  5   	       zaiko.WindowRegistration   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:zaiko     5   	     � Title$  WindowRegistration��5   	   $	��550��6$   $
��1000��61    � Triggers 5	      :� 5
   
    :� RoutedEvent$ Window.Loaded��6   .[�5      �� .��5      �� .��5      W�  �� TargetProperty$ Opacity�6&    W� From 05   &   	 W� To 16/    g� Duration$ 0:0:1D�66   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5!   
   �� $��0��6   $��0��6"   	 �� Orientation$	 
Horizontal=�62   $��Center=�6K   $��5q�6f   .+�5"      �� -   5        $��LabelTodayTitle��5"      $��12(�6+   $��フォーム起動日時��69   �� 5#      -   5        $��
LabelToday��5#      $��12(�6&   5$      �� 5'   
   $��0��6   $��1��6"   $	 
Horizontal=�62   $��Center=�6K   $��5q�6f   .+�5(      �� -   5        $��LabelGrpupTitle��5(      $��12(�6+   $��グループ番号��69   $��0,0,5,0q�6J   �� 5)      -   5        $��
LabelGroup��5)      $��12(�6&   5*      �� 5-   
   $��0��6   $��2��6"   $	 
Horizontal=�62   $��Right=�6K   $��5q�6g   .+�5.      	� -   5        
 	�Name$
 useNowSort_RadioButton��5.      $!��入荷後の登録ソート��6:    	� 	GroupName$
 Sort��6N   $��5,0,0,0q�6�   	� 5/      -   5        $%
 registrationNowSort_RadioButton��5/      $'��!入荷後本日登録分ソート��6C   $
 Sort��6Y   $��5,0,0,0q�6�   	� 50      -   5        $
 NowDeleteSort_RadioButton��50      $��入荷後削除リスト��6=   $
 Sort��6P   $��5,0,0,0q�6�   51      �� 54   
   $��1��6   $��3��6"   $��5q�66   .+�55      �� $	 
Horizontal=�6   $��3��62   $��5q�6F   $��Stretch=�6Q   .+�56      �� -   5        $��LabelDatabaseTitle��56   &   $��16(�6   $	��5,1q�6@   $��Database出力��6M   57      &   System.Windows.Controls.DataGrid  58      -	   5        $��DataGridDatabase��58      $��5q�6/      AutoGenerateColumns$
 True��59      $	��300��5:      "   VerticalScrollBarVisibility$
 Auto=�5;         CanUserAddRows$ False��5<         
IsReadOnly$
 True��5=         Columns 5>      4  .System.Windows.Controls.DataGridTemplateColumn  5@      ,  &System.Windows.Controls.DataGridColumn   Header$ 削除フラグ変更��6-      CellTemplate 5A      �� 5B      .R�5C   "   �� -
   5        $��変更��5C   )   66   5D      5F      5H      �� 5K      $	 
Horizontal=�6   $��5q�62   .+�5L      �� $��スキージ名称:��6   �� 5M      -   5        $��ComboBoxSqueegeeNames��5M      $	��180��68   $��0,0,5,0q�6D   �� 5N      $��測定長さ:��6   �� 5O      -   5        $��TextBoxMeasuredLength��5O      $	��180��67   $��0,0,5,0q�6C   �� 5P      -   5         ��Name$" DatabaseRegistrationSqueegee��5P      $!��データベースに入力��6?   $	��150��6S   $��10,0,0,0q�6_   6q   5Q      �� 5T      -   5        $ mainTransitionButton��5T      $��100,10q�63   $��メイン画面へ��6C   6T   5U      5V      5W      
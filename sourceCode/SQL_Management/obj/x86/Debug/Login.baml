   M S B A M L   `   `   `  ����  SQL_Management    SQL_Management.Login   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  连接到服务器��5   	   $	��256��6   $	��515��6%    � WindowStartupLocation$ CenterScreen=�61    � ShowInTaskbar .  � 
ResizeMode$ NoResize=�5   	   .��5      ]� .+�5   
   �� $$��SQL Management NSun For Shadow��6   $	��Top=�69   ���	#FF489D9D6N   � 5	   
   $��Bottom=�6    � RowDefinitions 5
      �� 5      $
��120* �6    �� 5      $	��40* �6    .+�5      �� -   5        $��label1��5   P   $	��0.5��6   $
��Left=�6!   $��
18,140,0,0q�6<   $	��Top=�6^   $	��453��6v   ���	#FFB3A9A96�   �� 5      -   5        $��button1��5   �   $��连接_C��6;   $��1��6J   $��23��6W   $
��Left=�6c   $��
215,10,0,0q�6~   $	��Top=�6�   $��75��6�   ��6   �� *ConnectionCommand+6�   �� 5      -   5        $��Clean��5   �   $��取消_Q��67   $��1��6F   $��23��6S   $
��Left=�6_   $��
305,10,0,0q�6z   $	��Top=�6�   $��75��6�   ��6   �� *CancelCommand+6�   �� 5      -   5        $��button3��5   l   $��帮助_H��6   $��1��6$   $��23��61   $
��Left=�6=   $��
396,10,0,0q�6X   $	��Top=�6{   $��75��6�   �� 5      -   5        $��label2��5   _   $��服务器类型:��6   $��28��6%   $
��Left=�61   $��	34,27,0,0q�6L   $	��Top=�6m   �� 5      -   5        $��label3��5   _   $��连接字符串:��6   $��28��6%   $
��Left=�61   $��	34,66,0,0q�6L   $	��Top=�6m   �� 5      -   5        $��	comboBox1��5   F   $��23��5       �� DisplayMemberPath$  ��5       �� SelectedValuePath$  ��6-    �� SelectedIndex$ 0��6B   $
��Left=�5      $��
151,27,0,0q�62   $	��Top=�6W   $	��311��6o   ��5      �� *ListSQLType+ �� SelectedValue 5      �� *ConnectionInfo.SqlType+	 �� Mode$	 OneWay=�5   o   �� 5      -   5        $��textBox1��5   �   $��23��6   $
��Left=�6]   $��
151,68,0,0q�6x   $	��Top=�6�   $	��311��6�   ��6"   �� *ConnInfoView.ConnectionString+$	 TwoWay=�6�   �� 5      -	   5        $��	checkBox1��5   �   $��记住设置(_M)��6   $��16��6Y   $
��Left=�6e   $��151,106,0,0q�6�   $	��Top=�6�   
 d� 	IsChecked
 6*   �� *IsCheckSave+$	 TwoWay=�6�   5      5      5      
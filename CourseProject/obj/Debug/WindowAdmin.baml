   M S B A M L   `   `   `  ����H  DCourseProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CourseProjectCourseProject  5   	       CourseProject.WindowAdmin   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:CourseProject     5   	     � Title$   Администратор��5   	   $	��430��6   $��780.667��6,   $	��450��6<   $	��800��6L   .��5	      �  � RowDefinitions 5
   
   �� 5      $	��33* �6   �� 5      $
��345* �6   �� 5      $	��43* �6   .+�5   
   �� $
��auto��6   $
��auto��6!   $��3��6/   ��5      �� 5      .��5      �  � Color$ 	LightBlue��6#    � Offset$ 1Q�65   � 5      $ LightYellow��6#   $ 0Q�67   5      5      �� 5   
   -   5         ��Name$
 Exit��5      $��
Выход��6   $��15(�6/   $��Right=�6=   $	��Top=�6Z   $��79��6r   ���	#FFE53F53
���   �$��Italic$�6�   $��27��6�   $��0,0,0.333,0q�6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ datagrid��5      $��0,0.667,0.333,3q�6%   $��1��6>      
IsReadOnly .    CanUserAddRows .     Columns 5      4  .System.Windows.Controls.DataGridTemplateColumn  5      	   CellTemplate	 5      �� 5      .R�5      �� -   5        $��Выбрать��5   %   $	��100��67   
���   ����	#FF3FE5E56�   5      5       5"      � 5#   
   $��79,-4,287.333,0.333q�6   $��Center=�6,   $	��427��6I   .+�5$      �� -   5        
 ��Name$
 tables��5$      $��Right=�6'   $	��213��6C   $��30��6O   $��0,4,10,0q�6[   6m   �� 5%      $%��Выберите таблицу��6   $��	0,0,255,2q�6/   $��17(�6B   $��Italic$�6P   $��Medium"�6c   5&      � 5'   
   $��30,1,30.333,12.667q�6   $��2��6+    � ColumnDefinitions 5(      �� 5)      $
��144* �6#   �� 5*      $
��164* �6#   �� 5+      $
��147* �6#   �� 5,      $
��149* �6#   �� 5-      $
��129* �6#   .+�50      �� -   5        $ Delete��50      $y�Hidden=�6%   $��Удалить��69   ���	#FF3FE5E5
���   �$��16,-1,17.333,-2q�6v   $��15(�6�   $��Medium"�6�   $��Italic$�6�   $��2��6�   �� 51      -   5        $ Update��51      $y�Hidden=�6%   $��Изменить��69   ���	#FF3FE5E5
���   �$��18,-2,30,-1q�6v   $��15(�6�   $��Medium"�6�   $��Italic$�6�   $��1��6�   �� 52      -   5        $ AddNew��52      $y�Hidden=�6%   $��Добавить��69   ���	#FF3FE5E5
���   �$��
0,-2,28,-1q�6v   $��15(�6�   $��Medium"�6�   $��Italic$�6�   6�   �� 53      -   5        $
 Save��53      $y�Hidden=�6#   $��Сохранить��67   ���	#FF3FE5E5
���   �$��16.667,-1,17.667,-2q�6u   $��15(�6�   $��Medium"�6�   $��Italic$�6�   $��3��6�   �� 54      -	   5        $ Reset��54      $y�Hidden=�6$   $��Отмена��68   ���	#FF3FE5E5
���   �$��15.333,-1,0.333,-2q�6s   $��15(�6�   $��Medium"�6�   $��Italic$�6�   $��4��6�   55      56      57      
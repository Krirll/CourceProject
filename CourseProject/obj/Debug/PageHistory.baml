   M S B A M L   `   `   `  ����H  DCourseProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CourseProjectCourseProject  5          CourseProject.PageHistory   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -localclr-namespace:CourseProject     5        N� Title$  PageHistory��5	      .��5      � 
�������$��0,0,24,0q�6"   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ HistoryGrid��5         
IsReadOnly . $
��Left=�6:   $
��Auto��6U   $
��auto��6c      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5         FontSize$ 15Q�6%      	FontStyle$ Italic$�63   ,  &System.Windows.Controls.DataGridColumn   Header$ Название��6G      Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 6Y   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6e   �� 	 �� Path$	 NameTour�
   HeaderStyle
 5      �� 5       �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6    .�5      ��  �� Property � �� �� Value$ Italic$�6:   �� 5       � ��$ 20(�69   �� 5       � ��$ Medium"�6;   5      5        5      $ 15Q�6%   $ Italic$�63   $ Цена��6G   $
 100* 6U    6b   �� $	 Price� �� StringFormat$ {0} РУБ.��
 5      �� 5      "  6    .�5      ��  � ��$ Italic$�6:   �� 5       � ��$ 20(�69   �� 5       � ��$ Medium"�6;   5      5        5       $ 15Q�6%   $ Italic$�63   $ Дата покупки��6G   $
 100* 6]    6j   �� $	 DateBuy�
 5!      �� 5"      "  6    .�5#      ��  � ��$ Italic$�6:   �� 5$       � ��$ 20(�69   �� 5%       � ��$ Medium"�6;   5&      5(      5*      5,      5-      
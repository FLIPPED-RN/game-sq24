   M S B A M L   `   `   `  ����  WpfApp1    SquashGame.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Squash Game��5   	   $	��500��6   $	��900��6*   $	��500��66   $	��900��6F   $	��500��6U   $	��900��6e   .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��200 �6   .+�5
   
   �� -   5         ��Name$ 
gameCanvas��5
      
�������$��10q�68   $��0��6D   .+�5      �� 
��� ���$��2��5      ��5      �� *ActualWidth+ �� ElementName$ 
gameCanvas����5      �� *ActualHeight+$ 
gameCanvas��5      A� 5      -   5         A�Name$
 ball��5      $��20��6$   $��20��6/   
���  �� �� Left$ 50��6F   
 �� Top$ 50��6W   �� 5      -   5         ��Name$ player1Paddle��5      $	��100��6/   $��20��6;   
����  �$	 100��6S   $	 400��6e   �� 5      -   5        $ player2Paddle��5      $	��100��6/   $��20��6;   
��� � �$	 500��6T   $	 400��6f   5      � 5   
   $��1��6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5      �� $��Рекорд:��6   $��5q�6'   $��20(�62   �� 5      -   5        	 ��Name$	 
recordText��5      $��0��6,   $��80,5,5,5q�69   $��20(�6K   �� 5      -   5        $��20(�5      $��START or RESTART��6#   $��1��6X   $��5,105,5,-159q�6e   5      5      5       
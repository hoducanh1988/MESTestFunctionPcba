   M S B A M L   `   `   `  ����A  =EW12CG, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:EW12CG.UserCtrlEW12CG.UserCtrl  5          EW12CG.UserCtrl.ucLED   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      /localclr-namespace:EW12CG.UserCtrl     5      .��5
      �� -   5          ��Name$  Scr_LogSystem��5
   %   $��0,10,0,0q�6   .��5   
   �� 
�������$��0,0,1,2q�6(    � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '	�       �� 5      " 	�6   .�5      ��  � ��$ 13(�61   �� 5       � ��$ SemiBold"�63   �� 5       � ��$ Center=�6:   -   5        5      5      .+�5      �� ���#777777$��0,0,0,1q�6+   $��0,10,0,5q�6E   ��5      �� 5      " ��6   .�5      ��  � ��
 �����67    �� Triggers 5      �� 5       �� Binding 6*   ��  �� Path$ ledWan� �� Value
 Passed6J   .a�5   "   ��  � �� �#18de3d6?   5      �� 5       6*   �� $ ledWan�
 Failed6J   .a�5    "   ��  � �� �#de4a186?   5!      5#      ._�5%      � .+�5&      �� $��LED WAN��6   $	��100��6.   $��SemiBold"�6:   	� 5'      
	 � Tag$	 ledwan_passed��6"   $��40,0,0,0q�66   $��Passed��6H   	� 5(      $	 ledwan_failed��6"   $��20,0,0,0q�66   $��Failed��6H   5)      5*      � 5+      .+�5,      �� 
��� ���.+�5-      �� $
��Wrap=�6    $��14(�64   $��SemiBold"�6B   
���  ��$9��3Quan sát trạng thái led wan trên sản phẩm:��5.       �� 50      $��'��6   
���  ��6(   �� 51      $��Italic$�6    $
��Wrap=�63   $��13(�6G   $��Normal"�6U   
���  ��$v��pLed sáng nhấp nháy tắt, xanh, đỏ => click nút Passed, các trường hợp khác => click nút Failed.��52       �� 54      $��'��6   
���  ��6(   �� 55      56      57      5:      5<      5@      
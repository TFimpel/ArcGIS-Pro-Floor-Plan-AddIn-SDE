   M S B A M L   `   `   `  ����I  EFloorPlanAddIn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:FloorPlanAddInFloorPlanAddIn  5      a ]ArcGIS.Desktop.Extensions, Version=12.1.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86hJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.ExtensionsArcGIS.Desktop.Extensions 5      "    FloorPlanAddIn.Dockpane2View   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +uiclr-namespace:FloorPlanAddIn     5      a
extensionsJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.Extensions     5        � 	Resources  5       �� MergedDictionaries<  6ArcGIS.Desktop.Extensions.DesignOnlyResourceDictionary   Source�� 5   
    5        5      $S Mpack://application:,,,/ArcGIS.Desktop.Framework;component\Themes\Default.xaml?�6:   %    5      .��5      �  � RowDefinitions 5*   
   �� 5+      $
��Auto �6   �� 5,      $��* �6    � ColumnDefinitions 5.   
   �� 5/      $
��Auto �6   .+�52   
   �� $��0��6   $
��Wrap=�6"   $.��(Commonly used style and export commands.��66    !  Esri_TextBlockDockPaneHeading#���   6f   $��Bottom=�6�   $��5,5,0,0q�6�   6�   �� 54   
   $
��Left=�6   $��1��63   $	��420��6G   $��2��6S   $	��300��6`   $
��Auto=�6p   .��55      �� .+�56      9�  9� 
IsExpanded . $��5,5,5,5q�6.   $	��400��6@   $��Set Map Extent��6M   $
��Left=�6g     Esri_ExpanderPlus#���  6�   .��57      �� $��5,5,5,5q�6!    �� Orientation$ Vertical=�62   $
��Left=�6J   .+�58      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $G��ACenter and zoom to the active layout map frame on the floor plan.��6d   $��11(�6�     Esri_TextBlockRegular#���  6�    �� TextAlignment$
 Left=�6�   6  �� 59      $
��Left=�6"   $��
Set Extent��6=     Esri_Button#���  6�   $	��100��6�   ��6R   �� *CmdZoomAndCenterActiveLayout+6�   5:      5;      9� 5<       . $��5,5,5,5q�6.   $	��400��6@   $��
Rotate Map��6M   $
��Left=�6c   #���  6~   .��5=      �� $��5,5,5,5q�6"   $ Vertical=�63   $
��Left=�6K   .+�5>      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $7��1Rotate the active layout map frame by 45 degrees.��6d   $��11(�6�   #���  6�   $
 Left=�6�   6�   �� 5?      -   5        $��Rotate 45°��5?   "   #���  6J   $
��Left=�6p   $	��100��6�   5@      5A      9� 5B       . $��5,5,5,5q�6.   $	��400��6@   $��Change Room Symbology��6M   $
��Left=�6n   #���  6�   .��5C      �� $ Vertical=�6!   $��5,5,5,5q�68   $
��Left=�6J   .+�5D      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $=��7Select the symbology to display the room polygons with.��6d   $��11(�6�   #���  6�   $
 Left=�6�   6�   	� 5E      -   5        $��All Transparent��5E   &   
 � Tag$
 None��6_   	 d� 	IsChecked$	 FalseY�6j   	� 5F      -   5        $��	All Beige��5F   &   $ Beige��6Y   $	 FalseY�6e   	� 5G      -   5        $(��"Color Code based on Group/Dept. ID��5G   &   $ Multicolor1��6r   $	 FalseY�6�   	� 5H      -   5        $$��Color Code based Function Code��5H   &   $ Multicolor2��6n   $	 FalseY�6�   	� 5I      -   5        $#��Color Code based on Use Group��5I   &   $ Multicolor3��6m   $	 FalseY�6   5J      5K      9� 5L       . $��5,5,5,5q�6.   $	��400��6@   $��Change Line Symbology��6M   $
��Left=�6n   #���  6�   .��5M      �� $��5,5,5,5q�6!   $ Vertical=�62   .+�5N      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $E��?Select the symbology to display the room detail polylines with.��6d   $��11(�6�   #���  6�   $
 Left=�6�   6   �� 5O      $��5,5,5,5q�6&   $ 
Horizontal=�68   $
��Left=�6R   .+�5P      	� -   5        $��5,0,10,0q�5P   *   $
��None��6<   $
 None��6j   $	 FalseY�6u   	� 5Q      -   5        $��5,0,10,0q�5Q   *   $
��Gray��6<   $
 Gray��6j   $	 FalseY�6u   	� 5R      -	   5        $��5,0,10,0q�5R   *   $��
Multicolor��6<   $ 
Multicolor��6p   $	 FalseY�6�   5S      5T      5U      9� 5V       . $��5,5,5,5q�6.   $	��400��6@   $��Choose Room Label Text��6L   $
��Left=�6n   #���  6�   .��5W      �� $ Vertical=�6!   $��5,5,5,5q�68   $
��Left=�6J   .+�5X      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $:��4Select the label text to show for each room polygon.��6d   $��11(�6�   #���  6�   $
 Left=�6�   6�   	� 5Y      -
   5        $��	No lables��5Y   &   $
 None��6W   $
	 TrueY�6b   	� 5Z      -   5        $��Room number��5Z   &   $ $feature.room��6Y   $	 FalseY�6m   	� 5[      -   5        $��Room number and area��5[   &   $I C$feature.room +  TextFormatting.NewLine  + $feature.sqr_feet + 'sf'��6b   $	 FalseY�6�   5\      5]      9� 5^       . $��5,5,5,5q�6.   $	��400��6@   $!��Change Room Label Font Size��6L   $
��Left=�6s   #���  6�   .��5_      �� $��5,5,5,5q�6!   $ Vertical=�62   .+�5`      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $/��)Increase or decrease the label font size.��6d   $��11(�6�   #���  6�   $
 Left=�6�   6�   �� 5a      $��5,5,5,5q�6&   $ 
Horizontal=�67   $
��Left=�6Q   .+�5b      �� -   5        $��Decrease��5b   ,   $
 down��6a   {�. $��70��6}   $��24��6�   $��Center=�6�   ���#FFFFFF
���   �$	��0.8q�6�   6�   �� 5c      -   5        $��Reset��5c   +   $ reset��6Z   {�. $��50��6x   $��24��6�   $��Center=�6�   ���#FFFFFF
���   �$	��0.8q�6�   6�   �� 5d      -   5        $��Increase��5d   +   $ up��6`   {�. $��70��6z   $��24��6�   ���#FFFFFF
���   �$	��0.8q�6�   6�   5e      5f      5g      9� 5h       . $��5,5,5,5q�6.   $	��400��6@   $��Export to File��6M   $
��Left=�6g   #���  6�   .��5i      �� $��5,5,5,5q�6"   $ Vertical=�64   $
��Left=�6L   .+�5j      �� $��0,0,0,5q�6$   $
��Left=�65   $
��Wrap=�6P   $B��<File will be saved in the current ArcGIS Pro Project folder.��6d   $��11(�6�   #���  6�   $
 Left=�6�   6�   �� 5k      -   5        $��5,5,5,0q�5k   !   $
��Left=�62   $	��100��6M   $��Export to .pdf��6[   #���  6�   5l      5m      5o      5p      5r      5s      
   M S B A M L   `   `   `  ����T  PAvalonDock, Version=2.0.1452.0, Culture=neutral, PublicKeyToken=96637cf0d3eefdfbA%clr-namespace:Microsoft.Windows.ShellMicrosoft.Windows.Shell  5      'clr-namespace:AvalonDock
AvalonDock  5      5clr-namespace:AvalonDock.LayoutAvalonDock.Layout  5      9!clr-namespace:AvalonDock.ControlsAvalonDock.Controls  5      =#clr-namespace:AvalonDock.ConvertersAvalonDock.Converters  5      =#clr-namespace:AvalonDock.PropertiesAvalonDock.Properties  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0895    /AvalonDock.Converters.BoolToVisibilityConverter   BoolToVisibilityConverter<   6AvalonDock.Converters.InverseBoolToVisibilityConverter $  InverseBoolToVisibilityConverter<   6AvalonDock.Converters.AnchorSideToOrientationConverter $  AnchorSideToOrientationConverter6   0AvalonDock.Converters.AnchorSideToAngleConverter  AnchorSideToAngleConverter4   .AvalonDock.Converters.NullToDoNothingConverter  NullToDoNothingConverter>   8AvalonDock.Converters.LayoutItemFromLayoutModelConverter & "LayoutItemFromLayoutModelConverterM   GAvalonDock.Converters.ActivateCommandLayoutItemFromLayoutModelConverter 5 1ActivateCommandLayoutItemFromLayoutModelConverter  DocumentPaneControlStyle  �� 
TargetType3   -AvalonDock.Controls.LayoutDocumentPaneControl �� Property �� Value �� 
TargetType J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation   TabNavigation � RowDefinitions K� ZIndex � ColumnDefinitions.	   (AvalonDock.Controls.DocumentPaneTabPanel	 	 Name
 K� IsItemsHost   TabIndex(
   "AvalonDock.Controls.DropDownButton 
 Name 
  DropDownContextMenu'   !AvalonDock.Controls.ContextMenuEx �� RelativeSource$   AvalonDock.Controls.MenuItemEx �� Path �� 	Converter ��Name   DirectionalNavigation ��Name �� Triggers P� Property P� Value �� Value �� Binding �� Mode �� 
TargetName �� Mode/   )AvalonDock.Controls.LayoutDocumentTabItem   Model/   )AvalonDock.Controls.LayoutDocumentControl   Model  AnchorablePaneControlStyle5   /AvalonDock.Controls.LayoutAnchorablePaneControl0   *AvalonDock.Controls.AnchorablePaneTabPanel  Name �� Triggers  �� AncestorType1   +AvalonDock.Controls.LayoutAnchorableTabItem!   Model1   +AvalonDock.Controls.LayoutAnchorableControl"   Model-   'AvalonDock.Controls.AnchorablePaneTitle-   'AvalonDock.Controls.DropDownControlArea#   DropDownContextMenu%$   DropDownContextMenuDataContext%   AvalonDock.Properties.Resources%   Anchorable_CxMenu_Hint& � ToolTip%' 
  DropDownContextMenuDataContext( ��Name")   Anchorable_BtnAutoHide_Hint*   Anchorable_BtnClose_Hint+ �� Angle 	 AnchorSideTemplate1   +AvalonDock.Controls.LayoutAnchorSideControl, �� Orientation 
 AnchorGroupTemplate2   ,AvalonDock.Controls.LayoutAnchorGroupControl- � LayoutTransform  AnchorTemplate-   'AvalonDock.Controls.LayoutAnchorControl. �� Padding   AvalonDock.DockingManager'   !AvalonDock.Controls.OverlayWindow/ ��Name0 J� Opacity1 F�Name2 �Name3 ��Name  DocumentHeaderTemplate  AnchorableHeaderTemplate4 ��Name5 �� Triggers  DocumentTitleTemplate  AnchorableTitleTemplate  IconContentTemplate  AnchorableContextMenu6   Anchorable_Float7 m� Command8   Anchorable_Dock 9   Anchorable_DockAsDocument:   Anchorable_AutoHide;   Anchorable_Hide  DocumentContextMenu<   Document_Close=   Document_CloseAllButThis>   Document_Float?   Document_DockAsDocument%@   Document_NewHorizontalTabGroup#A   Document_NewVerticalTabGroup"B   Document_MoveToNextTabGroup&C   Document_MoveToPreviousTabGroupD   LayoutRootPanelE   RightSidePanelF   LeftSidePanelG   TopSidePanelH   BottomSidePanelI   AutoHideWindow2   ,AvalonDock.Controls.LayoutGridResizerControlJ   ModelK ^� 
ConditionsL �� BindingM �� Value=   7AvalonDock.Controls.LayoutDocumentFloatingWindowControl*   $Microsoft.Windows.Shell.WindowChromeN   ResizeBorderThicknessO   CaptionHeightP   CornerRadiusQ   GlassFrameThicknessR   ShowSystemMenuS   IsHitTestVisibleInChrome,   &Microsoft.Windows.Shell.SystemCommandsT   MaximizeWindowCommandU   Window_MaximizeV   RestoreWindowCommandW   Window_Restore?   9AvalonDock.Controls.LayoutAnchorableFloatingWindowControl5    /AvalonDock.Controls.LayoutAutoHideWindowControl)!   #AvalonDock.Controls.NavigatorWindowX �� SelectedItemY � FocusVisualStyleZ � Orientation�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      /shell%clr-namespace:Microsoft.Windows.Shell  5      '
avalonDockclr-namespace:AvalonDock  5      4avalonDockLayoutclr-namespace:AvalonDock.Layout  5      8avalonDockControls!clr-namespace:AvalonDock.Controls  5      <avalonDockConverters#clr-namespace:AvalonDock.Converters  5      <avalonDockProperties#clr-namespace:AvalonDock.Properties  5      %�h  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 r     &	 �     7�7  7   &	 �	    '  +    7�7 �7   7 �7   &		 �    7  &	
 �    7  &	 �    '  H    &	 �(    &	 !)    7  &	 �*    &	 L+    &	 �+    7  &	 ,    7   &	 �-    7   7   7   7   7   '  u1    7  7  7 	 7 
 7  7  7  7  7  7  7  7  7  '  r7    '  &8    '  �9    7 �'  �?    '  �B    '  �F    7 �7  7 �7   7 �7   '  �N    7   7�7   7 �7  7 �7   7 �7   '   �Y    '!  �Y    7  7  7     5      65     5      6<     5      6<     5      66     5      64     5      6>     5      6M   �� 5      "   6-   .�5   
   ��  � �� 5      �� 5      "  6"   .R�5      � }�.  . $ Local=�6J    5      �� 5      $
��Auto �6,   �� 5      $��* �6,   .+�5      �� 
������ $��2��6:   � 5      $ 1��6     5       �� 5!   "   �� 5"   "   $
��Auto �63   .+�5$      	  $	 HeaderPanel��6F   $��0��6[   
 . $��2,2,2,0q�6~   $��0��6�   $ 1��6�   
  5%      $ MenuDropDownButton��5&   !   8��  |�.  $��1��5)   !    5*   "     5+   &   ��5,   )   �� *Model.ChildrenSorted+ �� *TemplatedParent+��5-   *   �� 5.   .   "   64   .�5/   2   ��  � �� 6S   �� $5 /Root.Manager.DocumentPaneMenuItemHeaderTemplate��� 50   2    � �� 6[   �� $= 7Root.Manager.DocumentPaneMenuItemHeaderTemplateSelector��� 51   2    �@ IconTemplate 6Q   �� $&  Root.Manager.IconContentTemplate��� 52   2    �@ IconTemplateSelector 6Y   �� $. (Root.Manager.IconContentTemplateSelector��� 53   2    �@m�Command 6L   �� $ .�8  54   /   56   '   .��58   "   �� $1��+/AvalonDock;component/Images/PinDocMenu.png��6(   59      5:      �� 5;      $ ContentPanel��6!   $��Stretch=�5<   !   $��Stretch=�5=   !   #��z��5>   !   #��z��5?   !   #��z��5@   !   $��0��5A   !   $ 	Contained=�5B   !   $��1��5C   !   $ 2��5D   !   $ Cycle=�5E   !   ._�5F      �� $ PART_SelectedContentHost��6/   $��SelectedContent��5G   /   #��z��5H   /   # z 5I   /   5J      5K       5L      P� 5M       � {� .  . �5N      �� # � ��6;    � ��5O      �� 5P      $ 0��6~    6&   ��  �� $
 Self=�$ Model.ChildrenCount�.a�5Q      �� $ MenuDropDownButton��6M    � y�$ 	Collapsed=�6;   6M   5R      5T      5V      �� 5W   
    � �� 5X      �� 5Y      "  ��6   .�5Z      ��  � y� 63   �� *	IsVisible+8  �� 5[       �@��
IsSelected 63   �� *
IsSelected+$ TwoWay=��� 5\       �@�ToolTip 60   �� *	ToolTip+5]      5_      �� 5`   
    � �� 5a      �� 5b      .R�5c         6?   �� 5d      5f      �� 5h   
    �@��ContentTemplate 5i      �� 5j      .R�5k         6?   �� 5l      5n      5o      �� 5r      "   6/   .�5s   
   ��  �@��TabStripPlacement$ Bottom=�6.   �� 5t   
    � �� 5u      �� 5v      "  6"   .R�5w      � }�.  . $ Local=�5y       5{      �� 5|      $��* �6,   �� 5}      $
��Auto �6,   .+�5�      �� 
������ $��2��6:   �� 5�      $ ContentPanel��6!   #��z��5�   !   #��z��5�   !   #��z��5�   !   $��0��5�   !   $ 	Contained=�5�   !   $��0��5�   !   $ 2��5�   !   $ Cycle=�5�   !   ._�5�      �� $ PART_SelectedContentHost��6/   $��SelectedContent��5�   /   #��z��5�   /   # z 5�   /   5�        5�      $ HeaderPanel��6D   $��2,0,2,2q�6Y   
 . $��1��6}   $ 1��6�   $ 1��6�   5�       5�      P� 5�       � {� .  . �5�      �� # � ��6;    � ��5�      5�      5�      �� 5�   
    � �� 5�      �� 5�      "  ��6   .�5�      ��  �@��
IsSelected 63   �� *
IsSelected+$ TwoWay=��� 5�       �@�ToolTip 60   �� *	ToolTip+ 5�      �� 5�       6&   ��  �� $ FindAncestor=�"  ��$ Items.Count� 16�   .a�5�      ��  � y�$ 	Collapsed=�6;   5�      5�      5�      �� 5�   
    � �� 5�      �� 5�      .R�5�        ! 6A   �� 5�      5�      �� 5�   
    �@��ContentTemplate 5�      �� 5�      .R�5�        " 6A   �� 5�      5�      5�      �� 5�      "   6   .�5�   
   ��  � �� 5�      �� 5�      .R�5�      �� #��z��6   #��z��5�      #��z��5�      ._�5�      �  5�      �� 5�   "   $��* �63   �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63   .+�5�        # 5�   !   �� **(Model.Root.Manager.AnchorableContextMenu+ �� *TemplatedParent+$ 5�   !   �� $ 
LayoutItem� �� *TemplatedParent+.��5�   "   �� ��63   �� *Model+ �� *TemplatedParent+��5�   /   �� *,*Model.Root.Manager.AnchorableTitleTemplate+ �� *TemplatedParent+��5�   /   �� *42Model.Root.Manager.AnchorableTitleTemplateSelector+ �� *TemplatedParent+5�      
  5�      8��  |�.  $��1��5�   !   #& Z% 5�   !    5�   !   �� **(Model.Root.Manager.AnchorableContextMenu+ �� *TemplatedParent+' 5�   !   �� $ 
LayoutItem� �� *TemplatedParent+.��5�   "   �� $.��(/AvalonDock;component/Images/PinMenu.png��6(   5�      �� 5�      $( PART_AutoHidePin��6%   $��2��5�   !   |�.  8�� #& Z) 5�   !   y�5�   !   �� $ 	IsEnabled� �� *Self+$ OneWay=�8  ��5�   !   �� $  LayoutItem.AutoHideCommand� �� *TemplatedParent+.��5�   "   �� $2��,/AvalonDock;component/Images/PinAutoHide.png��6(   5�      �� 5�      $( PART_HidePin��6%   $��3��5�   !   |�.  8�� #& Z* 5�   !   y�5�   !   �� $ 	IsEnabled� �� *Self+$ OneWay=�8  ��5�   !   �� $ LayoutItem.HideCommand� �� *TemplatedParent+.��5�   "   �� $/��)/AvalonDock;component/Images/PinClose.png��6(   5�      5�      5�       5�      �� 5�      $
 True��6x    6&   �� *Model.IsAutoHidden+ �� $
 Self=�.a�5�      �� $ PART_AutoHidePin��6@    �@�LayoutTransform 5�   "   �� 5�   &   $+ 90Q�66   5�      5�      5�      5�      5�      �� 5      "  61   .R�5  
   �� ��6   ��  �� *TemplatedParent+$ Children���5     �� 5     .R�5     �� , 6!   �� $ 
Model.Side� �� "   $ FindAncestor=�8   5     5     5     �� 5
     "  62   .R�5  
   �� ��5     ��  �� *TemplatedParent+$ Children�- 5     �� 5     + 6"   ��  �� *TemplatedParent+$ Model.Parent.Side�8   ��5     �� 5     .R�5     �� $, 
Horizontal=�6!   5     5     5     �� 5     "  6-   .R�5  
   �� $��1q�6   #��� ��5     $��2q�5     $. 2q�5     ._�5     �� ��6   �� *Model+ �� *TemplatedParent+��5     �� *AnchorableHeaderTemplate+$ OneWay=� �� "   $ FindAncestor=���5      �� *" AnchorableHeaderTemplateSelector+$ OneWay=� �� "   $ FindAncestor=�5!     5"     �� 5$     "   6>   .�5%  
   ��  � ��
 ���� 6'   �� 5&  
    � �� 5'     �� 5(     "  6"   .R�5)     �� $/ PART_DropTargetsContainer��6   $	0 0.9Q�6@   .+�5*     F� $1 PART_PreviewBox��6   
�������$��2��6I   #��� ��6]   � 5+     $$2 PART_DockingManagerDropTargets��6   .+�5,     �� $'3 !PART_DockingManagerDropTargetLeft��6$   $��Center=�6O   $
��Left=�6j   $/��)/AvalonDock;component/Images/DockLeft.PNG��6�   $
��None=�6�   �� 5-     $(3 "PART_DockingManagerDropTargetRight��6$   $��Center=�6P   $��Right=�6k   $0��*/AvalonDock;component/Images/DockRight.PNG��6�   $
��None=�6�   �� 5.     $)3 #PART_DockingManagerDropTargetBottom��6$   $��Bottom=�6Q   $��Center=�6l   $1��+/AvalonDock;component/Images/DockBottom.PNG��6�   $
��None=�6�   �� 5/     $&3  PART_DockingManagerDropTargetTop��6$   $	��Top=�6N   $��Center=�6f   $.��(/AvalonDock;component/Images/DockTop.PNG��6�   $
��None=�6�   50     � 51     $$2 PART_AnchorablePaneDropTargets��6   .+�52     � $��88��6#   $��88��6.   $��Center=�6:   $��Center=�6U    53  "   �� 54  &   �� 55  &   �� 56  &    58  "   �� 59  &   �� 5:  &   �� 5;  &   .+�5=  "   �� $��3��6(   $��3��6<   $4��./AvalonDock;component/Images/DockPaneEmpty.PNG��6M   $��Uniform=�6�   �� 5>  "   $&�� PART_AnchorablePaneDropTargetTop��6)   $��1��6Q   $��0��6a   ._�5?  &   �� $7��1/AvalonDock;component/Images/InnerDockPaneTop.png��6,   $
��None=�6h   5@  #   �� 5A  "   $(��"PART_AnchorablePaneDropTargetRight��6)   $��2��6S   $��1��6c   ._�5B  &   �� $9��3/AvalonDock;component/Images/InnerDockPaneRight.png��6,   $
��None=�6j   5C  #   �� 5D  "   $)��#PART_AnchorablePaneDropTargetBottom��6)   $��1��6T   $��2��6d   ._�5E  &   �� $:��4/AvalonDock;component/Images/InnerDockPaneBottom.png��6,   $
��None=�6k   5F  #   �� 5G  "   $'��!PART_AnchorablePaneDropTargetLeft��6)   $��0��6R   $��1��6b   ._�5H  &   �� $8��2/AvalonDock;component/Images/InnerDockPaneLeft.png��6,   $
��None=�6i   5I  #   �� 5J  "   $'��!PART_AnchorablePaneDropTargetInto��6)   $��1��6R   $��1��6b   ._�5K  &   �� $:��4/AvalonDock;component/Images/InnerDockPaneInside.png��6,   $
��None=�6k   5L  #   5M     5N     � 5O     $"2 PART_DocumentPaneDropTargets��6   .+�5P     � $��88��6#   $��88��6.   $��Center=�6:   $��Center=�6U    5Q  "   �� 5R  &   �� 5S  &   �� 5T  &    5V  "   �� 5W  &   �� 5X  &   �� 5Y  &   .+�5[  "   �� $��3��6(   $��3��6<   $4��./AvalonDock;component/Images/DockPaneEmpty.PNG��6M   $��Uniform=�6�   �� 5\  "   $$��PART_DocumentPaneDropTargetTop��6)   $��1��6O   $��0��6_   ._�5]  &   �� $:��4/AvalonDock;component/Images/InnerDockDocPaneTop.png��6,   $
��None=�6k   5^  #   �� 5_  "   $&�� PART_DocumentPaneDropTargetRight��6)   $��2��6Q   $��1��6a   ._�5`  &   �� $<��6/AvalonDock;component/Images/InnerDockDocPaneRight.png��6,   $
��None=�6m   5a  #   �� 5b  "   $'��!PART_DocumentPaneDropTargetBottom��6)   $��1��6R   $��2��6b   ._�5c  &   �� $=��7/AvalonDock;component/Images/InnerDockDocPaneBottom.png��6,   $
��None=�6n   5d  #   �� 5e  "   $%��PART_DocumentPaneDropTargetLeft��6)   $��0��6P   $��1��6`   ._�5f  &   �� $;��5/AvalonDock;component/Images/InnerDockDocPaneLeft.png��6,   $
��None=�6l   5g  #   �� 5h  "   $%��PART_DocumentPaneDropTargetInto��6)   $��1��6P   $��1��6`   ._�5i  &   �� $:��4/AvalonDock;component/Images/InnerDockPaneInside.png��6,   $
��None=�6k   5j  #   5k     5l     5m     5n     5p     5q     �� 5s     .R�5t  
   �� $��CharacterEllipsis=�6+   ��6   �� *Title+6+   5u     �� 5w     .R�5x  
   �  5y     �� 5z     $
��Auto �6#   �� 5{     .+�5}     �� $
3 icon��6   $��Uniform=�6u   ��6"   �� *
IconSource+8   6u   �� 5~     $4 title��6   $��4,0,0,0q�6'   $��CharacterEllipsis=�6O   $��1��6p   ��68   �� *Title+6p   5     5 5�  
   �� 5�      6   �� *
IconSource+ 69   Z� .a�5�     �� $ title��65    � ��$ 0q�6+   65   5�     5�     �� 5�     .R�5�  
   �� $��CharacterEllipsis=�6+   ��6   �� *Title+6+   5�     �� 5�     .R�5�  
   �� $��CharacterEllipsis=�6+   ��6   �� *Title+6+   5�     �� 5�     .R�5�  
   �� $��Uniform=�6c   ��6   �� *
IconSource+8   6c   5�     �� 5�     .@�5�  
   m� #��Z6 6   7 5�     �� $ FloatCommand�m� 5�  
   #��Z8 6   7 5�     �� $ DockCommand�m� 5�  
   #��Z9 6   7 5�     �� $ DockAsDocumentCommand�m� 5�  
   #��Z: 6   7 5�     �� $ AutoHideCommand�m� 5�  
   #��Z; 6   7 5�     �� $ CloseCommand�y�5�     �� $ 	IsEnabled� �� *Self+$ OneWay=�8   5�     �� 5�     .@�5�  
   m� #��Z< 6   7 5�     �� $ CloseCommand�y�5�     �� $ 	IsEnabled� �� *Self+$ OneWay=�8   m� 5�  
   #��Z= 6   7 5�     �� $ CloseAllButThisCommand�m� 5�  
   #��Z> 6   7 5�     �� $ FloatCommand�m� 5�  
   #��Z? 6   7 5�     �� $ DockAsDocumentCommand�m� 5�  
   #��Z@ 6   7 5�     �� $" NewHorizontalTabGroupCommand�y�5�     �� $ 	IsEnabled� �� *Self+$ OneWay=�8  m� 5�  
   #��ZA 6   7 5�     �� $  NewVerticalTabGroupCommand�y�5�     �� $ 	IsEnabled� �� *Self+$ OneWay=�8  m� 5�  
   #��ZB 6   7 5�     �� $ MoveToNextTabGroupCommand�y�5�     �� $ 	IsEnabled� �� *Self+$ OneWay=�8  m� 5�  
   #��ZC 6   7 5�     �� $# MoveToPreviousTabGroupCommand�y�5�     �� $ 	IsEnabled� �� *Self+$ OneWay=�8  5�     �� 5�     "   67   .�5�  
   ��   �@ DocumentPaneControlStyle8   65   �� 5�  
   " �@ AnchorablePaneControlStyle8  67   �� 5�  
    �@ AnchorSideTemplate8  6/   �� 5�  
    �@ AnchorGroupTemplate8  60   �� 5�  
    �@ AnchorTemplate8  6+   �� 5�  
    �@ DocumentHeaderTemplate8  63   �� 5�  
     �@ AnchorableHeaderTemplate8  65   �� 5�  
    �@ DocumentTitleTemplate8  62   �� 5�  
    �@ AnchorableTitleTemplate8  64   �� 5�  
    �@ AnchorableContextMenu8 	 62   �� 5�  
    �@ DocumentContextMenu8 
 60   �� 5�  
   * �@ "DocumentPaneMenuItemHeaderTemplate8  6?   �� 5�  
    �@ IconContentTemplate8  60   �� 5�  
    � �� 5�     �� 5�     "  6"   .R�5�     �� #��z��6   #��z��5�     #��z��5�     ._�5�     � $��LeftToRight=�6    5�     �� 5�  "   $
��Auto �60   �� 5�  "   $��* �60   �� 5�  "   $
��Auto �60    5�     �� 5�  "   $
��Auto �63   �� 5�  "   $��* �63   �� 5�  "   $
��Auto �63   .+�5�     �� #��zD 6/   $��1��6[   $��1��6k   �� 5�     #��zE 6/   $��2��6Z   $��1��6j   �� 5�     #��zF 6/   $��0��6Y   $��1��6i   �� 5�     #��zG 6/   $��1��6X   $��0��6h   �� 5�     #��zH 6/   $��1��6[   $��2��6k   �� 5�     $ PART_AutoHideArea��5�  !   #��zI 5�  !   #��z��5�  !   #��z��5�  !   $��1��5�  !   $��1��61   5�     5�     5�     5�     5�     �� 5�     "   6I   .�5�  
   ��  � ��
 ���� 6'   �� 5�  
    � �� 5�     �� 5�     "  6"   .R�5�     �� #��z��6   5�     5�     5�     �� 5�     "   6   .�5�  
   ��  � �� 5�     �� 5�     "  6"   .R�5�     �� #��z��6   #��z��5�     #��z��5�     ._�5�     �� ��6+   �� *Model.Content+ �� *TemplatedParent+��5�  +   �� *LayoutItemTemplate+$ OneWay=� �� "   $ FindAncestor=���5   +   �� *LayoutItemTemplateSelector+$ OneWay=� �� "   $ FindAncestor=�5     5     5     5     �� 5     "   6   .�5  
   ��  � �� 5	     �� 5
     "  6"   .R�5       # 5     �� *" Root.Manager.DocumentContextMenu+$ 5     �� *
LayoutItem+ �� *TemplatedParent+.��5     �� #��z��6!   #��z��5     #��z��5     ._�5     �  5  "   �� 5  &   $��* �67   �� 5  &   $
��Auto �67   .+�5  "   �� $��2��6)   
������ 6=   �� 5  "   ��63   �� *Model+ �� *TemplatedParent+��5  /   �� *DocumentHeaderTemplate+$ OneWay=� �� "   $ FindAncestor=���5  /   �� * DocumentHeaderTemplateSelector+$ OneWay=� �� "   $ FindAncestor=��� 5  "   $( DocumentCloseButton��6)   8��  $��1��6�   $��5,0,0,0q�6�   $y�Hidden=�6�   #& Z< 5  )   ��5  )   �� $ LayoutItem.CloseCommand� �� *TemplatedParent+.��5  &   �� $/��)/AvalonDock;component/Images/PinClose.png��6,   5  #   5      5!     5"      5#     �� 5$     $
 true��6J    6&   �� $ 
IsSelected�.a�5%     �� $ DocumentCloseButton��6K    � y�$ Visible=�6;   6K   5&     P� 5'      �@J�IsMouseOver . . �5(     �� $ DocumentCloseButton��6K    � y�$ Visible=�6;   6K   5)     �� 5*     $ false��6H    6&   �� $ CanClose�.a�5+     �� $ DocumentCloseButton��6M    � y�$ 	Collapsed=�6;   6M   5,     5.     50     51     �� 53     "   6   .�54  
   ��  � �� 55     �� 56     "  6"   .R�57       # 58     �� *$"Root.Manager.AnchorableContextMenu+$ 59     �� *
LayoutItem+ �� *TemplatedParent+.��5:     �� #��z��6!   #��z��5;  !   #��z��5<  !   ._�5=     � .+�5>  "   �� ��63   �� *Model+ �� *TemplatedParent+��5?  /   �� *AnchorableHeaderTemplate+$ OneWay=� �� "   $ FindAncestor=���5@  /   �� *" AnchorableHeaderTemplateSelector+$ OneWay=� �� "   $ FindAncestor=�  5A  "   $��0��5B  %   # 5C  %   �� **(Model.Root.Manager.AnchorableContextMenu+ �� *TemplatedParent+$ 5D  %   �� $ Model� �� *TemplatedParent+5E     5F     5G     5H     5J     5K     �� 5M     "   6   .�5N  
   ��  � �� 5O     �� 5P     "  6"   .R�5Q     �� #��z��6   #��z��5R     #��z��5S     $ Cycle=�5T     ._�5U     �  5V     �� 5W  "   $
��Auto �60   �� 5X  "   $��* �60   .+�5Z     �� $ Header��6%   ._�5[  "     J 6I   �� *Model+ �� *TemplatedParent+5\     �� 5^     #��z��5_  !   $��1��5c  !   ��5`  !   �� *Model.Content+ �� *TemplatedParent+��5a  !   �� *LayoutItemTemplate+$ OneWay=� �� "   $ FindAncestor=���5b  !   �� *LayoutItemTemplateSelector+$ OneWay=� �� "   $ FindAncestor=�5c  !   5d     5e      5f     ^� 5g     K 5h     �� 5i  "   L 6,   ��  �� *Self+$ Model.IsFloating�M True6|   �� 5j  "   L 6,   ��  �� *Self+$3 -Model.Parent.IsDirectlyHostedInFloatingWindow�M True6�   .0�5l     �� $ Header��6M    � y�$ 	Collapsed=�6;   6M   5m     5o     5q     5r     �� 5t     "   6T   .�5u  
   ��  � ��# � ��6'   �� 5v  
    �@ WindowChrome 5w       5x     $N 3q�5y     $O 16Q�5z     $P 3,3,3,3��5{     $Q 0q�5|     R .  5}     5     �� 5�  
    � �� 5�     �� 5�     "  6"   .R�5�     � .+�5�     �� $ WindowBorder��6!   $��3q�67   #��� ��6K   #��� ��6�   ._�5�     � $��3q�6#    5�  "   �� 5�  &   $��16 �64   �� 5�  &   $��* �64   .+�5�  "   �  5�  &   �� 5�  *   $��* �6;   �� 5�  *   $
��Auto �6;   �� 5�  *   $
��Auto �6;   �� 5�  *   $
��Auto �6;   .+�5�  &   �� ��67   �� *Model.RootDocument+ �� *TemplatedParent+��5�  /   �� **(Model.Root.Manager.DocumentTitleTemplate+ �� *TemplatedParent+��5�  /   �� *20Model.Root.Manager.DocumentTitleTemplateSelector+ �� *TemplatedParent+�� 5�  &   S . |�.  8��  #��ZT 5�  )   #& ZU 5�  )   $��2��5�  )   y�5�  )   �� *IsMaximized+ �� *TemplatedParent+8  ��5�  )   ��  �� *TemplatedParent+.��5�  *   �� $2��,/AvalonDock;component/Images/PinMaximize.png��60   5�  '   �� 5�  &   S . |�.  8�� #��ZV 5�  )   #& ZW 5�  )   $��2��5�  )   y�5�  )   �� *IsMaximized+ �� *TemplatedParent+8  ��5�  )   ��  �� *TemplatedParent+.��5�  *   �� $1��+/AvalonDock;component/Images/PinRestore.png��60   5�  '   �� 5�  &   S . |�.  8�� #& Z< 5�  -   $��3��5�  -   ��5�  -   �� $) #RootDocumentLayoutItem.CloseCommand� �� *TemplatedParent+y�5�  -   �� $ 	IsEnabled� �� *Self+$ OneWay=�8  .��5�  *   �� $/��)/AvalonDock;component/Images/PinClose.png��60   5�  '   5�  #   �� 5�  "   #��z��63   $��1��6W   5�     5�     5�      5�     P� 5�      �@�WindowState$ 	Maximized=�69   . �5�     �� $ WindowBorder��6B    �@��Padding$ 3q�68   6B   5�     5�     5�     5�     �� 5�     "   6V   .�5�  
   ��  � ��# � ��6'   �� 5�  
    �@ WindowChrome 5�       5�     $N 3q�5�     $O 16Q�5�     $P 3,3,3,3��5�     $Q 0q�5�     R . 5�     5�     �� 5�  
    � �� 5�     �� 5�     "  6"   .R�5�     � .+�5�     �� $ WindowBorder��6!   $��3q�67   #��� ��6K   #��� ��6�   ._�5�     � $��3q�6#    5�  "   �� 5�  &   $��16 �64   �� 5�  &   $��* �64   .+�5�  "   �  5�  &   �� 5�  *   $��* �6;   �� 5�  *   $
��Auto �6;   �� 5�  *   $
��Auto �6;   �� 5�  *   $
��Auto �6;   .+�5�  &   �� y�5�  )   �� $ Model.IsSinglePane� �� *TemplatedParent+8   ._�5�  *     # 5�  -   �� **(Model.Root.Manager.AnchorableContextMenu+ �� *TemplatedParent+$ 5�  -   �� $ SingleContentLayoutItem� �� *TemplatedParent+.��5�  .   �� ��6?   �� *" Model.SinglePane.SelectedContent+ �� *TemplatedParent+��5�  3   �� *,*Model.Root.Manager.AnchorableTitleTemplate+ �� *TemplatedParent+��5�  3   �� *42Model.Root.Manager.AnchorableTitleTemplateSelector+ �� *TemplatedParent+5�  +   5�  '   
  5�  &   $ SinglePaneContextMenu��5�  )   S . 8�� |�.  $��1��5�  )   #& Z% 5�  )   y�5�  )   �� $ Model.IsSinglePane� �� *TemplatedParent+8   5�  )   �� **(Model.Root.Manager.AnchorableContextMenu+ �� *TemplatedParent+' 5�  )   �� $ SingleContentLayoutItem� �� *TemplatedParent+.��5�  *   �� $.��(/AvalonDock;component/Images/PinMenu.png��60   5�  '   �� 5�  &   S . |�.  8�� #��ZT 5   )   #& ZU 5  )   $��2��5  )   y�5�  )   �� *IsMaximized+ �� *TemplatedParent+8  ��5  )   ��  �� *TemplatedParent+.��5  *   �� $2��,/AvalonDock;component/Images/PinMaximize.png��60   5  '   �� 5  &   S . |�.  8�� #��ZV 5  )   #& ZW 5  )   $��2��5  )   y�5
  )   �� *IsMaximized+ �� *TemplatedParent+8  ��5  )   ��  �� *TemplatedParent+.��5  *   �� $1��+/AvalonDock;component/Images/PinRestore.png��60   5  '   �� 5  &   S . |�.  8�� #& Z* 5  )   $��3��5  )   ��5  )   �� *HideWindowCommand+ �� *TemplatedParent+y�5  )   �� $ 	IsEnabled� �� *Self+$ OneWay=�8  .��5  *   �� $/��)/AvalonDock;component/Images/PinClose.png��60   5  '   5  #   �� 5  "   #��z��5  %   $��1��6I   5      5!     5"      5#     P� 5$      �@�WindowState$ 	Maximized=�69   . �5%     �� $ WindowBorder��6B    �@��Padding$ 3q�68   6B   5&     P� 5'      �@J�IsKeyboardFocused . . �5(     �� $ WindowBorder��6%   # � ��6W    � ��5)     5+     5-     5.     �� 50     "    6L   .�51  
   ��  �@  
Background# � ��6'   52     �� 54     "  ! 6@   .�55  
   ��  � ��# � ��6'   �� 56  
    �@�SizeToContent$ WidthAndHeight=�6*   �� 57  
    �@�
ResizeMode$ NoResize=�6'   �� 58  
    �@ WindowChrome 59       5:     $N 3q�5;     $O 16Q�5<     $P 3,3,3,3��5=     $Q 4q�5>     5@     �� 5A  
    � �� 5B     �� 5C     " ! 6"   .R�5D     � .+�5E     �� $ WindowBorder��6!   $��3q�67   #��� ��5F  !   #��� ��5G  !   ._�5H     � $��5q�6#    5I  "   �� 5J  &   $��54 �64   �� 5K  &   $��* �64   �� 5L  &   $��42 �64   .+�5O  "   �  5P  &   �� 5Q  *   �� 5R  *   .+�5T  &   �  5U  *   �� 5V  .   $
��Auto �6?   �� 5W  .   .+�5Y  *   �� $
��None=�6�   ��60   �� *+)SelectedDocument.LayoutElement.IconSource+8   6�   �� 5Z  *   $��CharacterEllipsis=�6j   $��1��6�   $��Center=�6�   $
��Bold"�6�   $��4,0,0,0q�6�   ��64   �� *&$SelectedDocument.LayoutElement.Title+6�   5[  '   �� 5\  &   $��CharacterEllipsis=�6l   $��Center=�6�   ��60   �� *,*SelectedDocument.LayoutElement.Description+6�   5]  #   � 5_  "   $��1��6'    5`  &   �� 5a  *   $
��Auto �6;   �� 5b  *   .+�5d  &   � $��5q�6+    5e  *   �� 5f  .   $
��Auto �6<   �� 5g  .   .+�5i  *   �� $��Active Tool Windows��64   $
��Bold"�6O   $��0,3,0,4q�6a   �� 5j  *   $��1��62   
������ $��0q�6�   $	��400��6�   ��6?   �� *Anchorables+X 6c   �� *SelectedAnchorable+$ TwoWay=�Y 6�   Z� ��5k  .   �� 5l  2   "  ��68   .�5m  6   ��  �@�Cursor$
 Hand��6O    5n  6   P� 5o  :    �@J�IsMouseOver . . �5p  >   ��  � ��# � ��6[   �� 5q  >    � ��# � ��6g   5r  ;   5t  3   ��5v  .   �� 5w  2   .R�5x  6   �  5y  :   �� 5z  >   $
��Auto �6O   �� 5{  >   .+�5}  :   �� $
��None=�6�   ��6@   �� *LayoutElement.IconSource+8  6�   �� 5~  :   $��CharacterEllipsis=�6i   $��1��6�   $��4,2,0,2q�6�   ��6D   �� *LayoutElement.Title+6�   5  7   5�  3   5�  +   5�  '   � 5�  &   $��1��6+   $��5q�6;    5�  *   �� 5�  .   $
��Auto �6<   �� 5�  .   .+�5�  *   �� $��Active Files��64   $
��Bold"�6H   $��0,3,0,4q�6Z   �� 5�  *   $��1��62   
������ $��0q�6�   $	��400��6�   ��6?   �� *	Documents+X 6a   �� *SelectedDocument+$ TwoWay=�Y 6�   Z� ��5�  .   �� 5�  2   "  ��68   .�5�  6   ��  �@�Cursor$
 Hand��6O    5�  6   P� 5�  :    �@J�IsMouseOver . . �5�  >   ��  � ��# � ��6[   �� 5�  >    � ��# � ��6g   5�  ;   5�  3   ��5�  .   �� 5�  2   .R�5�  6   �  5�  :   �� 5�  >   $
��Auto �6O   �� 5�  >   .+�5�  :   �� $
��None=�6�   ��6@   �� *LayoutElement.IconSource+8  6�   �� 5�  :   $��CharacterEllipsis=�6i   $��1��6�   $��4,2,0,2q�6�   ��6D   �� *LayoutElement.Title+6�   5�  7   5�  3   ��5�  .   �� 5�  2   .R�5�  6   � $Z Vertical=�6@   5�  3   5�  +   5�  '   5�  #   � 5�  "   $��2��6'   .+�5�  &   �� $��Center=�6h   ��60   �� *(&SelectedDocument.LayoutElement.ToolTip+6h   5�  #   5�     5�     5�     5�     5�     5�     5�     
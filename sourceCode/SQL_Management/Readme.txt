
        <avalonDock:DockingManager Grid.Row="1" x:Name="dockManager" AllowMixedOrientation="True">
            <!--使用皮肤-->
            <avalonDock:DockingManager.Theme>
                <avalonDock:AeroTheme/>
            </avalonDock:DockingManager.Theme>
            <!--标头模板-->
            <avalonDock:DockingManager.DocumentHeaderTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <!--列表前面的图标<Image Source="{Binding IconSource}" Margin="0,0,4,0"/>-->
                        <TextBlock Text="{Binding Title}" TextTrimming="CharacterEllipsis"/>
                    </StackPanel>
                </DataTemplate>
            </avalonDock:DockingManager.DocumentHeaderTemplate>

            <!--主体部分-->
            <avalonDock:LayoutRoot>
                <!--左面隐藏元素-->
                <avalonDock:LayoutRoot.RightSide>
                    <avalonDock:LayoutAnchorSide>
                        <avalonDock:LayoutAnchorGroup>
                            <avalonDock:LayoutAnchorable Title="AutoHide3 Content" ContentId="AutoHide1Content" >
                                <TextBox Text="{Binding TestTimer, Mode=OneWay, StringFormat='AutoHide Attached to Timer ->\{0\}'}"/>
                            </avalonDock:LayoutAnchorable>
                            <avalonDock:LayoutAnchorable Title="AutoHide4 Content" ContentId="AutoHide2Content">
                                <StackPanel Orientation="Vertical">
                                    <TextBox/>
                                    <TextBox/>
                                </StackPanel>
                            </avalonDock:LayoutAnchorable>
                        </avalonDock:LayoutAnchorGroup>
                    </avalonDock:LayoutAnchorSide>
                </avalonDock:LayoutRoot.RightSide>


                <avalonDock:LayoutPanel Orientation="Horizontal">
                    <!--左面打开窗口-->
                    <avalonDock:LayoutAnchorablePane DockWidth="100">
                        <avalonDock:LayoutAnchorable x:Name="WinFormsWindow"  ContentId="WinFormsWindow" Title="WinForms Window" ToolTip="My WinForms Tool" CanHide="False" CanClose="False" >

                        </avalonDock:LayoutAnchorable>
                    </avalonDock:LayoutAnchorablePane>

                    <!--中间的元素-->
                    <avalonDock:LayoutDocumentPaneGroup>
                        <avalonDock:LayoutDocumentPane>
                            <avalonDock:LayoutDocument ContentId="document1" Title="Document 1">
                                <StackPanel>
                                    <TextBox Text="Document 1 Content"/>
                                    <Button Content="Click to add 2 documents"/>
                                </StackPanel>
                            </avalonDock:LayoutDocument>
                            <avalonDock:LayoutDocument ContentId="document2" Title="Document 2">
                                <TextBox Text="{Binding TestTimer, Mode=OneWay, StringFormat='Document 2 Attached to Timer ->\{0\}'}"/>
                            </avalonDock:LayoutDocument>
                        </avalonDock:LayoutDocumentPane>
                    </avalonDock:LayoutDocumentPaneGroup>

                    <!--右面打开窗口-->
                    <avalonDock:LayoutAnchorablePaneGroup DockWidth="150">
                        <avalonDock:LayoutAnchorablePane>
                            <avalonDock:LayoutAnchorable ContentId="toolWindow1" Title="Tool Window 1">
                                <TextBox Text="{Binding TestTimer, Mode=OneWay, StringFormat='Tool Window 1 Attached to Timer ->\{0\}'}"/>
                            </avalonDock:LayoutAnchorable>
                            <avalonDock:LayoutAnchorable ContentId="toolWindow2" Title="Tool Window 2">
                                <TextBlock Text="{Binding FocusedElement}"/>
                            </avalonDock:LayoutAnchorable>
                        </avalonDock:LayoutAnchorablePane>
                    </avalonDock:LayoutAnchorablePaneGroup>

                </avalonDock:LayoutPanel>

                <!--左边隐藏的元素-->
                <avalonDock:LayoutRoot.LeftSide>
                    <avalonDock:LayoutAnchorSide>
                        <avalonDock:LayoutAnchorGroup>
                            <avalonDock:LayoutAnchorable Title="AutoHide1 Content" ContentId="AutoHide1Content" >
                                <TextBox Text="{Binding TestTimer, Mode=OneWay, StringFormat='AutoHide Attached to Timer ->\{0\}'}"/>
                            </avalonDock:LayoutAnchorable>
                            <avalonDock:LayoutAnchorable Title="AutoHide2 Content" ContentId="AutoHide2Content">
                                <StackPanel Orientation="Vertical">
                                    <TextBox/>
                                    <TextBox/>
                                </StackPanel>
                            </avalonDock:LayoutAnchorable>
                        </avalonDock:LayoutAnchorGroup>
                    </avalonDock:LayoutAnchorSide>
                </avalonDock:LayoutRoot.LeftSide>
            </avalonDock:LayoutRoot>
        </avalonDock:DockingManager>
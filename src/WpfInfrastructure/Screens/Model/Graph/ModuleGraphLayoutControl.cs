/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphSharp.Controls;
using QuickGraph.Objects;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model.Graph
{
    public class VertexClickEventArgs : EventArgs
    {
        public VertexClickEventArgs(ModuleVertexViewModelBase vertex)
        {
            Vertex = vertex;
        }

        public ModuleVertexViewModelBase Vertex { get; private set; }
    }

    public class EdgeClickEventArgs : EventArgs
    {
        public EdgeClickEventArgs(ModuleEdgeViewModel edge)
        {
            Edge = edge;
        }

        public ModuleEdgeViewModel Edge { get; private set; }
    }

    public class ModuleGraphLayoutControl : GraphLayout<ModuleVertexViewModelBase, ModuleEdgeViewModel, ModuleGraph>
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static bool _isVertexMovingEnabled;
        private static bool _isEdgeForcingEnabled;

        static ModuleGraphLayoutControl()
		{
			EventManager.RegisterClassHandler( typeof( VertexControl ), Mouse.MouseMoveEvent,
															new MouseEventHandler( OnVertexControlMouseMove ) );

			EventManager.RegisterClassHandler( typeof( VertexControl ), Mouse.MouseDownEvent,
															new MouseButtonEventHandler( OnVertexControlMouseDown ) );

			EventManager.RegisterClassHandler( typeof( VertexControl ), Mouse.MouseUpEvent,
															new MouseButtonEventHandler( OnVertexControlMouseUp ) );

			EventManager.RegisterClassHandler( typeof( EdgeControl ), Mouse.MouseDownEvent,
															new MouseButtonEventHandler( OnEdgeControlMouseDown ) );

			EventManager.RegisterClassHandler( typeof( EdgeControl ), Mouse.MouseUpEvent,
															new MouseButtonEventHandler( OnEdgeControlMouseUp ) );
		}

        public ModuleGraphLayoutControl()
        {
            EventManager.RegisterClassHandler(typeof (VertexControl), Mouse.MouseMoveEvent,
                                              new MouseEventHandler(OnVertexControlMouseMove));

            EventManager.RegisterClassHandler(typeof (VertexControl), Mouse.MouseDownEvent,
                                              new MouseButtonEventHandler(OnVertexControlMouseDown));

            EventManager.RegisterClassHandler(typeof (VertexControl), Mouse.MouseUpEvent,
                                              new MouseButtonEventHandler(OnVertexControlMouseUp));

            EventManager.RegisterClassHandler(typeof (EdgeControl), Mouse.MouseDownEvent,
                                              new MouseButtonEventHandler(OnEdgeControlMouseDown));

            EventManager.RegisterClassHandler(typeof (EdgeControl), Mouse.MouseUpEvent,
                                              new MouseButtonEventHandler(OnEdgeControlMouseUp));
        }

        public static event EventHandler<VertexClickEventArgs> VertextClickEvent;
        public static event EventHandler<EdgeClickEventArgs> EdgeClickEvent;
        public static event EventHandler NewVertexPositionsEvent;

        public bool IsVertexMovingEnabled
        {
            get { return (bool) GetValue(IsVertexMovingEnabledProperty); }
            set { SetValue(IsVertexMovingEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsVertexMovingEnabledProperty =
            DependencyProperty.Register("IsVertexMovingEnabled", typeof (bool), typeof (ModuleGraphLayoutControl),
                new PropertyMetadata(false, (dpo, ea) => _isVertexMovingEnabled = (bool)ea.NewValue));

        public bool IsEdgeForcingEnabled
        {
            get { return (bool) GetValue(IsEdgeForcingEnabledProperty); }
            set { SetValue(IsEdgeForcingEnabledProperty, value);}
        }

        public static readonly DependencyProperty IsEdgeForcingEnabledProperty =
            DependencyProperty.Register("IsEdgeForcingEnabled", typeof (bool), typeof (ModuleGraphLayoutControl),
                new PropertyMetadata(false, (dpo, ea) => _isEdgeForcingEnabled = (bool)ea.NewValue));

        private static void OnVertexControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            //+-------------------------------------
            //+ Simulate left button click behavior
            //+-------------------------------------
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Stopwatch.Start();
            }
        }

        private static void OnVertexControlMouseUp(object sender, MouseButtonEventArgs e)
        {
            //+-------------------------------------
            //+ Simulate left button click behavior
            //+-------------------------------------
            if (e.LeftButton == MouseButtonState.Released)
            {
                var vertexControl = (VertexControl)sender;
                    if (vertexControl == null) 
                        return;

                var vertex = vertexControl.Vertex as ModuleVertexViewModelBase;
                if (vertex == null) 
                    return;

                if (Stopwatch.IsRunning && Stopwatch.ElapsedMilliseconds < 200)
                {
                    if (VertextClickEvent != null)
                       VertextClickEvent(vertexControl, new VertexClickEventArgs(vertex));
                }

                Stopwatch.Reset();

                if (_vertexPositionChanged)
                {
                    _vertexPositionChanged = false;

                    var currentPosition = new Point { X = GetX(vertexControl), Y = GetY(vertexControl) };
                    vertex.Position = currentPosition;

                    if (NewVertexPositionsEvent != null)
                        NewVertexPositionsEvent(vertexControl, new EventArgs());
                }
            }
        }

        private static Point _oldMousePos;
        private static bool _vertexPositionChanged;

        private static void OnVertexControlMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition((Control)sender) != _oldMousePos)
                _vertexPositionChanged = true;

            _oldMousePos = e.GetPosition((Control) sender);

            //+------------------------------
            //+ Turn off Drag / Move behavior
            //+------------------------------
            //Debug.WriteLine( "MouseMove" );
            //_isVertexMovingEnabled = false; THIS IS WHEN IS FALSE DISABLES THE MOVING!!!
           // _isVertexMovingEnabled = false;
            e.Handled = !_isVertexMovingEnabled;//this is where IT IS TO BE CHANGED!!!
        }

        private static void OnEdgeControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            //+-------------------------------------
            //+ Simulate left button click behavior
            //+-------------------------------------
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Stopwatch.Start();
            }
        }

        private static void OnEdgeControlMouseUp(object sender, MouseButtonEventArgs e)
        {
            //+-------------------------------------
            //+ Simulate left button click behavior
            //+-------------------------------------
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (Stopwatch.IsRunning && Stopwatch.ElapsedMilliseconds < 200)
                {
                    var edgeControl = (EdgeControl) sender;

                    if (edgeControl == null) 
                        return;

                    var edge = edgeControl.Edge as ModuleEdgeViewModel;
                    if (edge == null) 
                        return;

                    if (_isEdgeForcingEnabled)
                    {
                        edge.IsForcingEnabled = !edge.IsForcingEnabled;
                    }

                    if (EdgeClickEvent != null)
                        EdgeClickEvent(edgeControl, new EdgeClickEventArgs(edge));
                }

                Stopwatch.Reset();
            }
        }

        protected override void RunCreationTransition(Control control)
        {
            //+--------------------------------
            //+ Turn off the creation animation
            //+--------------------------------
            //base.RunCreationTransition( control );
        }

        protected override void RunDestructionTransition(Control control, bool dontRemoveAfter)
        {
            //+---------------------------------------
            //+ Turn off the destruction animation ...
            //+---------------------------------------
            //base.RunDestructionTransition( control, dontRemoveAfter );

            //+------------------------------------------------
            //+ ... and remove all objects (vertices and edges)
            //+------------------------------------------------
            Children.Remove(control);
        }

        protected override void OnLayoutFinished()
        {
            base.OnLayoutFinished();

            //+-------------------------------------------
            //+ ... set the fixed position of all vertices
            //+-------------------------------------------
            foreach (var vertex in from vertex 
                                   in Graph.Vertices
                                   .Where(vertex => vertex != null &&
                                          !vertex.Position.X.Equals(0.0) &&
                                          !vertex.Position.Y.Equals(0.0))
                                   let vertexControl = GetVertexControl(vertex)
                                   where vertexControl != null
                                   select vertex)
            {
                SetX(GetVertexControl(vertex), vertex.Position.X);
                SetY(GetVertexControl(vertex), vertex.Position.Y);
            }

            foreach (var moduleEdgeViewModel in Graph.Edges)
            {
                var edge = GetEdgeControl(moduleEdgeViewModel);
                edge.StrokeThickness = 12;
            }
        }
    }
}

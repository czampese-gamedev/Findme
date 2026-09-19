using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

namespace HologramShadersPro
{
    public class HologramShaderGUI : ShaderGUI
    {
        private struct HologramProperty
        {
            public MaterialProperty prop;
            public readonly string name;
            public readonly GUIContent info;
            public readonly int id;

            private HologramProperty(string name, GUIContent info)
            {
                prop = null;
                this.name = name;
                this.info = info;
                id = Shader.PropertyToID(name);
            }

            public HologramProperty(string name, string prettyName, string description) : 
                this(name, new GUIContent(prettyName, description)) { }
        }

        // Basic PBR Properties.
        private HologramProperty _outputMode = new("_OUTPUT_MODE", "Output Mode", "Should the shader output to Base Color, Emission, or Both? Base Color is subject to shading based on lights, Emission always outputs colors at full strength.");
        private HologramProperty _baseEmissionColor = new("_BaseEmissionColor", "Base Color", "Controls the color of the hologram.");
        private HologramProperty _baseEmissionTexture = new("_BaseEmissionTexture", "Base Texture", "Also controls the color of the hologram, multiplied by Base Color.");
        private HologramProperty _normalTexture = new("_NormalTexture", "Normal Texture", "Controls the lighting on the mesh surface. This only has an effect on Base Color outputs.");
        private HologramProperty _normalStrength = new("_NormalStrength", "Normal Strength", "How strongly the normal texture influences surface normal vectors.");
        private HologramProperty _alphaClipThreshold = new("_AlphaClipThreshold", "Alpha Clip Threshold", "Any pixel with an alpha output below this value is culled.");
        private HologramProperty _metallic = new("_Metallic", "Metallic", "How metallic the object is.");
        private HologramProperty _smoothness = new("_Smoothness", "Smoothness", "How smooth the surface of the object is.");
        private HologramProperty _ambientOcclusion = new("_AmbientOcclusion", "Ambient Occlusion", "The amount of ambient occlusion on the surface of the object.");

        // Scanline Properties.
        private HologramProperty _scanlineMode = new("_SCANLINE_MODE", "Scanline Mode", "Choose whether to simulate the scanline pattern in Screen Space, World Space, or turn them off completely.");
        private HologramProperty _scanlineRotation = new("_ScanlineRotation", "Scanline Rotation", "Rotate the scanline pattern, in radians.");
        private HologramProperty _scanlineDirection = new("_ScanlineDirection", "Scanline Direction", "Orient the scanline pattern in this direction in world space.");
        private HologramProperty _scanlineTexture = new("_ScanlineTexture", "Scanline Texture", "The scanline pattern which will be overlaid onto the object.");
        private HologramProperty _scanlineVelocity = new("_ScanlineVelocity", "Scanline Velocity", "How quickly the scanline pattern scrolls across the object.");
        private HologramProperty _scanlineMinMaxAlpha = new("_ScanlineMinMaxAlpha", "Scanline Min-Max Alpha", "Remap the range of values in the scanline texture to this range instead. Black pixels from the scanline texture will take on the first (x) value, and white pixels from the scanline texture will take on the second (y) value.");

        // Vertex Glitch Properties.
        private HologramProperty _useVertexGlitches = new("_USE_VERTEX_GLITCHES", "Use Vertex Glitches?", "Should the hologram use glitches where individual vertices are pulled away from the mesh one at a time?");
        private HologramProperty _glitchSensitivity = new("_GlitchSensitivity", "Glitch Sensitivity", "Only vertices which roll a random number above this value in a given frame are glitched. Higher values mean fewer glitch vertices.");
        private HologramProperty _glitchNormalMultiplier = new("_GlitchNormalMultiplier", "Glitch Normal Multiplier", "A multiplier weight value applied to each component of the normal vector for calculating the offsets. e.g. (1, 0, 1) will pull vertices only along the X-Z plane.");
        private HologramProperty _glitchStrength = new("_GlitchStrength", "Glitch Strength", "How strongly each vertex gets pulled away from the mesh.");
        private HologramProperty _glitchOffset = new("_GlitchOffset", "Glitch Offset", "A time offset value that lets you use the same parameters for two identical objects, without them glitching in the same places each frame.");
        private HologramProperty _glitchFrequency = new("_GlitchFrequency", "Glitch Frequency", "How frequently glitches occur.");

        // Segment Glitch Properties.
        private HologramProperty _useSliceGlitches = new("_USE_SLICE_GLITCHES", "Use Slice Glitches?", "Should the hologram use glitches where a scrolling section of the mesh between two height values are pulled away from the mesh uniformly?");
        private HologramProperty _sliceWidth = new("_SliceWidth", "Slice Width", "The width of the slice segment at any one point in time, in world space units.");
        private HologramProperty _sliceSpeed = new("_GlitchSpeed", "Slice Speed", "How quickly the segment window scrolls over the object (the window still scrolls even while the glitch isn't active).");
        private HologramProperty _sliceFrequency = new("_SliceFrequency", "Slice Frequency", "How frequently the slice becomes active.");
        private HologramProperty _sliceJitter = new("_SliceJitter", "Slice Jitter", "A randomness factor added to the frequency value.");
        private HologramProperty _sliceDuration = new("_SliceDuration", "Slice Duration", "How long, in seconds, each instance of an active segment glitch persists for.");
        private HologramProperty _sliceDirection = new("_SliceDirection", "Slice Direction", "Which direction the scrolling segment window travels in.");

        // Noise Properties.
        private HologramProperty _noiseSpeed = new("_NoiseSpeed", "Noise Speed", "How fast the shader scrolls through random noise values.");
        private HologramProperty _noiseScale = new("_NoiseScale", "Noise Scale", "How fine or coarse the noise values are in screen space.");
        private HologramProperty _noiseStrength = new("_NoiseStrength", "Noise Strength", "How strongly the noise colors are overlaid onto the mesh.");
        private HologramProperty _noiseColor = new("_NoiseColor", "Noise Color", "The color of the noise values.");

        // Color Gradient Properties.
        private HologramProperty _gradientSpace = new("_GRADIENT_SPACE", "Gradient Space", "Which space should the gradient be calculated in?\n  Object space: relative to the mesh pivot point.\n  World space: relative the scene origin point.\n  Screen space: relative to position on the screen.");
        private HologramProperty _baseEmissionColor2 = new("_BaseEmissionColor2", "Base Color 2", "Controls the color of the other side of the hologram.");
        private HologramProperty _fresnelColor2 = new("_FresnelColor2", "Fresnel Color 2", "The color of the Fresnel highlight on the other side of the hologram.");
        private HologramProperty _gradientMinMaxY = new("_GradientMinMaxY", "Gradient Min-Max Y", "Which height values should be used as the start and end of the 'intermediate' portion of the gradient. Height values below the first (x) value output Base Color. Height values above the second (y) value output Base Color 2.");

        // Grid Properties.
        private HologramProperty _perAxisStrength = new("_PerAxisStrength", "Per Axis Strength", "How strongly the gridline appears per-axis.");
        private HologramProperty _rotationAxis = new("_RotationAxis", "Rotation Axis", "An axis, in world space, around which to rotate the gridline system.");
        private HologramProperty _gridDensity = new("_GridDensity", "Grid Density", "How closely packed the gridlines are.");
        private HologramProperty _lineThickness = new("_LineThickness", "Line Thickness", "How thick each gridline is.");
        private HologramProperty _lineFalloff = new("_LineFalloff", "Line Falloff", "A falloff value between the edge of a gridline and empty space. Useful for softening the edge of the gridlines.");
        private HologramProperty _gridOffset = new("_GridOffset", "Grid Offset", "How far to offset the gridline axis in world space.");
        private HologramProperty _gridVelocity = new("_GridVelocity", "Grid Velocity", "How quickly the gridlines scroll in world space.");

        // Dot Matrix Properties.
        private HologramProperty _dotSize = new("_DotSize", "Dot Size", "How large each dot is, as a number of square pixels.");
        private HologramProperty _dotSpace = new("_DotSpace", "Dot Space", "How large the space is between each dot, as a number of pixels.");

        private HologramProperty _dotsAreTransparent = new("_DotsAreTransparent", "Dots Are Transparent", "When enabled, the object uses full opacity except where the dots exist, which are transparent.");
        
        // Grid and Dot Matrix Common Properties.
        private HologramProperty _rotationRadians = new("_RotationRadians", "Rotation Radians", "How much to rotate the gridline system in radians.");

        // Fresnel Properties.
        private HologramProperty _fresnelPower = new("_FresnelPower", "Fresnel Power", "How large the Fresnel highlight appears. Higher values mean a thinner rim highlight.");
        private HologramProperty _fresnelColor = new("_FresnelColor", "Fresnel Color", "The color of the Fresnel highlight.");
        private HologramProperty _useSceneIntersections = new("_USE_SCENE_INTERSECTIONS", "Use Scene Intersections?", "Should the shader add a highlight to parts of the object that intersect other objects? This functionality depends on the depth buffer to detect intersections.");
        private HologramProperty _intersectionPower = new("_IntersectionPower", "Intersection Power", "How sensitively the shader should detect intersections. Higher values mean a thinner intersection.");

        // Color Separation Properties.
        private HologramProperty _useColorSeparation = new("_USE_COLOR_SEPARATION", "Use Color Separation?", "");
        private HologramProperty _channelSeparationDirection = new("_ChannelSeparationDirection", "Channel Separation", "");
        
        // Time Properties.
        private HologramProperty _useUnscaledTime = new("_USE_UNSCALED_TIME", "Use Unscaled Time?", "Should the shader use regular time, or unscaled time? Regular (scaled) time values are affected by Time.timeScale as set within a C# script.");
        private const string _unscaledTimeInfoMessage = "Unity does not provide an unscaled time parameter for shaders. This must be manually fed to the shader via a script. See Hologram Shaders Pro/Scripts/UnscaledTime.cs for an example.";

        // Dynamic Resolution Properties.
        private HologramProperty _upscalingAmount = new("_UpscalingAmount", "Upscaling Amount", "How large the pre-upscaling resolution is, as a proportion of the full screen size. This value is useful when using dynamic resolution (e.g. FSR or DLSS), as Unity will use the pre-upscaling resolution in shaders.");

        // Shader Graph Advanced Options.
        private HologramProperty _queueOffset = new("_QueueOffset", "Sorting Priority", "Determines the chronological rendering order for a Material. Materials with lower value are rendered first.");
        private const int _queueOffsetRange = 50;

        private readonly MaterialHeaderScopeList _materialScopeList = new MaterialHeaderScopeList(uint.MaxValue);
        private MaterialEditor _materialEditor;
        private bool _firstTimeOpen = true;

        private void FindProperties(MaterialProperty[] properties)
        {
            // Basic PBR Properties.
            _outputMode.prop = FindProperty(_outputMode.name, properties, true);
            _baseEmissionColor.prop = FindProperty(_baseEmissionColor.name, properties, true);
            _baseEmissionTexture.prop = FindProperty(_baseEmissionTexture.name, properties, true);
            _normalTexture.prop = FindProperty(_normalTexture.name, properties, true);
            _normalStrength.prop = FindProperty(_normalStrength.name, properties, true);
            _alphaClipThreshold.prop = FindProperty(_alphaClipThreshold.name, properties, true);
            _metallic.prop = FindProperty(_metallic.name, properties, true);
            _smoothness.prop = FindProperty(_smoothness.name, properties, true);
            _ambientOcclusion.prop = FindProperty(_ambientOcclusion.name, properties, true);

            // Scanline Properties.
            _scanlineMode.prop = FindProperty(_scanlineMode.name, properties, false);
            _scanlineRotation.prop = FindProperty(_scanlineRotation.name, properties, false);
            _scanlineDirection.prop = FindProperty(_scanlineDirection.name, properties, false);
            _scanlineTexture.prop = FindProperty(_scanlineTexture.name, properties, false);
            _scanlineVelocity.prop = FindProperty(_scanlineVelocity.name, properties, false);
            _scanlineMinMaxAlpha.prop = FindProperty(_scanlineMinMaxAlpha.name, properties, false);

            // Vertex Glitch Properties.
            _useVertexGlitches.prop = FindProperty(_useVertexGlitches.name, properties, false);
            _glitchSensitivity.prop = FindProperty(_glitchSensitivity.name, properties, false);
            _glitchNormalMultiplier.prop = FindProperty(_glitchNormalMultiplier.name, properties, false);
            _glitchStrength.prop = FindProperty(_glitchStrength.name, properties, false);
            _glitchOffset.prop = FindProperty(_glitchOffset.name, properties, false);
            _glitchFrequency.prop = FindProperty(_glitchFrequency.name, properties, false);

            // Segment Glitch Properties.
            _useSliceGlitches.prop = FindProperty(_useSliceGlitches.name, properties, false);
            _sliceWidth.prop = FindProperty(_sliceWidth.name, properties, false);
            _sliceSpeed.prop = FindProperty(_sliceSpeed.name, properties, false);
            _sliceFrequency.prop = FindProperty(_sliceFrequency.name, properties, false);
            _sliceJitter.prop = FindProperty(_sliceJitter.name, properties, false);
            _sliceDuration.prop = FindProperty(_sliceDuration.name, properties, false);
            _sliceDirection.prop = FindProperty(_sliceDirection.name, properties, false);

            // Noise Properties.
            _noiseSpeed.prop = FindProperty(_noiseSpeed.name, properties, false);
            _noiseScale.prop = FindProperty(_noiseScale.name, properties, false);
            _noiseStrength.prop = FindProperty(_noiseStrength.name, properties, false);
            _noiseColor.prop = FindProperty(_noiseColor.name, properties, false);

            // Color Gradient Properties.
            _gradientSpace.prop = FindProperty(_gradientSpace.name, properties, false);
            _baseEmissionColor2.prop = FindProperty(_baseEmissionColor2.name, properties, false);
            _gradientMinMaxY.prop = FindProperty(_gradientMinMaxY.name, properties, false);

            // Grid Properties.
            _perAxisStrength.prop = FindProperty(_perAxisStrength.name, properties, false);
            _gridDensity.prop = FindProperty(_gridDensity.name, properties, false);
            _rotationAxis.prop = FindProperty(_rotationAxis.name, properties, false);
            _lineThickness.prop = FindProperty(_lineThickness.name, properties, false);
            _lineFalloff.prop = FindProperty(_lineFalloff.name, properties, false);
            _gridOffset.prop = FindProperty(_gridOffset.name, properties, false);
            _gridVelocity.prop = FindProperty(_gridVelocity.name, properties, false);

            // Dot Matrix Properties.
            _dotSize.prop = FindProperty(_dotSize.name, properties, false);
            _dotSpace.prop = FindProperty(_dotSpace.name, properties, false);
            _dotsAreTransparent.prop = FindProperty(_dotsAreTransparent.name, properties, false);

            // Grid and Dot Matrix Common Properties.
            _rotationRadians.prop = FindProperty(_rotationRadians.name, properties, false);

            // Fresnel Properties.
            _fresnelPower.prop = FindProperty(_fresnelPower.name, properties, false);
            _fresnelColor.prop = FindProperty(_fresnelColor.name, properties, false);
            _useSceneIntersections.prop = FindProperty(_useSceneIntersections.name, properties, false);
            _intersectionPower.prop = FindProperty(_intersectionPower.name, properties, false);
            
            // Color Separation Properties.
            _useColorSeparation.prop = FindProperty(_useColorSeparation.name, properties, false);
            _channelSeparationDirection.prop = FindProperty(_channelSeparationDirection.name, properties, false);

            // Time Properties.
            _useUnscaledTime.prop = FindProperty(_useUnscaledTime.name, properties, false);

            // Dynamic Resolution Properties.
            _upscalingAmount.prop = FindProperty(_upscalingAmount.name, properties, false);

            // Shader Graph Advanced Options.
            _queueOffset.prop = FindProperty(_queueOffset.name, properties, false);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (materialEditor == null)
            {
                throw new ArgumentNullException("No MaterialEditor found (HologramShaderGUI).");
            }

            var material = materialEditor.target as Material;
            _materialEditor = materialEditor;

            if (material == null)
            {
                throw new ArgumentNullException("No Material found (HologramShaderGUI).");
            }

            FindProperties(properties);

            if (_firstTimeOpen)
            {
                _materialScopeList.RegisterHeaderScope(new GUIContent("Basic PBR Properties"), 1u << 0, DrawBasicPBRProperties);
                
                if(_scanlineMode.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Scanline Properties"), 1u << 1, DrawScanlineProperties);
                }

                if(_useVertexGlitches.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Vertex Glitches"), 1u << 2, DrawVertexGlitchProperties);
                }

                if(_useSliceGlitches.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Segment Glitches"), 1u << 3, DrawSegmentGlitchProperties);
                }

                if(_noiseStrength.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Noise Properties"), 1u << 4, DrawNoiseProperties);
                }

                if(_baseEmissionColor2.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Color Gradient Properties"), 1u << 5, DrawColorGradientProperties);
                }

                if(_perAxisStrength.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Gridline Properties"), 1u << 6, DrawGridlineProperties);
                }

                if(_dotSize.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Dot Matrix Properties"), 1u << 7, DrawDotMatrixProperties);
                }

                if(_fresnelColor.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Fresnel Properties"), 1u << 8, DrawFresnelProperties);
                }

                if(_useUnscaledTime.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Time Properties"), 1u << 9, DrawTimeProperties);
                }

                if(_upscalingAmount.prop != null)
                {
                    _materialScopeList.RegisterHeaderScope(new GUIContent("Dynamic Resolution Properties"), 1u << 10, DrawDynamicResolutionProperties);
                }

                _materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Options"), 1u << 12, DrawAdvancedOptions);

                _firstTimeOpen = false;
            }

            _materialScopeList.DrawHeaders(materialEditor, material);

            // Apply render queue offset.
            int renderQueue = material.shader.renderQueue;

            if (material.HasProperty(_queueOffset.name))
            {
                renderQueue += (int)material.GetFloat(_queueOffset.name);

                if (renderQueue != material.renderQueue)
                {
                    material.renderQueue = renderQueue;
                } 
            } 

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawBasicPBRProperties(Material material)
        {
            _materialEditor.ShaderProperty(_outputMode.prop, _outputMode.info);

            if(_baseEmissionColor2.prop == null)
            {
                _materialEditor.ShaderProperty(_baseEmissionColor.prop, _baseEmissionColor.info);
            }
            
            _materialEditor.ShaderProperty(_baseEmissionTexture.prop, _baseEmissionTexture.info);
            _materialEditor.ShaderProperty(_normalTexture.prop, _normalTexture.info);
            _materialEditor.ShaderProperty(_normalStrength.prop, _normalStrength.info);
            _materialEditor.ShaderProperty(_alphaClipThreshold.prop, _alphaClipThreshold.info);
            _materialEditor.ShaderProperty(_metallic.prop, _metallic.info);
            _materialEditor.ShaderProperty(_smoothness.prop, _smoothness.info);
            _materialEditor.ShaderProperty(_ambientOcclusion.prop, _ambientOcclusion.info);

            if (_useColorSeparation.prop != null)
            {
                _materialEditor.ShaderProperty(_useColorSeparation.prop, _useColorSeparation.info);

                if (material.GetFloat(_useColorSeparation.id) > 0.5f)
                {
                    EditorGUI.BeginChangeCheck();
                    var channelSeparation = material.GetVector(_channelSeparationDirection.id);
                    channelSeparation = EditorGUILayout.Vector2Field(_channelSeparationDirection.info, channelSeparation);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(material, "Modify Channel Separation");
                        material.SetVector(_channelSeparationDirection.id, channelSeparation);
                    }
                }
            }
        }

        private void DrawScanlineProperties(Material material)
        {
            _materialEditor.ShaderProperty(_scanlineMode.prop, _scanlineMode.info);

            if(_scanlineMode.prop.floatValue > 1.5f)         // Scanlines off.
            {
                return;
            }
            else if(_scanlineMode.prop.floatValue < 0.5f)    // Screen space mode.
            {
                _materialEditor.ShaderProperty(_scanlineRotation.prop, _scanlineRotation.info);
            }
            else                                            // World space mode.
            {
                _scanlineDirection.prop.vectorValue = EditorGUILayout.Vector2Field(_scanlineDirection.info, _scanlineDirection.prop.vectorValue);
            }

            _materialEditor.ShaderProperty(_scanlineTexture.prop, _scanlineTexture.info);
            _scanlineVelocity.prop.vectorValue = EditorGUILayout.Vector2Field(_scanlineVelocity.info, _scanlineVelocity.prop.vectorValue);
            _scanlineMinMaxAlpha.prop.vectorValue = EditorGUILayout.Vector2Field(_scanlineMinMaxAlpha.info, _scanlineMinMaxAlpha.prop.vectorValue);
        }

        private void DrawVertexGlitchProperties(Material material)
        {
            _materialEditor.ShaderProperty(_useVertexGlitches.prop, _useVertexGlitches.info);

            if(material.GetFloat(_useVertexGlitches.name) > 0.5f)
            {
                _materialEditor.ShaderProperty(_glitchSensitivity.prop, _glitchSensitivity.info);
                _glitchNormalMultiplier.prop.vectorValue = EditorGUILayout.Vector3Field(_glitchNormalMultiplier.info, _glitchNormalMultiplier.prop.vectorValue);
                _materialEditor.ShaderProperty(_glitchStrength.prop, _glitchStrength.info);
                _materialEditor.ShaderProperty(_glitchOffset.prop, _glitchOffset.info);
                _materialEditor.ShaderProperty(_glitchFrequency.prop, _glitchFrequency.info);
            }
        }

        private void DrawSegmentGlitchProperties(Material material)
        {
            _materialEditor.ShaderProperty(_useSliceGlitches.prop, _useSliceGlitches.info);

            if(material.GetFloat(_useSliceGlitches.name) > 0.5f)
            {
                _materialEditor.ShaderProperty(_sliceWidth.prop, _sliceWidth.info);
                _materialEditor.ShaderProperty(_sliceSpeed.prop, _sliceSpeed.info);
                _materialEditor.ShaderProperty(_sliceFrequency.prop, _sliceFrequency.info);
                _materialEditor.ShaderProperty(_sliceJitter.prop, _sliceJitter.info);
                _materialEditor.ShaderProperty(_sliceDuration.prop, _sliceDuration.info);
                _sliceDirection.prop.vectorValue = EditorGUILayout.Vector3Field(_sliceDirection.info, _sliceDirection.prop.vectorValue);
            }
        }

        private void DrawNoiseProperties(Material material)
        {
            _materialEditor.ShaderProperty(_noiseSpeed.prop, _noiseSpeed.info);
            _materialEditor.ShaderProperty(_noiseScale.prop, _noiseScale.info);
            _materialEditor.ShaderProperty(_noiseStrength.prop, _noiseStrength.info);
            _materialEditor.ShaderProperty(_noiseColor.prop, _noiseColor.info);
        }

        private void DrawColorGradientProperties(Material material)
        {
            _materialEditor.ShaderProperty(_gradientSpace.prop, _gradientSpace.info);
            _materialEditor.ShaderProperty(_baseEmissionColor.prop, _baseEmissionColor.info);
            _materialEditor.ShaderProperty(_baseEmissionColor2.prop, _baseEmissionColor2.info);
            _gradientMinMaxY.prop.vectorValue = EditorGUILayout.Vector2Field(_gradientMinMaxY.info, _gradientMinMaxY.prop.vectorValue);
        }

        private void DrawGridlineProperties(Material material)
        {
            _perAxisStrength.prop.vectorValue = EditorGUILayout.Vector3Field(_perAxisStrength.info, _perAxisStrength.prop.vectorValue);
            _materialEditor.ShaderProperty(_gridDensity.prop, _gridDensity.info);
            _rotationAxis.prop.vectorValue = EditorGUILayout.Vector3Field (_rotationAxis.info, _rotationAxis.prop.vectorValue);
            _materialEditor.ShaderProperty(_rotationRadians.prop, _rotationRadians.info);
            _materialEditor.ShaderProperty(_lineThickness.prop, _lineThickness.info);
            _materialEditor.ShaderProperty(_lineFalloff.prop, _lineFalloff.info);
            _gridOffset.prop.vectorValue = EditorGUILayout.Vector3Field(_gridOffset.info, _gridOffset.prop.vectorValue);
            _gridVelocity.prop.vectorValue = EditorGUILayout.Vector3Field(_gridVelocity.info, _gridVelocity.prop.vectorValue);
        }

        private void DrawDotMatrixProperties(Material material)
        {
            _materialEditor.ShaderProperty(_dotSize.prop, _dotSize.info);
            _materialEditor.ShaderProperty(_dotSpace.prop, _dotSpace.info);
            _materialEditor.ShaderProperty(_dotsAreTransparent.prop, _dotsAreTransparent.info);
            _materialEditor.ShaderProperty(_rotationRadians.prop, _rotationRadians.info);
        }

        private void DrawFresnelProperties(Material material)
        {
            _materialEditor.ShaderProperty(_fresnelPower.prop, _fresnelPower.info);
            _materialEditor.ShaderProperty(_fresnelColor.prop, _fresnelColor.info);

            if(_fresnelColor2.prop != null)
            {
                _materialEditor.ShaderProperty(_fresnelColor2.prop, _fresnelColor2.info);
            }

            _materialEditor.ShaderProperty(_useSceneIntersections.prop, _useSceneIntersections.info);

            if(material.GetFloat(_useSceneIntersections.name) > 0.5f)
            {
                _materialEditor.ShaderProperty(_intersectionPower.prop, _intersectionPower.info);
            }
        }

        private void DrawTimeProperties(Material material)
        {
            _materialEditor.ShaderProperty(_useUnscaledTime.prop, _useUnscaledTime.info);

            if(material.GetFloat(_useUnscaledTime.name) > 0.5f)
            {
                EditorGUILayout.HelpBox(_unscaledTimeInfoMessage, MessageType.Info);
            }
        }

        private void DrawDynamicResolutionProperties(Material material)
        {
            _materialEditor.ShaderProperty(_upscalingAmount.prop, _upscalingAmount.info);
        }

        private void DrawAdvancedOptions(Material material)
        {
            if (_queueOffset.prop != null)
            {
                _materialEditor.IntSliderShaderProperty(_queueOffset.prop, -_queueOffsetRange, _queueOffsetRange, _queueOffset.info);
            }

            _materialEditor.EnableInstancingField();
        }
    }
}

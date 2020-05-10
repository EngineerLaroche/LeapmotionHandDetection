/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System.Collections.Generic;

namespace Leap.Unity
{

    /// <summary>
    /// Use this component on a Game Object to allow it to be manipulated by a pinch gesture.  The component
    /// allows rotation, translation, and scale of the object (RTS).
    /// </summary>
    public class LeapRTS_Gesture : MonoBehaviour
    {
        public enum RotationMethod { None, Single, Full }

        [SerializeField]
        private PinchDetector _pinchDetectorA;
        [SerializeField]
        private PinchDetector _pinchDetectorB;
        [SerializeField]
        private RotationMethod _oneHandedRotationMethod = RotationMethod.Full;
        [SerializeField]
        private RotationMethod _twoHandedRotationMethod = RotationMethod.Full;
        [SerializeField]
        private bool _allowScale = true;
        [Header("GUI Options")]
        [SerializeField]
        private KeyCode _toggleGuiState = KeyCode.None;
        [SerializeField]
        private bool _showGUI = false;

        //private Transform _anchor;
        private float _defaultNearClip;

        //La liste des objets sélectionnés avec le raycast
        private List<SelectedObject> selectedObjects;

        private bool disableGravity = false;

        void Start()
        {
            //GameObject pinchControl = new GameObject("RTS Anchor");
            //_anchor = pinchControl.transform;
            //_anchor.transform.parent = transform.parent;
            //transform.parent = _anchor;
        }

        void Update()
        {
            if (Input.GetKeyDown(_toggleGuiState)) { _showGUI = !_showGUI; }

            //Si ole mode de manipulation contrôlé (restreint) n'est pas activé
            if (!PalmUIController.GetInstance().IsManipulationControlled())
            {
                selectedObjects = SelectionController.GetInstance().GetSelectedObjects();

                if (selectedObjects.Count > 0)
                {
                    foreach (SelectedObject objectToManipulate in selectedObjects)
                    {

                        if (!objectToManipulate.HasAnchor)
                        {
                            GameObject pinchControl = new GameObject("RTS Anchor");
                            Transform anchor = pinchControl.transform;
                            anchor.transform.parent = objectToManipulate.TransformObject.parent;
                            objectToManipulate.TransformObject.parent = anchor;
                            objectToManipulate.Anchor = anchor;
                            objectToManipulate.HasAnchor = true;
                        }

                        bool didUpdate = false;
                        if (_pinchDetectorA != null) didUpdate |= _pinchDetectorA.DidChangeFromLastFrame;
                        if (_pinchDetectorB != null) didUpdate |= _pinchDetectorB.DidChangeFromLastFrame;
                        if (didUpdate) objectToManipulate.TransformObject.SetParent(null, true);

                        //Si au moin un des deux Pinch est actif
                        if (_pinchDetectorA != null && _pinchDetectorB != null &&
                            (_pinchDetectorA.IsActive || _pinchDetectorB.IsActive))
                        {
                            if (disableGravity)
                            {
                                GravityController.GetInstance().ToAllowGravity(false, true);
                                disableGravity = false;
                            }  
                        }

                        //Si aucun des deux Pinch est actif
                        if (_pinchDetectorA != null && _pinchDetectorB != null &&
                            (!_pinchDetectorA.IsActive && !_pinchDetectorB.IsActive))
                        {
                            if (!disableGravity)
                            {
                                GravityController.GetInstance().ToAllowGravity(true, false);
                                disableGravity = true;
                            }
                        }

                        //Pinch main gauche et droite
                        if (_pinchDetectorA != null && _pinchDetectorA.IsActive &&
                            _pinchDetectorB != null && _pinchDetectorB.IsActive)
                        {
                            transformDoubleAnchor(objectToManipulate);
                        }
                        //Pinch main gauche
                        else if (_pinchDetectorA != null && _pinchDetectorA.IsActive)
                        {
                            transformSingleAnchor(_pinchDetectorA, objectToManipulate);
                        }
                        //Pinch main droite
                        else if (_pinchDetectorB != null && _pinchDetectorB.IsActive)
                        {
                            transformSingleAnchor(_pinchDetectorB, objectToManipulate);
                        }

                        if (didUpdate) { objectToManipulate.TransformObject.SetParent(objectToManipulate.Anchor, true); }
                    }
                }
            }
        }

        void OnGUI()
        {
            if (_showGUI)
            {
                GUILayout.Label("One Handed Settings");
                doRotationMethodGUI(ref _oneHandedRotationMethod);
                GUILayout.Label("Two Handed Settings");
                doRotationMethodGUI(ref _twoHandedRotationMethod);
                _allowScale = GUILayout.Toggle(_allowScale, "Allow Two Handed Scale");
            }
        }

        private void doRotationMethodGUI(ref RotationMethod rotationMethod)
        {
            GUILayout.BeginHorizontal();

            GUI.color = rotationMethod == RotationMethod.None ? Color.green : Color.white;
            if (GUILayout.Button("No Rotation"))
            {
                rotationMethod = RotationMethod.None;
            }

            GUI.color = rotationMethod == RotationMethod.Single ? Color.green : Color.white;
            if (GUILayout.Button("Single Axis"))
            {
                rotationMethod = RotationMethod.Single;
            }

            GUI.color = rotationMethod == RotationMethod.Full ? Color.green : Color.white;
            if (GUILayout.Button("Full Rotation"))
            {
                rotationMethod = RotationMethod.Full;
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }

        private void transformDoubleAnchor(SelectedObject _selectedObject)
        {
            _selectedObject.Anchor.position = (_pinchDetectorA.Position + _pinchDetectorB.Position) / 2.0f;

            switch (_twoHandedRotationMethod)
            {
                case RotationMethod.None:
                    break;
                case RotationMethod.Single:
                    Vector3 p = _pinchDetectorA.Position;
                    p.y = _selectedObject.Anchor.position.y;
                    _selectedObject.Anchor.LookAt(p);
                    break;
                case RotationMethod.Full:
                    Quaternion pp = Quaternion.Lerp(_pinchDetectorA.Rotation, _pinchDetectorB.Rotation, 0.5f);
                    Vector3 u = pp * Vector3.up;
                    _selectedObject.Anchor.LookAt(_pinchDetectorA.Position, u);
                    break;
            }

            if (_allowScale)
            {
                _selectedObject.Anchor.localScale = Vector3.one * Vector3.Distance(_pinchDetectorA.Position, _pinchDetectorB.Position);
            }
        }

        private void transformSingleAnchor(PinchDetector singlePinch, SelectedObject _selectedObject)
        {
            _selectedObject.Anchor.position = singlePinch.Position;

            switch (_oneHandedRotationMethod)
            {
                case RotationMethod.None:
                    break;
                case RotationMethod.Single:
                    Vector3 p = singlePinch.Rotation * Vector3.right;
                    p.y = _selectedObject.Anchor.position.y;
                    _selectedObject.Anchor.LookAt(p);
                    break;
                case RotationMethod.Full:
                    _selectedObject.Anchor.rotation = singlePinch.Rotation;
                    break;
            }
            _selectedObject.Anchor.localScale = Vector3.one;
        }

        //Get & Set pinch main gauche
        public PinchDetector PinchDetectorA
        {
            get { return _pinchDetectorA; }
            set { _pinchDetectorA = value; }
        }

        //Get & Set pinch main droite
        public PinchDetector PinchDetectorB
        {
            get { return _pinchDetectorB; }
            set { _pinchDetectorB = value; }
        }
    }
}

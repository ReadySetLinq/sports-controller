using System;
using System.Collections.Generic;
using XPression;

namespace SportsController.Shared
{
    public class Xpression
    {
        xpEngine _engine;

        public Xpression()
        {
            try
            {
                _engine = new xpEngine();
            } catch { }
        }

        #region Project

        public string ProjectPath()
        {
            try
            {
                return _engine.ProjectPath;
            }
            catch { return null; }
        }

        public string ProjectFileName()
        {
            try
            {
                return _engine.ProjectFileName;
            }
            catch { return null; }
        }

        public void CloseProject()
        {
            try
            {
                _engine.CloseProject();
            }
            catch { }
        }

        public bool LoadProject(string name)
        {
            try
            {
                bool loaded = _engine.LoadProject(FileName: name);
                if (loaded)
                    _engine.RestoreGUI(); // Fix GUI issue with the engine staying blank
                return loaded;
            }
            catch { return false; }
        }

        public bool SaveProject(string name)
        {
            try
            {
                return _engine.SaveProject(FileName: name);
            }
            catch { return false; }
        }

        #endregion

        #region TakeItem

        public xpTakeItem GetTakeItem(int takeID)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem)
                    return takeItem;
            }
            catch { return null; }

            return null;
        }

        public bool SetTakeItemOnline(int takeID)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem)
                    return takeItem.Execute();
            }
            catch { return false; }

            return true;
        }

        public bool SetTakeItemOffline(int takeID)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem)
                    return takeItem.SetOffline();
            }
            catch { return false; }

            return true;
        }

        public bool GetTakeItemStatus(int takeID)
        {
            try
            {
                // Retrieve take item 
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem)
                    return takeItem.IsOnline;
            }
            catch { return false; }

            return false;
        }

        public int GetTakeItemLayer(int takeID)
        {
            int layer = 0;
            try
            {
                // Retrieve take item 
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem)
                    return takeItem.Layer;
            }
            catch { return layer; }

            return layer;
        }

        public bool EditTakeItemProperty(int takeID, string objName, string propName, string value, string materialName = null)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem))
                {
                    if (baseTakeItem is xpTakeItem takeItem)
                    {
                        if (takeItem.GetPublishedObjectByName(objName, out xpPublishedObject publishedObject))
                        {
                            int propCount = publishedObject.PropertyCount;
                            // Loop through all properties until we find the one with our selected name
                            for (int propID = 0; propID < propCount; propID++)
                            {
                                publishedObject.GetPropertyInfo(propID, out string tempName, out PropertyType propType);
                                // Check if name is what we are looking for
                                if (tempName.Equals(propName, StringComparison.OrdinalIgnoreCase))
                                {
                                    switch (propType)
                                    {
                                        case PropertyType.pt_String:
                                            publishedObject.SetPropertyString(propID, value.Trim());
                                            break;
                                        case PropertyType.pt_Boolean:
                                            bool val;
                                            if (bool.TryParse(value.Trim(), out val))
                                                publishedObject.SetPropertyBool(propID, val);
                                            break;
                                        case PropertyType.pt_Material:
                                            if (int.TryParse(value.Trim(), out int face) && !string.IsNullOrEmpty(materialName))
                                            {
                                                if (_engine.GetMaterialByName(materialName, out xpMaterial material))
                                                {
                                                    publishedObject.SetPropertyMaterial(propID, face, material);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            takeItem.UpdateThumbnail();
                        }
                    }
                }
            }
            catch { return false; }

            return true;
        }

        public bool EditTakeItemProperty(int takeID, string objName, int propID, string value, string materialName = null)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem))
                {
                    if (baseTakeItem is xpTakeItem takeItem)
                    {
                        if (takeItem.GetPublishedObjectByName(objName, out xpPublishedObject publishedObject))
                        {
                            publishedObject.GetPropertyInfo(propID, out string propName, out PropertyType propType);
                            switch (propType)
                            {
                                case PropertyType.pt_String:
                                    publishedObject.SetPropertyString(propID, value.Trim());
                                    break;
                                case PropertyType.pt_Boolean:
                                    bool val;
                                    if (bool.TryParse(value.Trim(), out val))
                                        publishedObject.SetPropertyBool(propID, val);
                                    break;
                                case PropertyType.pt_Material:
                                    if (int.TryParse(value.Trim(), out int face) && !string.IsNullOrEmpty(materialName))
                                    {
                                        if (_engine.GetMaterialByName(materialName, out xpMaterial material))
                                        {
                                            publishedObject.SetPropertyMaterial(propID, face, material);
                                        }
                                    }
                                    break;
                            }
                            takeItem.UpdateThumbnail();
                        }
                    }
                }
            }
            catch { return false; }

            return true;
        }

        public bool EditTakeItemProperty(int takeID, int index, string propName, string value, string materialName = null)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem))
                {
                    if (baseTakeItem is xpTakeItem takeItem)
                    {
                        if (takeItem.GetPublishedObject(index, out xpPublishedObject publishedObject))
                        {
                            int propCount = publishedObject.PropertyCount;
                            // Loop through all properties until we find the one with our selected name
                            for (int propID = 0; propID < propCount; propID++)
                            {
                                publishedObject.GetPropertyInfo(propID, out string tempName, out PropertyType propType);
                                // Check if name is what we are looking for
                                if (tempName.Equals(propName, StringComparison.OrdinalIgnoreCase))
                                {
                                    switch (propType)
                                    {
                                        case PropertyType.pt_String:
                                            publishedObject.SetPropertyString(propID, value.Trim());
                                            break;
                                        case PropertyType.pt_Boolean:
                                            bool val;
                                            if (bool.TryParse(value.Trim(), out val))
                                                publishedObject.SetPropertyBool(propID, val);
                                            break;
                                        case PropertyType.pt_Material:
                                            if (int.TryParse(value.Trim(), out int face) && !string.IsNullOrEmpty(materialName))
                                            {
                                                if (_engine.GetMaterialByName(materialName, out xpMaterial material))
                                                {
                                                    publishedObject.SetPropertyMaterial(propID, face, material);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            takeItem.UpdateThumbnail();
                        }
                    }
                }
            }
            catch { return false; }

            return true;
        }

        public bool EditTakeItemProperty(int takeID, int index, int propID, string value, string materialName = null)
        {
            try
            {
                // Retrieve take item
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem))
                {
                    if (baseTakeItem is xpTakeItem takeItem)
                    {
                        if (takeItem.GetPublishedObject(index, out xpPublishedObject publishedObject))
                        {
                            publishedObject.GetPropertyInfo(propID, out string propName, out PropertyType propType);
                            switch (propType)
                            {
                                case PropertyType.pt_String:
                                    publishedObject.SetPropertyString(propID, value.Trim());
                                    break;
                                case PropertyType.pt_Boolean:
                                    bool val;
                                    if (bool.TryParse(value.Trim(), out val))
                                        publishedObject.SetPropertyBool(propID, val);
                                    break;
                                case PropertyType.pt_Material:
                                    if (int.TryParse(value.Trim(), out int face) && !string.IsNullOrEmpty(materialName))
                                    {
                                        if (_engine.GetMaterialByName(materialName, out xpMaterial material))
                                        {
                                            publishedObject.SetPropertyMaterial(propID, face, material);
                                        }
                                    }
                                    break;
                            }
                            takeItem.UpdateThumbnail();
                        }
                    }
                }
            }
            catch { return false; }

            return true;
        }

        #endregion

        #region Scene

        public bool TakeScene(string sceneName, bool online = true, int layerIndex = 0, int frameBuffer = 0)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                if (!_engine.GetSceneByName(sceneName, out xpScene _scene, false))
                    return false;
                return online
                    ? _scene.SetOnline(frameBuffer, layerIndex)
                    : _scene.SetOffline();
            }
            catch { return false; }
        }

        public bool TakeScene(int sceneID, bool online = true, int layerIndex = 0, int frameBuffer = 0)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                if (!_engine.GetSceneByID(sceneID, out xpScene _scene, false))
                    return false;
                // Set the scene online or offline
                return online
                    ? _scene.SetOnline(frameBuffer, layerIndex)
                    : _scene.SetOffline();
            }
            catch { return false; }
        }

        public bool PlaySceneDirector(int takeID, string directorName)
        {
            try
            {
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem && takeItem != null)
                {
                    if (!_engine.GetOutputFrameBuffer(takeItem.FrameBufferIndex, out xpOutputFrameBuffer outputBuffer)
                        || !outputBuffer.GetSceneOnLayer(takeItem.Layer, out xpScene _scene)
                        || !_scene.GetSceneDirectorByName(directorName, out xpSceneDirector _sceneDirector))
                        return false;
                    _sceneDirector.AutoStop = true;
                    return _sceneDirector.PlayRange(0, _sceneDirector.Duration);
                }
            }
            catch { return false; };

            return false;
        }

        public string GetSceneDirectorName(int takeID)
        {
            try
            {
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem && takeItem != null)
                {
                    return !_engine.GetOutputFrameBuffer(takeItem.FrameBufferIndex, out xpOutputFrameBuffer outputBuffer)
                        || !outputBuffer.GetSceneOnLayer(takeItem.Layer, out xpScene _scene)
                        ? ""
                        : _scene.SceneDirector.Name;
                }
            }
            catch { return ""; };

            return "";
        }

        public bool SetSceneDirector(int takeID, string directorName)
        {
            try
            {
                if (_engine.Sequencer.GetTakeItemByID(takeID, out xpBaseTakeItem baseTakeItem)
                    && baseTakeItem is xpTakeItem takeItem && takeItem != null)
                {
                    if (!_engine.GetOutputFrameBuffer(takeItem.FrameBufferIndex, out xpOutputFrameBuffer outputBuffer)
                        || !outputBuffer.GetSceneOnLayer(takeItem.Layer, out xpScene _scene)
                        || !_scene.GetSceneDirectorByName(directorName, out xpSceneDirector _sceneDirector))
                        return false;

                    _sceneDirector.Position = 0;
                    _sceneDirector.SetAsDefault();
                    return _scene.ApplySceneDirectorPositions();
                }
            }
            catch { return false; };

            return false;
        }

        public bool AnimateScene(string sceneName, string animName, bool waitFor = false, int timeOut = -1)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                return !_engine.GetSceneByName(sceneName, out xpScene _scene, false)
                    || !_scene.GetAnimControllerByName(animName, out xpAnimController _animController)
                    ? false
                    : _animController.Play(waitFor, timeOut);
            }
            catch { return false; }
        }

        public bool AnimateScene(int sceneID, string animName, bool waitFor = false, int timeOut = -1)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                return !_engine.GetSceneByID(sceneID, out xpScene _scene, false)
                    || !_scene.GetAnimControllerByName(animName, out xpAnimController _animController)
                    ? false
                    : _animController.Play(waitFor, timeOut);
            }
            catch { return false; }
        }

        public bool AnimateScene(string sceneName, int animID, bool waitFor = false, int timeOut = -1)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                return !_engine.GetSceneByName(sceneName, out xpScene _scene, false)
                    || !_scene.GetAnimController(animID, out xpAnimController _animController)
                    ? false
                    : _animController.Play(waitFor, timeOut);
            }
            catch { return false; }
        }

        public bool AnimateScene(int sceneID, int animID, bool waitFor = false, int timeOut = -1)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                return !_engine.GetSceneByID(sceneID, out xpScene _scene, false)
                    || !_scene.GetAnimController(animID, out xpAnimController _animController)
                    ? false
                    : _animController.Play(waitFor, timeOut);
            }
            catch { return false; }
        }

        #endregion

        #region EditScene

        // Edit scenes text value by scene ID and text name
        public bool EditSceneText(string sceneName, string txtName, string value)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                if (!_engine.GetSceneByName(sceneName, out xpScene _scene, false))
                    return false;

                _scene.GetObjectByName(txtName, out xpBaseObject obj);
                if (obj is xpTextObject txtObj)
                {
                    txtObj.Text = value;
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        // Edit scenes text value by scene ID and text name
        public bool EditSceneText(int sceneID, string txtName, string value)
        {
            try
            {
                // Get the scene object, if we fail to get it return false
                if (!_engine.GetSceneByID(sceneID, out xpScene _scene, false))
                    return false;

                _scene.GetObjectByName(txtName, out xpBaseObject obj);
                if (obj is xpTextObject txtObj)
                {
                    txtObj.Text = value;
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        #endregion

        #region Widget

        // Edit engines counter widget by name
        public int EditCounterWidget(string name, int value)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpCounterWidget countWidg)
                {
                    countWidg.Value = value;
                    return value;
                }
            }
            catch { return Globals.INVALID_INT; }

            return Globals.INVALID_INT;
        }

        // Increase engines counter widget from name by given amount
        public int IncreaseCounterWidget(string name, int value)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpCounterWidget countWidg)
                {
                    countWidg.Value += value;
                    return countWidg.Value;
                }
            }
            catch { return Globals.INVALID_INT; }

            return Globals.INVALID_INT;
        }

        // Decrease engines counter widget from name by given amount
        public int DecreaseCounterWidget(string name, int value)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpCounterWidget countWidg)
                {
                    countWidg.Value -= value;
                    return countWidg.Value;
                }
            }
            catch { return Globals.INVALID_INT; }

            return Globals.INVALID_INT;
        }

        // Get the value from a counter widget by name
        public int GetCounterWidgetValue(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpCounterWidget countWidg)
                    return countWidg.Value;
            }
            catch { return Globals.INVALID_INT; }

            return Globals.INVALID_INT;
        }

        // Set engines Text List widget index by name
        public bool SetTextListWidgetItemIndex(string name, int index)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    listWidg.ItemIndex = index;
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        // Set engines Text List widget to the next item by name
        public int TextListWidgetNext(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    listWidg.Next();
                    return listWidg.ItemIndex;
                }
            }
            catch { return -1; }

            return -1;
        }

        // Set engines Text List widget to the previous item by name
        public int TextListWidgetPrev(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    listWidg.Prev();
                    return listWidg.ItemIndex;
                }
            }
            catch { return -1; }

            return -1;
        }

        // Set engines Text List widget to the the first item by name
        public int ResetTextListWidgetIndex(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    listWidg.Reset();
                    return listWidg.ItemIndex;
                }
            }
            catch { return -1; }

            return -1;
        }

        // Set engines Text List widget to the previous item by name
        public int AddTextListWidgeString(string name, string value)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    return listWidg.AddString(value.Trim());
                }
            }
            catch { return -1; }

            return -1;
        }

        // Set the all text values from a Text List widget by name
        public bool SetTextListWidgetValues(string name, List<string> values)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    // Remove all old strings
                    listWidg.ClearStrings();
                    // Loop through values and add each to lis
                    foreach (string value in values)
                    {
                        listWidg.AddString(value.Trim());
                    }
                    listWidg.Reset();
                }
            }
            catch { return false; }

            return true;
        }

        // Get the all text values from a Text List widget by name
        public bool SetTextListWidgetToValue(string name, string value)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    // Log old index to reset with later
                    int _oldIndex = listWidg.ItemIndex;
                    listWidg.ItemIndex = 0;

                    // Loop through values until we find the matching string
                    for (int i = 0; i < listWidg.Count; i++)
                    {
                        if (String.Equals(listWidg.Value.Trim(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            listWidg.ItemIndex = i;
                            return true;
                        }
                    }
                    // Value wasn't found, so reset to old
                    listWidg.ItemIndex = _oldIndex;
                }
            }
            catch { return false; }

            return false;
        }

        // Set engines Text List widget to the previous item by name
        public bool ClearTextListWidget(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    listWidg.ClearStrings();
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        // Get the value from a Text List widget by name
        public string GetTextListWidgetValue(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                    return listWidg.Value;
            }
            catch { return ""; }

            return "";
        }

        // Get the all text values from a Text List widget by name
        public string[] GetTextListWidgetValues(string name)
        {
            List<string> _response = new List<string>();
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                {
                    // Log old index to reset with later
                    int _oldIndex = listWidg.ItemIndex;
                    listWidg.ItemIndex = 0;
                    for (int i = 0; i < listWidg.Count; i++)
                    {
                        listWidg.ItemIndex = i;
                        _response.Add(listWidg.Value.Trim());
                    }
                    listWidg.ItemIndex = _oldIndex;
                }
            }
            catch { return _response.ToArray(); }

            return _response.ToArray();
        }

        // Get the Item Index from a Text List widget by name
        public long GetTextListWidgetItemIndex(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                    return listWidg.ItemIndex;
            }
            catch { return -1; }

            return -1;
        }

        // Get the Item Index from a Text List widget by name
        public long GetTextListWidgetCount(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpTextListWidget listWidg)
                    return listWidg.Count;
            }
            catch { return -1; }

            return -1;
        }

        // Start a clock widget by name
        public int GetClockWidgetTimerValue(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                    return clockWidg.TimerValue;
            }
            catch { return 0; }

            return 0;
        }

        // Start a clock widget by name
        public string GetClockWidgetValue(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                    return clockWidg.Value;
            }
            catch { return "0"; }

            return "0";
        }

        // Start a clock widget by name
        public bool StartClockWidget(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                    return clockWidg.Start();
            }
            catch { return false; }

            return false;
        }

        // Stop a clock widget by name
        public bool StopClockWidget(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                    return clockWidg.Stop();
            }
            catch { return false; }

            return false;
        }

        // Stop a clock widget by name
        public bool ResetClockWidget(string name)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                    return clockWidg.Reset();
            }
            catch { return false; }

            return false;
        }

        // Edit a clock widgets format by name
        public bool EditClockWidgetFormat(string name, string format = "NN:SS")
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                {
                    // Make sure its a valid Xpression format
                    if (format.Equals("S") || format.Equals("SS") || format.Equals("SSS") ||
                        format.Equals("S.Z") || format.Equals("S.ZZ") || format.Equals("S.ZZZ") || format.Equals("SS.ZZZ") ||
                        format.Equals("HH:NN") || format.Equals("HH:NN:SS") || format.Equals("HH:NN:SS.ZZZ") ||
                        format.Equals("NN:SS") || format.Equals("NN:SS.ZZZ"))
                    {
                        clockWidg.Format = format;
                        return true;
                    }
                }
            }
            catch { return false; }

            return false;
        }

        // Edit a clock widgets format by name
        public bool EditClockWidgetStartTime(string name, string startTime = "")
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                {
                    clockWidg.StartAt = startTime;
                    clockWidg.Reset();
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        // Edit a clock widgets format by name
        public bool SetClockWidgetTimerValue(string name, int value)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                {
                    clockWidg.TimerValue = value;
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        // Edit a clock widgets format by name
        public bool SetClockWidgetCallback(string name, Action<int, int, int, int> callback)
        {
            try
            {
                _engine.GetWidgetByName(name, out xpBaseWidget baseWidg);
                if (baseWidg is xpClockTimerWidget clockWidg)
                {
                    if (clockWidg.SetEventMode(ClockTimerWidgetEventMode.temSeconds))
                    {
                        void OnClockChange(int Hours, int Minutes, int Seconds, int Milli)
                        {
                            callback(Hours, Minutes, Seconds, Milli);
                        }

                        clockWidg.OnChange += OnClockChange;
                        return true;
                    }
                }
            }
            catch { return false; }


            return false;
        }

        #endregion

        #region Metadata

        public bool EditMetadataAttribute(int sceneID, string attrName, string value)
        {
            try
            {
                if (!_engine.GetSceneByID(sceneID, out xpScene sceneSync, false)
                    || !sceneSync.GetMetadata(out xpMetadata mData))
                    return false;

                xpAttribute _attribute = mData.GetAttribByName(attrName);
                if (_attribute != null)
                    _attribute.SetString(value);
                else
                    mData.AddAttribute(attrName, value);
            }
            catch { return false; }

            return true;
        }

        #endregion
    }
}

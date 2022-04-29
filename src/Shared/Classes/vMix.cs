using SportsController.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SportsController.Shared
{


    public static class vMix
    {

        private static string base_url = "http://172.26.96.1:8088/api";

        #region Requests

        private static void _request(string url = "")
        {
            XmlReader xmlReader = XmlReader.Create(inputUri: $"{base_url}{url.Trim()}");
            xmlReader.ReadStartElement("input");
        }

        private static void _requestFunction(string functionName)
        {
            _request(url: $"/?Function={functionName.Trim()}");
        }

        private static void _requestFunctionInput(string functionName, string input)
        {
            _request(url: $"/?Function={functionName.Trim()}&Input={Uri.EscapeDataString(input.Trim())}");
        }

        private static void _requestFunctionInputValue(string functionName, string input, string value)
        {
            _request(url: $"/?Function={functionName.Trim()}&Input={Uri.EscapeDataString(input.Trim())}&Value={Uri.EscapeDataString(value.Trim())}");
        }

        private static void _requestFunctionInputSelectedName(string functionName, string input, string selectedName)
        {
            _request(url: $"/?Function={functionName.Trim()}&Input={Uri.EscapeDataString(input.Trim())}&SelectedName={Uri.EscapeDataString(selectedName.Trim())}");
        }

        private static void _requestFunctionInputValueSelectedName(string functionName, string input, string value, string selectedName)
        {
            _request(url: $"/?Function={functionName.Trim()}&Input={Uri.EscapeDataString(input.Trim())}&Value={Uri.EscapeDataString(value.Trim())}&SelectedName={Uri.EscapeDataString(selectedName.Trim())}");
        }

        #endregion

        #region Functions

        public static List<InputItem> GetInputs()
        {
            List<InputItem> items = new List<InputItem>();
            try
            {
                XmlReader xmlReader = XmlReader.Create(inputUri: base_url);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("input") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        items.Add(new InputItem(xmlReader.GetAttribute("key"), xmlReader.GetAttribute("number"), xmlReader.GetAttribute("type"), xmlReader.GetAttribute("title"), xmlReader.GetAttribute("shortTitle"), xmlReader.GetAttribute("state"), xmlReader.GetAttribute("position"), xmlReader.GetAttribute("duration"), xmlReader.GetAttribute("False"), xmlReader.GetAttribute("selectedIndex")));
                    }
                }
            }
            catch { }
            return items;
        }

        public static List<TitleItem> GetTitleItems(string input)
        {
            List<TitleItem> items = new List<TitleItem>();
            try
            {
                XmlReader xmlReader = XmlReader.Create(inputUri: base_url);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("input") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        //  key="38dead47-b168-46fe-80ec-4c36aa8c04bf" number="2" type="GT" title="bugScore" shortTitle="bugScore" state="Running" position="0" duration="0" loop="False" selectedIndex="0"
                        string title = xmlReader.GetAttribute("title");
                        if (title.ToLower().Equals(input.ToLower()))
                        {
                            if ((xmlReader.Equals("text") || xmlReader.Equals("image")) && (xmlReader.NodeType == XmlNodeType.Element))
                            {
                                do
                                {
                                    string index = xmlReader.GetAttribute("index");
                                    string name = xmlReader.GetAttribute("name");
                                    string value = xmlReader.GetValueAsync().Result;
                                    items.Add(new TitleItem(index, name, value));
                                } while (xmlReader.ReadToNextSibling("text"));
                            }
                        }
                    }
                }
            }
            catch { }
            return items;
        }

        public static List<InputItem> GetInputItems()
        {
            List<InputItem> items = new List<InputItem>();
            try
            {
                XmlReader xmlReader = XmlReader.Create(inputUri: base_url);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("input") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        items.Add(new InputItem(xmlReader.GetAttribute("key"), xmlReader.GetAttribute("number"), xmlReader.GetAttribute("type"), xmlReader.GetAttribute("title"), xmlReader.GetAttribute("shortTitle"), xmlReader.GetAttribute("state"), xmlReader.GetAttribute("position"), xmlReader.GetAttribute("duration"), xmlReader.GetAttribute("False"), xmlReader.GetAttribute("selectedIndex")));
                    }
                }
            }
            catch { }
            return items;
        }

        public static InputItem GetInputItem(string name)
        {
            try
            {
                XmlReader xmlReader = XmlReader.Create(inputUri: base_url);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("input") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        string key = xmlReader.GetAttribute("key");
                        string number = xmlReader.GetAttribute("number");
                        string type = xmlReader.GetAttribute("type");
                        string title = xmlReader.GetAttribute("title");
                        if (title.ToLower().Equals(name.ToLower()))
                            return new InputItem(key, number, type, title, xmlReader.GetAttribute("shortTitle"), xmlReader.GetAttribute("state"), xmlReader.GetAttribute("position"), xmlReader.GetAttribute("duration"), xmlReader.GetAttribute("False"), xmlReader.GetAttribute("selectedIndex"));
                    }
                }
            } catch { }
            return null;
        }

        public static int GetCounterWidgetValue(string input, string field)
        {
            int value = -1;
            try
            {
                XmlReader xmlReader = XmlReader.Create(inputUri: base_url);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("input") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        //  key="38dead47-b168-46fe-80ec-4c36aa8c04bf" number="2" type="GT" title="bugScore" shortTitle="bugScore" state="Running" position="0" duration="0" loop="False" selectedIndex="0"
                        string title = xmlReader.GetAttribute("title");
                        if (title.ToLower().Equals(input.ToLower()))
                        {
                            xmlReader.ReadToFollowing("images");
                            if (xmlReader.ReadToDescendant("text"))
                            {
                                do
                                {
                                    string name = xmlReader.GetAttribute("name");
                                    if ($"{name}.Text".Equals(field))
                                    {
                                        string _val = xmlReader.GetValueAsync().Result;
                                        if (int.TryParse(_val, out value))
                                            return value;
                                    }
                                } while (xmlReader.ReadToNextSibling("text"));
                            }
                        }
                    }
                }
            }
            catch { }
            return value;
        }

        public static string GetCounterWidgetString(string input, string field)
        {
            string value = string.Empty;
            try
            {
                XmlReader xmlReader = XmlReader.Create(inputUri: base_url);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name.Equals("input") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        //  key="38dead47-b168-46fe-80ec-4c36aa8c04bf" number="2" type="GT" title="bugScore" shortTitle="bugScore" state="Running" position="0" duration="0" loop="False" selectedIndex="0"
                        string title = xmlReader.GetAttribute("title");
                        if (title.ToLower().Equals(input.ToLower()))
                        {
                            xmlReader.ReadToFollowing("images");
                            if (xmlReader.ReadToDescendant("text"))
                            {
                                do
                                {
                                    string name = xmlReader.GetAttribute("name");
                                    if ($"{name}.Text".Equals(field))
                                        return xmlReader.GetValueAsync().Result;
                                } while (xmlReader.ReadToNextSibling("text"));
                            }
                        }
                    }
                }
            }
            catch { }
            return value;
        }

        #endregion

        #region Countdown

        public static void CountdownStart(string input) => _requestFunctionInput(functionName: "StartCountdown", input: input);

        public static void CountdownPause(string input) => _requestFunctionInput(functionName: "PauseCountdown", input: input);

        public static void CountdownSuspend(string input) => _requestFunctionInput(functionName: "SuspendCountdown", input: input);

        public static void CountdownStop(string input) => _requestFunctionInput(functionName: "StopCountdown", input: input);

        public static void CountdownSet(string input, string value) => _requestFunctionInputValue(functionName: "SetCountdown", input: input, value: value);

        public static void CountdownAdjust(string input, string value) => _requestFunctionInputValue(functionName: "AdjustCountdown", input: input, value: value);

        public static void CountdownChange(string input, string value) => _requestFunctionInputValue(functionName: "ChangeCountdown", input: input, value: value);

        #endregion

        #region Title

        public static void SetTitleImage(string input, string fileName) => _requestFunctionInputValue(functionName: "SetImage", input: input, value: fileName);

        public static void SetTitleImageVisible(string input, string selectedName) => _requestFunctionInputSelectedName(functionName: "SetImageVisible", input: input, selectedName: $"{selectedName}.Source");        

        public static void SetTitleImageVisibleOn(string input, string selectedName) => _requestFunctionInputSelectedName(functionName: "SetImageVisibleOn", input: input, selectedName: $"{selectedName}.Source");

        public static void SetTitleImageVisibleOff(string input, string selectedName) => _requestFunctionInputSelectedName(functionName: "SetImageVisibleOff", input: input, selectedName: $"{selectedName}.Source");

        public static void SetTitleText(string input, string value, string selectedName) => _requestFunctionInputValueSelectedName(functionName: "SetText", input: input, value: value, selectedName: $"{selectedName}.Text");

        public static void SetTitleTextVisible(string input, string selectedName) => _requestFunctionInputSelectedName(functionName: "SetTextVisible", input: input, selectedName: $"{selectedName}.Text");

        public static void SetTitleTextVisibleOn(string input, string selectedName) => _requestFunctionInputSelectedName(functionName: "SetTextVisibleOn", input: input, selectedName: $"{selectedName}.Text");

        public static void SetTitleTextVisibleOff(string input, string selectedName) => _requestFunctionInputSelectedName(functionName: "SetTextVisibleOff", input: input, selectedName: $"{selectedName}.Text");

        public static void TitleBeginAnimation(string input, string animationName) => _requestFunctionInputValue(functionName: "TitleBeginAnimation", input: input, value: $"[{animationName}]");

        #endregion

        #region OverlayInput

        public static void OverlayInput(int number, string input) => _requestFunctionInput(functionName: $"OverlayInput{number}", input: input);

        public static void OverlayInputIn(int number, string input) => _requestFunctionInput(functionName: $"OverlayInput{number}In", input: input);

        public static void OverlayInputOut(int number) => _requestFunction(functionName: $"OverlayInput{number}Out");

        public static void OverlayInputOff(int number) => _requestFunction(functionName: $"OverlayInput{number}Off");

        public static void OverlayInputLast(int number) => _requestFunction(functionName: $"OverlayInput{number}Last");

        public static void OverlayInputZoom(int number) => _requestFunction(functionName: $"OverlayInput{number}Zoom");

        public static void PreviewOverlayInput(int number) => _requestFunction(functionName: $"PreviewOverlayInput{number}");

        #endregion

    }
}
